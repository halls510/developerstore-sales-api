using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using System.Web;

namespace Ambev.DeveloperEvaluation.Application.Common.Messaging
{
    public class RabbitMqPublisher : IRabbitMqPublisher, IDisposable
    {
        private readonly IBus _bus;
        private readonly ILogger<RabbitMqPublisher> _logger;

        public RabbitMqPublisher(IConfiguration configuration, ILogger<RabbitMqPublisher> logger)
        {
            _logger = logger;

            var rabbitHost = configuration["RABBITMQ_HOST"] ?? "ambev.developerevaluation.rabbitmq";
            var rabbitUser = configuration["RABBITMQ_USER"] ?? "admin";
            var rabbitPass = configuration["RABBITMQ_PASS"] ?? "admin";

            var encodedUser = HttpUtility.UrlEncode(rabbitUser);
            var encodedPass = HttpUtility.UrlEncode(rabbitPass);

            var rabbitMqConnectionString = $"amqp://{encodedUser}:{encodedPass}@{rabbitHost}";

            _bus = ConfigureRabbitMq(rabbitMqConnectionString);
        }

        private IBus ConfigureRabbitMq(string connectionString)
        {
            _logger.LogInformation("Configurando RabbitMQ com conexão: {connectionString}", connectionString);

            try
            {
                return Configure.With(new BuiltinHandlerActivator())
                    .Transport(t => t.UseRabbitMqAsOneWayClient(connectionString)) // Cliente somente de envio
                    .Routing(r => r.TypeBased()
                        .Map<SaleCreatedEvent>("queue-sale-created")
                        .Map<SaleCancelledEvent>("queue-sale-cancelled")
                        .Map<ItemCancelledEvent>("queue-sale-item-cancelled")
                        .Map<SaleModifiedEvent>("queue-sale-updated"))

                    .Start();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao configurar RabbitMQ.");
                throw;
            }
        }

        /// <summary>
        /// Publica uma mensagem no RabbitMQ no modelo Pub/Sub.
        /// </summary>
        /// <typeparam name="T">Tipo do evento</typeparam>
        /// <param name="message">Mensagem do evento</param>
        public async Task PublishAsync<T>(T message) where T : class
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            try
            {
                _logger.LogInformation("Publicando mensagem do tipo {EventType}: {Message}", typeof(T).Name, message);
                await _bus.Publish(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao publicar mensagem no RabbitMQ.");
                throw;
            }
        }

        /// <summary>
        /// Envia uma mensagem diretamente para a fila sem precisar de um subscriber.
        /// </summary>
        /// <typeparam name="T">Tipo do evento</typeparam>
        /// <param name="message">Mensagem a ser enviada</param>
        public async Task SendAsync<T>(T message) where T : class
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            try
            {
                _logger.LogInformation("Enviando mensagem para fila: {EventType}: {Message}", typeof(T).Name, message);
                await _bus.Send(message); // Envia diretamente para a fila
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar mensagem no RabbitMQ.");
                throw;
            }
        }

        public void Dispose()
        {
            _logger.LogInformation("Liberando recursos do RabbitMQ Publisher.");
            _bus.Dispose();
        }
    }
}
