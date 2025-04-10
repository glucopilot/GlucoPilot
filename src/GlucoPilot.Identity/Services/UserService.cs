using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using GlucoPilot.AspNetCore.Exceptions;
using GlucoPilot.Data.Entities;
using GlucoPilot.Data.Repository;
using GlucoPilot.Identity.Models;
using GlucoPilot.Identity.Templates;
using GlucoPilot.Mail;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using static BCrypt.Net.BCrypt;

namespace GlucoPilot.Identity.Services;

public sealed class UserService : IUserService
{
    private readonly IRepository<User> _repository;
    private readonly ITokenService _tokenService;
    private readonly IMailService _mailService;
    private readonly ITemplateService _templateService;
    private readonly IdentityOptions _options;

    public UserService(IRepository<User> repository, ITokenService tokenService, IMailService mailService,
        ITemplateService templateService, IOptions<IdentityOptions> options)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _repository.FindOneAsync(u => u.Email == request.Email,
            new FindOptions { IsAsNoTracking = true, IsIgnoreAutoIncludes = true },
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (user is null || (_options.RequireEmailVerification && !user.IsVerified) ||
            !Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("EMAIL_OR_PASSWORD_INCORRECT");
        }

        var token = _tokenService.GenerateJwtToken(user);
        var response = new LoginResponse
        {
            Token = token,
        };
        return response;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request, string origin,
        CancellationToken cancellationToken = default)
    {
        if (await _repository.AnyAsync(x => x.Email == request.Email, cancellationToken).ConfigureAwait(false))
        {
            throw new ConflictException("USER_ALREADY_EXISTS");
        }

        User user;
        if (request.PatientId is null)
        {
            var patient = new Patient
            {
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                AcceptedTerms = request.AcceptedTerms,
                EmailVerificationToken = _options.RequireEmailVerification
                    ? await GenerateVerificationTokenAsync(cancellationToken).ConfigureAwait(false)
                    : null,
                IsVerified = !_options.RequireEmailVerification,
                FirstName = request.FirstName,
                LastName = request.LastName,
            };
            user = patient;

            await _repository.AddAsync(patient, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            var patient = await _repository.FindOneAsync(x => x.Id == request.PatientId.Value,
                    new FindOptions { IsAsNoTracking = true, IsIgnoreAutoIncludes = true }, cancellationToken)
                .ConfigureAwait(false);

            if (patient is not Patient patientEntity)
            {
                throw new NotFoundException("PATIENT_NOT_FOUND");
            }

            var careGiver = new CareGiver
            {
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                AcceptedTerms = request.AcceptedTerms,
                Patients = [patientEntity],
                EmailVerificationToken = _options.RequireEmailVerification
                    ? await GenerateVerificationTokenAsync(cancellationToken).ConfigureAwait(false)
                    : null,
                IsVerified = !_options.RequireEmailVerification,
                FirstName = request.FirstName,
                LastName = request.LastName,
            };
            user = careGiver;

            await _repository.AddAsync(careGiver, cancellationToken).ConfigureAwait(false);
        }

        if (_options.RequireEmailVerification)
        {
            var confirmEmailModel = new EmailConfirmation()
            {
                Email = user.Email,
                Url = GetEmailVerificationUrl(user.EmailVerificationToken!, origin),
            };

            var message = new MailRequest()
            {
                To = [user.Email],
                Subject = "Verify your email",
                Body = await _templateService
                    .RenderTemplateAsync("EmailConfirmation", confirmEmailModel, cancellationToken)
                    .ConfigureAwait(false),
            };
            await _mailService.SendAsync(message, cancellationToken).ConfigureAwait(false);
        }

        return new RegisterResponse
        {
            Id = user.Id,
            Email = user.Email,
            Created = user.Created,
            Updated = user.Updated,
            AcceptedTerms = user.AcceptedTerms,
            EmailVerified = !_options.RequireEmailVerification,
        };
    }

    private async Task<string> GenerateVerificationTokenAsync(CancellationToken cancellationToken)
    {
        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

        var tokenIsUnique = !await _repository.AnyAsync(x => x.EmailVerificationToken == token, cancellationToken)
            .ConfigureAwait(false);
        if (!tokenIsUnique)
        {
            return await GenerateVerificationTokenAsync(cancellationToken).ConfigureAwait(false);
        }

        return token;
    }

    private static string GetEmailVerificationUrl(string verificationToken, string origin)
    {
        const string route = "api/v1/identity/verify-email";
        var endpointUri = new Uri(string.Concat($"{origin}/", route));
        return QueryHelpers.AddQueryString(endpointUri.ToString(), "token", verificationToken);
    }

    public async Task VerifyEmailAsync(VerifyEmailRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _repository.FindOneAsync(x => x.EmailVerificationToken == request.Token,
                new FindOptions() { IsAsNoTracking = true, IsIgnoreAutoIncludes = true }, cancellationToken)
            .ConfigureAwait(false);
        if (user is null)
        {
            throw new UnauthorizedException("EMAIL_VERIFICATION_TOKEN_INVALID");
        }

        user.EmailVerificationToken = null;
        user.IsVerified = true;

        await _repository.UpdateAsync(user, cancellationToken).ConfigureAwait(false);
    }
}