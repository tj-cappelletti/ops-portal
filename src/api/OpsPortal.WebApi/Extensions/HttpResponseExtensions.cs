using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using OpsPortal.Contracts.Responses;

namespace OpsPortal.WebApi.Extensions;

public static class HttpResponseExtensions
{
    public static void AddPaginationHeaders<T>(
        this HttpResponse response,
        PaginatedResponse<T> paginatedResponse,
        HttpRequest request)
    {
        // Add X-Pagination header
        var metadata = new
        {
            paginatedResponse.TotalCount,
            paginatedResponse.PageSize,
            paginatedResponse.PageNumber,
            paginatedResponse.TotalPages,
            paginatedResponse.HasNextPage,
            paginatedResponse.HasPreviousPage
        };

        response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));

        // Build Link header
        var links = new List<string>();

        if (paginatedResponse.HasPreviousPage) links.Add(CreateLinkHeader(request, paginatedResponse.PageNumber - 1, paginatedResponse.PageSize, "prev"));

        if (paginatedResponse.HasNextPage) links.Add(CreateLinkHeader(request, paginatedResponse.PageNumber + 1, paginatedResponse.PageSize, "next"));

        if (links.Any()) response.Headers.Add("Link", string.Join(", ", links));
    }

    private static string CreateLinkHeader(HttpRequest request, int pageNumber, int pageSize, string rel)
    {
        var baseUrl = $"{request.Scheme}://{request.Host}{request.Path}";
        
        var queryParams = QueryHelpers.ParseQuery(request.QueryString.Value);
        queryParams["pageNumber"] = pageNumber.ToString();
        queryParams["pageSize"] = pageSize.ToString();

        var url = QueryHelpers.AddQueryString(baseUrl, queryParams.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.ToString())!);

        return $"<{url}>; rel=\"{rel}\"";
    }
}
