using FluentAssertions;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Infrastructure;

/// <summary>
/// Classe base para testes funcionais, garantindo autenticação sem dependência de banco de dados.
/// </summary>
public abstract class FunctionalTestBase : IClassFixture<CustomWebApplicationFactory>
{
    protected readonly HttpClient _client;
    private static string? _authToken;
    private static readonly object _lock = new(); // Evita problemas de concorrência

    public FunctionalTestBase(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        Console.WriteLine($"🔹 Teste rodando na URL base: {_client.BaseAddress}");
        AuthenticateClientAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Obtém um token de autenticação JWT para os testes.
    /// </summary>
    protected async Task<string> GetAuthToken(string email = "carlos@example.com", string password = "Secure@123")
    {
        var credentials = new
        {
            Email = email,
            Password = password
        };

        var content = new StringContent(JsonConvert.SerializeObject(credentials), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/auth/login", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorDetails = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"❌ Erro ao autenticar: {response.StatusCode}, Resposta: {errorDetails}");
        }

        response.StatusCode.Should().Be(HttpStatusCode.OK, "Falha ao autenticar usuário de teste");

        var responseData = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
        return responseData?.token;
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

        lock (_lock) // Evita múltiplas autenticações concorrentes
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