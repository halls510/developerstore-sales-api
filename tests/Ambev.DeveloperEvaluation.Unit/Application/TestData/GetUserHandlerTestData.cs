using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class GetUserHandlerTestData
{  
    private static readonly Faker<GetUserCommand> getUserHandlerFaker = new Faker<GetUserCommand>()
    .CustomInstantiator(f => new GetUserCommand(f.Random.Int(1, 1000)));

    public static GetUserCommand GenerateValidCommand()
    {
        return getUserHandlerFaker.Generate();
    }

    public static User GenerateValidUser(GetUserCommand command)
    {
        return new User
        {
            Id = command.Id,
            Username = "TestUser",
            Email = "testuser@example.com",
            Status = UserStatus.Active,
            Firstname = "John",
            Lastname = "Doe",
            Password = "hashedPassword123",
            Phone = "+55 11 99999-9999",
            Role = UserRole.Customer,
            Address = new Address
            {
                Street = "Av. Paulista",
                Number = 1000,
                City = "São Paulo",
                Zipcode = "01310-000",
            }
        };
    }

}
