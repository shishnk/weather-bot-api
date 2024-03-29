using DatabaseApp.Domain.Repositories;
using DatabaseApp.Persistence.DatabaseContext;

namespace DatabaseApp.Persistence.UnitOfWorkContext;

public sealed class UnitOfWork(
    IDatabaseContext context,
    IUserRepository userRepository,
    IWeatherSubscriptionRepository userWeatherSubscriptionRepository) : IUnitOfWork
{
    private bool _disposed;

    public IUserRepository UserRepository => userRepository;
    public IWeatherSubscriptionRepository UserWeatherSubscriptionRepository => userWeatherSubscriptionRepository;

    ~UnitOfWork() => Dispose(false);

    public Task SaveDbChangesAsync(CancellationToken cancellationToken) =>
        context.SaveDbChangesAsync(cancellationToken);

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            context.DisposeResources();
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}