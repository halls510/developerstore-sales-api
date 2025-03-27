using Ambev.DeveloperEvaluation.Common.Security;
using Bogus;

namespace Ambev.DeveloperEvaluation.Functional.Utils;

public static class TestDataGenerator
{

    public static object GenerateUser()
    {
        var faker = new Faker("pt_BR");

        string firstName = faker.Name.FirstName();
        string lastName = faker.Name.LastName();

        // Verifica se os nomes foram gerados corretamente, caso contrário, define valores padrão
        if (string.IsNullOrWhiteSpace(firstName)) firstName = "Carlos";
        if (string.IsNullOrWhiteSpace(lastName)) lastName = "Silva";

        return new
        {
            Name = new
            {
                Firstname = firstName,
                Lastname = lastName
            },
            Username = faker.Internet.UserName(firstName, lastName),
            Email = faker.Internet.Email(firstName, lastName),
            Password = "Secure@123", // Senha padrão segura
            Phone = "+" + faker.Random.Int(1, 9) + faker.Random.Replace("#############"), // Gera um telefone internacional válido
            Role = "Customer",
            Status = "Active",
            Address = new
            {
                City = faker.Address.City(),
                Street = faker.Address.StreetName(),
                Number = faker.Random.Int(1, 9999),
                Zipcode = faker.Address.ZipCode("#####-###"),
                Geolocation = new
                {
                    Lat = faker.Address.Latitude().ToString(),
                    Long = faker.Address.Longitude().ToString()
                }
            }
        };
    }




    public static object GenerateProduct()
    {
        var faker = new Faker("pt_BR");
        return new
        {
            Title = faker.Commerce.ProductName(),
            Price = faker.Random.Decimal(10, 500),
            Description = faker.Lorem.Sentence(),
            Category = faker.Commerce.Categories(1)[0],
            Image = faker.Image.PicsumUrl(),
            Rating = new
            {
                Rate = faker.Random.Decimal(1, 5),
                Count = faker.Random.Int(1, 1000)
            }
        };
    }

    public static object GenerateSale()
    {
        var faker = new Faker("pt_BR");
        return new
        {
            Number = faker.Random.Int(1000, 9999),
            Date = DateTime.UtcNow,
            Customer = faker.Name.FullName(),
            TotalAmount = faker.Random.Decimal(100, 5000),
            Branch = faker.Address.City(),
            Products = new[]
            {
                    new { ProductId = 1, Quantity = 3, UnitPrice = 10, Discount = 0, Total = 30 }
                },
            IsCancelled = false
        };
    }

    public static object GenerateAuthRequest(bool valid)
    {
        return valid ? new
        {
            Email = "halls510@hotmail.com",
            Password = "A#g7jfdsd#$%#"
        } : new
        {
            Email = "invalid@example.com",
            Password = "wrongPassword"
        };
    }
}