using DarkKitchen.ServiceFactory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddBusinessLogic();

builder.Services.AddDataAccess(builder.Configuration.GetConnectionString("DefaultConnection")!);

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
