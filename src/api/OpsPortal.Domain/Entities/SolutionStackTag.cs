namespace OpsPortal.Domain.Entities;

public class SolutionStackTag
{
    public virtual SolutionStack SolutionStack { get; private set; } = null!;

    public Guid SolutionStackId { get; private set; }

    public virtual Tag Tag { get; private set; } = null!;

    // Optional: Track who added the tag and when
    public DateTime TaggedAt { get; private set; }

    public string? TaggedBy { get; private set; }

    public Guid TagId { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // Constructor for EF Core
    // Disable nullable warning for this constructor
    private SolutionStackTag() { } // EF Core
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public static SolutionStackTag Create(Guid solutionStackId, Guid tagId, string taggedBy)
    {
        return new SolutionStackTag
        {
            SolutionStackId = solutionStackId,
            TagId = tagId,
            TaggedAt = DateTime.UtcNow,
            TaggedBy = taggedBy
        };
    }
}
