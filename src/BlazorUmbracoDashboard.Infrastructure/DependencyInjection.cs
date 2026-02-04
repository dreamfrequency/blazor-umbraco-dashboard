using System.Text;
using BlazorUmbracoDashboard.Application.Interfaces;
using BlazorUmbracoDashboard.Domain.Common;
using BlazorUmbracoDashboard.Domain.Entities;
using BlazorUmbracoDashboard.Infrastructure.ApiClients;
using BlazorUmbracoDashboard.Infrastructure.Authentication;
using BlazorUmbracoDashboard.Infrastructure.Data;
using BlazorUmbracoDashboard.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace BlazorUmbracoDashboard.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // SQLite / EF Core
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")
                ?? "Data Source=dashboard.db"));

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IContentNodeRepository, ContentNodeRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // Umbraco API client
        services.AddHttpClient<IUmbracoApiClient, UmbracoApiClient>(client =>
        {
            var baseUrl = configuration["Umbraco:BaseUrl"] ?? "https://localhost:44300";
            client.BaseAddress = new Uri(baseUrl);

            var apiKey = configuration["Umbraco:ApiKey"];
            if (!string.IsNullOrEmpty(apiKey))
            {
                client.DefaultRequestHeaders.Add("Api-Key", apiKey);
            }
        });

        // JWT Authentication
        var jwtSettings = new JwtSettings();
        configuration.GetSection(JwtSettings.SectionName).Bind(jwtSettings);
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Secret.PadRight(32, '!')))
            };
        });

        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
