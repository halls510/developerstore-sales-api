using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Validation;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Contains unit tests for the User entity class.
/// Tests cover status changes and validation scenarios.
/// </summary>
public class UserTests
{
    /// <summary>
    /// Tests that when a suspended user is activated, their status changes to Active.
    /// </summary>
    [Fact(DisplayName = "User status should change to Active when activated")]
    public void Given_SuspendedUser_When_Activated_Then_StatusShouldBeActive()
    {
        // Arrange
        var user = UserTestData.GenerateValidUser();
        user.Status = UserStatus.Suspended;

        // Act
        user.Activate();

        // Assert
        Assert.Equal(UserStatus.Active, user.Status);
    }

    /// <summary>
    /// Tests that when an active user is suspended, their status changes to Suspended.
    /// </summary>
    [Fact(DisplayName = "User status should change to Suspended when suspended")]
    public void Given_ActiveUser_When_Suspended_Then_StatusShouldBeSuspended()
    {
        // Arrange
        var user = UserTestData.GenerateValidUser();
        user.Status = UserStatus.Active;

        // Act
        user.Suspend();

        // Assert
        Assert.Equal(UserStatus.Suspended, user.Status);
    }

    /// <summary>
    /// Tests that when an active user is deactivated, their status changes to Inactive.
    /// </summary>
    [Fact(DisplayName = "User status should change to Inactive when deactivated")]
    public void Given_ActiveUser_When_Deactivated_Then_StatusShouldBeInactive()
    {
        // Arrange
        var user = UserTestData.GenerateValidUser();
        user.Status = UserStatus.Active;

        // Act
        user.Deactivate();

        // Assert
        user.Status.Should().Be(UserStatus.Inactive);
        user.UpdatedAt.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that validation passes when all user properties are valid.
    /// </summary>
    [Fact(DisplayName = "Validation should pass for valid user data")]
    public void Given_ValidUserData_When_Validated_Then_ShouldReturnValid()
    {
        // Arrange
        var user = UserTestData.GenerateValidUser();

        // Act
        var result = user.Validate();

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    /// <summary>
    /// Tests that validation fails when user properties are invalid.
    /// </summary>
    [Fact(DisplayName = "Validation should fail for invalid user data")]
    public void Given_InvalidUserData_When_Validated_Then_ShouldReturnInvalid()
    {
        // Arrange
        var user = new User
        {
            Username = "", // Invalid: empty
            Password = UserTestData.GenerateInvalidPassword(), // Invalid: doesn't meet password requirements
            Email = UserTestData.GenerateInvalidEmail(), // Invalid: not a valid email
            Phone = UserTestData.GenerateInvalidPhone(), // Invalid: doesn't match pattern
            Status = UserStatus.Unknown, // Invalid: cannot be Unknown
            Role = UserRole.None // Invalid: cannot be None
        };

        // Act
        var result = user.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }

    /// <summary>
    /// Tests that the CreatedAt timestamp is set correctly upon user creation.
    /// </summary>
    [Fact(DisplayName = "User should have CreatedAt timestamp set upon creation")]
    public void Given_NewUser_When_Created_Then_CreatedAtShouldBeSet()
    {
        // Act
        var user = new User();

        // Assert
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    /// <summary>
    /// Tests that an already active user cannot be activated again.
    /// </summary>
    [Fact(DisplayName = "User should not be activated if already active")]
    public void Given_ActiveUser_When_Activated_Then_ShouldThrowException()
    {
        // Arrange
        var user = UserTestData.GenerateValidUser();
        user.Status = UserStatus.Active;

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => user.Activate());
    }

    /// <summary>
    /// Tests that an already suspended user cannot be suspended again.
    /// </summary>
    [Fact(DisplayName = "User should not be suspended if already suspended")]
    public void Given_SuspendedUser_When_Suspended_Then_ShouldThrowException()
    {
        // Arrange
        var user = UserTestData.GenerateValidUser();
        user.Status = UserStatus.Suspended;

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => user.Suspend());
    }

    /// <summary>
    /// Tests that the phone number follows the expected pattern.
    /// </summary>
    [Theory(DisplayName = "Phone number should follow expected pattern")]
    [InlineData("+5511987654321", true)]  // Formato válido
    [InlineData("+19876543210", true)]    // Formato válido com código de país
    [InlineData("11987654321", true)]     // Formato válido sem o '+'
    [InlineData("+55", true)]             // Formato válido, apenas código do país
    [InlineData("+abcdefg", false)]       // Contém caracteres inválidos
    [InlineData("", false)]               // Vazio
    public void Given_PhoneNumber_When_Validated_Then_ShouldFollowPattern(string phone, bool expectedResult)
    {
        // Arrange
        var validator = new PhoneValidator();

        // Act
        var result = validator.Validate(phone);

        // Assert
        result.IsValid.Should().Be(expectedResult);
    }



    /// <summary>
    /// Tests that the user correctly stores and validates Firstname and Lastname.
    /// </summary>
    [Theory(DisplayName = "User should validate Firstname and Lastname correctly")]
    [InlineData("John", "Doe", true)]
    [InlineData("", "Doe", false)] // Nome vazio
    [InlineData("John", "", false)] // Sobrenome vazio
    [InlineData("ANameThatIsWayTooLongForThisFieldAndShouldNotBeValid", "Doe", false)] // Nome muito longo
    public void Given_FirstnameAndLastname_When_Validated_Then_ShouldBeCorrect(string firstname, string lastname, bool expectedIsValid)
    {
        // Arrange
        var user = UserTestData.GenerateValidUser();
        user.Firstname = firstname;
        user.Lastname = lastname;

        // Act
        var result = user.Validate();

        // Assert
        result.IsValid.Should().Be(expectedIsValid);
    }


    /// <summary>
    /// Tests that the user correctly stores and validates Address.
    /// </summary>
    [Theory(DisplayName = "User should validate Address correctly")]
    [InlineData("São Paulo", "Rua A", 123, "01000-000", 12.34, -56.78, true)]
    [InlineData("", "Rua A", 123, "01000-000", 12.34, -56.78, false)] // Cidade vazia
    [InlineData("São Paulo", "", 123, "01000-000", 12.34, -56.78, false)] // Rua vazia
    [InlineData("São Paulo", "Rua A", -1, "01000-000", 12.34, -56.78, false)] // Número inválido
    [InlineData("São Paulo", "Rua A", 123, "01000-000", double.NaN, -56.78, false)] // Latitude inválida
    [InlineData("São Paulo", "Rua A", 123, "01000-000", 12.34, double.NaN, false)] // Longitude inválida
    public void Given_Address_When_Validated_Then_ShouldBeCorrect(string city, string street, int number, string zipcode, double lat, double lng, bool expectedIsValid)
    {
        // Arrange
        var address = new Address
        {
            City = city,
            Street = street,
            Number = number,
            Zipcode = zipcode,
            Geolocation = new Geolocation { Lat = lat, Long = lng }
        };

        //var user = new User { Address = address };

        var user = UserTestData.GenerateValidUser();
        user.Address = address;

        // Act
        var result = user.Validate();

        // Assert
        result.IsValid.Should().Be(expectedIsValid);
    }


    /// <summary>
    /// Tests that the user role is validated correctly.
    /// </summary>
    [Theory(DisplayName = "User role should be valid and not None")]
    [InlineData(UserRole.Admin, true)]
    [InlineData(UserRole.Manager, true)]
    [InlineData(UserRole.Customer, true)]
    [InlineData(UserRole.None, false)] // Inválido
    public void Given_UserRole_When_Validated_Then_ShouldBeCorrect(UserRole role, bool expectedIsValid)
    {
        // Arrange
        var user = UserTestData.GenerateValidUser();
        user.Role = role;

        // Act
        var result = user.Validate();

        // Assert
        result.IsValid.Should().Be(expectedIsValid);
    }
}
