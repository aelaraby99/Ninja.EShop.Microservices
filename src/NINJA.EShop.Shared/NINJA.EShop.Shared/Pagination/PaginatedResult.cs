namespace NINJA.EShop.Shared.Pagination;

public class PaginatedResult<TEntity>(int pageSize,int pageIndex,long count,IEnumerable<TEntity> data) where TEntity : class
{
    public int PageIndex { get; } = pageIndex;
    public int PageSize { get; } = pageSize;
    public long Count { get; } = count;
    public IEnumerable<TEntity> Data { get; } = data;
}