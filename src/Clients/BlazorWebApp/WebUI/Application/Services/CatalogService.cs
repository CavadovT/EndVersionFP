using WebUI.Application.Interfaces;
using WebUI.Domain.Models.CatalogModels;
using WebUI.Domain.Models.PaginationViewModels;
using WebUI.Extensions;

namespace WebUI.Application.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient _client;

        public CatalogService(HttpClient client)
        {
            _client = client;
        }

        public async Task<PaginatedItemsViewModel<CatalogItem>> GetCatalogItems()
        {
            var response = await _client.GetResponseAsync<PaginatedItemsViewModel<CatalogItem>>("/catalog/items");

            return response;
        }
    }
}