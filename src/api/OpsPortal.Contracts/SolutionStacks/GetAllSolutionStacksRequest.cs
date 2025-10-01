namespace OpsPortal.Contracts.SolutionStacks;

public class GetAllSolutionStacksRequest
{
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public string? SearchTerm { get; set; }

    public string? SortBy { get; set; }

    public bool SortDescending { get; set; } = false;
}
