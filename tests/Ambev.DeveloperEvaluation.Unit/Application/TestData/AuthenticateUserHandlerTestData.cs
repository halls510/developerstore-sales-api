using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class AuthenticateUserHandlerTestData
{
    private static readonly Faker<AuthenticateUserCommand> authenticateUserFaker = new Faker<AuthenticateUserCommand>()
        .RuleFor(u => u.Email, f => f.Internet.Email())
        .RuleFor(u => u.Password, f => "ValidPassword123!");

    public static AuthenticateUserCommand GenerateValidCommand()
    {
        return authenticateUserFaker.Generate();
    }

    public static User GenerateValidUser(AuthenticateUserCommand command)
    {
        return new User
        {
            Id = new Random().Next(1, 1000),
            Email = command.Email,
            Password = "hashedPassword",
            Username = "TestUser",
            Role = UserRole.Customer,
            Status = UserStatus.Active
        };
    }

    public static User GenerateInactiveUser(AuthenticateUserCommand command)
    {
        return new User
        {
            Id = new Random().Next(1, 1000),
            Email = command.Email,
            Password = "hashedPassword",
            Username = "InactiveUser",
            Role = UserRole.Customer,
            Status = UserStatus.Suspended
        };
    }
}
