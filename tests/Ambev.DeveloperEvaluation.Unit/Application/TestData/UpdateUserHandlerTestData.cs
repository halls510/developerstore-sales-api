using Ambev.DeveloperEvaluation.Application.Users.UpdateUser;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class UpdateUserHandlerTestData
{
    private static readonly Faker<UpdateUserCommand> updateUserHandlerFaker = new Faker<UpdateUserCommand>()
        .RuleFor(u => u.Id, f => f.Random.Int(1, 1000))
        .RuleFor(u => u.Username, f => f.Internet.UserName())
        .RuleFor(u => u.Firstname, f => f.Name.FirstName())
        .RuleFor(u => u.Lastname, f => f.Name.LastName())
        .RuleFor(u => u.Email, f => f.Internet.Email())
        .RuleFor(u => u.Password, f => "ValidPassword123!")
        .RuleFor(u => u.Status, f => f.PickRandom(UserStatus.Active, UserStatus.Suspended, UserStatus.Inactive))         
        .RuleFor(u => u.Phone, f => $"+55{f.Random.Number(11, 99)}{f.Random.Number(100000000, 999999999)}") 
        .RuleFor(u => u.Role, f => f.PickRandom(UserRole.Customer, UserRole.Admin));

    public static UpdateUserCommand GenerateValidCommand()
    {
        return updateUserHandlerFaker.Generate();
    }

    public static User GenerateValidUser(UpdateUserCommand command)
    {
        return new User
        {
            Id = command.Id,
            Username = command.Username ?? "DefaultUser",
            Email = command.Email ?? "default@example.com",
            Password = "hashedPassword123",
            Status = command.Status,
            Firstname = command.Firstname ?? "John",
            Lastname = command.Lastname ?? "Doe",
            Phone = command.Phone ?? "+55 11 99999-9999",
            Role = command.Role,
            Address = command.Address ?? new Address
            {
                Street = "Av. Paulista",
                Number = 1000,
                City = "São Paulo",
                Zipcode = "01310-000"
            },
            UpdatedAt = DateTime.UtcNow
        };
    }

}