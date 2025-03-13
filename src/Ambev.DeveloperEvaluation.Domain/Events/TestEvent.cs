namespace Ambev.DeveloperEvaluation.Domain.Events;

public class TestEvent
{
    /// <summary>
    /// The Test that was created.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestEvent"/> class.
    /// </summary>
    /// <param name="message">The Message.</param>
    public TestEvent(string message)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message), "Message cannot be null.");
    }
}
