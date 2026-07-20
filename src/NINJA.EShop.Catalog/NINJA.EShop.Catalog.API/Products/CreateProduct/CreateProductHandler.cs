namespace NINJA.EShop.Catalog.API.Products.CreateProduct
{
    public record CreateProductCommand(string Name,List<string> Category,string Description,string ImageFile,decimal Price)
        : ICommand<CreateProductResult>;
    public record CreateProductResult(Guid Id);
    public class CreateProductValidator: AbstractValidator<CreateProductCommand>
    {
        public CreateProductValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Category).NotEmpty();
            RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
            RuleFor(x => x.ImageFile).NotEmpty();
            RuleFor(x => x.Price).GreaterThan(0);
        }
    }
    internal class CreateProductHandler(IDocumentSession session): ICommandHandler<CreateProductCommand,CreateProductResult>
    {
        public async Task<CreateProductResult> Handle(CreateProductCommand command,CancellationToken cancellationToken)
        {
            var product = command.Adapt<Product>();
            session.Store(product);
            await session.SaveChangesAsync(cancellationToken);
            return new CreateProductResult(product.Id);
        }
    }
}