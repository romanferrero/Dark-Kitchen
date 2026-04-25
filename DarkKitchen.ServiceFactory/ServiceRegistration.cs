using DarkKitchen.BusinessLogic.Discounts;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.BusinessLogic.ShippingCosts;
using DarkKitchen.BusinessLogic.Validators;
using DarkKitchen.DataAccess.Context;
using DarkKitchen.DataAccess.Repositories;
using DarkKitchen.Domain.Entities;
using DarkKitchen.IBusinessLogic.IDiscounts;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.IBusinessLogic.IShippingCost;
using DarkKitchen.IBusinessLogic.IValidators;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DarkKitchen.ServiceFactory;

public static class ServiceRegistration
{
    public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IPromotionService, PromotionService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IReportService, ReportService>();

        services.AddScoped<IJwtTokenService, JwtTokenService>();

        services.AddScoped<IShippingCostCalculatorFactory, ShippingCostCalculatorFactory>();
        services.AddScoped<IShippingCostCalculator, ShippingCostExpressCalculator>();
        services.AddScoped<IShippingCostCalculator, ShippingCost24hsCalculator>();

        services.AddScoped<IPhoneValidator, UruguayanPhoneValidator>();
        services.AddScoped<IDiscountCalculator, BestDiscountCalculator>();
        return services;
    }

    public static IServiceCollection AddDataAccess(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));
        services.AddScoped<IRepository<User>, Repository<User>>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IPromotionRepository, PromotionRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        return services;
    }
}
