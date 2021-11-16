using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Core.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Config
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsOne(order => order.ShipToAddress, relateEntity => {
                relateEntity.WithOwner();
            });

            // Convert enum variables to string rather than numeric which is the default one
            builder.Property(s => s.Status).HasConversion(
                o => o.ToString(),
                o => (OrderStatus) Enum.Parse(typeof(OrderStatus), o)
            );

            // Config one to many relationship
            // If Order is removed, order items are deleted as well
            builder.HasMany(order => order.OrderItems).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}