using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUser;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.ListUsers;

public class ListUsersResponse
{
    /// <summary>
    /// The list of users
    /// </summary>
    public List<GetUserResponse> Users { get; set; } = new List<GetUserResponse>();

    /// <summary>
    /// The total number of users in the system
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// The current page number
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// The total number of pages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// The number of items per page
    /// </summary>
    public int PageSize { get; set; }
}
