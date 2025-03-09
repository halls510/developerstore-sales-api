using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events;

/// <summary>
/// Handles the SaleCancelledEvent.
/// </summary>
public class SaleCancelledEventHandler : IHandleMessages<SaleCancelledEvent>
{
    private readonly ILogger<SaleCancelledEventHandler> _logger;

    public SaleCancelledEventHandler(ILogger<SaleCancelledEventHandler> logger)
    {
        _logger = logger;
        Console.WriteLine("🟢 SaleCancelledEventHandler registrado!");
    }

    public async Task Handle(SaleCancelledEvent message)
    {
        _logger.LogInformation($" Venda Criada: ID {message.Sale.SaleNumber} - Cliente: {message.Sale.CustomerName}");
        await Task.CompletedTask;
    }
}
