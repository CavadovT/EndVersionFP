using WebUI.Application.Dtos;
using WebUI.Domain.Models.BasketMoels;

namespace WebUI.Application.Interfaces
{
    public interface IBasketService
    {
        Task<Basket> GetBasket();

        Task<Basket> UpdateBasket(Basket basket);

        Task AddItemToBasket(int productId);

        Task Checkout(BasketDTO basket);
    }
}