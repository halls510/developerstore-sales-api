using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;

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
                    Id = 1,
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
                    Id = 2,
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

        var users = context.Users.ToList();
        if (!context.Products.Any())
        {
            var productsInsert = new[]
            {
                new Product { Id = 1, Title = "Product 1", Price = 10.00m, Description = "Description for product 1", CreatedAt = DateTime.UtcNow },
                new Product { Id = 2, Title = "Product 2", Price = 15.00m, Description = "Description for product 2", CreatedAt = DateTime.UtcNow },
                new Product { Id = 3, Title = "Product 3", Price = 20.00m, Description = "Description for product 3", CreatedAt = DateTime.UtcNow },
                new Product { Id = 4, Title = "Product 4", Price = 25.00m, Description = "Description for product 4", CreatedAt = DateTime.UtcNow },
                new Product { Id = 5, Title = "Product 5", Price = 30.00m, Description = "Description for product 5", CreatedAt = DateTime.UtcNow }
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
                    Id = 1,
                    UserId = users[0].Id,
                    UserName = $"{users[0].Firstname} {users[0].Lastname}",
                    Date = DateTime.UtcNow,
                    Status = CartStatus.Active,
                    Items = new List<CartItem>
                    {
                        new CartItem { CartId = 1, ProductId = 1, Quantity = 2, UnitPrice = 10.00m },
                        new CartItem { CartId = 1, ProductId = 2, Quantity = 3, UnitPrice = 15.00m }
                    }
                },
                new Cart
                {
                    Id = 2,
                    UserId = users[1].Id,
                    UserName = $"{users[1].Firstname} {users[1].Lastname}",
                    Date = DateTime.UtcNow,
                    Status = CartStatus.Completed,
                    Items = new List<CartItem>
                    {
                        new CartItem { CartId = 2, ProductId = 3, Quantity = 1, UnitPrice = 20.00m },
                        new CartItem { CartId = 2, ProductId = 4, Quantity = 2, UnitPrice = 25.00m },
                        new CartItem { CartId = 2, ProductId = 5, Quantity = 3, UnitPrice = 30.00m }
                    }
                }
            };
            context.Carts.AddRange(carts);
            context.SaveChanges();
        }
    }
}
