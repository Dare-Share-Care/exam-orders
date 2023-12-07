using Orders.Core.Models.ViewModels;

namespace Orders.Core.Interfaces;

public interface ICatalogueService
{
    Task<CatalogueViewModel> GetCatalogueAsync(long restaurantId);
}