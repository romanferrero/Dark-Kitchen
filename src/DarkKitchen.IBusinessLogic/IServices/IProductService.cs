using DarkKitchen.IBusinessLogic.DTOs.Entry.ProductDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.ProductDTOs;

namespace DarkKitchen.IBusinessLogic.IServices;

public interface IProductService
{
    ProductExitDto CreateProduct(ProductEntryDto dto, string responsibleUser);

    ProductExitDto UpdateProduct(int id, ProductEntryDto dto, string responsibleUser);

    List<ProductExitDto> GetProducts(string? line, List<string>? categories, string? name);
}
