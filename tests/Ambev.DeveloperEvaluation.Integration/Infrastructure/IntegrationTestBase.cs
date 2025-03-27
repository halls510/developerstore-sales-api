using Ambev.DeveloperEvaluation.ORM;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Xunit;
using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace Ambev.DeveloperEvaluation.Integration.Infrastructure;

/// <summary>
/// Classe base para testes de integração, utilizando o banco real com acesso ao contexto e autenticação via seed.
/// </summary>
public abstract class IntegrationTestBase 
{
    protected readonly HttpClient _client;
    protected readonly IServiceScopeFactory _scopeFactory;
    private string? _authToken;
    private static readonly object _lock = new(); // Evita concorrência
    private readonly IConfiguration _configuration;

    public IntegrationTestBase()
    {
        var factory = new CustomWebApplicationFactory(); 
        _client = factory.CreateClient();
        _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
        _configuration = factory.Services.GetRequiredService<IConfiguration>();

        Console.WriteLine($"🧪 Teste rodando na URL base: {_client.BaseAddress}");

        AuthenticateClientAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Executa ações síncronas diretamente no contexto do banco de dados.
    /// </summary>
    protected void ExecuteDbContext(Action<DefaultContext> action)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DefaultContext>();
        action(context);
    }

    /// <summary>
    /// Executa ações assíncronas diretamente no contexto do banco de dados.
    /// </summary>
    protected async Task ExecuteDbContextAsync(Func<DefaultContext, Task> action)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DefaultContext>();
        await action(context);
    }

    /// <summary>
    /// Obtém um token de autenticação JWT com base no usuário de seed.
    /// </summary>
    protected async Task<string> GetAuthToken()
    {
        var adminEmail = _configuration["AdminEmail"];
        var adminPassword = _configuration["AdminPassword"];

        var credentials = new
        {
            Email = adminEmail,
            Password = adminPassword
        };

        var content = new StringContent(JsonConvert.SerializeObject(credentials), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/auth", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorDetails = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"❌ Erro ao autenticar: {response.StatusCode}, Resposta: {errorDetails}");
        }

        response.StatusCode.Should().Be(HttpStatusCode.OK, "Falha ao autenticar usuário Admin da seed");

        var responseData = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
        return responseData?.data.token;
    }

    /// <summary>
    /// Autentica o cliente HTTP adicionando o token JWT.
    /// </summary>
    private async Task AuthenticateClientAsync()
    {
        if (!string.IsNullOrEmpty(_authToken))
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
            return;
        }

        lock (_lock)
        {
            if (!string.IsNullOrEmpty(_authToken)) return;
        }

        _authToken = await GetAuthToken();

        if (string.IsNullOrEmpty(_authToken))
            throw new InvalidOperationException("⚠️ Autenticação falhou. Nenhum token recebido.");

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
        Console.WriteLine($"✅ Token JWT recebido: {_authToken}");
    }
}