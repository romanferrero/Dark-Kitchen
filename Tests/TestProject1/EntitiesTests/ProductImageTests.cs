using DarkKitchen.Domain.Entities;

namespace DarkKitchen.Domain.Test.EntitiesTests;

[TestClass]
public class ProductImageTests
{
    [TestMethod]
    public void SizeInKb_AtMaxValue_DoesNotThrow()
    {
        var image = new ProductImage { Url = "http://img.com/burg1.jpg", SizeInKb = 500 };

        Assert.AreEqual(500, image.SizeInKb);
    }

    [TestMethod]
    public void SizeInKb_ExceedsMaxValue_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new ProductImage { Url = "http://img.com/burg1.jpg", SizeInKb = 501 });
    }

    [TestMethod]
    public void SizeInKb_Zero_DoesNotThrow()
    {
        var image = new ProductImage { Url = "http://img.com/burg1.jpg", SizeInKb = 0 };

        Assert.AreEqual(0, image.SizeInKb);
    }

    [TestMethod]
    public void Url_Valid_AssignsCorrectly()
    {
        var image = new ProductImage { Url = "http://img.com/test.jpg", SizeInKb = 100 };

        Assert.AreEqual("http://img.com/test.jpg", image.Url);
    }
}
