using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.DataAccess.Context;
using DarkKitchen.DataAccess.Repositories;
using DarkKitchen.Domain.Entities;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DarkKitchen.ServiceFactory.Test;

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
        AssertScoped<IDeliveryTypeService, DeliveryTypeService>(services);
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
        AssertScoped<IRepository<DeliveryType>, Repository<DeliveryType>>(services);
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
