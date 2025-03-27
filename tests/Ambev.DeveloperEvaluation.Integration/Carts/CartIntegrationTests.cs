using Ambev.DeveloperEvaluation.Integration.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Carts;

public class CartIntegrationTests : IntegrationTestBase
{    

    [Fact]
    public async Task CreateCart_ShouldAddCartToDatabase()
    {      

        var cart = new
        {
            UserId = 5,
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
            var dbCart = context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == 5).Result;
            Assert.NotNull(dbCart);
            Assert.Equal(2, dbCart.Items.Count);
        });

        Console.WriteLine("Carrinho criado e salvo no banco com sucesso!");
    }


    [Fact]
    public async Task GetCarts_ShouldReturnCartsFromDatabase()
    {

        // Arrange - Criar um carrinho antes de buscar
        ExecuteDbContext(context =>
        {
            context.Carts.Add(new Ambev.DeveloperEvaluation.Domain.Entities.Cart
            {
                UserId = 5,
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

        int cartId = 0;
        ExecuteDbContext(context =>
        {
            var cart = new Ambev.DeveloperEvaluation.Domain.Entities.Cart
            {
                UserId = 7, // Garantindo que o UserId é atribuído corretamente
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
        Assert.Equal(7, (int)cartResponse["data"]["userId"]);

        Console.WriteLine("Carrinho retornado corretamente!");
    }

    [Fact]
    public async Task DeleteCart_ShouldRemoveCartFromDatabase()
    {       

        int cartId = 0;
        ExecuteDbContext(context =>
        {
            var cart = new Ambev.DeveloperEvaluation.Domain.Entities.Cart
            {
                UserId = 6,
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
        var cart = new
        {
            UserId = 6,
            Date = DateTime.UtcNow,
            Products = new[]
            {
            new { ProductId = 1, Quantity = 2 }
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(cart), Encoding.UTF8, "application/json");
        var createCartResponse = await _client.PostAsync("api/carts", content);
        createCartResponse.EnsureSuccessStatusCode();

        // 🔹 Etapa 3: Obter o carrinho criado
        var createdCartJson = await createCartResponse.Content.ReadAsStringAsync();
        var createdCart = JsonConvert.DeserializeObject<dynamic>(createdCartJson);
        int cartId = createdCart?.data.id;

        // 🔹 Etapa 4: Realizar o checkout
        var checkoutResponse = await _client.PostAsync($"api/carts/{cartId}/checkout", null);

        if (!checkoutResponse.IsSuccessStatusCode)
        {
            var error = await checkoutResponse.Content.ReadAsStringAsync();
            Console.WriteLine($"Erro no checkout: {checkoutResponse.StatusCode} - {error}");
        }

        checkoutResponse.EnsureSuccessStatusCode();
        Console.WriteLine("Checkout realizado com sucesso!");
    }

}
