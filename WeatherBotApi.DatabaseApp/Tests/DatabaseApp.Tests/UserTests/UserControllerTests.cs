using DatabaseApp.Application.Users.Commands.CreateUser;
using DatabaseApp.Domain.Models;
using DatabaseApp.Tests.BasicTestContext;
using DatabaseApp.WebApi.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DatabaseApp.Tests.UserTests;

public class UserControllerTests(IntegrationWebAppFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task GetAllUsers_WithValidData_ReturnsOkObjectResult()
    {
        // Arrange
        var controller = new UserController(_sender);
        await AddUsersToDatabase(new User
        {
            TelegramId = 1234567890,
            Metadata = UserMetadata.Create("JohnDoe", "+1234567890").Value,
            RegisteredAt = DateTime.UtcNow
        });

        // Act
        var result = await controller.GetAllUsers();
        var okResult = result as OkObjectResult;

        // Assert
        okResult.Should().NotBeNull();
        okResult?.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task GetUser_WithValidData_ReturnsOkObjectResult()
    {
        // Arrange
        var controller = new UserController(_sender);
        const int userTelegramId = 1234567890;
        await AddUsersToDatabase(new User
        {
            TelegramId = userTelegramId,
            Metadata = UserMetadata.Create("JohnDoe", "+1234567890").Value,
            RegisteredAt = DateTime.UtcNow
        });

        // Act
        var result = await controller.GetUser(userTelegramId);
        var okResult = result as OkObjectResult;

        // Assert
        okResult.Should().NotBeNull();
        okResult?.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task CreateUser_WithValidData_ReturnsCreatedAtActionResult()
    {
        // Arrange
        var controller = new UserController(_sender);
        var command = new CreateUserCommand
        {
            TelegramId = 1234567890,
            Username = "JohnDoe",
            MobileNumber = "+1234567890",
            RegisteredAt = DateTime.UtcNow
        };

        // Act
        var result = await controller.CreateUser(command);
        var okResult = result as CreatedAtActionResult;

        // Assert
        okResult.Should().NotBeNull();
        okResult?.StatusCode.Should().Be(StatusCodes.Status201Created);
    }
}