using Ambev.DeveloperEvaluation.Application.Users.CreateUser;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class CreateUserHandlerTestData
{
    private static readonly Faker<CreateUserCommand> createUserHandlerFaker = new Faker<CreateUserCommand>()
        .RuleFor(u => u.Username, f => f.Internet.UserName())
        .RuleFor(u => u.Firstname, f => f.Name.FirstName())
        .RuleFor(u => u.Lastname, f => f.Name.LastName())
        .RuleFor(u => u.Password, f => $"Test@{f.Random.Number(100, 999)}")
        .RuleFor(u => u.Email, f => f.Internet.Email())
        .RuleFor(u => u.Phone, f => $"+55{f.Random.Number(11, 99)}{f.Random.Number(100000000, 999999999)}")
        .RuleFor(u => u.Status, f => f.PickRandom(UserStatus.Active, UserStatus.Suspended))
        .RuleFor(u => u.Role, f => f.PickRandom(UserRole.Customer, UserRole.Admin));

    public static CreateUserCommand GenerateValidCommand()
    {
        return createUserHandlerFaker.Generate();
    }

    public static User GenerateValidUser(CreateUserCommand command)
    {
        return new User
        {
            Id = new Random().Next(1, 1000),
            Username = command.Username,
            Firstname = command.Firstname,
            Lastname =command.Lastname,
            Password = "hashedPassword",
            Email = command.Email,
            Phone = command.Phone,
            Status = command.Status,
            Role = command.Role
        };
    }
}
