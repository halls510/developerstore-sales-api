using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

/// <summary>
/// Configuration for Cart entity.
/// </summary>
public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Carts");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(c => c.UserId)
            .IsRequired()
            .HasColumnType("uuid"); // External Identity for User

        builder.Property(c => c.Date)
            .IsRequired();

        builder.OwnsMany(c => c.Products, item =>
        {
            item.WithOwner();
            item.Property(p => p.ProductId).IsRequired().HasColumnType("uuid"); // External Identity for Product
            item.Property(p => p.Quantity).IsRequired();
        });
    }
}
