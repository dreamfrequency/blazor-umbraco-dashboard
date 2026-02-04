using BlazorUmbracoDashboard.Application.DTOs;

namespace BlazorUmbracoDashboard.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResultDto?> LoginAsync(LoginDto login, CancellationToken cancellationToken = default);
    Task<AuthResultDto?> RegisterAsync(RegisterDto register, CancellationToken cancellationToken = default);
}
