namespace Ambev.DeveloperEvaluation.Application.Common.Messaging;

public interface IRabbitMqPublisher
{
    Task PublishAsync<T>(T message) where T : class;
    Task SendAsync<T>(T message) where T : class;
}

