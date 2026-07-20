namespace NINJA.EShop.Catalog.API.Products.UpdateProduct
{
    public record UpdateProductCommand(Guid Id,string Name,string Description,string ImageFile,decimal Price,List<string> Category): ICommand<UpdateProductResult>;
    public record UpdateProductResult(bool IsSuccess);
    public class UpdateProductCommandValidator: AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Product Id is required");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Product Name is required");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Product Description is required");
            RuleFor(x => x.ImageFile).NotEmpty().WithMessage("Product ImageFile is required");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Product Price must be greater than 0");
            RuleFor(x => x.Category).NotEmpty().WithMessage("Product Category is required");
        }
    }

    public class UpdateProductCommandHandler(IDocumentSession session): ICommandHandler<UpdateProductCommand,UpdateProductResult>
    {
        public async Task<UpdateProductResult> Handle(UpdateProductCommand command,CancellationToken cancellationToken)
        {
            var product = await session.LoadAsync<Product>(command.Id,cancellationToken);
            if (product is null)
                throw new ProductNotFoundException(command.Id);
            product = command.Adapt<Product>();
            session.Update(product);
            await session.SaveChangesAsync(cancellationToken);
            return new UpdateProductResult(true);
        }
    }
}
