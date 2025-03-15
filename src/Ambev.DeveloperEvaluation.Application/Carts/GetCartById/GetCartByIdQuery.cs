using Ambev.DeveloperEvaluation.Application.Carts.Common;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Carts.GetCartById;

/// <summary>
/// Query to retrieve a cart by its ID
/// </summary>
public record GetCartByIdQuery : IRequest<CartDto>
{
    /// <summary>
    /// The unique identifier of the cart
    /// </summary>
    public int Id { get; init; }

    public GetCartByIdQuery() { }

    public GetCartByIdQuery(int id)
    {
        Id = Id;
    }
}