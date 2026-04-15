using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.DataAccess;
using DarkKitchen.DataAccess.Repositories;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
using DarkKitchen.ServiceFactory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DarkKitchen.BusinessLogic.Test;

[TestClass]
public class ServiceRegistrationTests
{
    [TestMethod]
    public void AddBusinessLogic_RegistersExpectedScopedServices()
    {
        IServiceCollection services = new ServiceCollection();

        var returned = services.AddBusinessLogic();

        Assert.AreSame(services, returned);
        AssertScoped<IAuthService, AuthService>(services);
        AssertScoped<IClientService, ClientService>(services);
        AssertScoped<IProductService, ProductService>(services);
        AssertScoped<IPromotionService, PromotionService>(services);
        AssertScoped<IOrderService, OrderService>(services);
        AssertScoped<IShippingCostCalculator, ShippingCostExpressCalculator>(services);
    }

    [TestMethod]
    public void AddDataAccess_RegistersExpectedServicesAndDbContext()
    {
        IServiceCollection services = new ServiceCollection();

        var returned = services.AddDataAccess("Server=(localdb)\\mssqllocaldb;Database=DarkKitchenTests;Trusted_Connection=True;");

        Assert.AreSame(services, returned);
        Assert.IsTrue(services.Any(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>)));
        AssertScoped<IUserRepository, UserRepository>(services);
        AssertScoped<IProductRepository, ProductRepository>(services);
        AssertScoped<IPromotionRepository, PromotionRepository>(services);
        AssertScoped<IOrderRepository, OrderRepository>(services);
    }

    private static void AssertScoped<TService, TImplementation>(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(TService));

        Assert.IsNotNull(descriptor);
        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
        Assert.AreEqual(typeof(TImplementation), descriptor.ImplementationType);
    }
}
