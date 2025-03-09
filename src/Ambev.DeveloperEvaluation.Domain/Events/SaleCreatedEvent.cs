using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Event triggered when a new sale is created.
/// </summary>
public class SaleCreatedEvent
{
    /// <summary>
    /// The sale that was created.
    /// </summary>
    public Sale Sale { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaleCreatedEvent"/> class.
    /// </summary>
    /// <param name="sale">The created sale.</param>
    public SaleCreatedEvent(Sale sale)
    {
        Sale = sale ?? throw new ArgumentNullException(nameof(sale), "Sale cannot be null.");
    }
}