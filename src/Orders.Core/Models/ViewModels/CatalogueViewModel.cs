namespace Orders.Core.Models.ViewModels;

public class CatalogueViewModel
{
    public long RestaurantId { get; set; }
    public List<MenuItemViewModel> Menu { get; set; } = new();
}