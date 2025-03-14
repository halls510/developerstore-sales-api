using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Validation;
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
    public int CustomerId { get; set; }

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

    public Sale()
    {
        SaleNumber = GenerateSaleNumber();
        Items = new List<SaleItem>();
        SaleDate = DateTime.UtcNow;
        Status = SaleStatus.Pending;
        AssignBranch(); // Chamar método para definir a filial automaticamente
    }

    /// <summary>
    /// Constructor for creating a new sale.
    /// </summary>    
    /// <param name="customerId">Customer's external ID</param>
    /// <param name="customerName">Denormalized customer name</param>
    public Sale(int customerId, string customerName)
    {
        SaleNumber = GenerateSaleNumber();
        SaleDate = DateTime.UtcNow;
        CustomerId = customerId;
        CustomerName = customerName ?? throw new ArgumentNullException(nameof(customerName));
        Items = new List<SaleItem>();
        Status = SaleStatus.Pending;
        AssignBranch(); // Chamar método para definir a filial automaticamente
    }

    public void AddItems(List<SaleItem> saleItems)
    {
        Items.AddRange(saleItems);
    }

    public void AddItem(SaleItem saleItem)
    {
        Items.Add(saleItem);
    }

    public void RecalculateTotal()
    {
        if (!Items.Any())
        {
            TotalValue = new Money(0);
            return;
        }

        TotalValue = Items.Where(item => item.Status == SaleItemStatus.Active)
                          .Sum(item => item.Total);
    }


    private string GenerateSaleNumber()
    {
        return $"SALE-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid().ToString("N").Substring(0, 6)}";
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

    /// <summary>
    /// Completes the sale if it is in a pending state and has active items.
    /// </summary>
    public void CompleteSale()
    {
        if (Status != SaleStatus.Pending)
            throw new InvalidOperationException("Only pending sales can be completed.");

        if (!Items.Any(item => item.Status == SaleItemStatus.Active))
            throw new InvalidOperationException("Cannot complete a sale with no active items.");

        Status = SaleStatus.Completed;
    }

    private void AssignBranch()
    {
        // Lista de filiais (CDDs) da Ambev
        var availableBranches = new List<string>
        {
            "CDD Moema (SP)", "CDD Mooca (SP)", "CDD Capital (SP)", "CDD Litoral (SP)",
            "CDD Piracicaba (SP)", "CDD Salto (SP)", "CDD Contagem (MG)", "CDD Sete Lagoas (MG)",
            "CDD Nova Rio (RJ)", "CDD Brasília (DF)", "CDD Gama (DF)", "CDD Olinda (PE)",
            "CDD Cabo de Santo Agostinho (PE)", "CDD Caruaru (PE)", "CDD Salgueiro (PE)",
            "CDD Belém (PA)", "CDD Manaus (AM)", "CDD Natal (RN)", "CDD Paraíba (PB)",
            "CDD Equatorial (MA)", "CDD Sergipe (SE)", "Sala de Vendas Campo Grande (MS)"
        };

        // Selecionar aleatoriamente uma filial
        var random = new Random();
        Branch = availableBranches[random.Next(availableBranches.Count)];
    }

    public ValidationResultDetail Validate()
    {
        var validator = new SaleValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}