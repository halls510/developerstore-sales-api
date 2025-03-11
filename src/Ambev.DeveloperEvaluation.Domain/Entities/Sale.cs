using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a sale transaction in the system.
/// </summary>
public class Sale : BaseEntity
{
    /// <summary>
    /// Gets or sets the sale number (unique identifier for the sale).
    /// </summary>
    public string SaleNumber { get; private set; }

    /// <summary>
    /// Gets or sets the date when the sale was completed.
    /// </summary>
    public DateTime SaleDate { get; private set; }

    /// <summary>
    /// Gets or sets the external user identifier for the client who made the purchase.
    /// </summary>
    public int CustomerId { get; private set; }

    /// <summary>
    /// Gets or sets the denormalized customer name.
    /// </summary>
    public string CustomerName { get; set; }

    /// <summary>
    /// Gets or sets the branch where the sale was made.
    /// </summary>
    public string Branch { get; private set; }

    /// <summary>
    /// Gets or sets the list of products sold in this sale.
    /// </summary>
    public List<SaleItem> Items { get; set; }

    /// <summary>
    /// Gets the total value of the sale.
    /// </summary>
    public Money TotalValue { get; set; }

    /// <summary>
    /// Gets or sets the sale status.
    /// </summary>
    public SaleStatus Status { get; private set; }

    public Sale() {
        SaleNumber = GenerateSaleNumber();
        Items = new List<SaleItem>();
        SaleDate = DateTime.UtcNow;
        Status = SaleStatus.Pending;
        AssignBranch(); // 🔹 Chamar método para definir a filial automaticamente
    }

    /// <summary>
    /// Constructor for creating a new sale.
    /// </summary>    
    /// <param name="customerId">Customer's external ID</param>
    /// <param name="customerName">Denormalized customer name</param>
    /// <param name="branch">Branch where the sale was made</param>
    /// <param name="items">List of items in the sale</param>
    public Sale(int customerId, string customerName, string branch, List<SaleItem> items)
    {       
        SaleNumber = GenerateSaleNumber();
        SaleDate = DateTime.UtcNow;
        CustomerId = customerId;
        CustomerName = customerName ?? throw new ArgumentNullException(nameof(customerName));
        Branch = branch ?? throw new ArgumentNullException(nameof(branch));
        Items = items ?? new List<SaleItem>();
        Status = SaleStatus.Pending;
        AssignBranch(); // 🔹 Chamar método para definir a filial automaticamente
    }

    private string GenerateSaleNumber()
    {
        return $"SALE-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid().ToString("N").Substring(0, 6)}";
    }

    /// <summary>
    /// Calculates the total value of the sale.
    /// </summary>
    private decimal CalculateTotalPrice()
    {
        decimal total = 0;
        foreach (var item in Items)
        {
            total += item.Total.Amount;
        }
        return total;
    }

    /// <summary>
    /// Cancels the sale if it is in a pending state.
    /// </summary>
    public void Cancel()
    {
        if (Status == SaleStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed sale.");

        Status = SaleStatus.Cancelled;

        // 🔹 Cancelar apenas os itens ativos
        foreach (var item in Items)
        {
            if (item.Status == SaleItemStatus.Active)  // Só cancela itens ativos
            {
                item.Cancel();
            }
        }
    }

    /// <summary>
    /// Checks if the sale has been canceled.
    /// </summary>
    /// <returns>True if the sale is canceled, otherwise false.</returns>
    public bool IsCancelled()
    {
        return Status == SaleStatus.Cancelled;
    }

    private void AssignBranch()
    {
        // 🔹 Simulando uma lista de filiais disponíveis (poderia vir de um serviço externo)
        var availableBranches = new List<string> { "Filial A", "Filial B", "Filial C", "Filial D" };

        // 🔹 Selecionar aleatoriamente uma filial
        var random = new Random();
        Branch = availableBranches[random.Next(availableBranches.Count)];
    }


}