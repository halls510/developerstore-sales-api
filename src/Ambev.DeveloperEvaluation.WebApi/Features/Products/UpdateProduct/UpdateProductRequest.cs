using Ambev.DeveloperEvaluation.WebApi.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.UpdateProduct
{
    public class UpdateProductRequest
    {    /// <summary>
         /// Gets or sets the title of the product. Must be a non-empty string.
         /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the price of the product. Must be a positive decimal value.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the product description. Should provide detailed information.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the category of the product as a string.
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the image URL of the product.
        /// </summary>
        public string Image { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the rating details of the product.
        /// </summary>
        public RatingRequest Rating { get; set; } = new RatingRequest();
    }
}
