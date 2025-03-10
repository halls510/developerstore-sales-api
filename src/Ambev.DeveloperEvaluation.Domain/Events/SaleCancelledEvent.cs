using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class SaleCancelledEvent
{
    /// <summary>
    /// The sale that was created.
    /// </summary>
    public Sale Sale { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaleCreatedEvent"/> class.
    /// </summary>
    /// <param name="sale">The cancelled sale.</param>
    public SaleCancelledEvent(Sale sale)
    {
        Sale = sale ?? throw new ArgumentNullException(nameof(sale), "Sale cannot be null.");
    }
}
