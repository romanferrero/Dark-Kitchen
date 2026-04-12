using DarkKitchen.DataAccess;
using DarkKitchen.ServiceFactory;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddBusinessLogic();

builder.Services.AddDataAccess();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
