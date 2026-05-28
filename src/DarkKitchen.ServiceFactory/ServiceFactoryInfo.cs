namespace DarkKitchen.ServiceFactory;

public static class ServiceFactoryInfo
{
    public static string GetAssemblyName()
    {
        return typeof(ServiceFactoryInfo).Assembly.GetName().Name!;
    }
}
