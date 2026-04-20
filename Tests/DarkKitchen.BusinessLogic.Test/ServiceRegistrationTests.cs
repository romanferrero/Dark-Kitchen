using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.BusinessLogic.ShippingCosts;
using DarkKitchen.DataAccess;
using DarkKitchen.DataAccess.Context;
using DarkKitchen.DataAccess.Repositories;
using DarkKitchen.Domain;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.IBusinessLogic.IShippingCost;
using DarkKitchen.IDataAccess;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;
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
        AssertScoped<IUserService, UserService>(services);
        AssertScoped<IProductService, ProductService>(services);
        AssertScoped<IPromotionService, PromotionService>(services);
        AssertScoped<IOrderService, OrderService>(services);

        AssertScoped<IShippingCostCalculator, ShippingCostExpressCalculator>(services);
        AssertScoped<IShippingCostCalculator, ShippingCost24hsCalculator>(services);
    }

    [TestMethod]
    public void AddDataAccess_RegistersExpectedServicesAndDbContext()
    {
        IServiceCollection services = new ServiceCollection();

        var returned = services.AddDataAccess(
            "Server=(localdb)\\mssqllocaldb;Database=DarkKitchenTests;Trusted_Connection=True;");

        Assert.AreSame(services, returned);

        Assert.IsTrue(services.Any(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>)));

        AssertScoped<IRepository<User>, Repository<User>>(services);
        AssertScoped<IProductRepository, ProductRepository>(services);
        AssertScoped<IPromotionRepository, PromotionRepository>(services);
        AssertScoped<IOrderRepository, OrderRepository>(services);
    }

    private static void AssertScoped<TService, TImplementation>(IServiceCollection services)
    {
        var descriptor = services.FirstOrDefault(d =>
            d.ServiceType == typeof(TService) &&
            d.ImplementationType == typeof(TImplementation));

        Assert.IsNotNull(descriptor,
            $"No se encontró el registro de {typeof(TService).Name} con implementación {typeof(TImplementation).Name}");

        Assert.AreEqual(ServiceLifetime.Scoped, descriptor.Lifetime);
    }
}
