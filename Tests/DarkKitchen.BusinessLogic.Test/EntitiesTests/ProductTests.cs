using DarkKitchen.Domain.Entities;

namespace DarkKitchen.BusinessLogic.Test.EntitiesTests;

[TestClass]
public class ProductTests
{
    private const string ValidName = "Hamburguesa clasica";
    private const string ValidDescription = "Hamburguesa con queso y lechuga fresca";
    private const string ValidLine = "Combo burgers";
    private const string ValidCategory = "Parrilla";
    private const string ValidImage = "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQ==";

    private static Product Build(string code = "BURG01") =>
        Product.Create(new CreateProductParamsDto(code, ValidName, 100m, ValidDescription, ValidLine, ValidCategory, ValidImage, true));

    [TestMethod]
    public void Create_CodeTooShort_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create(new CreateProductParamsDto("ABC", ValidName, 100m, ValidDescription, ValidLine, ValidCategory, ValidImage, true)));
    }

    [TestMethod]
    public void Create_CodeTooLong_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create(new CreateProductParamsDto(new string('X', 21), ValidName, 100m, ValidDescription, ValidLine, ValidCategory, ValidImage, true)));
    }

    [TestMethod]
    public void Create_NameTooShort_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create(new CreateProductParamsDto("BURG01", "Short", 100m, ValidDescription, ValidLine, ValidCategory, ValidImage, true)));
    }

    [TestMethod]
    public void Create_NameTooLong_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create(new CreateProductParamsDto("BURG01", new string('X', 51), 100m, ValidDescription, ValidLine, ValidCategory, ValidImage, true)));
    }

    [TestMethod]
    public void Create_DescriptionTooShort_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create(new CreateProductParamsDto("BURG01", ValidName, 100m, "Too short", ValidLine, ValidCategory, ValidImage, true)));
    }

    [TestMethod]
    public void Create_DescriptionTooLong_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create(new CreateProductParamsDto("BURG01", ValidName, 100m, new string('X', 501), ValidLine, ValidCategory, ValidImage, true)));
    }

    [TestMethod]
    public void Create_EmptyLine_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create(new CreateProductParamsDto("BURG01", ValidName, 100m, ValidDescription, string.Empty, ValidCategory, ValidImage, true)));
    }

    [TestMethod]
    public void Create_EmptyCategory_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create(new CreateProductParamsDto("BURG01", ValidName, 100m, ValidDescription, ValidLine, string.Empty, ValidImage, true)));
    }

    [TestMethod]
    public void Create_NoImages_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create(new CreateProductParamsDto("BURG01", ValidName, 100m, ValidDescription, ValidLine, ValidCategory, string.Empty, true)));
    }

    [TestMethod]
    public void Create_TooManyImages_Throws()
    {
        var fourImages = string.Join('\n', ValidImage, ValidImage, ValidImage, ValidImage);

        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create(new CreateProductParamsDto("BURG01", ValidName, 100m, ValidDescription, ValidLine, ValidCategory, fourImages, true)));
    }

    [TestMethod]
    public void Create_NonJpgImage_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Product.Create(new CreateProductParamsDto("BURG01", ValidName, 100m, ValidDescription, ValidLine, ValidCategory, "data:image/png;base64,iVBORw0KGgo=", true)));
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
            ValidImage,
            false);

        Assert.AreEqual("Nuevo nombre del producto", product.Name);
        Assert.AreEqual(200m, product.Price);
        Assert.IsFalse(product.Active);
    }
}
