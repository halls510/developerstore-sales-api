using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Ambev.DeveloperEvaluation.Application.Products.DeleteProduct;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Application.Products.ListCategories;
using Ambev.DeveloperEvaluation.Application.Products.ListProducts;
using Ambev.DeveloperEvaluation.Application.Products.ListProductsByCategory;
using Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.DeleteProduct;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.GetProduct;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.ListCategories;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.ListProducts;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.ListProductsByCategory;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.UpdateProduct;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products;

/// <summary>
/// Controller for managing product operations
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class ProductsController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of ProductsController
    /// </summary>
    /// <param name="mediator">The mediator instance</param>
    /// <param name="mapper">The AutoMapper instance</param>
    public ProductsController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves a paginated list of products with optional filters and ordering.
    /// </summary>
    /// <param name="_page">Page number for pagination (default: 1) (optional)</param>
    /// <param name="_size">Number of items per page (default: 10) (optional)</param>
    /// <param name="_order">Ordering of results (e.g., "title asc, category desc") (optional)</param>
    /// <param name="filters">Filtering parameters (optional, e.g., "category=men's clothing","title=Fjallraven*").</param>    
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of products</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<GetProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListProducts(
        [FromQuery] int? _page = null,
        [FromQuery] int? _size = null,
        [FromQuery] string? _order = null,
        [FromQuery] Dictionary<string, string[]>? filters = null,
        CancellationToken cancellationToken = default)
    {
        var request = new ListProductsRequest
        {
            Page = _page ?? 1,
            Size = _size ?? 10,
            OrderBy = _order,
            Filters = filters ?? new Dictionary<string, string[]>()
        };

        var validator = new ListProductsRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<ListProductsCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        var paginatedList = new PaginatedList<GetProductResponse>(
            _mapper.Map<List<GetProductResponse>>(response.Products),
            response.TotalItems,
            response.CurrentPage,
            response.PageSize
         );

        return OkPaginated(paginatedList);
    }

    /// <summary>
    /// Creates a new product
    /// </summary>
    /// <param name="request">The product creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created product details</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<CreateProductResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var validator = new CreateProductRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<CreateProductCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        return Created(string.Empty, new ApiResponseWithData<CreateProductResponse>
        {
            Success = true,
            Message = "Product created successfully",
            Data = _mapper.Map<CreateProductResponse>(response)
        });
    }

    /// <summary>
    /// Updates an existing product
    /// </summary>
    /// <param name="id">The unique identifier of the product to update</param>
    /// <param name="request">The product update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated product details</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<UpdateProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var validator = new UpdateProductRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<UpdateProductCommand>(request);
        command.Id = id;
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<UpdateProductResponse>
        {
            Success = true,
            Message = "Product updated successfully",
            Data = _mapper.Map<UpdateProductResponse>(response)
        });
    }

    /// <summary>
    /// Retrieves a product by their ID
    /// </summary>
    /// <param name="id">The unique identifier of the product</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The product details if found</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProduct([FromRoute] int id, CancellationToken cancellationToken)
    {
        var request = new GetProductRequest { Id = id };
        var validator = new GetProductRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<GetProductCommand>(request.Id);
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<GetProductResponse>
        {
            Success = true,
            Message = "User retrieved successfully",
            Data = _mapper.Map<GetProductResponse>(response)
        });
    }

    /// <summary>
    /// Deletes a product by their ID
    /// </summary>
    /// <param name="id">The unique identifier of the product to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response if the product was deleted</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<DeleteProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct([FromRoute] int id, CancellationToken cancellationToken)
    {
        var request = new DeleteProductRequest { Id = id };
        var validator = new DeleteProductRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<DeleteProductCommand>(request.Id);
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<DeleteProductResponse>
        {
            Success = true,
            Message = "Product deleted successfully",
            Data = _mapper.Map<DeleteProductResponse>(response)
        });
    }

    /// <summary>
    /// Retrieves a paginated list of categories with optional ordering.
    /// </summary>
    /// <param name="_page">Page number for pagination (default: 1)</param>
    /// <param name="_size">Number of items per page (default: 10)</param>
    /// <param name="_order">Ordering of results (e.g., "name asc")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of categories</returns>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(PaginatedList<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListCategories(
        [FromQuery] int _page = 1,
        [FromQuery] int _size = 10,
        [FromQuery] string? _order = null,
        CancellationToken cancellationToken = default)
    {
        var request = new ListCategoriesRequest
        {
            Page = _page,
            Size = _size,
            OrderBy = _order
        };

        var validator = new ListCategoriesRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<ListCategoriesCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        var paginatedList = new PaginatedList<string>(
            response.Categories,
            response.TotalItems,
            response.CurrentPage,
            response.PageSize
        );

        return OkPaginated(paginatedList);
    }

    /// <summary>
    /// Retrieves a paginated list of products in a specific category.
    /// </summary>
    /// <param name="category">The category name used to filter products.</param>
    /// <param name="_page">Page number for pagination (default: 1).</param>
    /// <param name="_size">Number of items per page (default: 10).</param>
    /// <param name="_order">Optional sorting criteria (e.g., "title asc, price desc").</param>
    /// <param name="cancellationToken">Cancellation token for async operation control.</param>
    /// <returns>
    /// Returns a paginated list of products within the specified category.
    /// If validation fails, returns a 400 Bad Request response with error details.
    /// </returns>
    [HttpGet("category/{category}")]
    [ProducesResponseType(typeof(PaginatedList<GetProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProductsByCategory(
        [FromRoute] string category,
        [FromQuery] int _page = 1,
        [FromQuery] int _size = 10,
        [FromQuery] string? _order = null,
        CancellationToken cancellationToken = default)
    {
        var request = new ListProductsByCategoryRequest
        {
            CategoryName = category,
            Page = _page,
            Size = _size,
            OrderBy = _order
        };

        var validator = new ListProductsByCategoryRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<ListProductsByCategoryCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        var paginatedList = new PaginatedList<GetProductResponse>(
            _mapper.Map<List<GetProductResponse>>(response.Products),
            response.TotalItems,
            response.CurrentPage,
            response.PageSize
        );

        return OkPaginated(paginatedList);
    }
}
