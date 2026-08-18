namespace NINJA.EShop.RazorClient.Models.Catalog;

public class ProductModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<string> Category { get; set; }
    public string Description { get; set; }
    public string ImageFile { get; set; }
    public decimal Price { get; set; }
}
// wrapper classes
public record GetProductsResponse(IEnumerable<ProductModel> Products);
public record GetProductByCategoryResponse(IEnumerable<ProductModel> Products);
public record GetProductByIdResponse(ProductModel Product);