namespace OpsPortal.Contracts.SolutionStacks;

public record CreateSolutionStackRequest(
    string Name,
    string Slug,
    string? Description,
    string? Category,
    string Status,
    string Owner);
