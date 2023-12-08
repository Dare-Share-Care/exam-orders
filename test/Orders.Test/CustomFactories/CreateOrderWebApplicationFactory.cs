using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Orders.Core.Interfaces;
using Orders.Core.Models.ViewModels;
using Orders.Infrastructure.Data;
using Moq;

namespace Orders.Test.CustomFactories;

public class CreateOrderWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Find the service descriptor that registers the DbContext.
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<OrderContext>));

            if (descriptor != null)
            {
                // Remove registration.
                services.Remove(descriptor);
            }

            // Add DbContext using an in-memory database for testing.
            services.AddDbContext<OrderContext>(options =>
            {
                options.UseInMemoryDatabase("CreateOrder");
            });

            // Replace CatalogueService with a mock
            var catalogueServiceDescriptor = services.FirstOrDefault(
                d => d.ServiceType == typeof(ICatalogueService));

            if (catalogueServiceDescriptor != null)
            {
                services.Remove(catalogueServiceDescriptor);
            }

            var mockCatalogueService = new Mock<ICatalogueService>();
            // Mock CatalogueService (from Restaurant.Grpc microservice) 
            mockCatalogueService.Setup(c => c.GetCatalogueAsync(It.IsAny<long>()))
                .ReturnsAsync(new CatalogueViewModel
                {
                    RestaurantId = 1,
                    Menu = new List<MenuItemViewModel>
                    {
                        new() { Id = 1, Name = "Item 1", Price = 100 },
                        new() { Id = 2, Name = "Item 2", Price = 150 }
                    }
                });

            // Register the mockCatalogueService in the service collection
            services.AddSingleton(mockCatalogueService.Object);
        });
    }
}
