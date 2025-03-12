using Ambev.DeveloperEvaluation.Application.Sales.CancelItem;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSaleById;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelItem;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSaleById;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SalesController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of SalesController
    /// </summary>
    /// <param name="mediator">The mediator instance</param>
    /// <param name="mapper">The AutoMapper instance</param>
    public SalesController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves a paginated list of sales with optional filters and ordering.
    /// </summary>
    /// <param name="_page">Page number for pagination (default: 1) (optional)</param>
    /// <param name="_size">Number of items per page (default: 10) (optional)</param>
    /// <param name="_order">Ordering of results (e.g., "totalAmount desc, date asc") (optional)</param>
    /// <param name="filters">Filtering parameters (optional, e.g., "_minDate=2025-02-04, _maxDate=2025-03-04").</param>    
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of sales</returns>
    [HttpGet]
    [Authorize(Roles = "Admin,Manager,Customer")]
    [ProducesResponseType(typeof(PaginatedList<GetSaleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListSales(
        [FromQuery] int? _page = null,
        [FromQuery] int? _size = null,
        [FromQuery] string? _order = null,
        [FromQuery] Dictionary<string, string[]>? filters = null,
        CancellationToken cancellationToken = default)
    {
        var userRole = User.FindFirst("role")?.Value;
        var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");

        // Se o usuário for Customer, ele só pode ver as próprias vendas
        if (userRole.Equals("Customer", StringComparison.OrdinalIgnoreCase))
        {
            filters ??= new Dictionary<string, string[]>();
            filters["UserId"] = new[] { userId.ToString() };
        }

        var request = new ListSalesRequest
        {
            Page = _page ?? 1,
            Size = _size ?? 10,
            OrderBy = _order,
            Filters = filters ?? new Dictionary<string, string[]>()
        };

        var validator = new ListSalesRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<ListSalesCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        var paginatedList = new PaginatedList<GetSaleResponse>(
            _mapper.Map<List<GetSaleResponse>>(response.Sales),
            response.TotalItems,
            response.CurrentPage,
            response.PageSize
        );

        return OkPaginated(paginatedList);
    }

    /// <summary>
    /// Retrieves a sale by their ID
    /// </summary>
    /// <param name="id">The unique identifier of the sale</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sale details if found</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetSaleByIdResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSale([FromRoute] int id, CancellationToken cancellationToken)
    {
        var userRole = User.FindFirst("role")?.Value;
        var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");

        // Obtém a venda pelo ID antes de retornar
        var sale = await _mediator.Send(new GetSaleByIdQuery { Id = id }, cancellationToken);

        if (sale == null)
        {
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = "Sale not found."
            });
        }

        // Se não for Admin ou Manager, o usuário só pode visualizar sua própria venda
        bool isAdminOrManager = userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                                userRole.Equals("Manager", StringComparison.OrdinalIgnoreCase);

        if (!isAdminOrManager && sale.CustomerId != userId)
        {
            return Forbid(); // 403 Forbidden - Apenas Admins e Managers podem visualizar qualquer venda
        }

        return Ok(new ApiResponseWithData<GetSaleByIdResponse>
        {
            Success = true,
            Message = "Sale retrieved successfully",
            Data = _mapper.Map<GetSaleByIdResponse>(sale)
        });
    }

    /// <summary>
    /// Updates an existing sale
    /// </summary>
    /// <param name="id">The unique identifier of the sale to update</param>
    /// <param name="request">The sale update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated sale details</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager,Customer")] // Apenas usuários autenticados podem atualizar vendas
    [ProducesResponseType(typeof(ApiResponseWithData<UpdateSaleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSale([FromRoute] int id, [FromBody] UpdateSaleRequest request, CancellationToken cancellationToken)
    {
        var userRole = User.FindFirst("role")?.Value;
        var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");

        // Obtém a venda pelo ID antes de atualizar
        var sale = await _mediator.Send(new GetSaleByIdQuery { Id = id }, cancellationToken);

        if (sale == null)
        {
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = "Sale not found."
            });
        }

        // Se não for Admin ou Manager, o usuário só pode atualizar sua própria venda
        bool isAdminOrManager = userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                                userRole.Equals("Manager", StringComparison.OrdinalIgnoreCase);

        if (!isAdminOrManager && sale.CustomerId != userId)
        {
            return Forbid(); // 403 Forbidden - Apenas Admins e Managers podem modificar qualquer venda
        }

        var validator = new UpdateSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<UpdateSaleCommand>(request);
        command.Id = id;
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<UpdateSaleResponse>
        {
            Success = true,
            Message = "Sale updated successfully",
            Data = _mapper.Map<UpdateSaleResponse>(response)
        });
    }

    /// <summary>
    /// Cancel a sale by their ID
    /// </summary>
    /// <param name="id">The unique identifier of the sale to canceled</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response if the sale was canceld</returns>
    [HttpPatch("{id}/cancel")]
    [Authorize(Roles = "Admin,Manager,Customer")] // Apenas usuários autenticados podem cancelar vendas
    [ProducesResponseType(typeof(ApiResponseWithData<CancelSaleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelSale([FromRoute] int id, CancellationToken cancellationToken)
    {
        var userRole = User.FindFirst("role")?.Value;
        var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");

        // Obtém a venda antes de cancelar
        var sale = await _mediator.Send(new GetSaleByIdQuery { Id = id }, cancellationToken);

        if (sale == null)
        {
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = "Sale not found."
            });
        }

        // Se não for Admin ou Manager, o usuário só pode cancelar a própria venda
        bool isAdminOrManager = userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                                userRole.Equals("Manager", StringComparison.OrdinalIgnoreCase);

        if (!isAdminOrManager && sale.CustomerId != userId)
        {
            return Forbid(); // 403 Forbidden - Apenas Admins e Managers podem cancelar qualquer venda
        }

        var request = new CancelSaleRequest { SaleId = id };
        var validator = new CancelSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<CancelSaleCommand>(request.SaleId);
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<CancelSaleResponse>
        {
            Success = true,
            Message = "Sale canceled successfully",
            Data = _mapper.Map<CancelSaleResponse>(response)
        });
    }

    /// <summary>
    /// Cancel a item of sale by their 
    /// </summary>
    /// <param name="saleId">The unique identifier of the sale</param>
    /// <param name="productId">The unique identifier of the product</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response if the item sale was canceled</returns>
    [HttpPatch("{saleId}/items/{productId}/cancel")]
    [ProducesResponseType(typeof(ApiResponseWithData<CancelItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelItem([FromRoute] int saleId, [FromRoute] int productId, CancellationToken cancellationToken)
    {
        var userRole = User.FindFirst("role")?.Value;
        var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");

        // Obtém a venda antes de cancelar o item
        var sale = await _mediator.Send(new GetSaleByIdQuery { Id = saleId }, cancellationToken);

        if (sale == null)
        {
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = "Sale not found."
            });
        }

        // Se não for Admin ou Manager, o usuário só pode cancelar itens de sua própria venda
        bool isAdminOrManager = userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                                userRole.Equals("Manager", StringComparison.OrdinalIgnoreCase);

        if (!isAdminOrManager && sale.CustomerId != userId)
        {
            return Forbid(); // 403 Forbidden - Apenas Admins e Managers podem cancelar qualquer item de venda
        }      

        var request = new CancelItemRequest { SaleId = saleId, ProductId = productId };
        var validator = new CancelItemRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<CancelItemCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<CancelItemResponse>
        {
            Success = true,
            Message = "Sale Item canceled successfully",
            Data = _mapper.Map<CancelItemResponse>(response)
        });
    }
}
