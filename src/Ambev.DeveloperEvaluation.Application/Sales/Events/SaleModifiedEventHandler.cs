using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events;

public class SaleModifiedEventHandler : IHandleMessages<SaleModifiedEvent>
{
    private readonly ILogger<SaleModifiedEventHandler> _logger;

    public SaleModifiedEventHandler(ILogger<SaleModifiedEventHandler> logger)
    {
        _logger = logger;        
    }

    public async Task Handle(SaleModifiedEvent message)
    {
        _logger.LogInformation($" Venda Modificado: ID {message.Sale.SaleNumber} - Cliente: {message.Sale.CustomerName}");
        await Task.CompletedTask;
    }
}