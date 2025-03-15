using Ambev.DeveloperEvaluation.Application.Users.DeleteUser;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class DeleteUserHandlerTestData
{
    private static readonly Faker<DeleteUserCommand> deleteUserHandlerFaker = new Faker<DeleteUserCommand>()
        .CustomInstantiator(f => new DeleteUserCommand(f.Random.Int(1, 1000)));

    public static DeleteUserCommand GenerateValidCommand()
    {
        return deleteUserHandlerFaker.Generate();
    }

    public static User GenerateValidUser(DeleteUserCommand command)
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
                Zipcode = "01310-000"
            }
        };
    }
}
