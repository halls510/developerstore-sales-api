using Ambev.DeveloperEvaluation.Application.Users.CreateUser;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.WebApi.Startup;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider, IConfiguration configuration, ILogger logger, CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<DefaultContext>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var mediator = services.GetRequiredService<IMediator>();
        var mapper = services.GetRequiredService<IMapper>();

        logger.LogInformation("Iniciando migração do banco de dados...");
        await context.Database.MigrateAsync(cancellationToken);
        logger.LogInformation("Migração concluída com sucesso.");

        // Criar roles se não existirem
        if (!await roleManager.Roles.AnyAsync(cancellationToken))
        {
            logger.LogInformation("Criando roles padrão...");
            await roleManager.CreateAsync(new IdentityRole(UserRole.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(UserRole.Manager.ToString()));
            await roleManager.CreateAsync(new IdentityRole(UserRole.Customer.ToString()));
            logger.LogInformation("Roles criadas com sucesso.");
        }

        // Buscar senha segura do Secret
        string adminPassword = configuration["AdminPassword"];
        string emailPassword = configuration["EmailPassword"];
        if (string.IsNullOrEmpty(adminPassword) && string.IsNullOrEmpty(emailPassword))
        {
            logger.LogError("A senha e email do administrador não estão configurados no Secret.");
            throw new Exception("A senha e email do administrador não estão configurados no Secret.");
        }

        // Criar usuário administrador se não existir
        if (!await userManager.Users.AnyAsync(u => u.Username == "admin", cancellationToken))
        {
            logger.LogInformation("Criando usuário administrador padrão...");
            var request = new CreateUserRequest
            {
                Username = "admin",
                Email = emailPassword,
                Password = adminPassword,
                Role = UserRole.Admin
            };

            // Validar request
            var validator = new CreateUserRequestValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                logger.LogError("Erro na validação do usuário admin: {Errors}", string.Join(", ", validationResult.Errors));
                throw new Exception("Erro na validação do usuário admin: " + string.Join(", ", validationResult.Errors));
            }

            // Criar comando e enviar para o MediatR
            var command = mapper.Map<CreateUserCommand>(request);
            await mediator.Send(command, cancellationToken);
            logger.LogInformation("Usuário administrador criado com sucesso.");
        }
        else
        {
            logger.LogInformation("Usuário administrador já existente, nenhuma ação necessária.");
        }
    }
}