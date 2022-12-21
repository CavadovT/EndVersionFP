using Web.ApiGateway.Models.Basket;

namespace Web.ApiGateway.Interfaces
{
    public interface IBasketService
    {
        Task<BasketData> GetById(string id);
        Task<BasketData> UpdateAsync(BasketData currentBasket);

    }
}
