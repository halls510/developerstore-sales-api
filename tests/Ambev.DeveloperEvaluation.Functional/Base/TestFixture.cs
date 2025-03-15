using Microsoft.AspNetCore.Mvc.Testing;
using Ambev.DeveloperEvaluation.WebApi;

namespace Ambev.DeveloperEvaluation.Functional.Base;


/// <summary>
/// Configuração da TestFixture para testes de integração.
/// </summary>
public class TestFixture : IDisposable
{
    public HttpClient Client { get; private set; }
    private readonly WebApplicationFactory<Program> _factory;

    public TestFixture()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("https_port", "8081"); // Mantém a porta da API real                   
            });

        Client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost:8081") // URL base da API real
        });
    }

    public void Dispose()
    {
        Client.Dispose();
        _factory.Dispose();
    }
}