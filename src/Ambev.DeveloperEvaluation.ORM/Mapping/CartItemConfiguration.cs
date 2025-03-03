using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItems");

        builder.HasKey(ci => ci.Id);
        builder.Property(u => u.Id).HasColumnType("integer").ValueGeneratedOnAdd();

        builder.Property(ci => ci.CartId).IsRequired();
        builder.Property(ci => ci.ProductId).IsRequired();
        builder.Property(ci => ci.ProductName).IsRequired().HasMaxLength(100);
        builder.Property(ci => ci.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(ci => ci.Quantity).IsRequired();

        builder.HasOne<Cart>()
               .WithMany(c => c.Items)
               .HasForeignKey(ci => ci.CartId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
