namespace WebUI.Domain.Models.BasketMoels
{
    public class Basket
    {
        public List<BasketItem> Items { get; init; } = new List<BasketItem>();
        public string? BuyerId { get; init; }

        public decimal Total()
        {
            return Math.Round(Items.Sum(i => i.UnitPrice * i.Quantity), 2);
        }
    }
}