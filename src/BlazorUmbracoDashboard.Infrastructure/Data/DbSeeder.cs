using System.Security.Cryptography;
using BlazorUmbracoDashboard.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazorUmbracoDashboard.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.ContentNodes.AnyAsync())
            return;

        var now = DateTime.UtcNow;

        // --- Users ---
        var users = new List<User>
        {
            new()
            {
                Username = "admin",
                Email = "admin@blazordashboard.com",
                PasswordHash = HashPassword("Admin123!"),
                Role = "Admin",
                CreatedAt = now.AddDays(-90)
            },
            new()
            {
                Username = "editor.jones",
                Email = "sarah.jones@blazordashboard.com",
                PasswordHash = HashPassword("Editor123!"),
                Role = "Editor",
                CreatedAt = now.AddDays(-75)
            },
            new()
            {
                Username = "editor.clark",
                Email = "mike.clark@blazordashboard.com",
                PasswordHash = HashPassword("Editor123!"),
                Role = "Editor",
                CreatedAt = now.AddDays(-60)
            },
            new()
            {
                Username = "contributor.lee",
                Email = "anna.lee@blazordashboard.com",
                PasswordHash = HashPassword("Contrib123!"),
                Role = "Contributor",
                CreatedAt = now.AddDays(-45)
            },
            new()
            {
                Username = "viewer.patel",
                Email = "raj.patel@blazordashboard.com",
                PasswordHash = HashPassword("Viewer123!"),
                Role = "Viewer",
                CreatedAt = now.AddDays(-30)
            }
        };

        context.Users.AddRange(users);

        // --- Content Nodes ---
        // Root pages
        var homepage = NewNode("Homepage", "Page", "Published", now.AddDays(-85));
        var blog = NewNode("Blog", "Page", "Published", now.AddDays(-80));
        var newsSection = NewNode("Company News", "Page", "Published", now.AddDays(-78));
        var aboutUs = NewNode("About Us", "Page", "Published", now.AddDays(-75));
        var contactPage = NewNode("Contact", "Page", "Published", now.AddDays(-70));
        var careersPage = NewNode("Careers", "Page", "Draft", now.AddDays(-10));

        // Blog posts (children of Blog)
        var blogPost1 = NewChild("Getting Started with Blazor", "BlogPost", "Published", now.AddDays(-60), blog);
        var blogPost2 = NewChild("Understanding Clean Architecture in .NET", "BlogPost", "Published", now.AddDays(-45), blog);
        var blogPost3 = NewChild("Entity Framework Core Best Practices", "BlogPost", "Published", now.AddDays(-30), blog);
        var blogPost4 = NewChild("Building Real-Time Dashboards with SignalR", "BlogPost", "Published", now.AddDays(-18), blog);
        var blogPost5 = NewChild("Migrating from MVC to Blazor Server", "BlogPost", "Draft", now.AddDays(-5), blog);
        var blogPost6 = NewChild("Securing Your Blazor App with JWT", "BlogPost", "Draft", now.AddDays(-2), blog);

        // News items (children of Company News)
        var news1 = NewChild("Company News Q4 2023 Recap", "NewsItem", "Published", now.AddDays(-70), newsSection);
        var news2 = NewChild("Q1 2024 Product Roadmap Announced", "NewsItem", "Published", now.AddDays(-50), newsSection);
        var news3 = NewChild("New Partnership with Umbraco", "NewsItem", "Published", now.AddDays(-25), newsSection);
        var news4 = NewChild("Team Expansion — Engineering Hiring Update", "NewsItem", "Draft", now.AddDays(-7), newsSection);

        // Articles (children of About Us)
        var article1 = NewChild("Our Mission & Values", "Article", "Published", now.AddDays(-74), aboutUs);
        var article2 = NewChild("Meet the Leadership Team", "Article", "Published", now.AddDays(-65), aboutUs);
        var article3 = NewChild("Sustainability Commitment 2024", "Article", "Draft", now.AddDays(-12), aboutUs);

        var allNodes = new List<ContentNode>
        {
            homepage, blog, newsSection, aboutUs, contactPage, careersPage,
            blogPost1, blogPost2, blogPost3, blogPost4, blogPost5, blogPost6,
            news1, news2, news3, news4,
            article1, article2, article3
        };

        context.ContentNodes.AddRange(allNodes);

        // --- Audit Logs ---
        var auditLogs = new List<AuditLog>
        {
            NewLog("Created", "ContentNode", homepage.Id, "admin",
                "Created homepage", now.AddDays(-85)),
            NewLog("Created", "ContentNode", blog.Id, "admin",
                "Created blog section", now.AddDays(-80)),
            NewLog("Created", "ContentNode", blogPost1.Id, "editor.jones",
                "Published introductory Blazor article", now.AddDays(-60)),
            NewLog("Updated", "ContentNode", homepage.Id, "editor.jones",
                "Updated hero banner and meta description", now.AddDays(-55)),
            NewLog("Created", "ContentNode", blogPost2.Id, "editor.clark",
                "Added Clean Architecture article", now.AddDays(-45)),
            NewLog("Created", "ContentNode", news2.Id, "admin",
                "Published Q1 2024 roadmap announcement", now.AddDays(-50)),
            NewLog("Created", "ContentNode", blogPost3.Id, "editor.jones",
                "Added EF Core best practices guide", now.AddDays(-30)),
            NewLog("Updated", "ContentNode", aboutUs.Id, "editor.clark",
                "Refreshed About Us page copy", now.AddDays(-28)),
            NewLog("Created", "ContentNode", news3.Id, "admin",
                "Announced Umbraco partnership", now.AddDays(-25)),
            NewLog("Created", "ContentNode", blogPost4.Id, "contributor.lee",
                "Drafted SignalR dashboard article", now.AddDays(-18)),
            NewLog("Updated", "ContentNode", blogPost4.Id, "editor.jones",
                "Reviewed and published SignalR article", now.AddDays(-16)),
            NewLog("Created", "ContentNode", careersPage.Id, "admin",
                "Started drafting careers page", now.AddDays(-10)),
            NewLog("Created", "User", users[3].Id, "admin",
                "Added contributor account for Anna Lee", now.AddDays(-45)),
            NewLog("Created", "ContentNode", blogPost5.Id, "contributor.lee",
                "Started MVC to Blazor migration guide", now.AddDays(-5)),
            NewLog("Created", "ContentNode", blogPost6.Id, "editor.clark",
                "Began JWT security article draft", now.AddDays(-2))
        };

        context.AuditLogs.AddRange(auditLogs);

        await context.SaveChangesAsync();
    }

    private static ContentNode NewNode(string name, string contentType, string status, DateTime createdAt)
    {
        return new ContentNode
        {
            Name = name,
            ContentType = contentType,
            Status = status,
            CreatedAt = createdAt,
            UpdatedAt = status == "Published" ? createdAt.AddHours(1) : null
        };
    }

    private static ContentNode NewChild(string name, string contentType, string status, DateTime createdAt, ContentNode parent)
    {
        return new ContentNode
        {
            Name = name,
            ContentType = contentType,
            Status = status,
            ParentId = parent.Id,
            CreatedAt = createdAt,
            UpdatedAt = status == "Published" ? createdAt.AddHours(1) : null
        };
    }

    private static AuditLog NewLog(string action, string entityType, Guid entityId, string performedBy, string details, DateTime createdAt)
    {
        return new AuditLog
        {
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            PerformedBy = performedBy,
            Details = details,
            CreatedAt = createdAt
        };
    }

    private static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }
}
