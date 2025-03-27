namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents an address associated with a user.
/// </summary>
public class Address
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
    /// <summary>
    /// Latitude of the address.
    /// Must be a valid coordinate between -90 and 90.
    /// </summary>
    public double Lat { get; set; } = 0.0;

    /// <summary>
    /// Longitude of the address.
    /// Must be a valid coordinate between -180 and 180.
    /// </summary>
    public double Long { get; set; } = 0.0;
}
