using Ambev.DeveloperEvaluation.Functional.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Controllers;

/// <summary>
/// Testes para o UserController.
/// </summary>
public class UserControllerTests : FunctionalTestBase
{
    public UserControllerTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task CreateUser_ShouldReturn_Created()
    {
        var userRequest = new
        {
            Name = new { Firstname = "Ana", Lastname = "Lima" },
            Username = "analima",
            Email = "ana@example.com",
            Password = "Ana@1234",
            Phone = "+5511988888888",
            Role = "Customer",
            Status = "Active"
        };

        var response = await _client.PostAsJsonAsync("/api/users", userRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Created, $"Erro ao criar usuário: {await response.Content.ReadAsStringAsync()}");
    }

    [Fact]
    public async Task GetUser_ShouldReturn_UserDetails()
    {
        var response = await _client.GetAsync("/api/users/1"); // Supondo que o usuário 1 existe
        response.StatusCode.Should().Be(HttpStatusCode.OK, $"Erro ao obter usuário: {await response.Content.ReadAsStringAsync()}");
    }

    [Fact]
    public async Task UpdateUser_ShouldReturn_Updated()
    {
        var updateRequest = new
        {
            Name = new { Firstname = "Ana", Lastname = "Lima" },
            Username = "analima",
            Email = "ana@example.com",
            Password = "Ana@123454",
            Phone = "+5511988888847",
            Role = "Customer",
            Status = "Active"
        };

        var response = await _client.PutAsJsonAsync("/api/users/1", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.OK, $"Erro ao atualizar usuário: {await response.Content.ReadAsStringAsync()}");
    }

    [Fact]
    public async Task DeleteUser_ShouldReturn_Success()
    {
        var response = await _client.DeleteAsync("/api/users/1"); // Supondo que o usuário 1 existe
        response.StatusCode.Should().Be(HttpStatusCode.OK, $"Erro ao excluir usuário: {await response.Content.ReadAsStringAsync()}");
    }

    [Fact]
    public async Task ListUsers_ShouldReturn_PaginatedList()
    {
        var response = await _client.GetAsync("/api/users?_page=1&_size=10");
        response.StatusCode.Should().Be(HttpStatusCode.OK, $"Erro ao listar usuários: {await response.Content.ReadAsStringAsync()}");
    }
}
