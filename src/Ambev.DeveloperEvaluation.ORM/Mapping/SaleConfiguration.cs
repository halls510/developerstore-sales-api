﻿using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

/// <summary>
/// Configuration for Sale entity in Entity Framework Core.
/// </summary>
public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

        builder.HasKey(s => s.Id);        
        builder.Property(u => u.Id).HasColumnType("integer").ValueGeneratedOnAdd();

        builder.Property(s => s.SaleNumber).IsRequired().HasMaxLength(100);
        builder.Property(s => s.SaleDate).IsRequired();
        builder.Property(s => s.CustomerId).IsRequired();
        builder.Property(s => s.CustomerName).IsRequired().HasMaxLength(100);
        builder.Property(s => s.TotalValue).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(s => s.Branch).IsRequired().HasMaxLength(50);        

        builder.HasMany(s => s.Items)
            .WithOne()
            .HasForeignKey("SaleId")
            .OnDelete(DeleteBehavior.Cascade);

        // Adicionando o campo Status ao banco de dados
        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        // Índice para otimizar buscas por status
        builder.HasIndex(s => s.Status)
            .HasDatabaseName("idx_sales_status");
    }
}
