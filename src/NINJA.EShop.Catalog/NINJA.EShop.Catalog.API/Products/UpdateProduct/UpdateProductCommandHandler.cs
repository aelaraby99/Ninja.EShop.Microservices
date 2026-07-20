namespace NINJA.EShop.Catalog.API.Products.UpdateProduct
{
    public record UpdateProductCommand(Guid Id,string Name,string Description,string ImageFile,decimal Price,List<string> Category): ICommand<UpdateProductResult>;
    public record UpdateProductResult(bool IsSuccess);

    public class UpdateProductCommandHandler(IDocumentSession session,ILogger<UpdateProductCommandHandler> logger): ICommandHandler<UpdateProductCommand,UpdateProductResult>
    {
        public async Task<UpdateProductResult> Handle(UpdateProductCommand command,CancellationToken cancellationToken)
        {
            logger.LogInformation("Handling UpdateProductCommand for Product Id: {ProductId}",command.Id);
            var product = await session.LoadAsync<Product>(command.Id,cancellationToken);
            if (product is null)
            {
                logger.LogWarning("Product with Id: {ProductId} not found",command.Id);
                throw new ProductNotFoundException();
            }
            product = command.Adapt<Product>();
            session.Update(product);
            await session.SaveChangesAsync(cancellationToken);
            return new UpdateProductResult(true);
        }
    }
}
