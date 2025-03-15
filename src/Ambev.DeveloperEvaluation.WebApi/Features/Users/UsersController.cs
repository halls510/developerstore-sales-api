using MediatR;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.DeleteUser;
using Ambev.DeveloperEvaluation.Application.Users.CreateUser;
using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Application.Users.DeleteUser;
using Ambev.DeveloperEvaluation.Application.Users.UpdateUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.UpdateUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.ListUsers;
using Ambev.DeveloperEvaluation.Application.Users.ListUsers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Ambev.DeveloperEvaluation.Domain.Enums;
using System.Security.Claims;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelItem;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users;

/// <summary>
/// Controller for managing user operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of UsersController
    /// </summary>
    /// <param name="mediator">The mediator instance</param>
    /// <param name="mapper">The AutoMapper instance</param>
    public UsersController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves a paginated list of users with optional filters and ordering.
    /// </summary>
    /// <param name="_page">Page number for pagination (default: 1)</param>
    /// <param name="_size">Number of items per page (default: 10)</param>
    /// <param name="_order">Ordering of results (e.g., "username asc, email desc")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of users</returns>
    [HttpGet]
    [Authorize(Roles = "Admin,Manager")] // Apenas Admins e Managers podem listar usuários
    [ProducesResponseType(typeof(PaginatedList<GetUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListUsers(
        [FromQuery] int? _page = null,
        [FromQuery] int? _size = null,
        [FromQuery] string? _order = null,
        [FromQuery] Dictionary<string, string[]>? filters = null,
        CancellationToken cancellationToken = default)
    {
        var request = new ListUsersRequest
        {
            Page = _page ?? 1,
            Size = _size ?? 10,
            OrderBy = _order,
            Filters = filters ?? new Dictionary<string, string[]>()
        };

        var validator = new ListUsersRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<ListUsersCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        var paginatedList = new PaginatedList<GetUserResponse>(
            _mapper.Map<List<GetUserResponse>>(response.Users),
            response.TotalItems,
            response.CurrentPage,
            response.PageSize
         );

        return Ok(paginatedList);
    }

    /// <summary>
    /// Creates a new user
    /// </summary>
    /// <param name="request">The user creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created user details</returns>
    [HttpPost]
    [AllowAnonymous] // Permite criação de usuários sem autenticação
    [ProducesResponseType(typeof(ApiResponseWithData<CreateUserResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        // Pega a Role do usuário autenticado (se existir)
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "None";

        // Se for Customer autenticado, NÃO pode criar usuários
        if (!string.IsNullOrEmpty(userRole) && userRole.Equals("Customer", StringComparison.OrdinalIgnoreCase))
        {
            return Forbid(); // 403 Forbidden
        }

        // Se for Manager, ele só pode criar Customers
        if (!string.IsNullOrEmpty(userRole) && userRole.Equals("Manager", StringComparison.OrdinalIgnoreCase))
        {
            if (request.Role != null && request.Role != UserRole.Customer)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Managers can only create Customer users."
                });
            }
        }

        // Se o usuário NÃO for Admin ou Manager, força que a Role do novo usuário seja "Customer"
        if (string.IsNullOrEmpty(userRole) ||
            (!userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase) &&
             !userRole.Equals("Manager", StringComparison.OrdinalIgnoreCase)))
        {
            request.Role = UserRole.Customer; // Força a criação apenas de Customer
        }

        var validator = new CreateUserRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<CreateUserCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        return Created(string.Empty, new ApiResponseWithData<CreateUserResponse>
        {
            Success = true,
            Message = "User created successfully",
            Data = _mapper.Map<CreateUserResponse>(response)
        });
    }

    /// <summary>
    /// Retrieves a user by their ID
    /// </summary>
    /// <param name="id">The unique identifier of the user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user details if found</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUser([FromRoute] int id, CancellationToken cancellationToken)
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "None";
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        // Se não for Admin ou Manager, só pode visualizar seu próprio perfil
        if (!userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase) &&
            !userRole.Equals("Manager", StringComparison.OrdinalIgnoreCase) &&
            userId != id)
        {
            return Forbid(); // 403 Forbidden
        }

        var request = new GetUserRequest { Id = id };
        var validator = new GetUserRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<GetUserCommand>(request.Id);
        var response = await _mediator.Send(command, cancellationToken);      

        return Ok(_mapper.Map<GetUserResponse>(response), "User retrieved successfully");
    }

    /// <summary>
    /// Updates an existing user
    /// </summary>
    /// <param name="id">The unique identifier of the user to update</param>
    /// <param name="request">The user update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated user details</returns>
    [HttpPut("{id}")]
    [Authorize] // Apenas usuários autenticados podem atualizar usuários
    [ProducesResponseType(typeof(ApiResponseWithData<UpdateUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "None";
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        // Se não for Admin, só pode atualizar o próprio usuário
        if (!userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase) && userId != id)
        {
            return Forbid(); // 403 Forbidden
        }

        // Nenhum usuário pode alterar sua própria Role
        if (userId == id && request.Role != null)
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Users cannot change their own role."
            });
        }

        var validator = new UpdateUserRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<UpdateUserCommand>(request);
        command.Id = id;
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(_mapper.Map<UpdateUserResponse>(response), "User updated successfully");
    }

    /// <summary>
    /// Deletes a user by their ID
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response if the user was deleted</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<DeleteUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser([FromRoute] int id, CancellationToken cancellationToken)
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "None";
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        // Se não for Admin, só pode deletar a própria conta
        if (!userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase) && userId != id)
        {
            return Forbid(); // 403 Forbidden
        }

        var request = new DeleteUserRequest { Id = id };
        var validator = new DeleteUserRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<DeleteUserCommand>(request.Id);
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(_mapper.Map<DeleteUserResponse>(response), "User deleted successfully");
    }

}
