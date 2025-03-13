using Ambev.DeveloperEvaluation.Domain.Events;
using Rebus.Handlers;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events;

public class TestEventHandler : IHandleMessages<TestEvent>
{
    public async Task Handle(TestEvent message)
    {
        Console.WriteLine($"📩 TestEvent recebido: {message}");
        await Task.CompletedTask;
    }
}