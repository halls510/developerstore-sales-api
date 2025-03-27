using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping
{
    /// <summary>
    /// Configuration for Sale entity in Entity Framework Core.
    /// </summary>
    public class SaleConfiguration : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> builder)
        {
            builder.ToTable("Sales");

            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id)
                   .UseIdentityAlwaysColumn()
                   .HasColumnType("integer");

            builder.Property(s => s.SaleNumber)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(s => s.SaleDate)
                   .IsRequired();

            builder.Property(s => s.CustomerId)
                   .IsRequired();

            builder.Property(s => s.CustomerName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(s => s.Branch)
                   .IsRequired()
                   .HasMaxLength(50);

            // Mapeamento correto do ValueObject Money (TotalValue)
            builder.OwnsOne(s => s.TotalValue, total =>
            {
                total.Property(m => m.Amount)
                    .HasColumnName("TotalValue")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
            });

            builder.HasMany(s => s.Items)
                   .WithOne()
                   .HasForeignKey("SaleId")
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(s => s.Status)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(20);

            builder.HasIndex(s => s.Status)
                   .HasDatabaseName("idx_sales_status");
        }
    }
}
