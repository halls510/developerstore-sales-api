using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;

namespace Ambev.DeveloperEvaluation.Application.Common.Messaging;

public static class RabbitMqSetup
{
    /// <summary>
    /// Garante que as filas do RabbitMQ sejam criadas antes do Rebus iniciar.
    /// </summary>
    public async static void EnsureRabbitMqQueuesExist(IConfiguration configuration)
    {
        var rabbitHost = configuration["RABBITMQ_HOST"] ?? "ambev.developerevaluation.rabbitmq";
        var rabbitUser = configuration["RABBITMQ_USER"] ?? "admin";
        var rabbitPass = configuration["RABBITMQ_PASS"] ?? "admin";

        var factory = new ConnectionFactory
        {
            HostName = rabbitHost,
            UserName = rabbitUser,
            Password = rabbitPass
        };

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        string[] queues = {
                "queue-sale-created",
                "queue-sale-cancelled",
                "queue-sale-item-cancelled",
                "queue-sale-updated",
                "queue-test"
            };

        foreach (var queue in queues)
        {
            await channel.QueueDeclareAsync(queue, durable: true, exclusive: false, autoDelete: false, arguments: null);
            Console.WriteLine($"✅ Fila criada: {queue}");
        }
    }
}