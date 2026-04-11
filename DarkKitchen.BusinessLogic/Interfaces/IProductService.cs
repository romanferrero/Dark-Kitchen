namespace DarkKitchen.BusinessLogic.Interfaces;

public interface IProductService
{
    string CreateProduct(string code, string name, string description,
                         string line, string category, string images, bool active);

    string UpdateProduct(string code, string name, string description,
                         string line, string category, string images, bool active);

    List<Product> GetProducts(string? line, List<string>? categories, string? name);
}
