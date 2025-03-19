using Ambev.DeveloperEvaluation.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Ambev.DeveloperEvaluation.Functional.Infrastructure;

/// <summary>
/// Fábrica customizada para testes funcionais, inicializando a API sem dependência de banco de dados.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public IConfiguration Configuration { get; private set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.UseSetting("https_port", "8081"); // Define a porta para evitar conflitos

        builder.ConfigureAppConfiguration((context, config) =>
        {
            //var testSettings = new Dictionary<string, string>
            //{
            //    { "AdminEmail", "admin@example.com" },
            //    { "AdminPassword", "Secure@123" }
            //};

            //config.AddInMemoryCollection(testSettings);
        });

        builder.ConfigureServices(services =>
        {
            // Aqui você pode remover a configuração do banco real e usar um banco em memória, se necessário.
        });

        Console.WriteLine("✅ API configurada para testes funcionais!");
    }
}