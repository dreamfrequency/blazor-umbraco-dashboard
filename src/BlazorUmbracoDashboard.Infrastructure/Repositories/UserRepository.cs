using BlazorUmbracoDashboard.Application.Interfaces;
using BlazorUmbracoDashboard.Domain.Entities;
using BlazorUmbracoDashboard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorUmbracoDashboard.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }
}
