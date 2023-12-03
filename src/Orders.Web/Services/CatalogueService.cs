using Grpc.Core;
using Grpc.Net.Client;
using Orders.Web.Interfaces.DomainServices;
using Orders.Web.Models.ViewModels;
using RestaurantNamespace;


namespace Orders.Web.Services;

public class CatalogueService : ICatalogueService
{
    private readonly IConfiguration _configuration = null!;

    public CatalogueService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // Empty constructor is for unit testing / easier mocking
    public CatalogueService()
    {
        
    }

    public async Task<CatalogueViewModel> GetCatalogueAsync(long restaurantId)
    {
        // Get the gRPC server URL from appsettings.json
        var grpcServerUrl = _configuration.GetValue<string>("GrpcServer:Url");
        
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