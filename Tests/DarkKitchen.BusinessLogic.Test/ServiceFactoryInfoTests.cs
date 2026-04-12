using DarkKitchen.ServiceFactory;

namespace DarkKitchen.BusinessLogic.Test;

[TestClass]
public class ServiceFactoryInfoTests
{
    [TestMethod]
    public void GetAssemblyName_ReturnsServiceFactoryAssemblyName()
    {
        var name = ServiceFactoryInfo.GetAssemblyName();

        Assert.AreEqual("DarkKitchen.ServiceFactory", name);
    }
}
