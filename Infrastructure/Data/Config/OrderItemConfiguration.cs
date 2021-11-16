using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Core.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Config
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.OwnsOne(orderItem => orderItem.ItemOrdered, relateEnitty => {
                relateEnitty.WithOwner();
            });

            builder.Property(oi => oi.Price).HasColumnType("decimal(18,2)");
        }
    }
}