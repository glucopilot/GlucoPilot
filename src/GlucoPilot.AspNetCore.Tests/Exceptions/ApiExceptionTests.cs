using System.Net;
using GlucoPilot.AspNetCore.Exceptions;

namespace GlucoPilot.AspNetCore.Tests.Exceptions;

[TestFixture]
internal sealed class ApiExceptionTests
{
    [Test]
    public void ConflictException_Should_Set_Properties_Correctly()
    {
        var message = "Conflict occurred";
        var exception = new ConflictException(message);

        Assert.That(exception.Message, Is.EqualTo(message));
        Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
    }

    [Test]
    public void ForbiddenException_Should_Set_Properties_Correctly()
    {
        var message = "Forbidden access";
        var exception = new ForbiddenException(message);

        Assert.That(exception.Message, Is.EqualTo(message));
        Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    public void NotFoundException_Should_Set_Properties_Correctly()
    {
        var message = "Resource not found";
        var exception = new NotFoundException(message);

        Assert.That(exception.Message, Is.EqualTo(message));
        Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public void UnauthorizedException_Should_Set_Properties_Correctly()
    {
        var message = "Unauthorized access";
        var exception = new UnauthorizedException(message);

        Assert.That(exception.Message, Is.EqualTo(message));
        Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}