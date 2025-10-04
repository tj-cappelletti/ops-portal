namespace OpsPortal.Domain.Entities;

public class SolutionStackStatus : Entity
{
    public string Name { get; private set; }

    public string? Description { get; private set; }

    public string Slug { get; private set; }

    // Optional: navigation property for reverse relationship
    public ICollection<SolutionStack> SolutionStacks { get; private set; } = new List<SolutionStack>();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // Constructor for EF Core
    // Disable nullable warning for this constructor
    private SolutionStackStatus() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public SolutionStackStatus(string name, string description)
    {
        Id = Guid.NewGuid();
        Description = description;
        Name = name;
        // TODO: Use a proper slug generator
        Slug = name.ToLowerInvariant().Replace(" ", "-");
    }
}
