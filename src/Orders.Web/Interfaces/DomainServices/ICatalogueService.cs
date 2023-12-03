using Orders.Web.Models.ViewModels;

namespace Orders.Web.Interfaces.DomainServices;

public interface ICatalogueService
{
    Task<CatalogueViewModel> GetCatalogueAsync(long restaurantId);
}