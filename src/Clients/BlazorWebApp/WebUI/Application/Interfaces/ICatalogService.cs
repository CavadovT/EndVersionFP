using WebUI.Domain.Models.CatalogModels;
using WebUI.Domain.Models.PaginationViewModels;

namespace WebUI.Application.Interfaces
{
    public interface ICatalogService
    {
        Task<PaginatedItemsViewModel<CatalogItem>> GetCatalogItems();
    }
}