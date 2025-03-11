using Ambev.DeveloperEvaluation.Application.Carts.Checkout;
using Ambev.DeveloperEvaluation.Application.Carts.CreateCart;
using Ambev.DeveloperEvaluation.Application.Carts.DeleteCart;
using Ambev.DeveloperEvaluation.Application.Carts.GetCart;
using Ambev.DeveloperEvaluation.Application.Carts.GetCartById;
using Ambev.DeveloperEvaluation.Application.Carts.ListCarts;
using Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Carts.Checkout;
using Ambev.DeveloperEvaluation.WebApi.Features.Carts.CreateCart;
using Ambev.DeveloperEvaluation.WebApi.Features.Carts.DeleteCart;
using Ambev.DeveloperEvaluation.WebApi.Features.Carts.GetCart;
using Ambev.DeveloperEvaluation.WebApi.Features.Carts.GetCartById;
using Ambev.DeveloperEvaluation.WebApi.Features.Carts.ListCarts;
using Ambev.DeveloperEvaluation.WebApi.Features.Carts.UpdateCart;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.GetProduct;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CartsController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of CartsController
    /// </summary>
    /// <param name="mediator">The mediator instance</param>
    /// <param name="mapper">The AutoMapper instance</param>
    public CartsController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves a paginated list of carts with optional filters and ordering.
    /// </summary>
    /// <param name="_page">Page number for pagination (default: 1) (optional)</param>
    /// <param name="_size">Number of items per page (default: 10) (optional)</param>
    /// <param name="_order">Ordering of results (e.g., "price desc, title asc") (optional)</param>
    /// <param name="filters">Filtering parameters (optional, e.g., "_minDate=2025-02-04, _maxDate=2025-03-04").</param>    
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of carts</returns>
    [HttpGet]
    [Authorize(Roles = "Admin,Manager,Customer")] // Apenas usuários autenticados podem acessar carrinhos
    [ProducesResponseType(typeof(PaginatedList<GetCartResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListCarts(
        [FromQuery] int? _page = null,
        [FromQuery] int? _size = null,
        [FromQuery] string? _order = null,
        [FromQuery] Dictionary<string, string[]>? filters = null,
        CancellationToken cancellationToken = default)
    {
        var userRole = User.FindFirst("role")?.Value;
        var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");

        // Se o usuário for Customer, ele só pode ver os próprios carrinhos
        if (userRole.Equals("Customer", StringComparison.OrdinalIgnoreCase))
        {
            filters ??= new Dictionary<string, string[]>();
            filters["UserId"] = new[] { userId.ToString() };
        }

        var request = new ListCartsRequest
        {
            Page = _page ?? 1,
            Size = _size ?? 10,
            OrderBy = _order,
            Filters = filters ?? new Dictionary<string, string[]>()
        };

        var validator = new ListCartsRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<ListCartsCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        var paginatedList = new PaginatedList<GetCartResponse>(
            _mapper.Map<List<GetCartResponse>>(response.Carts),
            response.TotalItems,
            response.CurrentPage,
            response.PageSize
         );

        return OkPaginated(paginatedList);
    }

    /// <summary>
    /// Creates a new cart
    /// </summary>
    /// <param name="request">The cart creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created cart details</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager,Customer")] // Admins, Managers e Customers podem criar carrinhos
    [ProducesResponseType(typeof(ApiResponseWithData<CreateCartResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCart([FromBody] CreateCartRequest request, CancellationToken cancellationToken)
    {
        var userRole = User.FindFirst("role")?.Value;
        var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");

        // Customers só podem criar carrinhos para si mesmos
        if (userRole.Equals("Customer", StringComparison.OrdinalIgnoreCase))
        {
            request.UserId = userId;
        }
        // Managers e Admins podem criar carrinhos para qualquer usuário
        else if (userRole.Equals("Manager", StringComparison.OrdinalIgnoreCase) || userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase))
        {
            if (request.UserId == null || request.UserId <= 0)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Admins and Managers must specify a valid CustomerId."
                });
            }
        }
        else
        {
            return Forbid(); // 403 Forbidden - Outros papéis não podem criar carrinhos
        }

        var validator = new CreateCartRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<CreateCartCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        return Created(string.Empty, new ApiResponseWithData<CreateCartResponse>
        {
            Success = true,
            Message = "Cart created successfully",
            Data = _mapper.Map<CreateCartResponse>(response)
        });
    }

    /// <summary>
    /// Retrieves a cart by their ID
    /// </summary>
    /// <param name="id">The unique identifier of the cart</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The cart details if found</returns>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Manager,Customer")] // Apenas usuários autenticados podem acessar carrinhos
    [ProducesResponseType(typeof(ApiResponseWithData<GetCartByIdResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCart([FromRoute] int id, CancellationToken cancellationToken)
    {  
        var userRole = User.FindFirst("role")?.Value;
        var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");

        // Obtém o carrinho pelo ID antes de retornar
        var cart = await _mediator.Send(new GetCartByIdQuery { Id = id }, cancellationToken);

        if (cart == null)
        {
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = "Cart not found."
            });
        }

        // Se não for Admin ou Manager, o usuário só pode visualizar seu próprio carrinho
        bool isAdminOrManager = userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                                userRole.Equals("Manager", StringComparison.OrdinalIgnoreCase);

        if (!isAdminOrManager && cart.CustomerId != userId)
        {
            return Forbid(); // 403 Forbidden - Apenas Admins e Managers podem visualizar qualquer carrinho
        }

        return Ok(new ApiResponseWithData<GetCartByIdResponse>
        {
            Success = true,
            Message = "Cart retrieved successfully",
            Data = _mapper.Map<GetCartByIdResponse>(cart)
        });
    }

    /// <summary>
    /// Updates an existing cart
    /// </summary>
    /// <param name="id">The unique identifier of the cart to update</param>
    /// <param name="request">The cart update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated cart details</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager,Customer")] // Admins, Managers e Customers podem atualizar carrinhos
    [ProducesResponseType(typeof(ApiResponseWithData<UpdateCartResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCart([FromRoute] int id, [FromBody] UpdateCartRequest request, CancellationToken cancellationToken)
    {
        var userRole = User.FindFirst("role")?.Value;
        var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");

        // Obtém o carrinho pelo ID antes de atualizar
        var cart = await _mediator.Send(new GetCartByIdQuery { Id = id }, cancellationToken);

        if (cart == null)
        {
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = "Cart not found."
            });
        }

        // Se não for Admin ou Manager, o usuário só pode atualizar o próprio carrinho
        if (!userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase) &&
            !userRole.Equals("Manager", StringComparison.OrdinalIgnoreCase) &&
            cart.CustomerId != userId)
        {
            return Forbid(); // 403 Forbidden - Apenas Admins e Managers podem modificar qualquer carrinho
        }

        var validator = new UpdateCartRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<UpdateCartCommand>(request);
        command.Id = id;
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<UpdateCartResponse>
        {
            Success = true,
            Message = "Cart updated successfully",
            Data = _mapper.Map<UpdateCartResponse>(response)
        });
    }

    /// <summary>
    /// Deletes a cart by their ID
    /// </summary>
    /// <param name="id">The unique identifier of the cart to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response if the cart was deleted</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Manager,Customer")] // Admins, Managers e Customers podem excluir carrinhos
    [ProducesResponseType(typeof(ApiResponseWithData<DeleteCartResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCart([FromRoute] int id, CancellationToken cancellationToken)
    {
        var userRole = User.FindFirst("role")?.Value;
        var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");

        // Se não for Admin ou Manager, só pode excluir o próprio carrinho
        if (!userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase) &&
            !userRole.Equals("Manager", StringComparison.OrdinalIgnoreCase))
        {
            var cart = await _mediator.Send(new GetCartByIdQuery { Id = id }, cancellationToken);
            if (cart == null || cart.CustomerId != userId)
            {
                return Forbid(); // 403 Forbidden - Apenas Admins e Managers podem excluir qualquer carrinho
            }
        }

        var request = new DeleteCartRequest { Id = id };
        var validator = new DeleteCartRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<DeleteCartCommand>(request.Id);
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<DeleteCartResponse>
        {
            Success = true,
            Message = "Cart deleted successfully",
            Data = _mapper.Map<DeleteCartResponse>(response)
        });
    }

    /// <summary>
    /// Finaliza o checkout de um carrinho e converte em uma venda.
    /// </summary>
    /// <param name="cartId">O ID do carrinho a ser finalizado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Detalhes da venda gerada</returns>
    [HttpPost("{cartId}/checkout")]
    [Authorize(Roles = "Customer")] // Somente Customers podem finalizar compras
    [ProducesResponseType(typeof(ApiResponseWithData<CheckoutResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Checkout([FromRoute] int cartId, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");

        // Obtém o carrinho antes de processar o checkout
        var cart = await _mediator.Send(new GetCartByIdQuery { Id = cartId }, cancellationToken);

        if (cart == null || cart.CustomerId != userId)
        {
            return Forbid(); // 403 Forbidden - O usuário só pode fazer checkout do próprio carrinho
        }

        var request = new CheckoutRequest { CartId = cartId };
        var validator = new CheckoutRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<CheckoutCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<CheckoutResponse>
        {
            Success = true,
            Message = "Checkout realizado com sucesso!",
            Data = _mapper.Map<CheckoutResponse>(response)
        });
    }

}
