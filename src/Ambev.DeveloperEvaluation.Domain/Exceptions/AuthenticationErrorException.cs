using System.Security.Authentication;

namespace Ambev.DeveloperEvaluation.Domain.Exceptions;

public class AuthenticationErrorException : AuthenticationException
{
    public string Error { get; }

    public AuthenticationErrorException(string error, string detail)
        : base(detail)
    {
        Error = error;
    }
}
