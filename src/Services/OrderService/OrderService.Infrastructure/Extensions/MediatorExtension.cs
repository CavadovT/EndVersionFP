using MediatR;
using OrderService.Domain.SeedWork;
using OrderService.Infrastructure.Context;

namespace OrderService.Infrastructure.Extensions
{
    public static class MediatorExtension
    {
        public static async Task DispatchDomainEventAsync(this IMediator mediator, OrderDbContext context)
        {

            var domainEntites = context.ChangeTracker
                    .Entries<BaseEntity>()
                    .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

            var domainEvents = domainEntites
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            domainEntites.ToList()
                .ForEach(enity => enity.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await mediator.Publish(domainEvent);
            }

        }
    }
}
