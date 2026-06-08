using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.ServiceFactory;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options => options.Filters.Add<CustomExceptionFilter>());
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddBusinessLogic();
builder.Services.AddDataAccess(builder.Configuration.GetConnectionString("DefaultConnection")!);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("AllowAngular");

// app.UseHttpsRedirection();
app.MapControllers();

app.Run();
