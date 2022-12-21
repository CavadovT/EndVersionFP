using MediatR;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.DomainEventHandlers
{
    public class OrderStartedDomainEventHandler : INotificationHandler<OrderStartedDomainEvent>
    {
        private readonly IBuyerRepository _bayerRepository;

        public OrderStartedDomainEventHandler(IBuyerRepository bayerRepository)
        {
            _bayerRepository = bayerRepository;
        }

        public async Task Handle(OrderStartedDomainEvent notification, CancellationToken cancellationToken)
        {
            var cardTypeId=(notification.CardTypeId!=0)?notification.CardTypeId:1;

            var buyer = await _bayerRepository.GetSingleAsync(b => b.Name == notification.UserName, b => b.PaymentMethods);

            bool buyerOrginallyExisted = buyer != null;
            if (!buyerOrginallyExisted)
            {
                buyer = new Buyer(notification.UserName);
            }

            buyer.VerifyOrAddPaymentMethod(cardTypeId,
                                            $"Payment Method on {DateTime.UtcNow}",
                                            notification.CardNumber,
                                            notification.CardSecurityNumber,
                                            notification.CardHolderName,
                                            notification.CardExpiration,
                                            notification.Order.Id);
            var buyerUpdate = buyerOrginallyExisted ?
                _bayerRepository.Update(buyer):
                await _bayerRepository.AddAsync(buyer);

            await _bayerRepository.UnitOfWork.SaveEntityAsync(cancellationToken);

            //order status changed event may be fired here
        }
    }
}
