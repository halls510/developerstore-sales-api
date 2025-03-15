using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class TestEvent
{
    public string Message { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestEvent"/> class.
    /// </summary>
    /// <param name="sale">The message.</param>
    public TestEvent(string message)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message), "Message cannot be null.");
    }
}
