namespace Ambev.DeveloperEvaluation.Domain.Exceptions;

public class ResourceNotFoundException : KeyNotFoundException
{
    public string Error { get; }

    public ResourceNotFoundException(string error, string detail)
        : base(detail)
    {
        Error = error;
    }
}
