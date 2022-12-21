using WebUI.Application.Dtos;
using WebUI.Application.Interfaces;
using WebUI.Domain.Models.BasketMoels;
using WebUI.Extensions;

namespace WebUI.Application.Services
{
    public class BasketService : IBasketService
    {
        private readonly HttpClient _client;
        private readonly IIdentityService _identityService;
        private readonly ILogger<BasketService> _logger;

        public BasketService(HttpClient client, IIdentityService identityService, ILogger<BasketService> logger)
        {
            _client = client;
            _identityService = identityService;
            _logger = logger;
        }

        public async Task AddItemToBasket(int productId)
        {
            var model = new
            {
                CatalogItemId = productId,
                Quantity = 1,
                BasketId = _identityService.GetUserName(),
            };

            await _client.PostAsync("basket/items", model);
        }

        public Task Checkout(BasketDTO basket)
        {
            return _client.PostAsync("basket/checkout", basket);
        }

        public async Task<Basket> GetBasket()
        {
            var response = await _client.GetResponseAsync<Basket>("basket/" + _identityService.GetUserName());

            return response ?? new Basket() { BuyerId = _identityService.GetUserName() };
        }

        public async Task<Basket> UpdateBasket(Basket basket)
        {
            var response = await _client.PostGetResponseAsync<Basket, Basket>("baskte/update", basket);

            return response;
        }
    }
}