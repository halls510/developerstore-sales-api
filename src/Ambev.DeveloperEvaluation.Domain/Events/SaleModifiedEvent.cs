using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class SaleModifiedEvent
{
    /// <summary>
    /// The sale that was created.
    /// </summary>
    public Sale Sale { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaleModifiedEvent"/> class.
    /// </summary>
    /// <param name="sale">The updated sale.</param>
    public SaleModifiedEvent(Sale sale)
    {
        Sale = sale ?? throw new ArgumentNullException(nameof(sale), "Sale cannot be null.");
    }
}
