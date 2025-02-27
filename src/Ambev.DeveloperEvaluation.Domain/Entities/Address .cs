using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents an address associated with a user.
/// </summary>
public class Address : BaseEntity
{
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public int Number { get; set; }
    public string Zipcode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the geolocation details.
    /// </summary>
    public Geolocation Geolocation { get; set; } = new Geolocation();
}

/// <summary>
/// Represents geolocation data for an address.
/// </summary>
public class Geolocation
{
    public string Lat { get; set; } = string.Empty;
    public string Long { get; set; } = string.Empty;
}
