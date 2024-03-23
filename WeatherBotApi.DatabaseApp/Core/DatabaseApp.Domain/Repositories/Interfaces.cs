using DatabaseApp.Domain.Models;

namespace DatabaseApp.Domain.Repositories;

public interface IUnitOfWork : IDisposable
{
    IUserRepository UserRepository { get; }
    IWeatherDescriptionRepository WeatherDescriptionRepository { get; }
    IWeatherSubscriptionRepository UserWeatherSubscriptionRepository { get; }

    Task SaveDbChangesAsync(CancellationToken cancellationToken);
}

public interface IRepository
{
    Task SaveDbChangesAsync(CancellationToken cancellationToken);
}

public interface IUserRepository : IRepository
{
    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(User user, CancellationToken cancellationToken);
    void Update(User user);
    void Delete(User user);
}

public interface IWeatherDescriptionRepository : IRepository
{
    Task<WeatherDescription?> GetByLocationAsync(string location, CancellationToken cancellationToken);
    Task<WeatherDescription?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(WeatherDescription weatherDescription, CancellationToken cancellationToken);
    void Update(WeatherDescription weatherDescription);
    void Delete(WeatherDescription weatherDescription);
}

public interface IWeatherSubscriptionRepository : IRepository
{
    Task<List<UserWeatherSubscription>> GetAllByUserId(int userId, CancellationToken cancellationToken);
    Task<UserWeatherSubscription?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(UserWeatherSubscription weatherSubscription, CancellationToken cancellationToken);
    void Update(UserWeatherSubscription weatherSubscription);
    void Delete(UserWeatherSubscription weatherSubscription);
}