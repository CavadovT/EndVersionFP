using Web.ApiGateway.Extensions;
using Web.ApiGateway.Interfaces;
using Web.ApiGateway.Models.Basket;

namespace Web.ApiGateway.Services
{
    public class BasketService : IBasketService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BasketService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<BasketData> GetById(string id)
        {
            var client = _httpClientFactory.CreateClient("basket");

            var response = await client.GetResponseAsync<BasketData>(id);

            return response?? new BasketData(id);// null getmesin
        }

        public async Task<BasketData> UpdateAsync(BasketData currentBasket)
        {
            var client = _httpClientFactory.CreateClient("basket");

            return await client.PostGetResponseAsync<BasketData, BasketData>("update", currentBasket);
        }
    }
}
