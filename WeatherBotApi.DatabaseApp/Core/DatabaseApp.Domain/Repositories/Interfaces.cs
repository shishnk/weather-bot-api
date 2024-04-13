using DatabaseApp.Domain.Models;

namespace DatabaseApp.Domain.Repositories;

public interface IUnitOfWork : IDisposable
{
    IUserRepository UserRepository { get; }
    IWeatherSubscriptionRepository UserWeatherSubscriptionRepository { get; }

    Task SaveDbChangesAsync(CancellationToken cancellationToken);
}

public interface IRepository
{
    Task SaveDbChangesAsync(CancellationToken cancellationToken);
}

public interface IUserRepository : IRepository
{
    Task<User?> GetByTelegramIdAsync(long telegramId, CancellationToken cancellationToken);
    Task AddAsync(User user, CancellationToken cancellationToken);
    Task<List<User>> GetAllAsync(CancellationToken cancellationToken);
}

public interface IWeatherSubscriptionRepository : IRepository
{
    Task<List<UserWeatherSubscription>> GetAllByUserTelegramId(long userTelegramId, CancellationToken cancellationToken);

    Task<UserWeatherSubscription?> GetByUserTelegramIdAndLocationAsync(long userTelegramId, Location location,
        CancellationToken cancellationToken);

    Task AddAsync(UserWeatherSubscription weatherSubscription, CancellationToken cancellationToken);
    void Update(UserWeatherSubscription weatherSubscription);
    void Delete(UserWeatherSubscription weatherSubscription);
}