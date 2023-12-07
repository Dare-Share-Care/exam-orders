using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Orders.Web.Consumers;
using Orders.Core.Interfaces;
using Orders.Core.Services;
using Orders.Infrastructure.Data;
using Orders.Infrastructure.Interfaces.Producers;
using Orders.Infrastructure.Interfaces;
using Orders.Infrastructure.Producers;

const string policyName = "AllowOrigin";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    options.AddPolicy(name: policyName,
        builder =>
        {
            builder
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

//DBContext
builder.Services.AddDbContext<OrderContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbContext"));
});

//Build services
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICatalogueService, CatalogueService>();

//Build Kafka producers
builder.Services.AddScoped<IKafkaProducer, KafkaProducer>();

//Build repositories
builder.Services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

//Build kafka consumers
builder.Services.AddHostedService<UpdatedClaimedOrdersConsumer>();
builder.Services.AddHostedService<UpdatedCompletedOrdersConsumer>();

//JWT Key
// var key = Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!);
var key = Encoding.UTF8.GetBytes("super_secret_key"); //TODO above doesn't work with docker.

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireCustomerRole", policy => policy.RequireRole("Customer"));
    options.AddPolicy("RequireCourierRole", policy => policy.RequireRole("Courier"));
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    // Add more policies for other roles as needed
});


var app = builder.Build();

app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(policyName);

app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();

public partial class Program
{
}