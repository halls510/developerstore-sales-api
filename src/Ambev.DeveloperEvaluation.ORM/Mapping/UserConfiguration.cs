using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.RegularExpressions;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnType("integer").ValueGeneratedOnAdd();

        builder.Property(u => u.Firstname).IsRequired().HasMaxLength(24);
        builder.Property(u => u.Lastname).IsRequired().HasMaxLength(24);
        builder.Property(u => u.Username).IsRequired().HasMaxLength(50);
        builder.Property(u => u.Password).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Phone).HasMaxLength(20);

        builder.Property(u => u.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(u => u.Role)
            .HasConversion<string>()
            .HasMaxLength(20);

        // Configuração do relacionamento com Address
        builder.OwnsOne(u => u.Address, address =>
        {
            address.Property(a => a.City).IsRequired().HasMaxLength(100);
            address.Property(a => a.Street).IsRequired().HasMaxLength(100);
            address.Property(a => a.Number).IsRequired();
            address.Property(a => a.Zipcode).IsRequired().HasMaxLength(20);

            // Criando índice dentro da propriedade Owned
            address.HasIndex(a => new { a.City, a.Street })
                .HasDatabaseName("idx_users_address");

            address.OwnsOne(a => a.Geolocation, geo =>
            {
                geo.Property(g => g.Lat).IsRequired().HasMaxLength(50);
                geo.Property(g => g.Long).IsRequired().HasMaxLength(50);

                // Criando índice dentro da propriedade Owned
                geo.HasIndex(g => new { g.Lat, g.Long })
                    .HasDatabaseName("idx_users_geolocation");
            });
        });

        // Índice para busca e ordenação por nome
        builder.HasIndex(u => new { u.Firstname, u.Lastname })
            .HasDatabaseName("idx_users_name");

        // Índice para ordenação por username
        builder.HasIndex(u => u.Username)
            .HasDatabaseName("idx_users_username");

        // Índice único para busca rápida por email
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("idx_users_email");

        // Índice para ordenação por data de criação
        builder.HasIndex(u => u.CreatedAt)
            .HasDatabaseName("idx_users_created_at");

        // Índice para status (ativos primeiro)
        builder.HasIndex(u => u.Status)
            .HasDatabaseName("idx_users_status");

        // Índice para ordenar por função (Admin, Manager, Customer)
        builder.HasIndex(u => u.Role)
            .HasDatabaseName("idx_users_role");    
    }
}
