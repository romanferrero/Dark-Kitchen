using DarkKitchen.Domain.Entities;

namespace DarkKitchen.BusinessLogic.Test.EntitiesTests;

[TestClass]
public class OrderProductTests
{
    [TestMethod]
    public void Quantity_PositiveValue_IsAccepted()
    {
        var op = new OrderProduct { Quantity = 3 };

        Assert.AreEqual(3, op.Quantity);
    }

    [TestMethod]
    public void Quantity_Zero_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new OrderProduct { Quantity = 0 });
    }

    [TestMethod]
    public void Quantity_Negative_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new OrderProduct { Quantity = -5 });
    }
}
