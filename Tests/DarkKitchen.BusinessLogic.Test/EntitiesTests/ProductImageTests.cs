using DarkKitchen.Domain.Entities;

namespace DarkKitchen.BusinessLogic.Test.EntitiesTests;

[TestClass]
public class ProductImageTests
{
    [TestMethod]
    public void SizeInKb_AtMax_DoesNotThrow()
    {
        var image = new ProductImage { SizeInKb = 500 };

        Assert.AreEqual(500, image.SizeInKb);
    }

    [TestMethod]
    public void SizeInKb_BelowMax_DoesNotThrow()
    {
        var image = new ProductImage { SizeInKb = 250 };

        Assert.AreEqual(250, image.SizeInKb);
    }

    [TestMethod]
    public void SizeInKb_AboveMax_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new ProductImage { SizeInKb = 501 });
    }
}
