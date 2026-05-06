using Domain.Dto;
using Web.Response;

namespace Web.Extensions;

public static class PaginateResponseExtension
{
    public static PaginateResponse<TResult> ToPaginatedResponse<T, TResult>(
        this PaginatedResult<T> result,
        Func<T, TResult> mappingFunction)
    {
        return new PaginateResponse<TResult>
        {
            Items = result.Items.Select(mappingFunction).ToList(),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalPages = result.TotalPages
        };
    }
}