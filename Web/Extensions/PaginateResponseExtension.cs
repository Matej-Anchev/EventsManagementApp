namespace Web.Extensions;

public static class PaginateResponseExtension
{
    public static PaginatedResponse<TResult> ToPaginatedResponse<T, TResult>(
        this PaginatedResponse<T> result,
        Func<T, TResult> mappingFunction)
    {
        return new PaginatedResponse<TResult>
        {
            Items = result.Items.Select(mappingFunction).ToList(),
            TotalCount = result.TotalCount,
            PageSize = result.PageSize,
            CurrentPage = result.CurrentPage
        }
    }
}