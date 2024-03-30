using DatabaseApp.Application.UserWeatherSubscriptions.Commands.CreateUserWeatherSubscription;
using DatabaseApp.Application.UserWeatherSubscriptions.Commands.DeleteUserWeatherSubscription;
using DatabaseApp.Application.UserWeatherSubscriptions.Commands.UpdateUserWeatherSubscription;
using DatabaseApp.Domain.Models;
using DatabaseApp.Tests.BasicTestContext;
using DatabaseApp.WebApi.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DatabaseApp.Tests.WeatherSubscriptionTests;

public class WeatherSubscriptionControllerTests(IntegrationWebAppFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task CreateWeatherSubscription_WithValidData_ReturnsOkObjectResult()
    {
        // Arrange
        var controller = new WeatherSubscriptionController(_sender);
        var command = new CreateUserWeatherSubscriptionCommand
        {
            TelegramUserId = 1234567890,
            Location = "London",
            ResendInterval = TimeSpan.FromHours(1)
        };

        // Act
        var result = await controller.CreateUserWeatherSubscription(command);
        var okResult = result as CreatedAtActionResult;

        // Assert
        okResult.Should().NotBeNull();
        okResult?.StatusCode.Should().Be(StatusCodes.Status201Created);
    }

    [Fact]
    public async Task GetWeatherSubscriptionsByUserId_WithValidData_ReturnsOkObjectResult()
    {
        // Arrange
        var controller = new WeatherSubscriptionController(_sender);
        const int userTelegramId = 1234567890;
        await AddUsersToDatabase(new User
        {
            TelegramId = userTelegramId,
            Metadata = UserMetadata.Create("JohnDoe", "+1234567890").Value,
            RegisteredAt = DateTime.UtcNow
        });

        // Act
        var result = await controller.GetWeatherSubscriptionsByUserId(userTelegramId);
        var okResult = result as OkObjectResult;

        // Assert
        okResult.Should().NotBeNull();
        okResult?.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task UpdateWeatherSubscription_WithValidData_ReturnsNoContentResult()
    {
        // Arrange
        var controller = new WeatherSubscriptionController(_sender);
        var command = new UpdateUserWeatherSubscriptionCommand
        {
            UserTelegramId = 1234567890,
            Location = "London",
            ResendInterval = TimeSpan.FromHours(1)
        };
        await AddUsersToDatabase(new User
        {
            TelegramId = command.UserTelegramId,
            Metadata = UserMetadata.Create("JohnDoe", "+1234567890").Value,
            RegisteredAt = DateTime.UtcNow
        });

        // Act
        var result = await controller.UpdateUserWeatherSubscription(command);
        var noContentResult = result as NoContentResult;

        // Assert
        noContentResult.Should().NotBeNull();
        noContentResult?.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }

    [Fact]
    public async Task DeleteWeatherSubscription_WithValidData_ReturnsNoContentResult()
    {
        // Arrange
        var controller = new WeatherSubscriptionController(_sender);
        var command = new DeleteUserWeatherSubscriptionCommand
        {
            UserTelegramId = 1234567890,
            Location = "London"
        };
        await AddUsersToDatabase(new User
        {
            TelegramId = command.UserTelegramId,
            Metadata = UserMetadata.Create("JohnDoe", "+1234567890").Value,
            RegisteredAt = DateTime.UtcNow
        });

        // Act
        var result = await controller.DeleteUserWeatherSubscription(command);
        var noContentResult = result as NoContentResult;

        // Assert
        noContentResult.Should().NotBeNull();
        noContentResult?.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }
}