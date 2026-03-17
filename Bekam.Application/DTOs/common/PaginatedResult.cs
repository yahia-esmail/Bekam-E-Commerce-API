namespace Bekam.Application.DTOs.common;

public sealed class PaginatedResult<T>(
    IReadOnlyList<T> items,
    int pageNumber,
    int pageSize,
    int totalCount)
{
    public int PageNumber { get; } = pageNumber;
    public int TotalCount { get; } = totalCount; // count after spec
    public int TotalPages { get; } = (int)Math.Ceiling(totalCount / (double)pageSize);// total after spec
    public IReadOnlyList<T> Items { get; } = items;

    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}