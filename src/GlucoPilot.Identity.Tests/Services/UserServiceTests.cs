using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using GlucoPilot.AspNetCore.Exceptions;
using GlucoPilot.Data;
using GlucoPilot.Data.Entities;
using GlucoPilot.Data.Repository;
using GlucoPilot.Identity.Models;
using GlucoPilot.Identity.Services;
using GlucoPilot.Mail;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;

namespace GlucoPilot.Identity.Tests.Services;

[TestFixture]
internal sealed class UserServiceTests
{
    private Mock<IRepository<User>> _userRepository;
    private Mock<ITokenService> _tokenService;
    private Mock<IMailService> _mailService;
    private Mock<ITemplateService> _templateService;
    private IdentityOptions _options;
    private Mock<IOptions<IdentityOptions>> _identityOptions;
    private UserService _sut;

    [SetUp]
    public void Setup()
    {
        _userRepository = new Mock<IRepository<User>>();
        _tokenService = new Mock<ITokenService>();
        _mailService = new Mock<IMailService>();
        _templateService = new Mock<ITemplateService>();
        _identityOptions = new Mock<IOptions<IdentityOptions>>();
        _options = new IdentityOptions() { RequireEmailVerification = false };
        _identityOptions.Setup(x => x.Value).Returns(_options);

        _sut = new UserService(_userRepository.Object, _tokenService.Object, _mailService.Object,
            _templateService.Object, _identityOptions.Object);
    }

    [Test]
    public void Constructor_Should_Throw_ArgumentNullExceptions()
    {
        Assert.Multiple(() =>
        {
            Assert.That(
                () => new UserService(null!, _tokenService.Object, _mailService.Object, _templateService.Object,
                    _identityOptions.Object), Throws.ArgumentNullException);
            Assert.That(
                () => new UserService(_userRepository.Object!, null!, _mailService.Object, _templateService.Object,
                    _identityOptions.Object), Throws.ArgumentNullException);
            Assert.That(
                () => new UserService(_userRepository.Object!, _tokenService.Object, _mailService.Object, null!,
                    _identityOptions.Object), Throws.ArgumentNullException);
            Assert.That(
                () => new UserService(_userRepository.Object!, _tokenService.Object, _mailService.Object,
                    _templateService.Object, null!), Throws.ArgumentNullException);
        });
    }

    [Test]
    public async Task LoginAsync_WithValidPatientCredentials_ReturnsLoginResponse()
    {
        var request = new LoginRequest { Email = "test@example.com", Password = "password" };
        var user = new Patient
        { Email = "test@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password") };

        _userRepository.Setup(r => r.FindOneAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<FindOptions>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _sut.LoginAsync(request);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Token, Is.Not.Empty);
    }

    [Test]
    public async Task LoginAsync_WithValidCareGiverCredentials_ReturnsLoginResponse()
    {
        var request = new LoginRequest { Email = "test@example.com", Password = "password" };
        var user = new CareGiver
        { Email = "test@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password") };
        _userRepository.Setup(r => r.FindOneAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<FindOptions>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _sut.LoginAsync(request);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Token, Is.Not.Empty);
    }

    [Test]
    public void LoginAsync_WithValidPatientCredentials_Unverified_Throws_Unauthorized()
    {
        _options.RequireEmailVerification = true;
        var request = new LoginRequest { Email = "test@example.com", Password = "password" };
        var user = new Patient
        { Email = "test@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password") };

        _userRepository.Setup(r => r.FindOneAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<FindOptions>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(user);

        Assert.That(() => _sut.LoginAsync(request), Throws.TypeOf<UnauthorizedException>());
    }

    [Test]
    public void LoginAsync_WithValidCareGiverCredentials_Unverified_Throws_Unauthorized()
    {
        _options.RequireEmailVerification = true;
        var request = new LoginRequest { Email = "test@example.com", Password = "password" };
        var user = new CareGiver
        { Email = "test@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password") };

        _userRepository.Setup(r => r.FindOneAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<FindOptions>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(user);

        Assert.That(() => _sut.LoginAsync(request), Throws.TypeOf<UnauthorizedException>());
    }

    [Test]
    public void LoginAsync_WithInvalidPatientCredentials_ThrowsUnauthorizedException()
    {
        var request = new LoginRequest { Email = "test@example.com", Password = "wrongpassword" };
        var user = new Patient
        { Email = "test@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password") };
        _userRepository.Setup(r => r.FindOneAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<FindOptions>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(user);

        Assert.That(() => _sut.LoginAsync(request), Throws.TypeOf<UnauthorizedException>());
    }

    [Test]
    public void LoginAsync_WithInvalidCareGiverCredentials_ThrowsUnauthorizedException()
    {
        var request = new LoginRequest { Email = "test@example.com", Password = "wrongpassword" };
        var user = new CareGiver
        { Email = "test@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password") };
        _userRepository.Setup(r => r.FindOneAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<FindOptions>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(user);

        Assert.That(() => _sut.LoginAsync(request), Throws.TypeOf<UnauthorizedException>());
    }

    [Test]
    public async Task RegisterAsync_WithNewPatient_ReturnsRegisterResponse()
    {
        var request = new RegisterRequest
        {
            Email = "newuser@example.com",
            Password = "password",
            ConfirmPassword = "password",
            AcceptedTerms = true,
            FirstName = "first name",
            LastName = "last name",
        };

        var result = await _sut.RegisterAsync(request, "http://localhost", CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Email, Is.EqualTo(request.Email));
    }

    [Test]
    public async Task RegisterAsync_WithNewCareGiver_ReturnsRegisterResponse()
    {
        var patient = new Patient()
        { Email = "test@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password") };

        _userRepository.Setup(r => r.FindOneAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<FindOptions>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(patient);

        var request = new RegisterRequest
        {
            Email = "newuser@example.com",
            Password = "password",
            ConfirmPassword = "password",
            AcceptedTerms = true,
            PatientId = patient.Id
        };

        var result = await _sut.RegisterAsync(request, "http://localhost", CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Email, Is.EqualTo(request.Email));
    }

    [Test]
    public async Task RegisterAsync_Patient_RequiresEmailVerification()
    {
        _options.RequireEmailVerification = true;
        var patient = new Patient()
        { Email = "test@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password") };

        _userRepository.Setup(r => r.FindOneAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<FindOptions>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(patient);

        var request = new RegisterRequest
        {
            Email = "newuser@example.com",
            Password = "password",
            ConfirmPassword = "password",
            AcceptedTerms = true,
            PatientId = patient.Id
        };

        _ = await _sut.RegisterAsync(request, "http://localhost", CancellationToken.None);

        _mailService.Verify(
            m => m.SendAsync(It.Is<MailRequest>(x => x.To.SequenceEqual(new[] { request.Email })),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task RegisterAsync_CareGiver_RequiresEmailVerification()
    {
        _options.RequireEmailVerification = true;

        var request = new RegisterRequest
        {
            Email = "newuser@example.com",
            Password = "password",
            ConfirmPassword = "password",
            AcceptedTerms = true,
        };

        _ = await _sut.RegisterAsync(request, "http://localhost", CancellationToken.None);

        _mailService.Verify(
            m => m.SendAsync(It.Is<MailRequest>(x => x.To.SequenceEqual(new[] { request.Email })),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void RegisterAsync_WithExistingUser_ThrowsConflictException()
    {
        var request = new RegisterRequest
        {
            Email = "existinguser@example.com",
            Password = "password",
            ConfirmPassword = "password",
            AcceptedTerms = true
        };
        var user = new CareGiver
        { Email = "existinguser@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password") };
        _userRepository.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        Assert.That(() => _sut.RegisterAsync(request, "http://localhost", CancellationToken.None),
            Throws.InstanceOf<ConflictException>());
    }

    [Test]
    public void RegisterAsync_WithNonExistentPatientId_ThrowsNotFoundException()
    {
        var request = new RegisterRequest
        {
            Email = "caregiver@example.com",
            Password = "password",
            ConfirmPassword = "password",
            AcceptedTerms = true,
            PatientId = Guid.NewGuid()
        };

        Assert.That(() => _sut.RegisterAsync(request, "http://localhost", CancellationToken.None),
            Throws.InstanceOf<NotFoundException>());
    }

    [Test]
    public void VerifyEmailAsync_Throws_UnauthorizedException()
    {
        var request = new VerifyEmailRequest
        {
            Token = "token"
        };

        _userRepository
            .Setup(r => r.FindOneAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<FindOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);

        Assert.That(() => _sut.VerifyEmailAsync(request), Throws.TypeOf<UnauthorizedException>());
    }

    [Test]
    public async Task VerifyEmailAsync_Updates_User()
    {
        var user = new Patient
        {
            Email = "user@nomail.com",
            PasswordHash = "password",
            EmailVerificationToken = "token",
        };
        var request = new VerifyEmailRequest
        {
            Token = "token"
        };

        _userRepository
            .Setup(r => r.FindOneAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<FindOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        await _sut.VerifyEmailAsync(request, CancellationToken.None);

        _userRepository.Verify(r => r.UpdateAsync(It.Is<User>(u => u.EmailVerificationToken == null && u.IsVerified), It.IsAny<CancellationToken>()), Times.Once);
    }
}