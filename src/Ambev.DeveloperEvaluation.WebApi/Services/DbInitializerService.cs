using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Ambev.DeveloperEvaluation.Application.Users.CreateUser;
using Ambev.DeveloperEvaluation.Common.Configuration;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Ambev.DeveloperEvaluation.WebApi.Services;

public class DbInitializerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DbInitializerService> _logger;
    private static bool _isInitialized = false;
    private static readonly object _lock = new();
    private readonly MinioSettings _minioSettings;

    public DbInitializerService(IServiceProvider serviceProvider, ILogger<DbInitializerService> logger, IOptions<MinioSettings> minioOptions)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _minioSettings = minioOptions.Value;
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
            if (configuration["ASPNETCORE_ENVIRONMENT"] == "Development")
            {
                _logger.LogInformation("Ambiente de desenvolvimento detectado. Recriando banco de dados...");
                await context.Database.EnsureDeletedAsync(stoppingToken);
            }

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
        await InitializeProducts(mediator, mapper, productService, stoppingToken);
    }

    private async Task InitializeUsers(
    IMediator mediator,
    IMapper mapper,
    IUserService userService,
    IConfiguration configuration,
    CancellationToken cancellationToken)
    {
        // Admin via variável de ambiente
        string adminPassword = configuration["AdminPassword"];
        string adminEmail = configuration["AdminEmail"];
        string adminPhone = configuration["AdminPhone"];

        if (string.IsNullOrEmpty(adminPassword) || string.IsNullOrEmpty(adminEmail))
        {
            _logger.LogError("A senha e o email do administrador não estão configurados no Secret.");
            throw new Exception("A senha e email do administrador não estão configurados no Secret.");
        }

        await CreateUserIfNotExists(
            mediator, mapper, userService, cancellationToken,
            role: UserRole.Admin,
            username: "admin",
            firstname: "Usuário",
            lastname: "Admin",
            email: adminEmail,
            password: adminPassword,
            phone: adminPhone
        );

        // Manager com valores fixos
        await CreateUserIfNotExists(
            mediator, mapper, userService, cancellationToken,
            role: UserRole.Manager,
            username: "manager",
            firstname: "Usuário",
            lastname: "Manager",
            email: "manager@devstore.com",
            password: "Manager123!",
            phone: "31988888888"
        );

        // Customer com valores fixos
        await CreateUserIfNotExists(
            mediator, mapper, userService, cancellationToken,
            role: UserRole.Customer,
            username: "customer",
            firstname: "Usuário",
            lastname: "Customer",
            email: "customer@devstore.com",
            password: "Customer123!",
            phone: "31977777777"
        );
    }

    private async Task CreateUserIfNotExists(
        IMediator mediator,
        IMapper mapper,
        IUserService userService,
        CancellationToken cancellationToken,
        UserRole role,
        string username,
        string firstname,
        string lastname,
        string email,
        string password,
        string phone)
    {
        var existingUser = await userService.GetUserByEmailAsync(email, cancellationToken);
        if (existingUser != null)
        {
            _logger.LogInformation($"Usuário {role} já existe. Nenhuma ação necessária.");
            return;
        }

        _logger.LogInformation($"Criando usuário {role} padrão...");
        var request = new CreateUserRequest
        {
            Username = username,
            Name = new NameRequest { Firstname = firstname, Lastname = lastname },
            Email = email,
            Password = password,
            Phone = phone,
            Role = role,
            Status = UserStatus.Active,
            Address = new AddressRequest
            {
                City = "Belo Horizonte",
                Street = "Avenida do Contorno",
                Number = 1000,
                Zipcode = "30110-936",
                Geolocation = new GeoLocationRequest
                {
                    Lat = "-19.9245",
                    Long = "-43.9352"
                }
            }
        };

        var validator = new CreateUserRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogError("Erro na validação do usuário {Role}: {Errors}", role, string.Join(", ", validationResult.Errors));
            throw new Exception($"Erro na validação do usuário {role}: " + string.Join(", ", validationResult.Errors));
        }

        var command = mapper.Map<CreateUserCommand>(request);
        await mediator.Send(command, cancellationToken);
        _logger.LogInformation($"Usuário {role} criado com sucesso.");
    }


    private async Task InitializeProducts(
     IMediator mediator,
     IMapper mapper,
     IProductService productService,
     CancellationToken cancellationToken)
    {
        _logger.LogInformation("Verificando produtos padrão...");

        var baseImageUrl = $"https://{_minioSettings.ApiEndpoint}/{_minioSettings.BucketName}";

        var products = new List<CreateProductRequest>
    {

            // Cervejas
           // Cervejas da Ambev
            new CreateProductRequest
            {
                Title = "Skol Pilsen",
                Description = "Cerveja Pilsen leve e refrescante, 600ml.",
                Price = 7.99M,
                Category = "Cervejas",
                Image = $"{baseImageUrl}/cerveja_skol_pilsen.png",
                Rating = new RatingRequest { Rate = 4.3, Count = 200 }
            },
            new CreateProductRequest
            {
                Title = "Brahma Chopp",
                Description = "Cerveja Brahma Chopp, tradicional e cremosa, 600ml.",
                Price = 8.99M,
                Category = "Cervejas",
                Image = $"{baseImageUrl}/cerveja_brahma_chopp.png",
                Rating = new RatingRequest { Rate = 4.6, Count = 180 }
            },
            new CreateProductRequest
            {
                Title = "Antarctica Original",
                Description = "Cerveja Antarctica Original, puro malte, 600ml.",
                Price = 9.49M,
                Category = "Cervejas",
                Image = $"{baseImageUrl}/cerveja_antarctica_original.webp",
                Rating = new RatingRequest { Rate = 4.7, Count = 160 }
            },
            new CreateProductRequest
            {
                Title = "Bohemia Puro Malte",
                Description = "Cerveja Bohemia puro malte, 600ml.",
                Price = 10.99M,
                Category = "Cervejas",
                Image = $"{baseImageUrl}/cerveja_bohemia_puro_malte.webp",
                Rating = new RatingRequest { Rate = 4.5, Count = 140 }
            },
            new CreateProductRequest
            {
                Title = "Stella Artois",
                Description = "Cerveja premium belga, 600ml.",
                Price = 12.99M,
                Category = "Cervejas",
                Image = $"{baseImageUrl}/cerveja_stella_artois.webp",
                Rating = new RatingRequest { Rate = 4.8, Count = 170 }
            },
            new CreateProductRequest
            {
                Title = "Budweiser",
                Description = "Cerveja lager americana, 600ml.",
                Price = 11.49M,
                Category = "Cervejas",
                Image = $"{baseImageUrl}/cerveja_budweiser.jpg",
                Rating = new RatingRequest { Rate = 4.6, Count = 190 }
            },


            // Vinhos
            new CreateProductRequest
            {
                Title = "Vinho Tinto Seco",
                Description = "Vinho tinto seco de alta qualidade, 750ml.",
                Price = 59.99M,
                Category = "Vinhos",
                Image = $"{baseImageUrl}/vinho_tinto_seco.webp",
                Rating = new RatingRequest { Rate = 4.7, Count = 150 }
            },
            new CreateProductRequest
            {
                Title = "Vinho Branco Chardonnay",
                Description = "Vinho branco suave e aromático, 750ml.",
                Price = 49.99M,
                Category = "Vinhos",
                Image = $"{baseImageUrl}/vinho_branco_chardonnay.webp",
                Rating = new RatingRequest { Rate = 4.6, Count = 100 }
            },
            new CreateProductRequest
            {
                Title = "Espumante Brut",
                Description = "Espumante fino, ideal para celebrações, 750ml.",
                Price = 79.99M,
                Category = "Vinhos",
                Image = $"{baseImageUrl}/espumante_brut.webp",
                Rating = new RatingRequest { Rate = 4.8, Count = 120 }
            },

            // Destilados
            new CreateProductRequest
            {
                Title = "Whisky 12 anos",
                Description = "Whisky envelhecido 12 anos, garrafa de 750ml.",
                Price = 149.99M,
                Category = "Destilados",
                Image = $"{baseImageUrl}/whisky_12_anos.jpg",
                Rating = new RatingRequest { Rate = 4.8, Count = 95 }
            },
            new CreateProductRequest
            {
                Title = "Vodka Premium",
                Description = "Vodka premium destilada cinco vezes, 1L.",
                Price = 79.99M,
                Category = "Destilados",
                Image = $"{baseImageUrl}/vodka_premium.webp",
                Rating = new RatingRequest { Rate = 4.6, Count = 85 }
            },
            new CreateProductRequest
            {
                Title = "Rum Añejo",
                Description = "Rum envelhecido por 8 anos, garrafa de 750ml.",
                Price = 89.99M,
                Category = "Destilados",
                Image = $"{baseImageUrl}/rum.webp",
                Rating = new RatingRequest { Rate = 4.4, Count = 60 }
            },
            new CreateProductRequest
            {
                Title = "Gin Artesanal",
                Description = "Gin artesanal premium com ervas selecionadas, 700ml.",
                Price = 99.99M,
                Category = "Destilados",
                Image = $"{baseImageUrl}/gin_Artesanal.webp",
                Rating = new RatingRequest { Rate = 4.9, Count = 110 }
            },
            new CreateProductRequest
            {
                Title = "Tequila Reposado",
                Description = "Tequila envelhecida 6 meses em barris de carvalho, 750ml.",
                Price = 119.99M,
                Category = "Destilados",
                Image = $"{baseImageUrl}/tequila_reposado.webp",
                Rating = new RatingRequest { Rate = 4.7, Count = 75 }
            },

            // Bebidas não alcoólicas
            new CreateProductRequest
            {
                Title = "Água Mineral sem Gás",
                Description = "Água mineral pura e refrescante, 500ml.",
                Price = 2.99M,
                Category = "Bebidas Não Alcoólicas",
                Image = $"{baseImageUrl}/agua_mineral_sem_gas.webp",
                Rating = new RatingRequest { Rate = 4.5, Count = 200 }
            },
            new CreateProductRequest
            {
                Title = "Refrigerante Guaraná Antarctica",
                Description = "Refrigerante sabor guaraná, lata de 350ml.",
                Price = 4.99M,
                Category = "Bebidas Não Alcoólicas",
                Image = $"{baseImageUrl}/refrigerante.jpg",
                Rating = new RatingRequest { Rate = 4.6, Count = 300 }
            },
            new CreateProductRequest
            {
                Title = "Energético Jack Power",
                Description = "Energético Jack Power",
                Price = 8.99M,
                Category = "Energéticos",
                Image = $"{baseImageUrl}/energetico_jack_power.jpg",
                Rating = new RatingRequest { Rate = 4.8, Count = 220 }
            },
            new CreateProductRequest
            {
                Title = "Chá Gelado de Pêssego",
                Description = "Chá gelado natural sabor pêssego, 500ml.",
                Price = 5.99M,
                Category = "Bebidas Não Alcoólicas",
                Image = $"{baseImageUrl}/cha_gelado_pessego.webp",
                Rating = new RatingRequest { Rate = 4.7, Count = 180 }
            },

            // Acessórios
            new CreateProductRequest
            {
                Title = "Kit de Taças para Vinho",
                Description = "Conjunto com 6 taças de cristal para vinho.",
                Price = 89.99M,
                Category = "Acessórios",
                Image = $"{baseImageUrl}/tacas.webp",
                Rating = new RatingRequest { Rate = 4.9, Count = 130 }
            },
            new CreateProductRequest
            {
                Title = "Cooler Térmico 24L",
                Description = "Cooler térmico ideal para manter suas bebidas geladas.",
                Price = 149.99M,
                Category = "Acessórios",
                Image = $"{baseImageUrl}/cooler_termico.jpg",
                Rating = new RatingRequest { Rate = 4.8, Count = 90 }
            },
            new CreateProductRequest
            {
                Title = "Abridor de Garrafas Profissional",
                Description = "Abridor de garrafas com alavanca para facilidade.",
                Price = 24.99M,
                Category = "Acessórios",
                Image = $"{baseImageUrl}/abridor_garrafas.jpg",
                Rating = new RatingRequest { Rate = 4.7, Count = 110 }
            }
        };

        foreach (var request in products)
        {
            var existingProduct = await productService.GetByTitleAsync(request.Title, cancellationToken);
            if (existingProduct != null)
            {
                _logger.LogInformation("Produto '{Title}' já existe. Nenhuma ação necessária.", request.Title);
                continue;
            }

            var validator = new CreateProductRequestValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogError("Erro na validação do produto '{Title}': {Errors}", request.Title, string.Join(", ", validationResult.Errors));
                continue; // Continua para o próximo produto em vez de lançar uma exceção
            }

            var command = mapper.Map<CreateProductCommand>(request);
            await mediator.Send(command, cancellationToken);

            _logger.LogInformation("Produto '{Title}' criado com sucesso.", request.Title);
        }
    }

}
