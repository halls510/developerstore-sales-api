using Ambev.DeveloperEvaluation.Application.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Common;

/// <summary>
/// Response model for address details.
/// </summary>
public class AddressResponse
{
    /// <summary>
    /// Gets or sets the city name.
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the street name.
    /// </summary>
    public string Street { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the house number.
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    /// Gets or sets the postal code.
    /// </summary>
    public string Zipcode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the geolocation.
    /// </summary>
    public GeoLocationResponse Geolocation { get; set; } = new GeoLocationResponse();
}

/// <summary>
/// Response model for geolocation coordinates.
/// </summary>
public class GeoLocationResponse
{
    /// <summary>
    /// Gets or sets the latitude.
    /// </summary>
    public string Lat { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the longitude.
    /// </summary>
    public string Long { get; set; } = string.Empty;
}