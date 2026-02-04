using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BlazorUmbracoDashboard.Application.DTOs;
using BlazorUmbracoDashboard.Application.Interfaces;
using BlazorUmbracoDashboard.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BlazorUmbracoDashboard.Infrastructure.Authentication;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    private readonly JwtSettings _jwtSettings;

    public AuthService(IUserRepository userRepo, IOptions<JwtSettings> jwtSettings)
    {
        _userRepo = userRepo;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResultDto?> LoginAsync(LoginDto login, CancellationToken cancellationToken = default)
    {
        var user = await _userRepo.GetByUsernameAsync(login.Username, cancellationToken);
        if (user is null || !VerifyPassword(login.Password, user.PasswordHash))
        {
            return new AuthResultDto { Success = false, Error = "Invalid username or password" };
        }

        var token = GenerateToken(user);
        return new AuthResultDto
        {
            Success = true,
            Token = token,
            Username = user.Username,
            Role = user.Role
        };
    }

    public async Task<AuthResultDto?> RegisterAsync(RegisterDto register, CancellationToken cancellationToken = default)
    {
        var existing = await _userRepo.GetByUsernameAsync(register.Username, cancellationToken);
        if (existing is not null)
        {
            return new AuthResultDto { Success = false, Error = "Username already exists" };
        }

        var user = new User
        {
            Username = register.Username,
            Email = register.Email,
            PasswordHash = HashPassword(register.Password),
            Role = "User"
        };

        await _userRepo.AddAsync(user, cancellationToken);

        var token = GenerateToken(user);
        return new AuthResultDto
        {
            Success = true,
            Token = token,
            Username = user.Username,
            Role = user.Role
        };
    }

    private string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    private static bool VerifyPassword(string password, string passwordHash)
    {
        var parts = passwordHash.Split('.');
        if (parts.Length != 2) return false;

        var salt = Convert.FromBase64String(parts[0]);
        var storedHash = Convert.FromBase64String(parts[1]);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);

        return CryptographicOperations.FixedTimeEquals(hash, storedHash);
    }
}
