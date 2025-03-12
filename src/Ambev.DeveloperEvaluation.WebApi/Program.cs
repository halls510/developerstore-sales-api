using Ambev.DeveloperEvaluation.Application;
using Ambev.DeveloperEvaluation.Application.Sales.Events;
using Ambev.DeveloperEvaluation.Common.HealthChecks;
using Ambev.DeveloperEvaluation.Common.Logging;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.IoC;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi.Configurations;
using Ambev.DeveloperEvaluation.WebApi.Middleware;
using MediatR;
using Rebus;
using Microsoft.EntityFrameworkCore;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Serilog;
using System.Web;

namespace Ambev.DeveloperEvaluation.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            Log.Information("Starting web application");

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            builder.AddDefaultLogging();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.AddBasicHealthChecks();           

            builder.Services.AddDbContext<DefaultContext>(options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("Ambev.DeveloperEvaluation.ORM")
                )
            );

            // Configuração do Swagger
            builder.Services.AddSwaggerDocumentation();

            builder.Services.AddJwtAuthentication(builder.Configuration);

            builder.RegisterDependencies();

            builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(ApplicationLayer).Assembly);

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(
                    typeof(ApplicationLayer).Assembly,
                    typeof(Program).Assembly
                );
            });

            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));     

            var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "ambev.developerevaluation.rabbitmq";
            var rabbitUser = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "admin";
            var rabbitPass = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? "admin";

            // Codifica a senha para evitar problemas com caracteres especiais
            var encodedUser = HttpUtility.UrlEncode(rabbitUser);
            var encodedPass = HttpUtility.UrlEncode(rabbitPass);

            var rabbitMqConnectionString = $"amqp://{encodedUser}:{encodedPass}@{rabbitHost}";

            Console.WriteLine($"🔵 Conectando ao RabbitMQ: {rabbitMqConnectionString}");      

            builder.Services.AddRebus(config => config
             .Transport(t => t.UseRabbitMq(rabbitMqConnectionString, "sales-exchange"))
             .Routing(r => r.TypeBased()
                 .Map<SaleCreatedEvent>("queue_sales_created")
                 .Map<SaleModifiedEvent>("queue_sales_updated")
                 .Map<SaleCancelledEvent>("queue_sales_cancelled")
                 .Map<ItemCancelledEvent>("queue_sales_item_cancelled"))
             .Logging(l => l.Serilog())
            );

            var app = builder.Build();

            // Rodar migrations automaticamente ao iniciar
            using (var scope = app.Services.CreateScope())
            {                
                var dbContext = scope.ServiceProvider.GetRequiredService<DefaultContext>();
                //.Database.Migrate();
                dbContext.Database.EnsureCreated();                
            }

            app.UseMiddleware<GlobalExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Sale API V1");
                });
            }                        

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseBasicHealthChecks();

            app.MapControllers();

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
