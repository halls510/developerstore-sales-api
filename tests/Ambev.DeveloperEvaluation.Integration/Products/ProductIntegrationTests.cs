using Ambev.DeveloperEvaluation.Integration.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Products;

public class ProductIntegrationTests : IntegrationTestBase
{
    public ProductIntegrationTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task CreateProduct_ShouldAddProductToDatabase()
    {
        await AuthenticateClientAsync(); // Adiciona o token JWT ao cliente

        var product = new
        {
            Title = "Notebook Gamer",
            Price = 5999.99M,
            Description = "Notebook Gamer com processador Intel i7 e 16GB RAM",
            Category = "Eletrônicos",
            Image = "https://example.com/notebook.jpg",
            Rating = new
            {
                Rate = 4.8,
                Count = 200
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("api/products", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorDetails = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Erro ao criar produto: {response.StatusCode}, Resposta: {errorDetails}");
        }

        response.EnsureSuccessStatusCode();

        // Assert - Verifica se o produto foi salvo no banco de dados
        ExecuteDbContext(context =>
        {
            var dbProduct = context.Products.FirstOrDefaultAsync(p => p.Title == "Notebook Gamer").Result;
            Assert.NotNull(dbProduct);
            Assert.Equal("Notebook Gamer", dbProduct.Title);
        });

        Console.WriteLine("Produto criado com sucesso!");
    }

    [Fact]
    public async Task GetProducts_ShouldReturnProductsFromDatabase()
    {
        await AuthenticateClientAsync(); // Adiciona o token JWT ao cliente

        // Arrange - Criar um produto no banco de dados antes de buscar
        ExecuteDbContext(context =>
        {
            context.Products.Add(new Ambev.DeveloperEvaluation.Domain.Entities.Product
            {
                Title = "Smartphone",
                Price = new Ambev.DeveloperEvaluation.Domain.ValueObjects.Money(2999.99M),
                Description = "Smartphone Android com 128GB",
                Image = "https://example.com/smartphone.jpg",
                CreatedAt = DateTime.UtcNow,
                Category = new Ambev.DeveloperEvaluation.Domain.Entities.Category
                {
                    Name = "Eletrônicos"
                }
            });
            context.SaveChanges();
        });

        // Act - Buscar produtos
        var response = await _client.GetAsync("api/products");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Resposta da API: {jsonResponse}");

        var responseData = JObject.Parse(jsonResponse); // Converte a resposta JSON para JObject
        var products = responseData["data"].ToObject<List<JObject>>(); // Converte "data" para uma lista de JObject

        // Assert
        Assert.Contains(products, p => p["title"]?.ToString() == "Smartphone"); 
        Console.WriteLine(" Produtos retornados com sucesso!");
    }
    [Fact]
    public async Task GetProductById_ShouldReturnCorrectProduct()
    {
        await AuthenticateClientAsync(); // Adiciona o token JWT ao cliente

        int productId = 0;

        // Arrange - Criar um produto no banco de dados antes de buscá-lo
        ExecuteDbContext(context =>
        {
            var product = new Ambev.DeveloperEvaluation.Domain.Entities.Product
            {
                Title = "Monitor 4K",
                Price = new Ambev.DeveloperEvaluation.Domain.ValueObjects.Money(1899.99M),
                Description = "Monitor 4K de 27 polegadas",
                Image = "https://example.com/monitor.jpg",
                CreatedAt = DateTime.UtcNow,
                Category = new Ambev.DeveloperEvaluation.Domain.Entities.Category
                {
                    Name = "Eletrônicos"
                }
            };

            context.Products.Add(product);
            context.SaveChanges();

            productId = product.Id;
        });

        // Act - Buscar produto pelo ID
        var response = await _client.GetAsync($"api/products/{productId}");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Resposta da API: {jsonResponse}");

        var productResponse = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

        // Assert - Agora acessamos o título corretamente dentro de "data"
        Assert.Equal("Monitor 4K", (string)productResponse["data"]["title"]);
        Console.WriteLine("Produto retornado corretamente!");
    }


    [Fact]
    public async Task DeleteProduct_ShouldRemoveProductFromDatabase()
    {
        await AuthenticateClientAsync(); // 🔹 Adiciona o token JWT ao cliente

        int productId = 0;

        // Arrange - Criar um produto no banco de dados antes de excluí-lo
        ExecuteDbContext(context =>
        {
            var product = new Ambev.DeveloperEvaluation.Domain.Entities.Product
            {
                Title = "Teclado Mecânico",
                Price = new Ambev.DeveloperEvaluation.Domain.ValueObjects.Money(499.99M),
                Description = "Teclado mecânico RGB",
                Image = "https://example.com/teclado.jpg",
                CreatedAt = DateTime.UtcNow,
                Category = new Ambev.DeveloperEvaluation.Domain.Entities.Category
                {
                    Name = "Periféricos"
                }
            };

            context.Products.Add(product);
            context.SaveChanges();

            productId = product.Id;
        });

        // Act - Excluir produto
        var response = await _client.DeleteAsync($"api/products/{productId}");
        response.EnsureSuccessStatusCode();

        // Assert - Verificar se o produto foi removido
        ExecuteDbContext(context =>
        {
            var dbProduct = context.Products.FirstOrDefaultAsync(p => p.Id == productId).Result;
            Assert.Null(dbProduct);
        });

        Console.WriteLine("Produto deletado com sucesso!");
    }

    [Fact]
    public async Task UpdateProduct_ShouldModifyProductInDatabase()
    {
        await AuthenticateClientAsync();

        int productId = 0;
        ExecuteDbContext(context =>
        {
            var product = new Ambev.DeveloperEvaluation.Domain.Entities.Product
            {
                Title = "Mouse Sem Fio",
                Price = new Ambev.DeveloperEvaluation.Domain.ValueObjects.Money(199.99M),
                Description = "Mouse sem fio recarregável",
                Image = "https://example.com/mouse.jpg",
                CreatedAt = DateTime.UtcNow,
                Category = new Ambev.DeveloperEvaluation.Domain.Entities.Category
                {
                    Name = "Periféricos"
                }
            };

            context.Products.Add(product);
            context.SaveChanges();
            productId = product.Id;
        });

        var updatedProduct = new
        {
            Title = "Mouse Sem Fio PRO",
            Price = 249.99M,
            Description = "Mouse sem fio recarregável com DPI ajustável",
            Category = "Periféricos",
            Image = "https://example.com/mouse-pro.jpg",
            Rating = new { Rate = 4.5, Count = 300 }
        };

        var content = new StringContent(JsonConvert.SerializeObject(updatedProduct), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"api/products/{productId}", content);
        response.EnsureSuccessStatusCode();

        // Assert
        ExecuteDbContext(context =>
        {
            var dbProduct = context.Products.FirstOrDefaultAsync(p => p.Id == productId).Result;
            Assert.NotNull(dbProduct);
            Assert.Equal("Mouse Sem Fio PRO", dbProduct.Title);
            Assert.Equal(249.99M, dbProduct.Price.Amount);
        });

        Console.WriteLine("Produto atualizado com sucesso!");
    }

    [Fact]
    public async Task GetCategories_ShouldReturnProductCategories()
    {
        await AuthenticateClientAsync();

        // Arrange - Criar algumas categorias no banco de dados
        ExecuteDbContext(context =>
        {
            context.Categories.AddRange(new[]
            {
            new Ambev.DeveloperEvaluation.Domain.Entities.Category { Name = "Eletrônicos" },
            new Ambev.DeveloperEvaluation.Domain.Entities.Category { Name = "Periféricos" }
        });
            context.SaveChanges();
        });

        // Act - Buscar categorias
        var response = await _client.GetAsync("api/products/categories");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var responseData = JObject.Parse(jsonResponse);
        var categories = responseData["data"].ToObject<List<string>>();

        // Assert
        Assert.Contains("Eletrônicos", categories);
        Assert.Contains("Periféricos", categories);

        Console.WriteLine("Categorias retornadas com sucesso!");
    }

}
