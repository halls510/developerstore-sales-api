using Ambev.DeveloperEvaluation.Application.Users.ListUsers;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class ListUsersHandlerTestData
{
    private static readonly Faker<ListUsersCommand> listUsersHandlerFaker = new Faker<ListUsersCommand>()
        .RuleFor(c => c.Page, f => f.Random.Int(1, 10))
        .RuleFor(c => c.Size, f => f.Random.Int(5, 50))
        .RuleFor(c => c.OrderBy, f => f.Lorem.Word());
       // .RuleFor(c => c.Filters, f => new Dictionary<string, string> { { "username", f.Internet.UserName() } });

    public static ListUsersCommand GenerateValidCommand()
    {
        return listUsersHandlerFaker.Generate();
    }

    public static List<User> GenerateUsersList()
    {
        return new List<User>
            {
                new User { Id = 1, Username = "User1", Email = "user1@example.com" },
                new User { Id = 2, Username = "User2", Email = "user2@example.com" }
            };
    }
}
