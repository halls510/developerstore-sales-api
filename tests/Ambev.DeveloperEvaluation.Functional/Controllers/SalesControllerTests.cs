using Ambev.DeveloperEvaluation.Functional.Infrastructure;
using Ambev.DeveloperEvaluation.WebApi.Common;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Controllers;

/// <summary>
/// Testes funcionais para o SalesController.
/// </summary>
public class SalesControllerTests : FunctionalTestBase
{    

    [Fact]
    public async Task GetSale_ShouldReturn_SaleDetails()
    {
        var response = await _client.GetAsync("/api/sales/1"); // Supondo que a venda 1 existe
        response.StatusCode.Should().Be(HttpStatusCode.OK, $"Erro ao obter venda: {await response.Content.ReadAsStringAsync()}");
    }

    [Fact]
    public async Task ListSales_ShouldReturn_PaginatedList()
    {
        var response = await _client.GetAsync("/api/sales?_page=1&_size=10");
        response.StatusCode.Should().Be(HttpStatusCode.OK, $"Erro ao listar vendas: {await response.Content.ReadAsStringAsync()}");
    }

    [Fact]
    public async Task UpdateSale_ShouldReturn_Updated()
    {
        var updateRequest = new
        {
            CustomerId = 3,
            Items = new List<SaleItemRequest>()
            {
                new SaleItemRequest()
                {
                    ProductId = 1,
                    Quantity = 10
                }
            }
        };

        var response = await _client.PutAsJsonAsync("/api/sales/1", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.OK, $"Erro ao atualizar venda: {await response.Content.ReadAsStringAsync()}");
    }

    [Fact]
    public async Task CancelSale_ShouldReturn_Success()
    {
        var response = await _client.PatchAsync("/api/sales/1/cancel", null);
        response.StatusCode.Should().Be(HttpStatusCode.OK, $"Erro ao cancelar venda: {await response.Content.ReadAsStringAsync()}");
    }

    [Fact]
    public async Task CancelItem_ShouldReturn_Success()
    {
        var response = await _client.PatchAsync("/api/sales/3/items/1/cancel", null);
        response.StatusCode.Should().Be(HttpStatusCode.OK, $"Erro ao cancelar item da venda: {await response.Content.ReadAsStringAsync()}");
    }
}
