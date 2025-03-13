using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Ambev.DeveloperEvaluation.Application.Users.CreateUser;
using Ambev.DeveloperEvaluation.Application.Users.Services;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.WebApi.Services;

public class DbInitializerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DbInitializerService> _logger;
    private static bool _isInitialized = false;
    private static readonly object _lock = new();

    public DbInitializerService(IServiceProvider serviceProvider, ILogger<DbInitializerService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        lock (_lock)
        {
            if (_isInitialized)
            {
                _logger.LogWarning("Inicialização do banco de dados já foi executada. Ignorando nova tentativa.");
                return;
            }
            _isInitialized = true;
        }

        using var scope = _serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<DefaultContext>();    
        var mediator = services.GetRequiredService<IMediator>();
        var mapper = services.GetRequiredService<IMapper>();
        var configuration = services.GetRequiredService<IConfiguration>();
        var userService = services.GetRequiredService<IUserService>();
        var productService = services.GetRequiredService<IProductService>();

        try
        {
            //if (configuration["ASPNETCORE_ENVIRONMENT"] == "Development")
            //{
            //    _logger.LogInformation("Ambiente de desenvolvimento detectado. Recriando banco de dados...");
            //    await context.Database.EnsureDeletedAsync(stoppingToken);
            //}

            _logger.LogInformation("Aplicando migrações pendentes no banco de dados...");
            await context.Database.MigrateAsync(stoppingToken);
            _logger.LogInformation("Migrações aplicadas com sucesso.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao aplicar migrações no banco de dados.");
            return;
        }

        await InitializeUsers(mediator, mapper, userService, configuration, stoppingToken);
        await InitializeProducts(mediator, mapper,productService, stoppingToken);
    }

    private async Task InitializeUsers(
        IMediator mediator,
        IMapper mapper,
        IUserService userService,
        IConfiguration configuration,
        CancellationToken cancellationToken)
    {
        string adminPassword = configuration["AdminPassword"];
        string adminEmail = configuration["AdminEmail"];
        string adminPhone = configuration["AdminPhone"];
        if (string.IsNullOrEmpty(adminPassword) && string.IsNullOrEmpty(adminEmail))
        {
            _logger.LogError("A senha e o email do administrador não estão configurados no Secret.");
            throw new Exception("A senha e email do administrador não estão configurados no Secret.");
        }

        var existingUser = await userService.GetUserByEmailAsync(adminEmail, cancellationToken);
        if (existingUser != null)
        {
            _logger.LogInformation("Usuário administrador já existe. Nenhuma ação necessária.");
            return;
        }

        _logger.LogInformation("Criando usuário administrador padrão...");
        var request = new CreateUserRequest
        {
            Username = "admin",
            Name = new NameRequest
            {
                Firstname = "Usuário",
                Lastname = "Admin"
            },
            Email = adminEmail,
            Password = adminPassword,
            Phone = adminPhone,
            Role = UserRole.Admin,
            Status = UserStatus.Active
        };

        var validator = new CreateUserRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogError("Erro na validação do usuário admin: {Errors}", string.Join(", ", validationResult.Errors));
            throw new Exception("Erro na validação do usuário admin: " + string.Join(", ", validationResult.Errors));
        }

        var command = mapper.Map<CreateUserCommand>(request);
        await mediator.Send(command, cancellationToken);
        _logger.LogInformation("Usuário administrador criado com sucesso.");
    }

    private async Task InitializeProducts(
        IMediator mediator,
        IMapper mapper,
        IProductService productService,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Verificando produtos padrão...");

        var request = new CreateProductRequest
        {
            Title = "Cerveja Puro Malte",
            Description = "Cerveja puro malte premium, 600ml.",
            Price = 9.99M,
            Category = "Bebidas",
            Image = "https://example.com/cerveja.jpg",
            Rating = new RatingRequest { Rate = 4.5, Count = 120 }
        };

        var existingProduct = await productService.GetByTitleAsync(request.Title, cancellationToken);
        if (existingProduct != null)
        {
            _logger.LogInformation("Produto já existe. Nenhuma ação necessária.");
            return;
        }

        var validator = new CreateProductRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            _logger.LogError("Erro na validação do produto: {Errors}", string.Join(", ", validationResult.Errors));
            throw new Exception("Erro na validação do produto: " + string.Join(", ", validationResult.Errors));
        }

        var command = mapper.Map<CreateProductCommand>(request);
        await mediator.Send(command, cancellationToken);

        _logger.LogInformation("Produto padrão criado com sucesso.");
    }

}
