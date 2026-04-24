using DarkKitchen.IBusinessLogic.DTOs.Exit.ProductDTOs;

namespace DarkKitchen.IBusinessLogic.IServices;

public interface IProductService
{
    ProductExitDTO CreateProduct(string code, string name, string description,
                         string line, string category, string images, bool active);

    ProductExitDTO UpdateProduct(string code, string name, string description,
                         string line, string category, string images, bool active);

    List<ProductExitDTO> GetProducts(string? line, List<string>? categories, string? name);
}
