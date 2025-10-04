namespace OpsPortal.Domain.Entities;

public class Tag : Entity
{
    public string Name { get; private set; }
    
    public string Slug { get; private set; }
    
    public string? Description { get; private set; }
    
    public string? Color { get; private set; } // For UI display (hex color)
    
    public TagType Type { get; private set; }
    
    public bool IsSystem { get; private set; } // Protected tags that can't be deleted
    
    public bool IsActive { get; private set; }
    
    public Guid? ParentTagId { get; private set; }
    
    public virtual Tag? ParentTag { get; private set; }

    private readonly List<SolutionStackTag> _solutionStackTags = new();
    
    public IReadOnlyCollection<SolutionStackTag> SolutionStackTags => _solutionStackTags.AsReadOnly();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // Constructor for EF Core
    // Disable nullable warning for this constructor
    private Tag() { } // EF Core
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public static Tag Create(string name, TagType type, string? description = null, string? color = null)
    {
        return new Tag
        {
            Id = Guid.NewGuid(),
            Name = name,
            Slug = GenerateSlug(name),
            Description = description,
            Type = type,
            Color = color ?? GenerateDefaultColor(type),
            IsSystem = false,
            IsActive = true
        };
    }

    private static string GenerateSlug(string name)
    {
        return name.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("_", "-")
            .Replace(".", "-");
    }

    private static string GenerateDefaultColor(TagType type)
    {
        return type switch
        {
            TagType.Category => "#0066CC",     // Blue
            TagType.Technology => "#00AA00",   // Green
            TagType.Environment => "#FF6600",  // Orange
            TagType.Team => "#9933CC",        // Purple
            TagType.Custom => "#666666",      // Gray
            _ => "#333333"
        };
    }
}

public enum TagType
{
    Category,      // Traditional categories (Web, Database, etc.)
    Technology,    // Tech stack tags (Docker, Kubernetes, .NET)
    Environment,   // Deployment environments (Production, Staging)
    Team,          // Ownership/team tags
    Compliance,    // Regulatory/compliance tags (HIPAA, PCI)
    Custom        // User-defined tags
}