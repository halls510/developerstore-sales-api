using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events;

/// <summary>
/// Handles the SaleCreatedEvent.
/// </summary>
public class SaleCreatedEventHandler : IHandleMessages<SaleCreatedEvent>
{
    private readonly ILogger<SaleCreatedEventHandler> _logger;

    public SaleCreatedEventHandler(ILogger<SaleCreatedEventHandler> logger)
    {
        _logger = logger;
        Console.WriteLine("🟢 SaleCreatedEventHandler registrado!");
    }

    public async Task Handle(SaleCreatedEvent message)
    {
        _logger.LogInformation($" Venda Criada: ID {message.Sale.SaleNumber} - Cliente: {message.Sale.CustomerName}");
        await Task.CompletedTask;
    }
}
