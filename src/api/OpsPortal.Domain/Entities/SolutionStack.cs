namespace OpsPortal.Domain.Entities;

public class SolutionStack
{
    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string Slug { get; private set; }

    public string Description { get; private set; }

    public string Category { get; private set; }

    public string Status { get; private set; }

    public string Owner { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    // Constructor for EF Core
    private SolutionStack() { }

    public SolutionStack(string name, string description, string category, string owner)
    {
        Id = Guid.NewGuid();
        Name = name;
        Slug = name.ToLowerInvariant().Replace(" ", "-");
        Description = description;
        Category = category;
        Status = "Active";
        Owner = owner;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
