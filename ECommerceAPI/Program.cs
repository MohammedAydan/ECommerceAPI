using ECommerceAPI.Data;
using ECommerceAPI.Helpers;
using ECommerceAPI.Mappings;
using ECommerceAPI.Middlewares;
using ECommerceAPI.Model.Entities;
using ECommerceAPI.Repositories.Implementations;
using ECommerceAPI.Repositories.Interfaces;
using ECommerceAPI.Services.Implementations;
using ECommerceAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// -------------------- Services Registration --------------------

builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

// DbContext
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection") ?? "",
    new MySqlServerVersion(new Version(8, 0, 33))
));

// Identity
builder.Services.AddIdentity<UserModel, RoleModel>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IEmailRepository, EmailRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

// Helpers
builder.Services.AddScoped<SeedHelper>();
builder.Services.AddSingleton<ApiKeyMiddleware>();
builder.Services.AddSingleton<TokenHelper>();
builder.Services.AddSingleton<HttpClient>();
builder.Services.AddSingleton<UploadImagesHelper>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

var emailConfig = builder.Configuration.GetSection("EmailConfig").Get<EmailConfig>();
if (emailConfig != null) builder.Services.AddSingleton<EmailConfig>(emailConfig);

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JWT");
builder.Services.AddAuthentication(op =>
{
    op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(op =>
{
    op.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? "")),
        RequireExpirationTime = true,
    };
});

// CORS (Allow Any URL / Path)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(
                "https://ecommerce-app-git-master-mohammedaydans-projects.vercel.app",
                "https://ecommerce-app-rho-peach.vercel.app",
                "http://localhost:3000", //
                "http://192.168.1.8:3000" //
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();  // Allow credentials (cookies, HTTP authentication)
    });
});


var app = builder.Build();

app.MapDefaultEndpoints();


// -------------------- Middleware Pipeline --------------------

// Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Seed data
    //using (var scope = app.Services.CreateScope())
    //{
    //    var seedHelper = scope.ServiceProvider.GetRequiredService<SeedHelper>();
    //    await seedHelper.SeedAsync();
    //}
}
else
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

// HTTPS Redirect
app.UseHttpsRedirection();

// Use Static Files
app.UseStaticFiles();

// Use CORS (IMPORTANT: must be before Authentication)
app.UseCors("AllowSpecificOrigins");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Apply ApiKeyMiddleware only for /api paths
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api"), appBuilder =>
{
    appBuilder.UseMiddleware<ApiKeyMiddleware>();
});

// Map Controllers
app.MapControllers();

// Run the application
app.Run();
