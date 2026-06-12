namespace DarkKitchen.WebApi.Models;

public static class PagedResult
{
    public const int DefaultPageSize = 10;

    public const int MaxPageSize = 100;

    public static PagedResultResponseModel<T> Create<T>(IReadOnlyList<T> source, int? pageNumber, int? pageSize)
    {
        // Sin pageSize explícito se devuelve todo en una sola página (no se trunca).
        if(pageSize is not > 0)
        {
            return new PagedResultResponseModel<T>
            {
                Items = source,
                PageNumber = 1,
                PageSize = source.Count,
                TotalCount = source.Count
            };
        }

        var size = Math.Min(pageSize.Value, MaxPageSize);
        var number = pageNumber is > 0 ? pageNumber.Value : 1;

        var items = source
            .Skip((number - 1) * size)
            .Take(size)
            .ToList();

        return new PagedResultResponseModel<T>
        {
            Items = items,
            PageNumber = number,
            PageSize = size,
            TotalCount = source.Count
        };
    }
}
