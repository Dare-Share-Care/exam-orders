using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Orders.Core.Interfaces;
using Orders.Core.Models.ViewModels;
using RestaurantNamespace;


namespace Orders.Core.Services;

public class CatalogueService : ICatalogueService
{
    private readonly IConfiguration _configuration = null!;
    private readonly ILoggingService _loggingService;
    public CatalogueService(IConfiguration configuration, ILoggingService loggingService)
    {
        _configuration = configuration;
        _loggingService = loggingService;
    }

    // Empty constructor is for unit testing / easier mocking
    public CatalogueService()
    {
    }
    

    public async Task<CatalogueViewModel> GetCatalogueAsync(long restaurantId)
    {
        //TODO Reimplement appSettings configuration
        // Get the gRPC server URL from appsettings.json
        // var grpcServerUrl = _configuration.GetValue<string>("GrpcServer:Url");
        const string grpcServerUrl = "http://mtogo-restaurant-grpc:8000";

        using var channel = GrpcChannel.ForAddress(grpcServerUrl!);
        var client = new Catalogue.CatalogueClient(channel);

        var request = new RestaurantRequest { RestaurantId = restaurantId };

        try
        {
            var response = await client.SendCatalogueAsync(request);
            var restaurant = response.Restaurant;

            var catalogueViewModel = MapToViewModel(restaurant);

            return catalogueViewModel;
        }
        catch (RpcException ex)
        {
            await _loggingService.LogToFile(LogLevel.Error, $"gRPC Error: {ex.Status.Detail}", ex);
            throw new Exception($"gRPC Error: {ex.Status.Detail}", ex);
        }
    }

    private static CatalogueViewModel MapToViewModel(Restauranten restaurant)
    {
        // Map the gRPC response to your CatalogueViewModel
        var catalogueViewModel = new CatalogueViewModel
        {
            RestaurantId = restaurant.Id,
            Menu = new List<MenuItemViewModel>()
        };

        foreach (var menuItem in restaurant.Menu)
        {
            var menuItemViewModel = new MenuItemViewModel
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Price = (decimal)menuItem.Price
            };
            catalogueViewModel.Menu.Add(menuItemViewModel);
        }

        return catalogueViewModel;
    }
}