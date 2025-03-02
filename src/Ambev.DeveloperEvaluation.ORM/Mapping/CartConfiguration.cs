﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Carts");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(c => c.UserId).IsRequired();
        builder.Property(c => c.UserName).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Date).IsRequired();

        builder.Property(c => c.Status)
           .IsRequired()
           .HasConversion<string>() // Armazena como string no banco de dados
           .HasMaxLength(20);

        // Relacionamento 1:N com CartItem
        builder.HasMany(c => c.Items)
               .WithOne()
               .HasForeignKey(ci => ci.CartId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.Status)
            .HasDatabaseName("idx_carts_status");
    }
}
