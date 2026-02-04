using BlazorUmbracoDashboard.Domain.Entities;

namespace BlazorUmbracoDashboard.Application.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
}
