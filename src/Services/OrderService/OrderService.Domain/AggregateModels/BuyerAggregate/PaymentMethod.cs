﻿using OrderService.Domain.Exceptions;
using OrderService.Domain.SeedWork;

namespace OrderService.Domain.AggregateModels.BuyerAggregate
{
    public class PaymentMethod : BaseEntity
    {
        public string? Alias { get; set; }
        public string? CardNumber { get; set; }
        public string? SecurityNumber { get; set; }
        public string? CardHolderName { get; set; }
        public DateTime CardExpiration { get; set; }
        public int CardTypeId { get; set; }
        public CardType? CardType { get; private set; }
        public PaymentMethod()
        { }
        public PaymentMethod(int cardTypeId, string alias, string cardNumber, string securityNumber, string cardHolderName, DateTime cardExpiration)
        {
            CardNumber = !string.IsNullOrEmpty(cardNumber) ? cardNumber : throw new OrderingDomainException(nameof(cardNumber));
            SecurityNumber = !string.IsNullOrEmpty(securityNumber) ? securityNumber : throw new OrderingDomainException(nameof(securityNumber));
            CardHolderName = !string.IsNullOrEmpty(cardHolderName) ? cardHolderName : throw new OrderingDomainException(nameof(cardHolderName));

            if (cardExpiration < DateTime.UtcNow)
            {
                throw new OrderingDomainException(nameof(cardExpiration));
            }
            Alias = alias;
            CardExpiration = cardExpiration;
            CardTypeId = cardTypeId;
            //CardType = CardType.FromValue<CardType>(cardTypeId);

        }
        public bool IsEqualTo(int cardTypeId, string cardNumber, DateTime expiration)
        {
            return CardTypeId == cardTypeId && CardNumber == cardNumber && CardExpiration == expiration;
        }
    }
}
