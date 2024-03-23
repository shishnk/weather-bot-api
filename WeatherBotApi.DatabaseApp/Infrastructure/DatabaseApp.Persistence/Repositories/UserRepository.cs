using DatabaseApp.Domain.Models;
using DatabaseApp.Domain.Repositories;
using DatabaseApp.Persistence.DatabaseContext;

namespace DatabaseApp.Persistence.Repositories;

public class UserRepository(IDatabaseContext context)
    : RepositoryBase<User>(context), IUserRepository;