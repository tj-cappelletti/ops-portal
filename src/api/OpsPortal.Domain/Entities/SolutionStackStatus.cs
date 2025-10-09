using OpsPortal.Domain.Common.Auditing;
using OpsPortal.Domain.Common.Utilities;
using OpsPortal.Domain.Constants;

namespace OpsPortal.Domain.Entities;

public class SolutionStackStatus : AuditableEntity
{
    public string? Color { get; private set; }

    public string? Description { get; private set; }

    public int DisplayOrder { get; private set; }

    public bool IsActive { get; private set; }

    public bool IsSystemStatus { get; private set; }

    public string Name { get; private set; }


    public string Slug { get; private set; }

    // Optional: navigation property for reverse relationship
    public ICollection<SolutionStack> SolutionStacks { get; private set; } = new List<SolutionStack>();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // Constructor for EF Core
    // Disable nullable warning for this constructor
    private SolutionStackStatus() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public static SolutionStackStatus CreateSystemStatus(SeedStatusData seedStatusData)
    {
        return new SolutionStackStatus
        {
            Color = seedStatusData.Color,
            CreatedAt = SystemDefaults.BaseDateTime,
            CreatedBy = SystemDefaults.SystemUsers.System.Identifier,
            CreatedById = SystemDefaults.SystemUsers.System.Id,
            Description = seedStatusData.Description,
            DisplayOrder = seedStatusData.DisplayOrder,
            Id = seedStatusData.Id,
            IsActive = true,
            IsSystemStatus = true,
            Name = seedStatusData.Name,
            Slug = SlugGenerator.Generate(seedStatusData.Name),
            UpdatedAt = SystemDefaults.BaseDateTime,
            UpdatedBy = SystemDefaults.SystemUsers.System.Identifier,
            UpdatedById = SystemDefaults.SystemUsers.System.Id,
        };
    }
}

public record SeedStatusData(
    Guid Id,
    string Name,
    string Description,
    string Color,
    int DisplayOrder);
