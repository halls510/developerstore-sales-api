using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Users.ListUsers;

/// <summary>
/// Command for listing users with filters
/// </summary>
public class ListUsersCommand : IRequest<ListUsersResult>
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? OrderBy { get; set; }
}
