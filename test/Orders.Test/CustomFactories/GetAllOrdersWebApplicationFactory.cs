using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Orders.Web.Data;
using Orders.Web.Producers;

namespace Orders.Test.CustomFactories;

public class GetAllOrdersWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            //Build Kafka producers
            services.AddSingleton<KafkaProducer>();
            
            // Find the service descriptor that registers the DbContext.
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<OrderContext>));

            if (descriptor != null)
            {
                // Remove registration.
                services.Remove(descriptor);
            }

            // Add DbContext using an in-memory database for testing.
            services.AddDbContext<OrderContext>(options =>
            {
                options.UseInMemoryDatabase("GetAllOrders");
            });
        });
    }
}