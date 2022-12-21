using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Infrastructure.Context;

namespace OrderService.Infrastructure.EntityConfigurations
{
    public class PaymentMethodEntityConfiguration : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.ToTable("paymentmethods", OrderDbContext.DEFAULT_SCHEMA);

            builder.Ignore(x => x.DomainEvents);

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property<int>("BuyerId")
                .IsRequired();

            builder
                .Property(x => x.CardHolderName)
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("CardHolderName")
                .HasMaxLength(200)
                .IsRequired();

            builder
                .Property(x => x.Alias)
                .UsePropertyAccessMode((PropertyAccessMode)PropertyAccessMode.Field)
                .HasColumnName("Alias")
                .HasMaxLength(200)
                .IsRequired();

            builder
              .Property(x => x.CardNumber)
              .UsePropertyAccessMode((PropertyAccessMode)PropertyAccessMode.Field)
              .HasColumnName("CardNumber")
              .HasMaxLength(25)
              .IsRequired();

            builder
               .Property(x => x.CardExpiration)
               .UsePropertyAccessMode((PropertyAccessMode)PropertyAccessMode.Field)
               .HasColumnName("CardExpiration")
               .HasMaxLength(25)
               .IsRequired();

            builder
             .Property(x => x.CardTypeId)
             .UsePropertyAccessMode((PropertyAccessMode)PropertyAccessMode.Field)
             .HasColumnName("CardTypeId")
             .HasMaxLength(25)
             .IsRequired();

            builder.HasOne(x => x.CardType)
                .WithMany()
                .HasForeignKey(x => x.CardTypeId);

        }
    }
}
