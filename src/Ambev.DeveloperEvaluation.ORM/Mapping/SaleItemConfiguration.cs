using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.ORM.Mapping
{
    public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
    {
        public void Configure(EntityTypeBuilder<SaleItem> builder)
        {
            builder.ToTable("SaleItems");

            builder.HasKey(si => si.Id);
            builder.Property(si => si.Id)
                   .UseIdentityAlwaysColumn()
                   .HasColumnType("integer");

            builder.Property(si => si.SaleId)
                   .IsRequired();

            builder.Property(si => si.ProductId)
                   .IsRequired();

            builder.Property(si => si.ProductName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(si => si.Quantity)
                   .IsRequired();

            // Mapeamento correto do ValueObject Money (UnitPrice)
            builder.OwnsOne(si => si.UnitPrice, unitPrice =>
            {
                unitPrice.Property(m => m.Amount)
                         .HasColumnName("UnitPrice")
                         .HasColumnType("decimal(18,2)")
                         .IsRequired();
            });

            // Mapeamento correto do ValueObject Money (Discount)
            builder.OwnsOne(si => si.Discount, discount =>
            {
                discount.Property(m => m.Amount)
                        .HasColumnName("Discount")
                        .HasColumnType("decimal(18,2)")
                        .IsRequired();
            });

            // Mapeamento correto do ValueObject Money (Total)
            builder.OwnsOne(si => si.Total, total =>
            {
                total.Property(m => m.Amount)
                     .HasColumnName("Total")
                     .HasColumnType("decimal(18,2)")
                     .IsRequired();
            });

            builder.HasOne<Sale>()
                   .WithMany(s => s.Items)
                   .HasForeignKey(si => si.SaleId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(si => si.Status)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(20);

            builder.HasIndex(si => si.Status)
                   .HasDatabaseName("idx_saleitems_status");
        }
    }
}