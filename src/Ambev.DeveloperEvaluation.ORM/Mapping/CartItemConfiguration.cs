﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItems");

        builder.HasKey(ci => ci.Id);
        builder.Property(u => u.Id).UseIdentityAlwaysColumn().HasColumnType("integer");

        builder.Property(ci => ci.CartId).IsRequired();
        builder.Property(ci => ci.ProductId).IsRequired();
        builder.Property(ci => ci.ProductName).IsRequired().HasMaxLength(200);
        builder.Property(ci => ci.Quantity).IsRequired();

        builder.OwnsOne(ci => ci.UnitPrice, unitPrice =>
        {
            unitPrice.Property(m => m.Amount)
                .HasColumnName("UnitPrice")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        });

        builder.OwnsOne(ci => ci.Discount, discount =>
        {
            discount.Property(m => m.Amount)
                .HasColumnName("Discount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        });

        builder.OwnsOne(ci => ci.Total, total =>
        {
            total.Property(m => m.Amount)
                .HasColumnName("Total")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        });

        builder.HasOne<Cart>()
               .WithMany(c => c.Items)
               .HasForeignKey(ci => ci.CartId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
