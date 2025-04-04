﻿using FluentAssertions;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Ambev.DeveloperEvaluation.Functional.Infrastructure;

/// <summary>
/// Classe base para testes funcionais, garantindo autenticação sem dependência de banco de dados.
/// </summary>
public abstract class FunctionalTestBase 
{
    protected readonly HttpClient _client;
    private static string? _authToken;
    private static readonly object _lock = new(); // Evita problemas de concorrência
    private readonly IConfiguration _configuration;

    public FunctionalTestBase()
    {
        var factory = new CustomWebApplicationFactory(); 
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("http://localhost:8080")
        });
        _configuration = factory.Services.GetRequiredService<IConfiguration>(); // Obtém a configuração da fábrica
        Console.WriteLine($"Teste rodando na URL base: {_client.BaseAddress}");
        AuthenticateClientAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Obtém um token de autenticação JWT para os testes.
    /// </summary>
    protected async Task<string> GetAuthToken()
    {
        string adminEmail = _configuration["AdminEmail"];
        string adminPassword = _configuration["AdminPassword"];

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
            Console.WriteLine($"Erro ao autenticar: {response.StatusCode}, Resposta: {errorDetails}");
        }

        response.StatusCode.Should().Be(HttpStatusCode.OK, "Falha ao autenticar usuário de teste");

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

        lock (_lock) // Evita múltiplas autenticações concorrentes
        {
            if (!string.IsNullOrEmpty(_authToken)) return;
        }

        _authToken = await GetAuthToken();

        if (string.IsNullOrEmpty(_authToken))
            throw new InvalidOperationException("⚠️ Autenticação falhou. Nenhum token recebido.");

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
        Console.WriteLine($"Token JWT recebido: {_authToken}");
    }
}