using Ambev.DeveloperEvaluation.Functional.Base;
using Ambev.DeveloperEvaluation.Functional.Utils;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Controllers;

public class UserControllerTests : TestBase
{
    public UserControllerTests(TestFixture fixture) : base(fixture, requiresAuth: true) { }

    [Fact]
    public async Task CreateUser_ShouldReturn_Created()
    {
        var userRequest = TestDataGenerator.GenerateUser();
        var response = await _client.PostAsJsonAsync("/api/users", userRequest);
        if (!response.IsSuccessStatusCode)
        {
            var errorDetails = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Erro: {response.StatusCode}, Resposta: {errorDetails}");
        }

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task GetUser_ShouldReturn_UserDetails()
    {
        var response = await _client.GetAsync("/api/users/1"); // Supondo que o usuário 1 existe
        if (!response.IsSuccessStatusCode)
        {
            var errorDetails = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Erro: {response.StatusCode}, Resposta: {errorDetails}");
        }
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_ShouldReturn_Updated()
    {
        var updateRequest = new
        {
            Firstname = "Updated Name",
            Lastname = "Updated Lastname",
            Phone = "+5511988888888"
        };

        var response = await _client.PutAsJsonAsync("/api/users/1", updateRequest);
        if (!response.IsSuccessStatusCode)
        {
            var errorDetails = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Erro: {response.StatusCode}, Resposta: {errorDetails}");
        }
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_ShouldReturn_Success()
    {
        var response = await _client.DeleteAsync("/api/users/1"); // Supondo que o usuário 1 existe
        if (!response.IsSuccessStatusCode)
        {
            var errorDetails = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Erro: {response.StatusCode}, Resposta: {errorDetails}");
        }
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ListUsers_ShouldReturn_PaginatedList()
    {
        var response = await _client.GetAsync("/api/users?_page=1&_size=10");
        if (!response.IsSuccessStatusCode)
        {
            var errorDetails = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Erro: {response.StatusCode}, Resposta: {errorDetails}");
        }
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}