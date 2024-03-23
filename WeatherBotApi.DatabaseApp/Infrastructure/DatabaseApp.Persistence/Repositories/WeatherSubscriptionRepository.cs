using DatabaseApp.Domain.Models;
using DatabaseApp.Domain.Repositories;
using DatabaseApp.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace DatabaseApp.Persistence.Repositories;

public class WeatherSubscriptionRepository(IDatabaseContext context)
    : RepositoryBase<UserWeatherSubscription>(context), IWeatherSubscriptionRepository
{
    public Task<List<UserWeatherSubscription>> GetAllByUserId(int userId, CancellationToken cancellationToken) =>
        _context.UserWeatherSubscriptions
            .Include(subscription => subscription.WeatherDescription)
            .Where(subscription => subscription.UserId == userId)
            .ToListAsync(cancellationToken);
}