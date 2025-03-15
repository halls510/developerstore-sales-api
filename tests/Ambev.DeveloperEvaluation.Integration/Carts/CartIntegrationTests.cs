using Ambev.DeveloperEvaluation.Integration.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Carts;

public class CartIntegrationTests : IntegrationTestBase
{
    public CartIntegrationTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task CreateCart_ShouldAddCartToDatabase()
    {
        await AuthenticateClientAsync(); // 🔹 Adiciona o token JWT ao cliente

        // 🔹 Insere os produtos no banco ANTES de criar o carrinho
        ExecuteDbContext(context =>
        {
            context.Products.Add(new Ambev.DeveloperEvaluation.Domain.Entities.Product
            {
                Title = "Produto 1",
                Price = new Ambev.DeveloperEvaluation.Domain.ValueObjects.Money(100.00M),
                Description = "Produto de teste 1",
                Image = "https://example.com/produto1.jpg",
                CreatedAt = DateTime.UtcNow,
                Category = new Ambev.DeveloperEvaluation.Domain.Entities.Category { Name = "Teste" }
            });

            context.Products.Add(new Ambev.DeveloperEvaluation.Domain.Entities.Product
            {
                Title = "Produto 2",
                Price = new Ambev.DeveloperEvaluation.Domain.ValueObjects.Money(200.00M),
                Description = "Produto de teste 2",
                Image = "https://example.com/produto2.jpg",
                CreatedAt = DateTime.UtcNow,
                Category = new Ambev.DeveloperEvaluation.Domain.Entities.Category { Name = "Teste" }
            });

            context.SaveChanges();
        });

        var cart = new
        {
            UserId = 1,
            Date = DateTime.UtcNow,
            Products = new[]
            {
            new { ProductId = 1, Quantity = 2 },
            new { ProductId = 2, Quantity = 1 }
        }
        };

        var content = new StringContent(JsonConvert.SerializeObject(cart), Encoding.UTF8, "application/json");

        // Enviar requisição
        var response = await _client.PostAsync("api/carts", content);

        // Capturar erro se falhar
        if (!response.IsSuccessStatusCode)
        {
            var errorDetails = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Erro ao criar carrinho: {response.StatusCode}, Resposta: {errorDetails}");
        }

        response.EnsureSuccessStatusCode();

        // Assert - Verificar se o carrinho foi salvo no banco
        ExecuteDbContext(context =>
        {
            var dbCart = context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == 1).Result;
            Assert.NotNull(dbCart);
            Assert.Equal(2, dbCart.Items.Count);
        });

        Console.WriteLine("Carrinho criado e salvo no banco com sucesso!");
    }


    [Fact]
    public async Task GetCarts_ShouldReturnCartsFromDatabase()
    {
        await AuthenticateClientAsync();

        // Arrange - Criar um carrinho antes de buscar
        ExecuteDbContext(context =>
        {
            context.Carts.Add(new Ambev.DeveloperEvaluation.Domain.Entities.Cart
            {
                UserId = 1,
                Date = DateTime.UtcNow,
                Items = new List<Ambev.DeveloperEvaluation.Domain.Entities.CartItem>
                {
                    new() { ProductId = 1, Quantity = 3 }
                }
            });
            context.SaveChanges();
        });

        // Act
        var response = await _client.GetAsync("api/carts");
        // Capturar erro se falhar
        if (!response.IsSuccessStatusCode)
        {
            var errorDetails = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Erro ao criar carrinho: {response.StatusCode}, Resposta: {errorDetails}");
        }
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var responseData = JObject.Parse(jsonResponse);
        var carts = responseData["data"].ToObject<List<JObject>>();

        // Assert
        Assert.NotEmpty(carts);
        Console.WriteLine("Carrinhos retornados com sucesso!");
    }

    [Fact]
    public async Task GetCartById_ShouldReturnCorrectCart()
    {
        await AuthenticateClientAsync();

        int cartId = 0;
        ExecuteDbContext(context =>
        {
            var cart = new Ambev.DeveloperEvaluation.Domain.Entities.Cart
            {
                UserId = 1, // Garantindo que o UserId é atribuído corretamente
                Date = DateTime.UtcNow,
                Items = new List<Ambev.DeveloperEvaluation.Domain.Entities.CartItem>
            {
                new() { ProductId = 1, Quantity = 1 }
            }
            };
            context.Carts.Add(cart);
            context.SaveChanges();
            cartId = cart.Id;
        });

        // Act
        var response = await _client.GetAsync($"api/carts/{cartId}");

        // Capturar erro se falhar
        if (!response.IsSuccessStatusCode)
        {
            var errorDetails = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Erro ao buscar carrinho: {response.StatusCode}, Resposta: {errorDetails}");
        }
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Resposta da API: {jsonResponse}");

        var cartResponse = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

        // 🔹 Assert - Confirma que os dados são retornados corretamente
        Assert.Equal(1, (int)cartResponse["data"]["userId"]);

        Console.WriteLine("Carrinho retornado corretamente!");
    }


    [Fact]
    public async Task UpdateCart_ShouldModifyCartInDatabase()
    {
        await AuthenticateClientAsync();

        int cartId = 0;

        ExecuteDbContext(context =>
        {
            //  Criar produto 2 antes de atualizar o carrinho
            if (!context.Products.Any(p => p.Id == 2))
            {
                context.Products.Add(new Ambev.DeveloperEvaluation.Domain.Entities.Product
                {
                    Id = 2, // Definir ID manualmente para garantir que o produto existe
                    Title = "Mouse Gamer",
                    Price = new Ambev.DeveloperEvaluation.Domain.ValueObjects.Money(199.99M),
                    Description = "Mouse gamer com iluminação RGB",
                    Image = "https://example.com/mouse.jpg",
                    CreatedAt = DateTime.UtcNow,
                    Category = new Ambev.DeveloperEvaluation.Domain.Entities.Category
                    {
                        Name = "Periféricos"
                    }
                });
                context.SaveChanges();
            }

            //  Criar carrinho com um produto existente
            var cart = new Ambev.DeveloperEvaluation.Domain.Entities.Cart
            {
                UserId = 1,
                Date = DateTime.UtcNow.Date, // 🔹 Apenas a data
                Items = new List<Ambev.DeveloperEvaluation.Domain.Entities.CartItem>
            {
                new() { ProductId = 1, Quantity = 1 } // 🔹 Produto 1 já existente
            }
            };
            context.Carts.Add(cart);
            context.SaveChanges();
            cartId = cart.Id;
        });

        //  Atualizar carrinho com produto 2 (agora criado corretamente)
        var updateCart = new
        {
            UserId = 1,
            Date = DateTime.UtcNow.Date, // 🔹 Apenas a data
            Products = new[] { new { ProductId = 2, Quantity = 5 } } //  Produto 2 agora existe no banco
        };

        var content = new StringContent(JsonConvert.SerializeObject(updateCart), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"api/carts/{cartId}", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorDetails = await response.Content.ReadAsStringAsync();
            Console.WriteLine($" Erro ao atualizar o carrinho: {response.StatusCode}, Resposta: {errorDetails}");
        }

        response.EnsureSuccessStatusCode();

        Console.WriteLine(" Carrinho atualizado com sucesso!");
    }


    [Fact]
    public async Task DeleteCart_ShouldRemoveCartFromDatabase()
    {
        await AuthenticateClientAsync();

        int cartId = 0;
        ExecuteDbContext(context =>
        {
            var cart = new Ambev.DeveloperEvaluation.Domain.Entities.Cart
            {
                UserId = 1,
                Date = DateTime.UtcNow,
                Items = new List<Ambev.DeveloperEvaluation.Domain.Entities.CartItem>
                {
                    new() { ProductId = 1, Quantity = 1 }
                }
            };
            context.Carts.Add(cart);
            context.SaveChanges();
            cartId = cart.Id;
        });

        var response = await _client.DeleteAsync($"api/carts/{cartId}");
        response.EnsureSuccessStatusCode();

        ExecuteDbContext(context =>
        {
            var dbCart = context.Carts.FirstOrDefaultAsync(c => c.Id == cartId).Result;
            Assert.Null(dbCart);
        });

        Console.WriteLine("Carrinho deletado com sucesso!");
    }

    [Fact]
    public async Task Checkout_ShouldProcessCheckoutSuccessfully()
    {
        await AuthenticateClientAsync();

        int cartId = 0;
        ExecuteDbContext(context =>
        {
            var cart = new Ambev.DeveloperEvaluation.Domain.Entities.Cart
            {
                UserId = 1,
                Date = DateTime.UtcNow,
                Items = new List<Ambev.DeveloperEvaluation.Domain.Entities.CartItem>
                {
                    new() { ProductId = 1, Quantity = 2 }
                }
            };
            context.Carts.Add(cart);
            context.SaveChanges();
            cartId = cart.Id;
        });

        var response = await _client.PostAsync($"api/carts/{cartId}/checkout", null);
        response.EnsureSuccessStatusCode();

        Console.WriteLine("Checkout realizado com sucesso!");
    }
}
