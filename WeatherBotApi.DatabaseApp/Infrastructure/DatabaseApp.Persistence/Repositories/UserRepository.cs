using DatabaseApp.Domain.Models;
using DatabaseApp.Domain.Repositories;
using DatabaseApp.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace DatabaseApp.Persistence.Repositories;

public class UserRepository(IDatabaseContext context)
    : RepositoryBase<User>(context), IUserRepository
{
    public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken) =>
        await _context.Users.ToListAsync(cancellationToken: cancellationToken);
}