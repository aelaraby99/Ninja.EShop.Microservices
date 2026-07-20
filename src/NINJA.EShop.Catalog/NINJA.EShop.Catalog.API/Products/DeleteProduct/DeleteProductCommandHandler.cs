namespace NINJA.EShop.Catalog.API.Products.DeleteProduct
{
    public record DeleteProductCommand(Guid Id): ICommand<DeleteProductResult>;
    public record DeleteProductResult(bool IsSuccess);
    internal class DeleteProductCommandHandler(IDocumentSession session,ILogger<DeleteProductCommandHandler> logger): ICommandHandler<DeleteProductCommand,DeleteProductResult>
    {
        public async Task<DeleteProductResult> Handle(DeleteProductCommand command,CancellationToken cancellationToken)
        {
            logger.LogInformation("Deleting product with ID: {ProductId}",command.Id);
            var product = await session.LoadAsync<Product>(command.Id);
            if (product == null)
                throw new ProductNotFoundException();
            session.Delete(product);
            //session.Delete(command.Id);
            await session.SaveChangesAsync(cancellationToken);
            return new DeleteProductResult(true);
        }
    }
}