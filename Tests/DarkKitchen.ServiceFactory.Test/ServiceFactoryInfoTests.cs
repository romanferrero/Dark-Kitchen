namespace DarkKitchen.ServiceFactory.Test;

[TestClass]
public class ServiceFactoryInfoTests
{
    [TestMethod]
    public void GetAssemblyName_ReturnsServiceFactoryAssemblyName()
    {
        var name = ServiceFactoryInfo.GetAssemblyName();

        Assert.AreEqual("DarkKitchen.ServiceFactory", name);
    }

    [TestMethod]
    public void GetAssemblyName_NeverReturnsNull()
    {
        var name = ServiceFactoryInfo.GetAssemblyName();

        Assert.IsNotNull(name);
    }

    [TestMethod]
    public void GetAssemblyName_IsNotEmpty()
    {
        var name = ServiceFactoryInfo.GetAssemblyName();

        Assert.IsFalse(string.IsNullOrWhiteSpace(name));
    }

    [TestMethod]
    public void GetAssemblyName_MatchesReflection()
    {
        var expected = typeof(ServiceFactoryInfo)
            .Assembly
            .GetName()
            .Name;

        var result = ServiceFactoryInfo.GetAssemblyName();

        Assert.AreEqual(expected, result);
    }
}
