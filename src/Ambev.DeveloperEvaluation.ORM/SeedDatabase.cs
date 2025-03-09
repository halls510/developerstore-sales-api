using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.ORM;
public static class SeedDatabase
{
    public static void Initialize(DefaultContext context)
    {
        // **Inserindo usuários SEM definir manualmente o ID**
        if (!context.Users.Any())
        {
            var usersInsert = new[]
            {
                new User
                {
                    Firstname = "John",
                    Lastname = "Doe",
                    Username = "johndoe",
                    Email = "john.doe@example.com",
                    Phone = "+12125551234",
                    Password = "Secure@123",
                    Role = UserRole.Customer,
                    Status = UserStatus.Active,
                    CreatedAt = DateTime.UtcNow,
                    Address = new Address
                    {
                        City = "New York",
                        Street = "5th Avenue",
                        Number = 123,
                        Zipcode = "10001",
                        Geolocation = new Geolocation
                        {
                            Lat = "40.7128",
                            Long = "-74.0060"
                        }
                    }
                },
                new User
                {
                    Firstname = "Jane",
                    Lastname = "Smith",
                    Username = "janesmith",
                    Email = "jane.smith@example.com",
                    Phone = "+5511987654321",
                    Password = "Pass@Word1",
                    Role = UserRole.Customer,
                    Status = UserStatus.Inactive,
                    CreatedAt = DateTime.UtcNow,
                    Address = new Address
                    {
                        City = "São Paulo",
                        Street = "Avenida Paulista",
                        Number = 987,
                        Zipcode = "01310-000",
                        Geolocation = new Geolocation
                        {
                            Lat = "-23.5611",
                            Long = "-46.6564"
                        }
                    }
                }
            };

            context.Users.AddRange(usersInsert);
            context.SaveChanges();
        }

        // **Buscando os usuários com IDs gerados automaticamente**
        var users = context.Users.ToList();

        // **Inserindo produtos SEM definir manualmente o ID**
        if (!context.Products.Any())
        {
            var productsInsert = new[]
            {
                new Product { Title = "Product 1", Price = new Money(10.00m), Description = "Description for product 1", CreatedAt = DateTime.UtcNow },
                new Product { Title = "Product 2", Price = new Money(15.00m), Description = "Description for product 2", CreatedAt = DateTime.UtcNow },
                new Product { Title = "Product 3", Price = new Money(20.00m), Description = "Description for product 3", CreatedAt = DateTime.UtcNow },
                new Product { Title = "Product 4", Price = new Money(25.00m), Description = "Description for product 4", CreatedAt = DateTime.UtcNow },
                new Product { Title = "Product 5", Price = new Money(30.00m), Description = "Description for product 5", CreatedAt = DateTime.UtcNow }
            };

            context.Products.AddRange(productsInsert);
            context.SaveChanges();
        }

        // **Buscando os produtos com IDs gerados automaticamente**
        var products = context.Products.ToList();

        // **Inserindo carrinhos SEM definir manualmente o ID**
        if (!context.Carts.Any())
        {
            var carts = new[]
            {
                new Cart
                {
                    UserId = users[0].Id,
                    UserName = $"{users[0].Firstname} {users[0].Lastname}",
                    Date = DateTime.UtcNow,
                    Status = CartStatus.Active,
                    Items = new List<CartItem>
                    {
                        new CartItem { ProductId = products[0].Id, Quantity = 2, UnitPrice = new Money(10.00m) },
                        new CartItem { ProductId = products[1].Id, Quantity = 3, UnitPrice = new Money(15.00m) }
                    }
                },
                new Cart
                {
                    UserId = users[1].Id,
                    UserName = $"{users[1].Firstname} {users[1].Lastname}",
                    Date = DateTime.UtcNow,
                    Status = CartStatus.Completed,
                    Items = new List<CartItem>
                    {
                        new CartItem { ProductId = products[2].Id, Quantity = 1, UnitPrice = new Money(20.00m)  },
                        new CartItem { ProductId = products[3].Id, Quantity = 2, UnitPrice = new Money(25.00m) },
                        new CartItem { ProductId = products[4].Id, Quantity = 3, UnitPrice = new Money(30.00m) }
                    }
                }
            };

            context.Carts.AddRange(carts);
            context.SaveChanges();
        }
    }
}
