using Ambev.DeveloperEvaluation.Application.Common.Messaging;
using Ambev.DeveloperEvaluation.Functional.Utils;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Ambev.DeveloperEvaluation.Functional.Infrastructure;

/// <summary>
/// Fábrica customizada para testes funcionais, inicializando a API sem dependência de banco de dados.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public IConfiguration Configuration { get; private set; }
    private readonly string _databaseName;

    public CustomWebApplicationFactory(string? databaseName = null)
    {
        _databaseName = databaseName ?? Guid.NewGuid().ToString(); // Garante banco único
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {


        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "IS_TEST_ENVIRONMENT", "true" }
            });

            EnvLoader.LoadEnvFromProjectRoot();

            // Define que estamos em ambiente de testes
            Environment.SetEnvironmentVariable("IS_TEST_ENVIRONMENT", "true");

            config.AddEnvironmentVariables();

            // Constrói o IConfiguration completo para acesso imediato
            var builtConfig = config.Build();
            Configuration = builtConfig;
        });

        builder.ConfigureServices(services =>
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
                options.UseInMemoryDatabase(_databaseName);
            });            

            // ✅ Mock de IRabbitMqPublisher
            var publisherDescriptor = services.FirstOrDefault(s =>
                s.ServiceType == typeof(IRabbitMqPublisher));
            if (publisherDescriptor != null) services.Remove(publisherDescriptor);

            var mockPublisher = Substitute.For<IRabbitMqPublisher>();
            services.AddSingleton(mockPublisher);

            Console.WriteLine("✅ Banco InMemory configurado.");
            Console.WriteLine("✅ Mock de IRabbitMqPublisher registrado.");
        });

        Console.WriteLine("API de Teste inicializada com sucesso!");

    }
}