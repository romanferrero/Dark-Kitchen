using DarkKitchen.IBusinessLogic.DTOs.Entry.ProductDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.ProductDTOs;

namespace DarkKitchen.IBusinessLogic.IServices;

public interface IProductService
{
    ProductExitDTO CreateProduct(ProductEntryDto dto);

    ProductExitDTO UpdateProduct(string prodCode, ProductEntryDto dto);

    List<ProductExitDTO> GetProducts(string? line, List<string>? categories, string? name);
}
