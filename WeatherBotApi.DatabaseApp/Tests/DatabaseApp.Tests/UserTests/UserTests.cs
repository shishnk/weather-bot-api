using DatabaseApp.Application.Users.Commands.CreateUser;
using DatabaseApp.Application.Users.Queries.GetAllUsers;
using DatabaseApp.Application.Users.Queries.GetUser;
using DatabaseApp.Domain.Models;
using DatabaseApp.Tests.BasicTestContext;
using FluentAssertions;
using FluentValidation;
using Xunit;

namespace DatabaseApp.Tests.UserTests;

public class UserTests(IntegrationWebAppFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task CreateUser_WithValidData_ShouldAddUserToDatabaseAndReturnsUserTelegramId()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Username = "JohnDoe",
            MobileNumber = "+1234567890",
            TelegramId = 1234567890,
            RegisteredAt = DateTime.UtcNow
        };

        // Act
        var result = await _sender.Send(command);
        var user = await _unitOfWork.UserRepository.GetByTelegramIdAsync(command.TelegramId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().Be(true);
        result.Value.Should().Be(command.TelegramId);
        user.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateUser_WhenUserAlreadyExists_ShouldNotAddUserToDatabase()
    {
        // Arrange
        const int telegramId = 1234567890;
        const string username = "JohnDoe";
        const string mobileNumber = "+1234567890";

        var user = new User
        {
            TelegramId = telegramId,
            Metadata = UserMetadata.Create(username, mobileNumber).Value,
            RegisteredAt = DateTime.UtcNow
        };
        var command = new CreateUserCommand
        {
            Username = username,
            MobileNumber = mobileNumber,
            TelegramId = telegramId,
            RegisteredAt = DateTime.UtcNow
        };
        await AddUsersToDatabase(user);

        // Act
        var result = await _sender.Send(command);

        // Assert
        result.IsFailed.Should().Be(true);
        result.Errors.Should().ContainSingle().Which.Message.Should().Be("User already exists");
    }

    [Fact]
    public async Task CreateUser_WhenUsernameIsInvalid_ShouldNotAddUserToDatabase()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Username = new('a', UserMetadata.MaxUsernameLength + 1),
            MobileNumber = "+1234567890",
            TelegramId = 1234567890,
            RegisteredAt = DateTime.UtcNow
        };

        // Act
        Func<Task> act = async () => await _sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateUser_WhenTelegramIdIsInvalid_ShouldNotAddUserToDatabase()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Username = "JohnDoe",
            MobileNumber = "+1234567890",
            TelegramId = 0,
            RegisteredAt = DateTime.UtcNow
        };

        // Act
        Func<Task> act = async () => await _sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateUser_WhenMobileNumberIsInvalid_ShouldNotAddUserToDatabase()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Username = "JohnDoe",
            MobileNumber = string.Empty,
            TelegramId = 1234567890,
            RegisteredAt = DateTime.UtcNow
        };

        // Act
        Func<Task> act = async () => await _sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateUser_WhenRegisteredAtIsInvalid_ShouldNotAddUserToDatabase()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Username = "JohnDoe",
            MobileNumber = "+1234567890",
            TelegramId = 1234567890,
            RegisteredAt = DateTime.MinValue
        };

        // Act
        Func<Task> act = async () => await _sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task GetUser_WhenUserExists_ShouldReturnUserFromDatabase()
    {
        // Arrange
        const int telegramId = 1234567890;
        const string username = "JohnDoe";
        const string mobileNumber = "+1234567890";

        var existingUser = new User
        {
            TelegramId = telegramId,
            Metadata = UserMetadata.Create(username, mobileNumber).Value,
            RegisteredAt = DateTime.UtcNow
        };
        var query = new GetUserQuery
        {
            UserTelegramId = telegramId
        };
        await AddUsersToDatabase(existingUser);

        // Act
        var user = await _sender.Send(query);

        // Assert
        user.IsSuccess.Should().Be(true);
        user.Value.Should().NotBeNull();
        user.Value.TelegramId.Should().Be(telegramId);
        user.Value.Username.Should().Be(username);
        user.Value.MobileNumber.Should().Be(mobileNumber);
    }

    [Fact]
    public async Task GetUser_WhenUserDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var query = new GetUserQuery
        {
            UserTelegramId = 1234567891
        };

        // Act
        var result = await _sender.Send(query);

        // Assert
        result.IsFailed.Should().Be(true);
        result.Errors.Should().ContainSingle().Which.Message.Should().Be("User not found");
    }

    [Fact]
    public async Task GetAllUsers_WhenUsersExist_ShouldReturnAllUsersFromDatabase()
    {
        // Arrange
        const int telegramId1 = 1234567890;
        const string username1 = "JohnDoe";
        const string mobileNumber1 = "+1234567890";

        const int telegramId2 = 1234567891;
        const string username2 = "JaneDoe";
        const string mobileNumber2 = "+1234567891";

        var user1 = new User
        {
            TelegramId = telegramId1,
            Metadata = UserMetadata.Create(username1, mobileNumber1).Value,
            RegisteredAt = DateTime.UtcNow
        };
        var user2 = new User
        {
            TelegramId = telegramId2,
            Metadata = UserMetadata.Create(username2, mobileNumber2).Value,
            RegisteredAt = DateTime.UtcNow
        };
        await AddUsersToDatabase(user1, user2);
        var query = new GetAllUsersQuery();

        // Act
        var users = await _sender.Send(query);

        // Assert
        users.Should().NotBeNullOrEmpty();
        users.Should().HaveCount(2);
        users.Should().ContainSingle(u =>
            u.TelegramId == telegramId1 && u.Username == username1 &&
            u.MobileNumber == mobileNumber1);
        users.Should().ContainSingle(u =>
            u.TelegramId == telegramId2 && u.Username == username2 &&
            u.MobileNumber == mobileNumber2);
    }
}