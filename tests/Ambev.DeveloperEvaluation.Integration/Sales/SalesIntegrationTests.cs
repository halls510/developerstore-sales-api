using Ambev.DeveloperEvaluation.Integration.Infrastructure;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Sales;

public class SalesIntegrationTests : IntegrationTestBase
{
    public SalesIntegrationTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task ListSales_ShouldReturnSalesFromDatabase()
    {
        await AuthenticateClientAsync();

        // Arrange - Criar uma venda antes de buscar
        ExecuteDbContext(context =>
        {
            var sale = new Sale(1, "Carlos Silva");
            sale.AddItem(new SaleItem(1, "Produto Teste", 2, new Money(50M), new Money(0M), new Money(100M)));
            sale.CompleteSale();

            context.Sales.Add(sale);
            context.SaveChanges();
        });

        // Act
        var response = await _client.GetAsync("api/sales");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var responseData = JObject.Parse(jsonResponse);
        var sales = responseData["data"].ToObject<List<JObject>>();

        // Assert
        Assert.NotEmpty(sales);
        Console.WriteLine("Vendas retornadas com sucesso!");
    }

    [Fact]
    public async Task GetSaleById_ShouldReturnCorrectSale()
    {
        await AuthenticateClientAsync();

        int saleId = 0;
        ExecuteDbContext(context =>
        {
            var sale = new Sale(1, "Carlos Silva");
            sale.AddItem(new SaleItem(1, "Produto Teste", 2, new Money(50M), new Money(0M), new Money(100M)));
            sale.CompleteSale();

            context.Sales.Add(sale);
            context.SaveChanges();
            saleId = sale.Id;
        });

        // Act
        var response = await _client.GetAsync($"api/sales/{saleId}");
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var saleResponse = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

        Assert.Equal(1, (int)saleResponse["data"]["customerId"]);
        Console.WriteLine("Venda retornada corretamente!");
    }

    [Fact]
    public async Task CancelSale_ShouldMarkSaleAsCancelled()
    {
        await AuthenticateClientAsync();

        int saleId = 0;
        ExecuteDbContext(context =>
        {
            var sale = new Sale(1, "Carlos Silva");
            sale.AddItem(new SaleItem(1, "Produto Teste", 2, new Money(50M), new Money(0M), new Money(100M)));            

            context.Sales.Add(sale);
            context.SaveChanges();
            saleId = sale.Id;
        });

        
        var response = await _client.PatchAsync($"api/sales/{saleId}/cancel", null);
        if (!response.IsSuccessStatusCode)
        {
            var errorDetails = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Erro ao cancelar venda: {response.StatusCode}, Resposta: {errorDetails}");
        }
        response.EnsureSuccessStatusCode();

        ExecuteDbContext(context =>
        {
            var dbSale = context.Sales.FirstOrDefault(s => s.Id == saleId);
            Assert.NotNull(dbSale);
            Assert.Equal(SaleStatus.Cancelled, dbSale.Status);
        });

        Console.WriteLine("Venda cancelada com sucesso!");
    }

    [Fact]
    public async Task CancelItem_ShouldMarkItemAsCancelled()
    {
        await AuthenticateClientAsync();

        int saleId = 0, productId = 2;
        ExecuteDbContext(context =>
        {
            var sale = new Sale(1, "Carlos Silva");
            sale.AddItems(new List<SaleItem>
        {
            new(1, "Produto 1", 2, new Money(50M), new Money(0M), new Money(100M)),
            new(2, "Produto 2", 1, new Money(100M), new Money(0M), new Money(100M))
        });

            context.Sales.Add(sale);
            context.SaveChanges();
            saleId = sale.Id;
        });

        var response = await _client.PatchAsync($"api/sales/{saleId}/items/{productId}/cancel", null);
        if (!response.IsSuccessStatusCode)
        {
            var errorDetails = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Erro ao cancelar item da venda: {response.StatusCode}, Resposta: {errorDetails}");
        }
        response.EnsureSuccessStatusCode();

        ExecuteDbContext(context =>
        {
            var dbSale = context.Sales.Include(s => s.Items).FirstOrDefault(s => s.Id == saleId);
            Assert.NotNull(dbSale);

            // 🔹 Verifica que o item ainda está na venda, mas agora está CANCELADO
            var cancelledItem = dbSale.Items.FirstOrDefault(i => i.ProductId == productId);
            Assert.NotNull(cancelledItem);
            Assert.Equal(SaleItemStatus.Cancelled, cancelledItem.Status);
        });

        Console.WriteLine("Item marcado como cancelado com sucesso!");
    }

}
