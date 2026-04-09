using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.DataAccess;
using DarkKitchen.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IClientService, ClientService>();

builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
