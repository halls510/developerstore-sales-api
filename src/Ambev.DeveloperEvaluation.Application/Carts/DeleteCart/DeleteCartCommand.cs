using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Carts.DeleteCart;

/// <summary>
/// Command for deleting a cart
/// </summary>
public record DeleteCartCommand : IRequest<DeleteCartResult>
{
    /// <summary>
    /// The unique identifier of the cart to delete
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Initializes a new instance of DeleteCartCommand
    /// </summary>
    /// <param name="id">The ID of the cart to delete</param>
    public DeleteCartCommand(int id)
    {
        Id = id;
    }
}
