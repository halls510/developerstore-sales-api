using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

/// <summary>
/// Configuration for Product entity in EF Core.
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);
        builder.Property(u => u.Id).HasColumnType("integer").ValueGeneratedOnAdd();
        builder.Property(p => p.Title).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Price).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(p => p.Description).HasMaxLength(1000);
        builder.Property(p => p.Image).HasMaxLength(500);

        builder.OwnsOne(p => p.Rating, rating =>
        {
            rating.Property(r => r.Rate).HasColumnType("decimal(3,2)");
            rating.Property(r => r.Count).IsRequired();
        });

        builder.Property(p => p.CategoryId)
           .IsRequired()
           .HasDefaultValue(Category.DefaultCategoryId);

        builder.HasIndex(p => p.CategoryId)
            .HasDatabaseName("idx_products_category");

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
