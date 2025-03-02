using Ambev.DeveloperEvaluation.Domain.Enums;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ambev.DeveloperEvaluation.Application.Users.GetUser;

/// <summary>
/// Response model for GetUser operation
/// </summary>
public class GetUserResult
{
    /// <summary>
    /// The unique identifier of the user
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The user's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The user's username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The user's full name as an object.
    /// </summary>
    public NameResult Name { get; set; } = new NameResult();

    /// <summary>
    /// The user's phone number
    /// </summary>
    public string Phone { get; set; } = string.Empty;   

    /// <summary>
    /// The user's role in the system
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// The current status of the user
    /// </summary>
    public UserStatus Status { get; set; }
}

/// <summary>
/// Represents the user's name structure in the result model.
/// </summary>
public class NameResult
{
    public string Firstname { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;
}