namespace NINJA.EShop.Catalog.API.Products.GetProducts
{
    public record GeProductsQuery(): IQuery<GetProductsResult>;
    public record GetProductsResult(IEnumerable<Product> Products);
    public class GetProductsHandler(IDocumentSession session,ILogger<GetProductsHandler> logger): IQueryHandler<GeProductsQuery,GetProductsResult>
    {
        public async Task<GetProductsResult> Handle(GeProductsQuery query,CancellationToken cancellationToken)
        {
            logger.LogInformation("GetProductsHandler.Handle called with {@Query}",query);
            var products = await session.Query<Product>().ToListAsync(cancellationToken);
            return new GetProductsResult(products);
        }
    }
}