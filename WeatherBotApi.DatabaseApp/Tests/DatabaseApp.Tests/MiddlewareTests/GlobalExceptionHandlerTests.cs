using DatabaseApp.WebApi.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace DatabaseApp.Tests.MiddlewareTests;

public class GlobalExceptionHandlerTests
{
    [Fact]
    public async Task TryHandleAsync_ShouldReturnTrue_WhenExceptionOccurs()
    {
        // Arrange
        var mockLogger = Substitute.For<ILogger<GlobalExceptionHandler>>();
        var handler = new GlobalExceptionHandler(mockLogger);
        var context = new DefaultHttpContext();
        var exception = new Exception("Test exception");

        // Act
        var result = await handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        result.Should().Be(true);
        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }
}