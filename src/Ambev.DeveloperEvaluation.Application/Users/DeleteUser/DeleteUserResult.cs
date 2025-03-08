using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Application.Users.DeleteUser;

public class DeleteUserResult
{ /// <summary>
  /// The unique identifier of the user
  /// </summary>
    public int Id { get; set; }

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
    /// The user's address.
    /// </summary>
    public AddressResult Address { get; set; } = new AddressResult();

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
