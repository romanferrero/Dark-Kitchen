using DarkKitchen.Domain.Entities;

namespace DarkKitchen.Domain.Test.EntitiesTests;

[TestClass]
public class ProductTests
{
    private const decimal ValidPrice = 100;

    [TestMethod]
    public void Create_ValidData_ReturnsProduct()
    {
        var product = Product.Create(
            code: "BURG01",
            name: "Hamburguesa clasica",
            price: ValidPrice,
            description: "Hamburguesa con lechuga y tomate fresco",
            line: "Combo burgers",
            category: "Parrilla",
            images: "http://img.com/burg1.jpg",
            active: true);

        Assert.AreEqual("BURG01", product.Code);
        Assert.AreEqual("Hamburguesa clasica", product.Name);
        Assert.AreEqual(ValidPrice, product.Price);
        Assert.AreEqual(1, product.Images.Count);
    }

    [TestMethod]
    public void Create_CodeTooShort_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create(
                "AB",
                "Hamburguesa clasica",
                ValidPrice,
                "Hamburguesa con lechuga y tomate fresco",
                "Combo burgers",
                "Parrilla",
                "http://img.com/burg1.jpg",
                true));
    }

    [TestMethod]
    public void Create_CodeTooLong_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create(
                "ABCDEFGHIJ12345678901",
                "Hamburguesa clasica",
                ValidPrice,
                "Hamburguesa con lechuga y tomate fresco",
                "Combo burgers",
                "Parrilla",
                "http://img.com/burg1.jpg",
                true));
    }

    [TestMethod]
    public void Create_NameTooShort_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create(
                "BURG01",
                "Burguer",
                ValidPrice,
                "Hamburguesa con lechuga y tomate fresco",
                "Combo burgers",
                "Parrilla",
                "http://img.com/burg1.jpg",
                true));
    }

    [TestMethod]
    public void Create_NameTooLong_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create(
                "BURG01",
                "Hamburguesa clasica con extra queso cheddar y bacon ahumado demasiado largo",
                ValidPrice,
                "Hamburguesa con lechuga y tomate fresco",
                "Combo burgers",
                "Parrilla",
                "http://img.com/burg1.jpg",
                true));
    }

    [TestMethod]
    public void Create_DescriptionTooShort_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create(
                "BURG01",
                "Hamburguesa clasica",
                ValidPrice,
                "Corta",
                "Combo burgers",
                "Parrilla",
                "http://img.com/burg1.jpg",
                true));
    }

    [TestMethod]
    public void Create_DescriptionTooLong_ThrowsArgumentException()
    {
        var longDescription = new string('A', 501);

        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create(
                "BURG01",
                "Hamburguesa clasica",
                ValidPrice,
                longDescription,
                "Combo burgers",
                "Parrilla",
                "http://img.com/burg1.jpg",
                true));
    }

    [TestMethod]
    public void Create_NoImages_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create(
                "BURG01",
                "Hamburguesa clasica",
                ValidPrice,
                "Hamburguesa con lechuga y tomate fresco",
                "Combo burgers",
                "Parrilla",
                string.Empty,
                true));
    }

    [TestMethod]
    public void Create_TooManyImages_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create(
                "BURG01",
                "Hamburguesa clasica",
                ValidPrice,
                "Hamburguesa con lechuga y tomate fresco",
                "Combo burgers",
                "Parrilla",
                "http://img.com/1.jpg,http://img.com/2.jpg,http://img.com/3.jpg,http://img.com/4.jpg",
                true));
    }

    [TestMethod]
    public void Create_ImageNotJpg_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create(
                "BURG01",
                "Hamburguesa clasica",
                ValidPrice,
                "Hamburguesa con lechuga y tomate fresco",
                "Combo burgers",
                "Parrilla",
                "http://img.com/burg1.png",
                true));
    }

    [TestMethod]
    public void Create_ImageAtMaxSize_DoesNotThrow()
    {
        var product = Product.Create(
            "BURG01",
            "Hamburguesa clasica",
            ValidPrice,
            "Hamburguesa con lechuga y tomate fresco",
            "Combo burgers",
            "Parrilla",
            "http://img.com/burg1.jpg|500",
            true);

        Assert.IsNotNull(product);
        Assert.AreEqual(1, product.Images.Count);
    }

    [TestMethod]
    public void Create_EmptyLine_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create(
                "BURG01",
                "Hamburguesa clasica",
                100m,
                "Hamburguesa con lechuga y tomate fresco",
                string.Empty,
                "Parrilla",
                "http://img.com/burg1.jpg",
                true));
    }

    [TestMethod]
    public void Create_WhitespaceLine_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create(
                "BURG01",
                "Hamburguesa clasica",
                100m,
                "Hamburguesa con lechuga y tomate fresco",
                "   ",
                "Parrilla",
                "http://img.com/burg1.jpg",
                true));
    }

    [TestMethod]
    public void Create_NullLine_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create(
                "BURG01",
                "Hamburguesa clasica",
                100m,
                "Hamburguesa con lechuga y tomate fresco",
                null!,
                "Parrilla",
                "http://img.com/burg1.jpg",
                true));
    }
}
