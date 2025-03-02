using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace Ambev.DeveloperEvaluation.ORM;
public static class SeedDatabase
{
    public static void Initialize(DefaultContext context)
    {
        if (!context.Users.Any())
        {
            var usersInsert = new[]
            {
            new User
            {
                Id = Guid.NewGuid(),
                Firstname = "Admin",
                Lastname = "User",
                Username = "admin",
                Email = "admin@example.com",
                Phone = "(11) 99999-9999",
                Password = "Admin@123",
                Role = UserRole.Admin,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow,
                Address = new Address
                {
                    City = "São Paulo",
                    Street = "Avenida Paulista",
                    Number = 1000,
                    Zipcode = "01310-100",
                    Geolocation = new Geolocation
                    {
                        Lat = "-23.561684",
                        Long = "-46.656139"
                    }
                }
            },
            new User
            {
                Id = Guid.NewGuid(),
                Firstname = "João",
                Lastname = "Silva",
                Username = "joaosilva",
                Email = "joao@example.com",
                Phone = "(11) 98888-8888",
                Password = "Joao@123",
                Role = UserRole.Customer,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow,
                Address = new Address
                {
                    City = "Rio de Janeiro",
                    Street = "Rua do Comércio",
                    Number = 50,
                    Zipcode = "20010-020",
                    Geolocation = new Geolocation
                    {
                        Lat = "-22.906847",
                        Long = "-43.172896"
                    }
                }
            }
        };

            context.Users.AddRange(usersInsert);
            context.SaveChanges();
        }

        var users = context.Users.ToList();
      

        // Busca todas as categorias do banco
        var existingCategories = context.Categories.ToList();

        // Adiciona outras categorias APENAS se não existirem ainda
        var categoriesToAdd = new List<Category>
        {
            new Category { Id = Guid.NewGuid(), Name = "Eletrônicos" },
            new Category { Id = Guid.NewGuid(), Name = "Livros" },
            new Category { Id = Guid.NewGuid(), Name = "Moda" },
            new Category { Id = Guid.NewGuid(), Name = "Alimentos" }
        };  

        // Remove categorias que já existem para evitar duplicação
        categoriesToAdd = categoriesToAdd
            .Where(c => !existingCategories.Any(ec => ec.Name == c.Name))
            .ToList();

        if (categoriesToAdd.Any())
        {
            context.Categories.AddRange(categoriesToAdd);
            context.SaveChanges();
        }

        if (!context.Products.Any())
        {
            var categoriesList = context.Categories.ToList();         

            var productsInsert = new[]
            {
                new Product
                {
                    Id = Guid.NewGuid(),
                    Title = "Smartphone 5G",
                    Price = 3999.99m,
                    Description = "Smartphone de última geração com tecnologia 5G e câmera ultra-wide.",
                    Image = "https://example.com/images/smartphone.jpg",
                    CreatedAt = DateTime.UtcNow,
                    CategoryId = categoriesList[1].Id,
                    Rating = new Rating { Rate = 4.8, Count = 120 }
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Title = "Notebook Gamer",
                    Price = 7599.99m,
                    Description = "Notebook potente com placa de vídeo dedicada para jogos.",
                    Image = "https://example.com/images/notebook.jpg",
                    CreatedAt = DateTime.UtcNow,
                    CategoryId = categoriesList[1].Id,
                    Rating = new Rating { Rate = 4.5, Count = 95 }
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Title = "Sofá em L",
                    Price = 7599.99m,
                    Description = "Sofá em L tamanho Família",
                    Image = "https://example.com/images/sofa-em-l.jpg",
                    CreatedAt = DateTime.UtcNow,                    
                    Rating = new Rating { Rate = 4.5, Count = 95 }
                }
            };

            context.Products.AddRange(productsInsert);
            context.SaveChanges();
        }

        var products = context.Products.ToList();

        if (!context.Carts.Any())
        {
            var carts = new[]
            {
                new Cart
                {
                    Id = Guid.NewGuid(),
                    UserId = users[0].Id,
                    UserName = $"{users[0].Firstname} {users[0].Lastname}",
                    Date = DateTime.UtcNow,
                    Status = CartStatus.Active
                },
                new Cart
                {
                    Id = Guid.NewGuid(),
                    UserId = users[1].Id,
                    UserName = $"{users[1].Firstname} {users[1].Lastname}",
                    Date = DateTime.UtcNow,
                    Status = CartStatus.Completed // Esse carrinho será convertido para uma venda
                }
            };

            context.Carts.AddRange(carts);
            context.SaveChanges();

            var cartItems = new[]
            {
                new CartItem
                {
                    Id = Guid.NewGuid(),
                    CartId = carts[0].Id,
                    ProductId = products[0].Id,
                    ProductName = products[0].Title,
                    UnitPrice = products[0].Price,
                    Quantity = 1
                },
                new CartItem
                {
                    Id = Guid.NewGuid(),
                    CartId = carts[0].Id,
                    ProductId = products[1].Id,
                    ProductName = products[1].Title,
                    UnitPrice = products[1].Price,
                    Quantity = 2
                },
                new CartItem
                {
                    Id = Guid.NewGuid(),
                    CartId = carts[1].Id, // Carrinho que será convertido para venda
                    ProductId = products[1].Id,
                    ProductName = products[1].Title,
                    UnitPrice = products[1].Price,
                    Quantity = 3
                }
            };

            context.CartItems.AddRange(cartItems);
            context.SaveChanges();
        }

        var completedCarts = context.Carts.Where(c => c.Status == CartStatus.Completed).ToList();

        if (completedCarts.Any())
        {
            var sales = new List<Sale>();

            foreach (var cart in completedCarts)
            {
                var sale = new Sale
                {
                    Id = Guid.NewGuid(),
                    SaleNumber = $"SALE-{DateTime.UtcNow:yyyyMMddHHmmss}-{cart.Id.ToString().Substring(0, 6)}",
                    SaleDate = DateTime.UtcNow,
                    CustomerId = cart.UserId,
                    CustomerName = cart.UserName,
                    TotalValue = cart.TotalPrice,
                    Branch = "Online Store",
                    Items = new List<SaleItem>(),
                    Status = SaleStatus.Confirmed,
                };

                var saleItems = context.CartItems
                    .Where(ci => ci.CartId == cart.Id)
                    .Select(ci => new SaleItem
                    {
                        Id = Guid.NewGuid(),
                        SaleId = sale.Id,
                        ProductId = ci.ProductId,
                        ProductName = ci.ProductName,
                        Quantity = ci.Quantity,
                        UnitPrice = ci.UnitPrice,
                        Discount = 0,
                        Status = SaleItemStatus.Active
                    })
                    .ToList();

                sale.Items.AddRange(saleItems);
                sales.Add(sale);
            }

            context.Sales.AddRange(sales);
            context.SaveChanges();
        }
    }
}
