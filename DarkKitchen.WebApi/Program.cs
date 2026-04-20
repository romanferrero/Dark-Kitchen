using DarkKitchen.IBusinessLogic;
using DarkKitchen.ServiceFactory;
using DarkKitchen.WebApi.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options => options.Filters.Add<CustomExceptionFilter>());

builder.Services.AddBusinessLogic();

builder.Services.AddDataAccess(builder.Configuration.GetConnectionString("DefaultConnection")!);

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
