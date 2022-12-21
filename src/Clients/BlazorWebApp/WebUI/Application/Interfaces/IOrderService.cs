using WebUI.Application.Dtos;
using WebUI.Domain.Models.BasketMoels;

namespace WebUI.Application.Interfaces
{
    public interface IOrderService
    {
        BasketDTO MapOrderToBasket(Order order);
    }
}