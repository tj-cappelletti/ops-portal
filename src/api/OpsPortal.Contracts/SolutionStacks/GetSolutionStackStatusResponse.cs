namespace OpsPortal.Contracts.SolutionStacks;

public record GetSolutionStackStatusResponse(
    Guid Id,
    string Name,
    string Slug,
    string? Description
);
