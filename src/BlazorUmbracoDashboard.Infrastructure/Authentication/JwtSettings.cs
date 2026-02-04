namespace BlazorUmbracoDashboard.Infrastructure.Authentication;

public class JwtSettings
{
    public const string SectionName = "Jwt";
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = "BlazorUmbracoDashboard";
    public string Audience { get; set; } = "BlazorUmbracoDashboard";
    public int ExpirationInMinutes { get; set; } = 60;
}
