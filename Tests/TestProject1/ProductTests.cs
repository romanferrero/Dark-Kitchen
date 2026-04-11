namespace DarkKitchen.Domain.Test;

[TestClass]
public class ProductTests
{
    [TestMethod]
    public void Create_ValidData_ReturnsProduct()
    {
        var product = Product.Create(
            code: "BURG01",
            name: "Hamburguesa clasica",
            description: "Hamburguesa con lechuga y tomate fresco",
            line: "Combo burgers",
            category: "Parrilla",
            images: "http://img.com/burg1.jpg",
            active: true);

        Assert.AreEqual("BURG01", product.Code);
        Assert.AreEqual("Hamburguesa clasica", product.Name);
    }

    [TestMethod]
    public void Create_CodeTooShort_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create(
                code: "AB",
                name: "Hamburguesa clasica",
                description: "Hamburguesa con lechuga y tomate fresco",
                line: "Combo burgers",
                category: "Parrilla",
                images: "http://img.com/burg1.jpg",
                active: true));
    }

    [TestMethod]
    public void Create_CodeTooLong_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create(
                code: "ABCDEFGHIJ12345678901",
                name: "Hamburguesa clasica",
                description: "Hamburguesa con lechuga y tomate fresco",
                line: "Combo burgers",
                category: "Parrilla",
                images: "http://img.com/burg1.jpg",
                active: true));
    }

    [TestMethod]
    public void Create_NameTooShort_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create(
                code: "BURG01",
                name: "Burguer",
                description: "Hamburguesa con lechuga y tomate fresco",
                line: "Combo burgers",
                category: "Parrilla",
                images: "http://img.com/burg1.jpg",
                active: true));
    }

    [TestMethod]
    public void Create_NameTooLong_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create(
                code: "BURG01",
                name: "Hamburguesa clasica con extra queso cheddar y bacon ahumado",
                description: "Hamburguesa con lechuga y tomate fresco",
                line: "Combo burgers",
                category: "Parrilla",
                images: "http://img.com/burg1.jpg",
                active: true));
    }

    [TestMethod]
    public void Create_DescriptionTooShort_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create(
                code: "BURG01",
                name: "Hamburguesa clasica",
                description: "Corta",
                line: "Combo burgers",
                category: "Parrilla",
                images: "http://img.com/burg1.jpg",
                active: true));
    }

    [TestMethod]
    public void Create_DescriptionTooLong_ThrowsArgumentException()
    {
        var longDescription = new string('A', 501);

        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create(
                code: "BURG01",
                name: "Hamburguesa clasica",
                description: longDescription,
                line: "Combo burgers",
                category: "Parrilla",
                images: "http://img.com/burg1.jpg",
                active: true));
    }

    [TestMethod]
    public void Create_NoImages_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create(
                code: "BURG01",
                name: "Hamburguesa clasica",
                description: "Hamburguesa con lechuga y tomate fresco",
                line: "Combo burgers",
                category: "Parrilla",
                images: string.Empty,
                active: true));
    }

    [TestMethod]
    public void Create_TooManyImages_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create(
                code: "BURG01",
                name: "Hamburguesa clasica",
                description: "Hamburguesa con lechuga y tomate fresco",
                line: "Combo burgers",
                category: "Parrilla",
                images: "http://img.com/1.jpg,http://img.com/2.jpg,http://img.com/3.jpg,http://img.com/4.jpg",
                active: true));
    }

    [TestMethod]
    public void Create_ImageNotJpg_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create(
                code: "BURG01",
                name: "Hamburguesa clasica",
                description: "Hamburguesa con lechuga y tomate fresco",
                line: "Combo burgers",
                category: "Parrilla",
                images: "http://img.com/burg1.png",
                active: true));
    }

    [TestMethod]
    public void Create_ImageTooLarge_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create(
                code: "BURG01",
                name: "Hamburguesa clasica",
                description: "Hamburguesa con lechuga y tomate fresco",
                line: "Combo burgers",
                category: "Parrilla",
                images: "http://img.com/burg1.jpg|600",
                active: true));
    }

    [TestMethod]
    public void Create_ImageAtMaxSize_DoesNotThrow()
    {
        var product = Product.Create(
            code: "BURG01",
            name: "Hamburguesa clasica",
            description: "Hamburguesa con lechuga y tomate fresco",
            line: "Combo burgers",
            category: "Parrilla",
            images: "http://img.com/burg1.jpg|500",
            active: true);

        Assert.IsNotNull(product);
    }
}
