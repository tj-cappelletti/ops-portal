using OpsPortal.Domain.Common.Auditing;

namespace OpsPortal.Domain.Entities;

public class SolutionStack : AuditableEntity
{
    // ReSharper disable once CollectionNeverUpdated.Local
    // EF Core requires a backing field for collections
    private readonly List<SolutionStackTag> _solutionStackTags = [];

    public string? Category { get; private set; }

    public string? Description { get; private set; }

    public string Name { get; private set; }

    public string Owner { get; private set; }

    public string Slug { get; private set; }

    public IReadOnlyCollection<SolutionStackTag> SolutionStackTags => _solutionStackTags.AsReadOnly();

    public SolutionStackStatus Status { get; private set; } = default!;


    public Guid StatusId { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // Constructor for EF Core
    // Disable nullable warning for this constructor
    private SolutionStack() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public static SolutionStack Create(string name, string description, string category, string owner, SolutionStackStatus status)
    {
        return new SolutionStack
        {
            Id = Guid.NewGuid(),
            Name = name,
            // TODO: Use a proper slug generator
            Slug = name.ToLowerInvariant().Replace(" ", "-"),
            Description = description,
            Category = category,
            Status = status,
            StatusId = status.Id,
            Owner = owner,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
