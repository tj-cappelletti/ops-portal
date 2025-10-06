namespace OpsPortal.Contracts.SolutionStacks;

public record GetSolutionStackResponse(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    string? Category,
    string Status,
    string StatusSlug,
    string Owner,
    DateTime CreatedAt,
    DateTime UpdatedAt);
