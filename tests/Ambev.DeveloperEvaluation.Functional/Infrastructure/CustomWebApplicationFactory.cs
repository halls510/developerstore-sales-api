using Ambev.DeveloperEvaluation.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Ambev.DeveloperEvaluation.Functional.Infrastructure;

/// <summary>
/// Fábrica customizada para testes funcionais, inicializando a API sem banco de dados.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("https_port", "8081"); // Define a porta para evitar conflitos

        Console.WriteLine("✅ API configurada para testes funcionais!");
    }
}