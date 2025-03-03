namespace Ambev.DeveloperEvaluation.Application.Users.UpdateUser;

/// <summary>
/// Result of updating a user.
/// </summary>
public class UpdateUserResult
{
    public int Id { get; }

    public UpdateUserResult(int id)
    {
        Id = id;
    }
}
