using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Xunit;
using Ambev.DeveloperEvaluation.Common.Security;

namespace Ambev.DeveloperEvaluation.Integration.Infrastructure;

/// <summary>
/// Classe base para testes de integração, fornecendo autenticação e acesso ao banco de dados.
/// </summary>
public abstract class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>
{
    protected readonly HttpClient _client;
    protected readonly IServiceScopeFactory _scopeFactory;

    public IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();

        Console.WriteLine($"Teste rodando na URL base: {_client.BaseAddress}");

        EnsureAdminUser(); // Criar usuário Admin se não existir
    }

    /// <summary>
    /// Executa ações no contexto do banco de dados de testes.
    /// </summary>
    protected void ExecuteDbContext(Action<DefaultContext> action)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DefaultContext>();
            action(context);
        }
    }

    /// <summary>
    /// Garante que um usuário Admin existe no banco de dados.
    /// </summary>
    protected void EnsureAdminUser()
    {
        ExecuteDbContext(context =>
        {
            var adminUser = context.Users.FirstOrDefault(u => u.Email == "admin@example.com");
            if (adminUser == null)
            {
                var passwordHasher = new BCryptPasswordHasher(); // Instancia o BCrypt
                var hashedPassword = passwordHasher.HashPassword("Admin@123"); // Gera a senha hash

                context.Users.Add(new User
                {
                    Firstname = "Admin",
                    Lastname = "User",
                    Email = "admin@example.com",
                    Password = hashedPassword, // 🔹 Agora a senha está armazenada corretamente
                    Phone = "+5511999999999",
                    Role = UserRole.Admin,
                    Status = UserStatus.Active
                });

                context.SaveChanges();
                Console.WriteLine("Usuário Admin criado com senha criptografada!");
            }
        });
    }


    /// <summary>
    /// Obtém um token de autenticação JWT para testes.
    /// </summary>
    protected async Task<string> GetAuthToken(string email = "admin@example.com", string password = "Admin@123")
    {
        var credentials = new
        {
            Email = email, // 🔹 Corrigido para usar Email em vez de Username
            Password = password
        };

        var content = new StringContent(JsonConvert.SerializeObject(credentials), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("api/auth", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorDetails = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Erro ao autenticar: {response.StatusCode}, Resposta: {errorDetails}");
        }

        response.EnsureSuccessStatusCode(); // Se falhar, significa que as credenciais estão erradas

        var responseData = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
        return responseData.data.token;
    }

    /// <summary>
    /// Adiciona o token de autenticação ao HttpClient.
    /// </summary>
    protected async Task AuthenticateClientAsync()
    {
        var token = await GetAuthToken();

        if (string.IsNullOrEmpty(token))
        {
            Console.WriteLine("Erro: O token JWT não foi gerado!");
        }
        else
        {
            Console.WriteLine($"Token JWT recebido: {token}");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

}
