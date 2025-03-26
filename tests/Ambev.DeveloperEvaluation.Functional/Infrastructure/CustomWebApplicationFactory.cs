using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi;
using Ambev.DeveloperEvaluation.WebApi.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.Functional.Infrastructure;

/// <summary>
/// Fábrica customizada para testes funcionais, inicializando a API sem dependência de banco de dados.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public IConfiguration Configuration { get; private set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Define que estamos em ambiente de testes
        Environment.SetEnvironmentVariable("IS_TEST_ENVIRONMENT", "true");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "IS_TEST_ENVIRONMENT", "true" }
            });
        });

        builder.ConfigureServices(async services =>
        {
            Console.WriteLine("Configurando WebApplicationFactory para testes...");

            // Remover configuração do banco de dados real
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DefaultContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
                Console.WriteLine("Configuração de banco de dados removida.");
            }

            // Configurar banco de dados em memória para testes
            services.AddDbContext<DefaultContext>(options =>
            {
                options.UseInMemoryDatabase("TestDatabase");
            });

            // Criar banco de dados para testes
            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DefaultContext>();
            db.Database.EnsureCreated();

            Configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            string adminEmail = Configuration["AdminEmail"];
            string adminPassword = Configuration["AdminPassword"];

            var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
            var hashedPassword = hasher.HashPassword(adminPassword);

            db.Users.Add(new User
            {
                Email = adminEmail,
                Password = hashedPassword,
                Status = Domain.Enums.UserStatus.Active,
                Role = Domain.Enums.UserRole.Admin,
                Firstname = "Usuário",
                Lastname = "Admin",
                Username = "adminusuario",
                
            });
            db.SaveChanges();

            var teste = db.Users.FirstOrDefault();

            Console.WriteLine("Banco de dados de testes inicializado.");
        });

        Console.WriteLine("API de Teste inicializada com sucesso!");

    }
}