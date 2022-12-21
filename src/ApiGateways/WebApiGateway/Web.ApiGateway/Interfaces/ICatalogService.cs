using Web.ApiGateway.Models.Catalog;

namespace Web.ApiGateway.Interfaces
{
    public interface ICatalogService
    {
        Task<CatalogItem> GetCatalogItemAsync(int id);
        Task<IEnumerable<CatalogItem>> GetCatalogItemsAsync(IEnumerable<int> ids);
    }
}
