namespace NINJA.EShop.Catalog.API.Products.GetProductById
{
    public record GetProductByIdQuery(Guid Id): IQuery<GetProductByIdResult>;
    public record GetProductByIdResult(Product Product);
    public class GetProductByIdQueryHandler(IDocumentSession session,ILogger<GetProductByIdQueryHandler> logger): IQueryHandler<GetProductByIdQuery,GetProductByIdResult>
    {
        public async Task<GetProductByIdResult> Handle(GetProductByIdQuery request,CancellationToken cancellationToken)
        {
            logger.LogInformation("Handling GetProductByIdQuery for Id: {Id}",request.Id);
            var product = await session.LoadAsync<Product>(request.Id,cancellationToken);
            if (product is null)
            {
                logger.LogWarning("Product with Id: {Id} not found",request.Id);
                throw new ProductNotFoundException();
            }
            return new GetProductByIdResult(product);
        }
    }
}