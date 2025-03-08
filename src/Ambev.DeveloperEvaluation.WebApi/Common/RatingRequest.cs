namespace Ambev.DeveloperEvaluation.WebApi.Common;

/// <summary>
/// Represents the rating details of a product in the request.
/// </summary>
public class RatingRequest
{
    /// <summary>
    /// Gets or sets the average rating of the product.
    /// </summary>
    public double Rate { get; set; }

    /// <summary>
    /// Gets or sets the count of total ratings received.
    /// </summary>
    public int Count { get; set; }
}
