using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.Integration.Infrastructure;

/// <summary>
/// Fábrica customizada para testes de integração.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            Console.WriteLine("🔹 Configurando WebApplicationFactory para testes...");

            // Remover configuração do banco de dados real
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DefaultContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
                Console.WriteLine("✅ Configuração de banco de dados removida.");
            }

            // Configurar banco de dados em memória para testes
            services.AddDbContext<DefaultContext>(options =>
            {
                options.UseInMemoryDatabase("TestDatabase");
            });

            // Criar banco de dados para testes
            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DefaultContext>();
            db.Database.EnsureCreated();

            Console.WriteLine("✅ Banco de dados de testes inicializado.");
        });

        Console.WriteLine("✅ API de Teste inicializada com sucesso!");
    }
}
