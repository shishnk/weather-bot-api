using DatabaseApp.Application.UserWeatherSubscriptions.Commands.CreateUserWeatherSubscription;
using DatabaseApp.Application.UserWeatherSubscriptions.Commands.DeleteUserWeatherSubscription;
using DatabaseApp.Application.UserWeatherSubscriptions.Commands.UpdateUserWeatherSubscription;
using DatabaseApp.Application.UserWeatherSubscriptions.Queries.GetWeatherSubscriptions;
using DatabaseApp.Domain.Models;
using DatabaseApp.Tests.BasicTestContext;
using FluentAssertions;
using FluentValidation;
using Xunit;

namespace DatabaseApp.Tests.WeatherSubscriptionTests;

public class WeatherSubscriptionTests(IntegrationWebAppFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task CreateWeatherSubscription_WithValidData_ShouldAddWeatherSubscriptionToDatabase()
    {
        // Arrange
        var command = new CreateUserWeatherSubscriptionCommand
        {
            TelegramUserId = 1234567890,
            Location = "London",
            ResendInterval = TimeSpan.FromHours(1)
        };
        var user = new User
        {
            TelegramId = 1234567890,
            Metadata = UserMetadata.Create("JohnDoe", "+1234567890").Value,
            RegisteredAt = DateTime.UtcNow
        };
        await AddUsersToDatabase(user);

        // Act
        var result = await _sender.Send(command);
        var weatherSubscription =
            await _unitOfWork.UserWeatherSubscriptionRepository.GetByUserTelegramIdAndLocationAsync(
                command.TelegramUserId, Location.Create(command.Location).Value, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().Be(true);
        weatherSubscription.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateWeatherSubscription_WhenUserDoesNotExist_ShouldNotAddWeatherSubscriptionToDatabase()
    {
        // Arrange
        var command = new CreateUserWeatherSubscriptionCommand
        {
            TelegramUserId = 1234567890,
            Location = "London",
            ResendInterval = TimeSpan.FromHours(1)
        };

        // Act
        var result = await _sender.Send(command);
        var weatherSubscription =
            await _unitOfWork.UserWeatherSubscriptionRepository.GetByUserTelegramIdAndLocationAsync(
                command.TelegramUserId, Location.Create(command.Location).Value, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().Be(false);
        result.Errors.Should().ContainSingle().Which.Message.Should().Be("User not found");
        weatherSubscription.Should().BeNull();
    }

    [Fact]
    public async Task CreateWeatherSubscription_WithInvalidLocation_ShouldNotAddWeatherSubscriptionToDatabase()
    {
        // Arrange
        var command = new CreateUserWeatherSubscriptionCommand
        {
            TelegramUserId = 1234567890,
            Location = string.Empty,
            ResendInterval = TimeSpan.FromHours(1)
        };
        var user = new User
        {
            TelegramId = 1234567890,
            Metadata = UserMetadata.Create("JohnDoe", "+1234567890").Value,
            RegisteredAt = DateTime.UtcNow
        };
        await AddUsersToDatabase(user);

        // Act
        Func<Task> act = async () => await _sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateWeatherSubscription_WithInvalidResendInterval_ShouldNotAddWeatherSubscriptionToDatabase()
    {
        // Arrange
        var command = new CreateUserWeatherSubscriptionCommand
        {
            TelegramUserId = 1234567890,
            Location = "London",
            ResendInterval = TimeSpan.Zero
        };
        var user = new User
        {
            TelegramId = 1234567890,
            Metadata = UserMetadata.Create("JohnDoe", "+1234567890").Value,
            RegisteredAt = DateTime.UtcNow
        };
        await AddUsersToDatabase(user);

        // Act
        Func<Task> act = async () => await _sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task
        CreateWeatherSubscription_WhenWeatherSubscriptionAlreadyExists_ShouldNotAddWeatherSubscriptionToDatabase()
    {
        // Arrange
        const string location = "London";

        var command = new CreateUserWeatherSubscriptionCommand
        {
            TelegramUserId = 1234567890,
            Location = location,
            ResendInterval = TimeSpan.FromHours(1)
        };
        var user = new User
        {
            TelegramId = 1234567890,
            Metadata = UserMetadata.Create("JohnDoe", "+1234567890").Value,
            RegisteredAt = DateTime.UtcNow
        };
        var weatherSubscription = new UserWeatherSubscription
        {
            User = user,
            Location = Location.Create(location).Value,
            ResendInterval = TimeSpan.FromHours(1)
        };
        await AddUsersToDatabase(user);
        await AddWeatherSubscriptionsToDatabase(weatherSubscription);

        // Act
        var result = await _sender.Send(command);

        // Assert
        result.IsFailed.Should().Be(true);
        result.Errors.Should().ContainSingle().Which.Message.Should().Be("Weather subscription already exists");
    }

    [Fact]
    public async Task CreateWeatherSubscription_WhenTelegramIdIsInvalid_ShouldNotAddWeatherSubscriptionToDatabase()
    {
        // Arrange
        var command = new CreateUserWeatherSubscriptionCommand
        {
            TelegramUserId = 0,
            Location = "London",
            ResendInterval = TimeSpan.FromHours(1)
        };

        // Act
        Func<Task> act = async () => await _sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateWeatherSubscription_WithValidData_ShouldUpdateWeatherSubscriptionInDatabase()
    {
        // Arrange
        const string location = "London";

        var user = new User
        {
            TelegramId = 1234567890,
            Metadata = UserMetadata.Create("JohnDoe", "+1234567890").Value,
            RegisteredAt = DateTime.UtcNow
        };
        var weatherSubscription = new UserWeatherSubscription
        {
            User = user,
            Location = Location.Create(location).Value,
            ResendInterval = TimeSpan.FromHours(1)
        };
        await AddUsersToDatabase(user);
        await AddWeatherSubscriptionsToDatabase(weatherSubscription);

        var command = new UpdateUserWeatherSubscriptionCommand
        {
            UserTelegramId = 1234567890,
            Location = location,
            ResendInterval = TimeSpan.FromHours(2)
        };

        // Act
        var result = await _sender.Send(command);
        var updatedWeatherSubscription =
            await _unitOfWork.UserWeatherSubscriptionRepository.GetByUserTelegramIdAndLocationAsync(
                command.UserTelegramId, Location.Create(command.Location).Value, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().Be(true);
        updatedWeatherSubscription.Should().NotBeNull();
        updatedWeatherSubscription!.ResendInterval.Should().Be(TimeSpan.FromHours(2));
    }

    [Fact]
    public async Task UpdateWeatherSubscription_WhenUserDoesNotExist_ShouldNotUpdateWeatherSubscriptionInDatabase()
    {
        // Arrange
        const string location = "London";

        var user = new User
        {
            TelegramId = 1234567890,
            Metadata = UserMetadata.Create("JohnDoe", "+1234567890").Value,
            RegisteredAt = DateTime.UtcNow
        };
        var weatherSubscription = new UserWeatherSubscription
        {
            User = user,
            Location = Location.Create(location).Value,
            ResendInterval = TimeSpan.FromHours(1)
        };
        await AddUsersToDatabase(user);
        await AddWeatherSubscriptionsToDatabase(weatherSubscription);

        var command = new UpdateUserWeatherSubscriptionCommand
        {
            UserTelegramId = 1234567891,
            Location = location,
            ResendInterval = TimeSpan.FromHours(2)
        };

        // Act
        var result = await _sender.Send(command);
        var updatedWeatherSubscription =
            await _unitOfWork.UserWeatherSubscriptionRepository.GetByUserTelegramIdAndLocationAsync(
                command.UserTelegramId, Location.Create(command.Location).Value, CancellationToken.None);

        // Assert
        result.IsFailed.Should().Be(true);
        result.Errors.Should().ContainSingle().Which.Message.Should().Be("User not found");
        updatedWeatherSubscription.Should().BeNull();
    }

    [Fact]
    public async Task
        UpdateWeatherSubscription_WhenWeatherSubscriptionDoesNotExist_ShouldNotUpdateWeatherSubscriptionInDatabase()
    {
        // Arrange
        var user = new User
        {
            TelegramId = 1234567890,
            Metadata = UserMetadata.Create("JohnDoe", "+1234567890").Value,
            RegisteredAt = DateTime.UtcNow
        };
        var weatherSubscription = new UserWeatherSubscription
        {
            User = user,
            Location = Location.Create("London").Value,
            ResendInterval = TimeSpan.FromHours(1)
        };
        await AddUsersToDatabase(user);
        await AddWeatherSubscriptionsToDatabase(weatherSubscription);

        var command = new UpdateUserWeatherSubscriptionCommand
        {
            UserTelegramId = 1234567890,
            Location = "Paris",
            ResendInterval = TimeSpan.FromHours(2)
        };

        // Act
        var result = await _sender.Send(command);
        var updatedWeatherSubscription =
            await _unitOfWork.UserWeatherSubscriptionRepository.GetByUserTelegramIdAndLocationAsync(
                command.UserTelegramId, Location.Create(command.Location).Value, CancellationToken.None);

        // Assert
        result.IsFailed.Should().Be(true);
        result.Errors.Should().ContainSingle().Which.Message.Should().Be("Subscription not found");
        updatedWeatherSubscription.Should().BeNull();
    }

    [Fact]
    public async Task UpdateWeatherSubscription_WithInvalidLocation_ShouldNotUpdateWeatherSubscriptionInDatabase()
    {
        // Arrange
        var user = new User
        {
            TelegramId = 1234567890,
            Metadata = UserMetadata.Create("JohnDoe", "+1234567890").Value,
            RegisteredAt = DateTime.UtcNow
        };
        var weatherSubscription = new UserWeatherSubscription
        {
            User = user,
            Location = Location.Create("London").Value,
            ResendInterval = TimeSpan.FromHours(1)
        };
        await AddUsersToDatabase(user);
        await AddWeatherSubscriptionsToDatabase(weatherSubscription);

        var command = new UpdateUserWeatherSubscriptionCommand
        {
            UserTelegramId = 1234567890,
            Location = string.Empty,
            ResendInterval = TimeSpan.FromHours(2)
        };

        // Act
        Func<Task> act = async () => await _sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateWeatherSubscription_WithInvalidResendInterval_ShouldNotUpdateWeatherSubscriptionInDatabase()
    {
        // Arrange
        var user = new User
        {
            TelegramId = 1234567890,
            Metadata = UserMetadata.Create("JohnDoe", "+1234567890").Value,
            RegisteredAt = DateTime.UtcNow
        };
        var weatherSubscription = new UserWeatherSubscription
        {
            User = user,
            Location = Location.Create("London").Value,
            ResendInterval = TimeSpan.FromHours(1)
        };
        await AddUsersToDatabase(user);
        await AddWeatherSubscriptionsToDatabase(weatherSubscription);

        var command = new UpdateUserWeatherSubscriptionCommand
        {
            UserTelegramId = 1234567890,
            Location = "London",
            ResendInterval = TimeSpan.Zero
        };

        // Act
        Func<Task> act = async () => await _sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateWeatherSubscription_WhenTelegramIdIsInvalid_ShouldNotUpdateWeatherSubscriptionInDatabase()
    {
        // Arrange
        var command = new UpdateUserWeatherSubscriptionCommand
        {
            UserTelegramId = 0,
            Location = "London",
            ResendInterval = TimeSpan.FromHours(1)
        };

        // Act
        Func<Task> act = async () => await _sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task DeleteWeatherSubscription_WithValidData_ShouldDeleteWeatherSubscriptionFromDatabase()
    {
        // Arrange
        const string location = "London";

        var user = new User
        {
            TelegramId = 1234567890,
            Metadata = UserMetadata.Create("JohnDoe", "+1234567890").Value,
            RegisteredAt = DateTime.UtcNow
        };
        var command = new DeleteUserWeatherSubscriptionCommand
        {
            UserTelegramId = 1234567890,
            Location = location
        };
        var weatherSubscription = new UserWeatherSubscription
        {
            User = user,
            Location = Location.Create(location).Value,
            ResendInterval = TimeSpan.FromHours(1)
        };
        await AddUsersToDatabase(user);
        await AddWeatherSubscriptionsToDatabase(weatherSubscription);

        // Act
        var result = await _sender.Send(command);
        var deletedWeatherSubscription =
            await _unitOfWork.UserWeatherSubscriptionRepository.GetByUserTelegramIdAndLocationAsync(
                command.UserTelegramId, Location.Create(command.Location).Value, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().Be(true);
        deletedWeatherSubscription.Should().BeNull();
    }

    [Fact]
    public async Task
        DeleteWeatherSubscription_WhenSubscriptionDoesNotExist_ShouldNotDeleteWeatherSubscriptionFromDatabase()
    {
        // Arrange
        const string location = "London";

        var user = new User
        {
            TelegramId = 1234567890,
            Metadata = UserMetadata.Create("JohnDoe", "+1234567890").Value,
            RegisteredAt = DateTime.UtcNow
        };
        var command = new DeleteUserWeatherSubscriptionCommand
        {
            UserTelegramId = 1234567890,
            Location = location
        };
        await AddUsersToDatabase(user);

        // Act
        var result = await _sender.Send(command);

        // Assert
        result.IsFailed.Should().Be(true);
        result.Errors.Should().ContainSingle().Which.Message.Should().Be("Subscription not found");
    }

    [Fact]
    public async Task DeleteWeatherSubscription_WithInvalidLocation_ShouldNotDeleteWeatherSubscriptionFromDatabase()
    {
        // Arrange
        var command = new DeleteUserWeatherSubscriptionCommand
        {
            UserTelegramId = 1234567890,
            Location = string.Empty
        };

        // Act
        Func<Task> act = async () => await _sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task DeleteWeatherSubscription_WhenTelegramIdIsInvalid_ShouldNotDeleteWeatherSubscriptionFromDatabase()
    {
        // Arrange
        var command = new DeleteUserWeatherSubscriptionCommand
        {
            UserTelegramId = 0,
            Location = "London"
        };

        // Act
        Func<Task> act = async () => await _sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
    
    [Fact]
    public async Task GetWeatherSubscriptions_WithValidData_ShouldReturnWeatherSubscriptionsFromDatabase()
    {
        // Arrange
        var user = new User
        {
            TelegramId = 1234567890,
            Metadata = UserMetadata.Create("JohnDoe", "+1234567890").Value,
            RegisteredAt = DateTime.UtcNow
        };
        var weatherSubscription1 = new UserWeatherSubscription
        {
            User = user,
            Location = Location.Create("London").Value,
            ResendInterval = TimeSpan.FromHours(1)
        };
        var weatherSubscription2 = new UserWeatherSubscription
        {
            User = user,
            Location = Location.Create("Paris").Value,
            ResendInterval = TimeSpan.FromHours(2)
        };
        var query = new GetUserWeatherSubscriptionsQuery
        {
            UserTelegramId = 1234567890
        };
        await AddUsersToDatabase(user);
        await AddWeatherSubscriptionsToDatabase(weatherSubscription1, weatherSubscription2);

        // Act
        var weatherSubscriptions = await _sender.Send(query);

        // Assert
        weatherSubscriptions.Should().NotBeEmpty();
        weatherSubscriptions.Should().HaveCount(2);
    }
}