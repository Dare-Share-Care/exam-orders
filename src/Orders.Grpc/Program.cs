using Microsoft.EntityFrameworkCore;
using Orders.Core.Interfaces;
using Orders.Core.Services;
using Orders.Grpc.Services;
using Orders.Infrastructure.Data;
using Orders.Infrastructure.Interfaces;
using Orders.Infrastructure.Interfaces.Producers;
using Orders.Infrastructure.Producers;

var builder = WebApplication.CreateBuilder(args);

//DBContext
builder.Services.AddDbContext<OrderContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbContext"));
});

//Build services
builder.Services.AddScoped<ICatalogueService, CatalogueService>();
builder.Services.AddScoped<IOrderService, OrderService>();

//Build repositories
builder.Services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

//Build Kafka producers
builder.Services.AddScoped<IKafkaProducer, KafkaProducer>();

// Add services to the container.
builder.Services.AddGrpc();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<DeliveryService>();
app.MapGrpcService<ReviewService>();

app.MapGet("/delivery",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.MapGet("/completed",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();