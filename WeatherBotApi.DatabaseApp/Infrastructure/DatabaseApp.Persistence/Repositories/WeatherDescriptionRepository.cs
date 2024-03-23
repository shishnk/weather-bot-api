using DatabaseApp.Domain.Models;
using DatabaseApp.Domain.Repositories;
using DatabaseApp.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace DatabaseApp.Persistence.Repositories;

public class WeatherDescriptionRepository(IDatabaseContext context)
    : RepositoryBase<WeatherDescription>(context), IWeatherDescriptionRepository
{
    public Task<WeatherDescription?> GetByLocationAsync(string location, CancellationToken cancellationToken) =>
        _context.WeatherDescriptions
            .Where(wd => wd.Location == location)
            .FirstOrDefaultAsync(cancellationToken);
}