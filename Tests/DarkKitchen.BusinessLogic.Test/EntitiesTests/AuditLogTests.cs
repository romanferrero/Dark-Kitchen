using DarkKitchen.Domain.Entities;

namespace DarkKitchen.BusinessLogic.Test.EntitiesTests;

[TestClass]
public class AuditLogTests
{
    [TestMethod]
    public void Create_ValidData_SetsAllProperties()
    {
        var before = DateTime.UtcNow;

        var log = AuditLog.Create("PRODUCTO", 42, "Creación", "admin@darkkitchen.com");

        Assert.AreEqual("PRODUCTO", log.EntityName);
        Assert.AreEqual(42, log.EntityId);
        Assert.AreEqual("Creación", log.Description);
        Assert.AreEqual("admin@darkkitchen.com", log.ResponsibleUser);
        Assert.IsTrue(log.Timestamp >= before);
        Assert.IsTrue(log.Timestamp <= DateTime.UtcNow);
    }
}
