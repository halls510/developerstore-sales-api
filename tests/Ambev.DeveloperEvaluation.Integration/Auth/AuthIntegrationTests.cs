using Ambev.DeveloperEvaluation.Integration.Infrastructure;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Xunit;
using Ambev.DeveloperEvaluation.Common.Security;

namespace Ambev.DeveloperEvaluation.Integration.Auth
{
    public class AuthIntegrationTests : IntegrationTestBase
    {
        public AuthIntegrationTests(CustomWebApplicationFactory factory) : base(factory) { }

        [Fact]
        public async Task AuthenticateUser_ShouldReturnToken_WhenCredentialsAreValid()
        {
            var passwordHasher = new BCryptPasswordHasher(); // Instancia o BCrypt
            var hashedPassword = passwordHasher.HashPassword("Secure@123");

            // Arrange - Criar usuário no banco antes de autenticar
            ExecuteDbContext(context =>
            {
                context.Users.Add(new User
                {
                    Firstname = "Carlos",
                    Lastname = "Silva",
                    Username = "carlossilva",
                    Password = hashedPassword, // Certifique-se de armazenar a senha criptografada
                    Email = "carlos@example.com",
                    Phone = "+5511999999999",
                    Role = UserRole.Customer,
                    Status = UserStatus.Active
                });
                context.SaveChanges();
            });

            var loginRequest = new
            {
                Email = "carlos@example.com",  // 🔹 Agora usa Email
                Password = "Secure@123"
            };

            var content = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth", content);
            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Contains("token", jsonResponse);

            Console.WriteLine("Login realizado com sucesso!");
        }

        [Fact]
        public async Task AuthenticateUser_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
        {
            var loginRequest = new
            {
                Email = "usuarioinexistente@example.com",  // Testa Email inválido
                Password = "SenhaErrada@123"
            };

            var content = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
            }

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            Console.WriteLine("Autenticação falhou como esperado para credenciais inválidas.");
        }

        [Fact]
        public async Task AuthenticateUser_ShouldReturnBadRequest_WhenMissingFields()
        {
            var loginRequest = new
            {
                Email = "carlos@example.com"
                // 🔹 Sem o campo "Password"
            };

            var content = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
            }

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            Console.WriteLine("Validação de campos ausentes no login funcionando corretamente.");
        }

        [Fact]
        public async Task AuthenticateUser_ShouldReturnBadRequest_WhenInvalidEmailFormat()
        {
            var loginRequest = new
            {
                Email = "emailinvalido",  // 🔹 Testando Email sem formato válido
                Password = "Secure@123"
            };

            var content = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
            }

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            Console.WriteLine("✅ Validação de formato de Email inválido funcionando corretamente.");
        }
    }
}
