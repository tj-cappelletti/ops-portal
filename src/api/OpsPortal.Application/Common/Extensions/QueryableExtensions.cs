using Microsoft.EntityFrameworkCore;
using OpsPortal.Contracts.Common;

namespace OpsPortal.Application.Common.Extensions;

public static class QueryableExtensions
{
    public static async Task<PaginatedResponse<T>> ToPaginatedResponseAsync<T>(
        this IQueryable<T> source,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var count = await source.CountAsync(cancellationToken);
        var items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PaginatedResponse<T>.Create(items, pageNumber, pageSize, count);
    }

    public static async Task<PaginatedResponse<TResult>> ToPaginatedResponseAsync<TSource, TResult>(
        this IQueryable<TSource> source,
        int pageNumber,
        int pageSize,
        Func<IQueryable<TSource>, IQueryable<TResult>> mapper,
        CancellationToken cancellationToken = default)
    {
        var count = await source.CountAsync(cancellationToken);

        var paginatedQuery = source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        var mappedQuery = mapper(paginatedQuery);
        var items = await mappedQuery.ToListAsync(cancellationToken);

        return PaginatedResponse<TResult>.Create(items, pageNumber, pageSize, count);
    }
}
