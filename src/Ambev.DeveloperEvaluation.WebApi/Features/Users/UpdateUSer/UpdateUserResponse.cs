using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.WebApi.Common;
using System.Text.Json.Serialization;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.UpdateUser;

/// <summary>
/// API response model for UpdateUser operation.
/// </summary>
public class UpdateUserResponse
{
    /// <summary>
    /// The unique identifier of the updated user.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The user's email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The user's username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The user's full name as an object.
    /// </summary>
    public NameResponse Name { get; set; } = new NameResponse();

    /// <summary>
    /// Gets or sets the user's address.
    /// </summary>
    public AddressResponse Address { get; set; } = new AddressResponse();

    /// <summary>
    /// The user's phone number.
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// The user's role in the system.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserRole Role { get; set; }

    /// <summary>
    /// The current status of the user.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserStatus Status { get; set; }
}