using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Integration.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Users;

public class UserIntegrationTests : IntegrationTestBase
{
    public UserIntegrationTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task CreateUser_ShouldAddUserToDatabase()
    {
        await AuthenticateClientAsync(); // Adiciona o token JWT ao cliente

        var user = new
        {
            Name = new
            {
                Firstname = "Carlos",
                Lastname = "Silva"
            },
            Username = "carlossilva",
            Password = "Secure@123",
            Email = "carlos@example.com",
            Phone = "+5511999999999",
            Role = "Customer",
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

        var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("api/users", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorDetails = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Erro ao criar usuário: {response.StatusCode}, Resposta: {errorDetails}");
        }

        response.EnsureSuccessStatusCode();
        Console.WriteLine("Usuário criado com sucesso!");
    }

    [Fact]
    public async Task GetUsers_ShouldReturnUsersFromDatabase()
    {
        await AuthenticateClientAsync(); // Adiciona o token JWT ao cliente

        var passwordHasher = new BCryptPasswordHasher(); // Instancia o BCrypt
        var hashedPassword = passwordHasher.HashPassword("Ana@123");

        // Arrange - Criar um usuário no banco de dados antes de buscar
        ExecuteDbContext(context =>
        {
            context.Users.Add(new Ambev.DeveloperEvaluation.Domain.Entities.User
            {
                Firstname = "Ana",
                Lastname = "Pereira",
                Username = "anapereira",
                Password = hashedPassword,
                Email = "ana@example.com",
                Phone = "+5511988888888",
                Role = Ambev.DeveloperEvaluation.Domain.Enums.UserRole.Customer,
                Status = Ambev.DeveloperEvaluation.Domain.Enums.UserStatus.Active
            });
            context.SaveChanges();
        });

        // Act - Buscar usuários
        var response = await _client.GetAsync("api/users");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Resposta da API: {jsonResponse}");

        var responseData = JObject.Parse(jsonResponse);
        var users = responseData["data"].ToObject<List<JObject>>();

        // Assert
        Assert.Contains(users, u => u["email"]?.ToString() == "ana@example.com");
        Console.WriteLine("Usuários retornados com sucesso!");
    }

    [Fact]
    public async Task DeleteUser_ShouldRemoveUserFromDatabase()
    {
        await AuthenticateClientAsync(); // Adiciona o token JWT ao cliente

        int userId = 0;
        var passwordHasher = new BCryptPasswordHasher(); // Instancia o BCrypt
        var hashedPassword = passwordHasher.HashPassword("Lucas@123");
        // Arrange - Criar um usuário no banco de dados antes de excluí-lo
        ExecuteDbContext(context =>
        {
            var user = new Ambev.DeveloperEvaluation.Domain.Entities.User
            {
                Firstname = "Lucas",
                Lastname = "Santos",
                Username = "lucassantos",
                Password = hashedPassword,
                Email = "lucas@example.com",
                Phone = "+5511977777777",
                Role = Ambev.DeveloperEvaluation.Domain.Enums.UserRole.Customer,
                Status = Ambev.DeveloperEvaluation.Domain.Enums.UserStatus.Active
            };

            context.Users.Add(user);
            context.SaveChanges();

            userId = user.Id;
        });

        // Act - Excluir usuário
        var response = await _client.DeleteAsync($"api/users/{userId}");
        response.EnsureSuccessStatusCode();

        // Assert - Verificar se o usuário foi removido
        ExecuteDbContext(context =>
        {
            var dbUser = context.Users.FirstOrDefaultAsync(u => u.Id == userId).Result;
            Assert.Null(dbUser);
        });

        Console.WriteLine("Usuário deletado com sucesso!");
    }

    [Fact]
    public async Task GetUserById_ShouldReturnCorrectUser()
    {
        await AuthenticateClientAsync(); // Adiciona o token JWT ao cliente

        int userId = 0;
        var passwordHasher = new BCryptPasswordHasher(); // Instancia o BCrypt
        var hashedPassword = passwordHasher.HashPassword("Mariana@123");
        // Arrange - Criar um usuário no banco de dados antes de buscá-lo
        ExecuteDbContext(context =>
        {
            var user = new Ambev.DeveloperEvaluation.Domain.Entities.User
            {
                Firstname = "Mariana",
                Lastname = "Souza",
                Username = "marianasouza",
                Password = hashedPassword,
                Email = "mariana@example.com",
                Phone = "+5511966666666",
                Role = Ambev.DeveloperEvaluation.Domain.Enums.UserRole.Customer,
                Status = Ambev.DeveloperEvaluation.Domain.Enums.UserStatus.Active
            };

            context.Users.Add(user);
            context.SaveChanges();
            userId = user.Id;
        });

        // Act - Buscar usuário pelo ID
        var response = await _client.GetAsync($"api/users/{userId}");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Resposta da API: {jsonResponse}");

        var userResponse = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

        // Assert - Verifica se o usuário retornado está correto
        Assert.Equal("mariana@example.com", (string)userResponse["data"]["email"]);
        Console.WriteLine("Usuário retornado corretamente!");
    }

    [Fact]
    public async Task UpdateUser_ShouldModifyUserInDatabase()
    {
        await AuthenticateClientAsync(); // Adiciona o token JWT ao cliente

        int userId = 0;
        var passwordHasher = new BCryptPasswordHasher(); // Instancia o BCrypt
        var hashedPassword = passwordHasher.HashPassword("Roberto@123");

        // Arrange - Criar um usuário no banco de dados antes de atualizá-lo
        ExecuteDbContext(context =>
        {
            var user = new Ambev.DeveloperEvaluation.Domain.Entities.User
            {
                Firstname = "Roberto",
                Lastname = "Alves",
                Username = "robertoalves",
                Password = hashedPassword, // 🔹 Senha válida com maiúscula, minúscula, número e símbolo
                Email = "roberto@example.com",
                Phone = "+5511955555555",
                Role = Ambev.DeveloperEvaluation.Domain.Enums.UserRole.Customer,
                Status = Ambev.DeveloperEvaluation.Domain.Enums.UserStatus.Active
            };

            context.Users.Add(user);
            context.SaveChanges();
            userId = user.Id;
        });
       
        var hashedPasswordNew = passwordHasher.HashPassword("Novo@Pass123");

        var updatedUser = new
        {
            Name = new
            {
                Firstname = "Roberto",
                Lastname = "Alves Lima"
            },
            Username = "robertoalves",
            Password = hashedPasswordNew, // 🔹 Senha válida com critérios exigidos
            Email = "roberto.lima@example.com",
            Phone = "+5511955555555",
            Role = "Customer",
            Status = "Active",
            Address = new
            {
                City = "Rio de Janeiro",
                Street = "Av. Copacabana",
                Number = 456,
                Zipcode = "22060-002",
                Geolocation = new
                {
                    Lat = "-22.971177",
                    Long = "-43.182543"
                }
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(updatedUser), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"api/users/{userId}", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorDetails = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Erro ao atualizar usuário: {response.StatusCode}, Resposta: {errorDetails}");
        }

        response.EnsureSuccessStatusCode();

        // Assert - Verifica se o usuário foi atualizado corretamente
        ExecuteDbContext(context =>
        {
            var dbUser = context.Users.FirstOrDefaultAsync(u => u.Id == userId).Result;
            Assert.NotNull(dbUser);
            Assert.Equal("roberto.lima@example.com", dbUser.Email);
            Assert.Equal("Alves Lima", dbUser.Lastname);
        });

        Console.WriteLine("Usuário atualizado com sucesso!");
    }
}