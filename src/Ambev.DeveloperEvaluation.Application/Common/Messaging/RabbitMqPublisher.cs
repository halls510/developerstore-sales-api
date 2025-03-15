using Ambev.DeveloperEvaluation.Domain.Events;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using System.Web;

namespace Ambev.DeveloperEvaluation.Application.Common.Messaging;

/// <summary>
/// Publicador de eventos no RabbitMQ via Rebus.
/// </summary>
public class RabbitMqPublisher : IDisposable
{
    private readonly IBus _bus;

    public RabbitMqPublisher()
    {
        // 🔹 Obtém variáveis de ambiente para o RabbitMQ
        var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "ambev.developerevaluation.rabbitmq";
        var rabbitUser = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "admin";
        var rabbitPass = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? "admin";

        // 🔹 Codifica a senha e o usuário para evitar problemas com caracteres especiais
        var encodedUser = HttpUtility.UrlEncode(rabbitUser);
        var encodedPass = HttpUtility.UrlEncode(rabbitPass);

        // 🔹 Monta a string de conexão para o RabbitMQ
        var rabbitMqConnectionString = $"amqp://{encodedUser}:{encodedPass}@{rabbitHost}";

        // 🔹 Configura o Rebus como Publisher
        _bus = Configure.With(new BuiltinHandlerActivator())
            .Transport(t => t.UseRabbitMq(rabbitMqConnectionString, "queue-sale"))
            .Routing(r => r.TypeBased()
                .Map<SaleCreatedEvent>("queue-sale-created")
                .Map<SaleCancelledEvent>("queue-sale-cancelled")
                .Map<ItemCancelledEvent>("queue-sale-item-cancelled")
                .Map<SaleModifiedEvent>("queue-sale-modified")
                .Map<TestEvent>("queue-test"))
            .Start();
    }

    /// <summary>
    /// Publica uma mensagem no RabbitMQ.
    /// </summary>
    /// <typeparam name="T">Tipo do evento</typeparam>
    /// <param name="message">Mensagem do evento</param>
    public async Task PublishAsync<T>(T message) where T : class
    {
        if (message == null) throw new ArgumentNullException(nameof(message));
        await _bus.Publish(message);
    }

    /// <summary>
    /// Libera os recursos do Rebus.
    /// </summary>
    public void Dispose()
    {
        _bus.Dispose();
    }
}
