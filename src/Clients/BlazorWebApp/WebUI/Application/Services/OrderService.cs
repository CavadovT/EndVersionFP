using WebUI.Application.Dtos;
using WebUI.Application.Interfaces;
using WebUI.Domain.Models.BasketMoels;

namespace WebUI.Application.Services
{
    public class OrderService : IOrderService
    {
        public BasketDTO MapOrderToBasket(Order order)
        {
            order.CardExpirationApiFormat();

            return new BasketDTO()
            {
                City = order.City,
                Country = order.Country,
                Street = order.Street,
                State = order.State,
                ZipCode = order.ZipCode,
                CardExpiration = order.CardExpiration,
                CardNumber = order.CardNumber,
                CardHolderName = order.CardHolderName,
                CardSecurityNumber = order.CardSecurityNumber,
                CardTypeId = 1,
                Buyer = order.Buyer,
            };
        }
    }
}