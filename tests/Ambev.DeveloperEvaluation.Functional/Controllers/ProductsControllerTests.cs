using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Functional.Infrastructure;
using Ambev.DeveloperEvaluation.WebApi.Common;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using static System.Net.Mime.MediaTypeNames;

namespace Ambev.DeveloperEvaluation.Functional.Controllers;

/// <summary>
/// Testes funcionais para o ProductsController.
/// </summary>
public class ProductsControllerTests : FunctionalTestBase
{    

    [Fact]
    public async Task CreateProduct_ShouldReturn_Created()
    {
        var productRequest = new
        {
            Title = "Smartphone X",
            Price = 1299.99,
            Description = "Smartphone de última geração",
            Category = "Electronics",
            Image = "https://example.com/smartphone.jpg"
        };                

        var response = await _client.PostAsJsonAsync("/api/products", productRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Created, $"Erro ao criar produto: {await response.Content.ReadAsStringAsync()}");
    }

    [Fact]
    public async Task GetProduct_ShouldReturn_ProductDetails()
    {
        var response = await _client.GetAsync("/api/products/1"); // Supondo que o produto 1 existe
        response.StatusCode.Should().Be(HttpStatusCode.OK, $"Erro ao obter produto: {await response.Content.ReadAsStringAsync()}");
    }

    [Fact]
    public async Task UpdateProduct_ShouldReturn_Updated()
    {
        var updateRequest = new
        {
           Title = "Skol Pilsen Plus",
           Description = "Cerveja Pilsen leve e refrescante, 600ml.",
           Price = 8.99M,
           Category = "Cervejas",
           Image = "http://example.com/skol.png"
        };

        var response = await _client.PutAsJsonAsync("/api/products/1", updateRequest);
        response.StatusCode.Should().Be(HttpStatusCode.OK, $"Erro ao atualizar produto: {await response.Content.ReadAsStringAsync()}");
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturn_Success()
    {
        var response = await _client.DeleteAsync("/api/products/21"); // Supondo que o produto 21 existe
        response.StatusCode.Should().Be(HttpStatusCode.OK, $"Erro ao excluir produto: {await response.Content.ReadAsStringAsync()}");
    }

    [Fact]
    public async Task ListProducts_ShouldReturn_PaginatedList()
    {
        var response = await _client.GetAsync("/api/products?_page=1&_size=10");
        response.StatusCode.Should().Be(HttpStatusCode.OK, $"Erro ao listar produtos: {await response.Content.ReadAsStringAsync()}");
    }

    [Fact]
    public async Task GetProductsByCategory_ShouldReturn_PaginatedList()
    {
        var response = await _client.GetAsync("/api/products/category/Electronics?_page=1&_size=10");
        response.StatusCode.Should().Be(HttpStatusCode.OK, $"Erro ao listar produtos por categoria: {await response.Content.ReadAsStringAsync()}");
    }

    [Fact]
    public async Task ListCategories_ShouldReturn_CategoryList()
    {
        var response = await _client.GetAsync("/api/products/categories");
        response.StatusCode.Should().Be(HttpStatusCode.OK, $"Erro ao listar categorias: {await response.Content.ReadAsStringAsync()}");
    }
}
