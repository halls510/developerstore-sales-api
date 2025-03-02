namespace Ambev.DeveloperEvaluation.WebApi.Common
{
    /// <summary>
    /// Represents the name of a user.
    /// </summary>
    public class NameRequest
    {
        public string Firstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
    }
}
