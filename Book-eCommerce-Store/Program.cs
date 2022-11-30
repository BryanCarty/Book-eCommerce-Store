global using Book_eCommerce_Store.Models;
using Book_eCommerce_Store.Data;
using Book_eCommerce_Store.Services.ObserverService;
using Book_eCommerce_Store.Services.OrdersService;
using Book_eCommerce_Store.Services.ProductsService;
using Book_eCommerce_Store.Services.ProductsService.Factory;
using Book_eCommerce_Store.Services.UsersService;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DataContext>(options =>{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); 
    options.EnableSensitiveDataLogging(true);
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddScoped<IProductFactory, ProductFactory>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<BooksService>()
    .AddScoped<IProductsService, BooksService>(s => s.GetService<BooksService>());
builder.Services.AddScoped<StationaryService>()
    .AddScoped<IProductsService, StationaryService>(s => s.GetService<StationaryService>());
builder.Services.AddScoped<IOrdersService, OrdersService>();
builder.Services.AddScoped<IObserverService, ObserverService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
