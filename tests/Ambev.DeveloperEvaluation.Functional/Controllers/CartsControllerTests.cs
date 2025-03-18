using Ambev.DeveloperEvaluation.Functional.Infrastructure;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Controllers;

/// <summary>
/// Testes funcionais para o CartsController.
/// </summary>
public class CartsControllerTests : FunctionalTestBase
{
    public CartsControllerTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task CreateCart_ShouldReturn_Created()
    {
        var cartRequest = new
        {
            UserId = 1,
            Products = new[]
            {
                new { ProductId = 1, Quantity = 2 }
            }
        };

        var response = await _client.PostAsJsonAsync("/api/carts", cartRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Created, $"Erro ao criar carrinho: {await response.Content.ReadAsStringAsync()}");
    }

    [Fact]
    public async Task GetCart_ShouldReturn_CartDetails()
    {
        var response = await _client.GetAsync("/api/carts/1"); // Supondo que o carrinho 1 existe
        response.StatusCode.Should().Be(HttpStatusCode.OK, $"Erro ao obter carrinho: {await response.Content.ReadAsStringAsync()}");
    }

    [Fact]
    public async Task UpdateCart_ShouldReturn_Updated()
    {
        var updateRequest = new
        {
            Products = new[]
            {
                new { ProductId = 1, Quantity = 5 } // Alterando quantidade
            }
        };

        var response = await _client.PutAsJsonAsync("/api/carts/1", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.OK, $"Erro ao atualizar carrinho: {await response.Content.ReadAsStringAsync()}");
    }

    [Fact]
    public async Task DeleteCart_ShouldReturn_Success()
    {
        var response = await _client.DeleteAsync("/api/carts/1"); // Supondo que o carrinho 1 existe
        response.StatusCode.Should().Be(HttpStatusCode.OK, $"Erro ao excluir carrinho: {await response.Content.ReadAsStringAsync()}");
    }

    [Fact]
    public async Task ListCarts_ShouldReturn_PaginatedList()
    {
        var response = await _client.GetAsync("/api/carts?_page=1&_size=10");
        response.StatusCode.Should().Be(HttpStatusCode.OK, $"Erro ao listar carrinhos: {await response.Content.ReadAsStringAsync()}");
    }

    [Fact]
    public async Task CheckoutCart_ShouldReturn_Success()
    {
        var response = await _client.PostAsync("/api/carts/1/checkout", null);
        response.StatusCode.Should().Be(HttpStatusCode.Created, $"Erro ao finalizar checkout: {await response.Content.ReadAsStringAsync()}");
    }
}
