using DatabaseApp.Domain.Models;
using DatabaseApp.Domain.Repositories;
using DatabaseApp.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace DatabaseApp.Persistence.Repositories;

public class UserRepository(IDatabaseContext context)
    : RepositoryBase<User>(context), IUserRepository
{
    public Task<User?> GetByTelegramIdAsync(int telegramId, CancellationToken cancellationToken) =>
        _context.Users
            .FirstOrDefaultAsync(u => u.TelegramId == telegramId, cancellationToken);

    public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken) =>
        await _context.Users.ToListAsync(cancellationToken);
}