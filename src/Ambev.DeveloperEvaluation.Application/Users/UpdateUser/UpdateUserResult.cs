namespace Ambev.DeveloperEvaluation.Application.Users.UpdateUser;

/// <summary>
/// Result of updating a user.
/// </summary>
public class UpdateUserResult
{
    public Guid Id { get; }

    public UpdateUserResult(Guid id)
    {
        Id = id;
    }
}
