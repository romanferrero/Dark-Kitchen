using DarkKitchen.Domain.Entities;

namespace DarkKitchen.BusinessLogic.Test.EntitiesTests;

[TestClass]
public class ProductTests
{
    private const string ValidName = "Hamburguesa clasica";
    private const string ValidDescription = "Hamburguesa con queso y lechuga fresca";
    private const string ValidLine = "Combo burgers";
    private const string ValidCategory = "Parrilla";
    private const string ValidImage = "http://img.com/test.jpg|100";

    private static Product Build(string code = "BURG01") =>
        Product.Create(code, ValidName, 100m, ValidDescription, ValidLine, ValidCategory, ValidImage, true);

    [TestMethod]
    public void Create_CodeTooShort_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create("ABC", ValidName, 100m, ValidDescription, ValidLine, ValidCategory, ValidImage, true));
    }

    [TestMethod]
    public void Create_CodeTooLong_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create(new string('X', 21), ValidName, 100m, ValidDescription, ValidLine, ValidCategory, ValidImage, true));
    }

    [TestMethod]
    public void Create_NameTooShort_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create("BURG01", "Short", 100m, ValidDescription, ValidLine, ValidCategory, ValidImage, true));
    }

    [TestMethod]
    public void Create_NameTooLong_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create("BURG01", new string('X', 51), 100m, ValidDescription, ValidLine, ValidCategory, ValidImage, true));
    }

    [TestMethod]
    public void Create_DescriptionTooShort_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create("BURG01", ValidName, 100m, "Too short", ValidLine, ValidCategory, ValidImage, true));
    }

    [TestMethod]
    public void Create_DescriptionTooLong_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create("BURG01", ValidName, 100m, new string('X', 501), ValidLine, ValidCategory, ValidImage, true));
    }

    [TestMethod]
    public void Create_EmptyLine_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create("BURG01", ValidName, 100m, ValidDescription, string.Empty, ValidCategory, ValidImage, true));
    }

    [TestMethod]
    public void Create_EmptyCategory_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create("BURG01", ValidName, 100m, ValidDescription, ValidLine, string.Empty, ValidImage, true));
    }

    [TestMethod]
    public void Create_NoImages_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create("BURG01", ValidName, 100m, ValidDescription, ValidLine, ValidCategory, string.Empty, true));
    }

    [TestMethod]
    public void Create_TooManyImages_Throws()
    {
        var fourImages = "a.jpg|10,b.jpg|10,c.jpg|10,d.jpg|10";

        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create("BURG01", ValidName, 100m, ValidDescription, ValidLine, ValidCategory, fourImages, true));
    }

    [TestMethod]
    public void Create_NonJpgImage_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create("BURG01", ValidName, 100m, ValidDescription, ValidLine, ValidCategory, "image.png|100", true));
    }

    [TestMethod]
    public void Update_ChangesFields()
    {
        var product = Build();

        product.Update(
            "Nuevo nombre del producto",
            200m,
            "Nueva descripcion de longitud valida",
            "Otra linea",
            "Otra categoria",
            "nuevo.jpg|50",
            false);

        Assert.AreEqual("Nuevo nombre del producto", product.Name);
        Assert.AreEqual(200m, product.Price);
        Assert.IsFalse(product.Active);
    }
}
