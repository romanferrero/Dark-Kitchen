namespace DarkKitchen.BusinessLogic.Interfaces;

public interface IProductService
{
    string CreateProduct(int code, string name, string description,
                         string line, string category, string images, bool active);

    string UpdateProduct(int code, string name, string description,
                         string line, string category, string images, bool active);
}
