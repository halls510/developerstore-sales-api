using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;
using FluentAssertions;
using Ambev.DeveloperEvaluation.Functional.Utils;
using System.Net;

namespace Ambev.DeveloperEvaluation.Functional.Base;

public class TestBase : IClassFixture<TestFixture>
{
    protected readonly HttpClient _client;
    private static string? _authToken;

    public TestBase(TestFixture fixture, bool requiresAuth = true)
    {
        _client = fixture.Client;

        Console.WriteLine($"Teste rodando na URL base: {_client.BaseAddress}");

        if (requiresAuth)
        {
            AuthenticateAsync().GetAwaiter().GetResult(); // Garantir login apenas se necessário
        }
    }

    private async Task AuthenticateAsync()
    {
        if (!string.IsNullOrEmpty(_authToken))
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
            return;
        }

        // 🔹 Garante que o usuário existe antes de autenticar
        await EnsureTestUserExists();

        var loginRequest = new
        {
            Email = "carlos@example.com",
            Password = "Secure@123"
        };

        var response = await _client.PostAsJsonAsync("/auth/login", loginRequest);

        if (!response.IsSuccessStatusCode)
        {
            var errorDetails = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Erro ao autenticar: {response.StatusCode}, Resposta: {errorDetails}");
        }

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var authResult = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        _authToken = authResult?.Token;

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
    }

    private async Task EnsureTestUserExists()
    {
        var userRequest = new
        {
            Name = new
            {
                Firstname = "Carlos",
                Lastname = "Silva"
            },
            Username = "carlossilva",
            Email = "carlos@example.com",
            Password = "Secure@123",
            Phone = "+5511999999999",
            Role = "Admin",
            Status = "Active",
            Address = new
            {
                City = "São Paulo",
                Street = "Av. Paulista",
                Number = 123,
                Zipcode = "01311-000",
                Geolocation = new
                {
                    Lat = "-23.561414",
                    Long = "-46.656041"
                }
            }
        };

        var response = await _client.PostAsJsonAsync("/api/users", userRequest);

        if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.Conflict)
        {
            var errorDetails = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Erro ao criar usuário: {response.StatusCode}, Resposta: {errorDetails}");
        }
    }

}