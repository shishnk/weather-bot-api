using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using WeatherApp.Application.Services;
using WeatherApp.Tests.ServiceFixtures;
using WeatherApp.WebApi.Controllers;
using Xunit;

namespace WeatherApp.Tests;

public class WeatherServiceTests(WeatherServiceFixture weatherServiceFixture) : IClassFixture<WeatherServiceFixture>
{
    private readonly IWeatherService _weatherServiceFixture = weatherServiceFixture.GetService<IWeatherService>();
    private readonly ILogger<WeatherController> _mockLogger = Substitute.For<ILogger<WeatherController>>();

    [Theory]
    [InlineData("London")]
    [InlineData("Novosibirsk")]
    [InlineData("New York")]
    [InlineData("Moscow")]
    public async Task GetWeatherForecast_WithValidLocation_ReturnsOkObjectResult(string location)
    {
        // Arrange
        var controller = new WeatherController(_weatherServiceFixture, _mockLogger);

        // Act
        var result = await controller.GetWeatherForecast(location);
        var okResult = result as OkObjectResult;

        // Assert
        okResult.Should().NotBeNull();
        okResult?.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task GetWeatherForecast_WithInvalidLocation_ReturnsBadRequest()
    {
        // Arrange
        var controller = new WeatherController(_weatherServiceFixture, _mockLogger);

        // Act
        var result = await controller.GetWeatherForecast("InvalidLocation");
        var badRequestResult = result as BadRequestObjectResult;

        // Assert
        badRequestResult.Should().NotBeNull();
        badRequestResult?.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }
}