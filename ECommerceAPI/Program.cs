using ECommerceAPI.Data;
using ECommerceAPI.Helpers;
using ECommerceAPI.Model.Entities;
using ECommerceAPI.Repositories.Implementations;
using ECommerceAPI.Repositories.Interfaces;
using ECommerceAPI.Services.Implementations;
using ECommerceAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection")??"",
    new MySqlServerVersion(new Version(8,0,33))
));

builder.Services.AddIdentity<UserModel, RoleModel>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IUserRepository,UserRepository>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<SeedHelper>();

// Register repositories
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();

// Register services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartService, CartService>();

builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Seed data if necessary
    using (var scope = app.Services.CreateScope())
    {
        var seedHelper = scope.ServiceProvider.GetRequiredService<SeedHelper>();
        await seedHelper.SeedAsync();
    }
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//ProgramTester2.Main();
app.Run();

