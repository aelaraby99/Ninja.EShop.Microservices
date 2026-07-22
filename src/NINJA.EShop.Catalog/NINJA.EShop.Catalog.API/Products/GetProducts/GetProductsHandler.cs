using Marten.Pagination;

namespace NINJA.EShop.Catalog.API.Products.GetProducts
{
    public record GeProductsQuery(int PageNumber = 1,int PageSize = 10): IQuery<GetProductsResult>;
    public record GetProductsResult(IEnumerable<Product> Products);
    public class GetProductsHandler(IDocumentSession session): IQueryHandler<GeProductsQuery,GetProductsResult>
    {
        public async Task<GetProductsResult> Handle(GeProductsQuery query,CancellationToken cancellationToken)
        {
            var products = await session.Query<Product>().ToPagedListAsync(query.PageNumber,query.PageSize,cancellationToken);
            return new GetProductsResult(products);
        }
    }
}