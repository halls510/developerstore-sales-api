using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class ItemCancelledEvent
{
    /// <summary>
    /// The sale that was created.
    /// </summary>
    public SaleItem SaleItem { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaleCreatedEvent"/> class.
    /// </summary>
    /// <param name="sale">The cancelled sale.</param>
    public ItemCancelledEvent(SaleItem saleItem)
    {
        SaleItem = saleItem ?? throw new ArgumentNullException(nameof(saleItem), "Sale Item cannot be null.");
    }
}
