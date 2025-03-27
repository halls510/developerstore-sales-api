using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Carts");

        builder.HasKey(c => c.Id);
        builder.Property(u => u.Id).UseIdentityAlwaysColumn().HasColumnType("integer");

        builder.Property(c => c.UserId).IsRequired();
        builder.Property(c => c.UserName).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Date)
            .HasConversion(
                v => v.ToUniversalTime(),  // Converte para UTC ao salvar
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)) // Garante que ao ler do banco seja UTC
            .IsRequired();

        builder.Property(c => c.Status)
           .IsRequired()
           .HasConversion<string>() // Armazena como string no banco de dados
           .HasMaxLength(20);

        builder.OwnsOne(c => c.TotalPrice, total =>
        {
            total.Property(m => m.Amount)
                .HasColumnName("TotalPrice")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        });

        // Relacionamento 1:N com CartItem
        builder.HasMany(c => c.Items)
               .WithOne()
               .HasForeignKey(ci => ci.CartId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.Status)
            .HasDatabaseName("idx_carts_status");
    }
}
