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
                    Id = 2,
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
        var existingCategories = context.Categories.ToList();

        var categoriesToAdd = new List<Category>
        {
            new Category { Id = 2, Name = "Eletrônicos" },
            new Category { Id = 3, Name = "Livros" },
            new Category { Id = 4, Name = "Moda" },
            new Category { Id = 5, Name = "Alimentos" }
        };

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
                    Id = 1,
                    Title = "Smartphone 5G",
                    Price = 3999.99m,
                    Description = "Smartphone de última geração com tecnologia 5G e câmera ultra-wide.",
                    Image = "https://example.com/images/smartphone.jpg",
                    CreatedAt = DateTime.UtcNow,
                    CategoryId = categoriesList[0].Id,
                    Rating = new Rating { Rate = 4.8, Count = 120 }
                },
                new Product
                {
                    Id = 2,
                    Title = "Notebook Gamer",
                    Price = 7599.99m,
                    Description = "Notebook potente com placa de vídeo dedicada para jogos.",
                    Image = "https://example.com/images/notebook.jpg",
                    CreatedAt = DateTime.UtcNow,
                    CategoryId = categoriesList[0].Id,
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
                    Id = 1,
                    UserId = users[0].Id,
                    UserName = $"{users[0].Firstname} {users[0].Lastname}",
                    Date = DateTime.UtcNow,
                    Status = CartStatus.Active
                },
                new Cart
                {
                    Id = 2,
                    UserId = users[1].Id,
                    UserName = $"{users[1].Firstname} {users[1].Lastname}",
                    Date = DateTime.UtcNow,
                    Status = CartStatus.Completed
                }
            };
            context.Carts.AddRange(carts);
            context.SaveChanges();
        }
    }
}
