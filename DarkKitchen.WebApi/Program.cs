using DarkKitchen.IBusinessLogic;
using DarkKitchen.ServiceFactory;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options => options.Filters.Add<CustomExceptionFilter>());

builder.Services.AddBusinessLogic();

builder.Services.AddDataAccess(builder.Configuration.GetConnectionString("DefaultConnection")!);

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
