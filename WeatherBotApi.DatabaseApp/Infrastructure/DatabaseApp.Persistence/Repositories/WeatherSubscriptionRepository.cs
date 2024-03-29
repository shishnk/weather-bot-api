using DatabaseApp.Domain.Models;
using DatabaseApp.Domain.Repositories;
using DatabaseApp.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace DatabaseApp.Persistence.Repositories;

public class WeatherSubscriptionRepository(IDatabaseContext context)
    : RepositoryBase<UserWeatherSubscription>(context), IWeatherSubscriptionRepository
{
    public Task<List<UserWeatherSubscription>> GetAllByUserTelegramId(int userTelegramId,
        CancellationToken cancellationToken) =>
        _context.UserWeatherSubscriptions
            .Include(s => s.User)
            .Where(s => s.User.TelegramId == userTelegramId)
            .ToListAsync(cancellationToken);

    public Task<UserWeatherSubscription?> GetByUserTelegramIdAndLocationAsync(int userTelegramId, Location location,
        CancellationToken cancellationToken) =>
        _context.UserWeatherSubscriptions
            .Include(s => s.User)
            .FirstOrDefaultAsync(s =>
                s.User.TelegramId == userTelegramId && s.Location.Value == location.Value, cancellationToken);
}