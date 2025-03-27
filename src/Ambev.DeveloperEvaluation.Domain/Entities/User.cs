using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Validation;
using System.Security.Cryptography;
using System.Text;

namespace Ambev.DeveloperEvaluation.Domain.Entities;


/// <summary>
/// Represents a product in the system.
/// </summary>
public class User : BaseEntity, IUser
{
    /// <summary>
    /// Gets or sets the user's first name.
    /// </summary>
    public string Firstname { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's last name.
    /// </summary>
    public string Lastname { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's address.
    /// </summary>
    public Address Address { get; set; } = new Address();  // Adicionando o Address

    /// <summary>
    /// Gets the user's full name.
    /// Must not be null or empty and should contain both first and last names.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets the user's email address.
    /// Must be a valid email format and is used as a unique identifier for authentication.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets the user's phone number.
    /// Must be a valid phone number format following the pattern (XX) XXXXX-XXXX.
    /// </summary>
    public string Phone { get; set; } = string.Empty ;

    /// <summary>
    /// Gets the hashed password for authentication.
    /// Password must meet security requirements: minimum 8 characters, at least one uppercase letter,
    /// one lowercase letter, one number, and one special character.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets the user's role in the system.
    /// Determines the user's permissions and access levels.
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// Gets the user's current status.
    /// Indicates whether the user is active, inactive, or blocked in the system.
    /// </summary>
    public UserStatus Status { get; set; }

    /// <summary>
    /// Gets the date and time when the user was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets the date and time of the last update to the user's information.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets the unique identifier of the user.
    /// </summary>
    /// <returns>The user's ID as a string.</returns>
    string IUser.Id => Id.ToString();

    /// <summary>
    /// Gets the username.
    /// </summary>
    /// <returns>The username.</returns>
    string IUser.Username => Username;

    /// <summary>
    /// Gets the user's role in the system.
    /// </summary>
    /// <returns>The user's role as a string.</returns>
    string IUser.Role => Role.ToString();

    // Campo para armazenar o hash das informações do usuário
    public string LastHash { get; private set; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of the User class.
    /// </summary>
    public User()
    {
        CreatedAt = DateTime.UtcNow;
        LastHash = CalculateHash(); // Define o hash inicial na criação do usuário
    }

    /// <summary>
    /// Performs validation of the user entity using the UserValidator rules.
    /// </summary>
    /// <returns>
    /// A <see cref="ValidationResultDetail"/> containing:
    /// - IsValid: Indicates whether all validation rules passed
    /// - Errors: Collection of validation errors if any rules failed
    /// </returns>
    /// <remarks>
    /// <listheader>The validation includes checking:</listheader>
    /// <list type="bullet">Username format and length</list>
    /// <list type="bullet">Email format</list>
    /// <list type="bullet">Phone number format</list>
    /// <list type="bullet">Password complexity requirements</list>
    /// <list type="bullet">Role validity</list>
    /// 
    /// </remarks>
    public ValidationResultDetail Validate()
    {
        var validator = new UserValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }

    /// <summary>
    /// Calcula um hash baseado nos dados do usuário.
    /// </summary>
    public string CalculateHash()
    {
        using var sha256 = SHA256.Create();

        var rawData = $"{Firstname}{Lastname}{Email}{Phone}{Role}{Status}" +
                      $"{Address.City}{Address.Street}{Address.Number}{Address.Zipcode}" +
                      $"{Address.Geolocation.Lat}{Address.Geolocation.Long}";

        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Atualiza o hash após uma modificação.
    /// </summary>
    public void UpdateHash()
    {
        LastHash = CalculateHash();
    }

    /// <summary>
    /// Activates the user account if it is not already active.
    /// Changes the user's status to Active.
    /// </summary>
    public void Activate()
    {
        if (Status == UserStatus.Active)
        {
            throw new InvalidOperationException("User is already active.");
        }

        Status = UserStatus.Active;
        UpdatedAt = DateTime.UtcNow;
        UpdateHash(); // Atualiza o hash quando o status é alterado
    }

    /// <summary>
    /// Deactivates the user account if it is not already inactive.
    /// Changes the user's status to Inactive.
    /// </summary>
    public void Deactivate()
    {
        if (Status == UserStatus.Inactive)
        {
            throw new InvalidOperationException("User is already inactive.");
        }

        Status = UserStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
        UpdateHash(); // Atualiza o hash quando o status é alterado
    }

    /// <summary>
    /// Suspends the user account if it is not already suspended.
    /// Changes the user's status to Suspended.
    /// </summary>
    public void Suspend()
    {
        if (Status == UserStatus.Suspended)
        {
            throw new InvalidOperationException("User is already suspended.");
        }

        Status = UserStatus.Suspended;
        UpdatedAt = DateTime.UtcNow;
        UpdateHash(); // Atualiza o hash quando o status é alterado
    }


}