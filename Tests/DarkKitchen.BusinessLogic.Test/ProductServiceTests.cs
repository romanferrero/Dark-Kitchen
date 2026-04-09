using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain;
using Moq;

namespace DarkKitchen.BusinessLogic.Test;

[TestClass]
public class ProductServiceTests
{
    private Mock<IProductRepository> _productRepoMock = null!;
    private ProductService _productService = null!;

    [TestInitialize]
    public void Initialize()
    {
        _productRepoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        _productService = new ProductService(_productRepoMock.Object);
    }

    [TestMethod]
    public void GetProducts_WithLineFilter_DelegatesToRepository()
    {
        var storedProducts = new List<Product>
        {
            new Product
            {
                Code = "BURG01",
                Name = "Hamburguesa clasica",
                Price = 250m,
                Line = "Combo burgers",
                Category = "Parrilla",
                ImageUrls = ["http://img.com/burg1.jpg"],
                Active = true,
            }
        };

        _productRepoMock
            .Setup(r => r.GetFiltered("Combo burgers", null, null))
            .Returns(storedProducts);

        var result = _productService.GetProducts("Combo burgers", null, null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("BURG01", result[0].Code);
        _productRepoMock.Verify(r => r.GetFiltered("Combo burgers", null, null), Times.Once);
    }
}
