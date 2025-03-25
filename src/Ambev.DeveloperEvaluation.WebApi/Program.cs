using Ambev.DeveloperEvaluation.Application;
using Ambev.DeveloperEvaluation.Common.HealthChecks;
using Ambev.DeveloperEvaluation.Common.Logging;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.IoC;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi.Configurations;
using Ambev.DeveloperEvaluation.WebApi.Middleware;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Ambev.DeveloperEvaluation.WebApi.Services;
using Ambev.DeveloperEvaluation.Application.Common.Messaging;
using Ambev.DeveloperEvaluation.Application.Uploads;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.ORM.Services;
using Microsoft.AspNetCore.Http.Features;
using Ambev.DeveloperEvaluation.Common.Configuration;

namespace Ambev.DeveloperEvaluation.WebApi;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            Log.Information("Starting web application");

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            builder.AddDefaultLogging();

            // 1️⃣ Adiciona a política de CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularDev", policy =>
                {
                    policy.WithOrigins(
                        "http://localhost:4200", // acesso pelo host
                        "http://ambev.developerevaluation.frontend" // acesso interno entre containers
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });


            // Habilita suporte para uploads de arquivos grandes
            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 104857600; // 100MB de limite
            });

            // ORDEM DE CARREGAMENTO DAS CONFIGS
            builder.Configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables(); // .env e docker-compose vars

            if (builder.Environment.IsDevelopment())
            {
                builder.Configuration.AddUserSecrets<Program>(); // só local
            }

            builder.Services.AddControllers()
                  .AddMvcOptions(options =>
                  {
                      options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
                  });

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

            // Criação das filas no RabbitMq
            RabbitMqSetup.EnsureRabbitMqQueuesExist(builder.Configuration);

            // Configurar inicialização de Seed do Banco de dados e Minio em segundo plano
            builder.Services.AddSingleton<InitializerSeedService>();

            // Registra o Publisher no container de DI
            builder.Services.AddTransient<IRabbitMqPublisher, RabbitMqPublisher>();


            // Adiciona UploadImageHandler
            builder.Services.AddScoped<UploadImageHandler>();

            // Aqui precisa ter:
            builder.Services.Configure<MinioSettings>(
                builder.Configuration.GetSection("MinioSettings"));

            // Adiciona MinioFileStorageService implementando IFileStorageService
            builder.Services.AddScoped<IFileStorageService, MinioFileStorageService>();           

            var app = builder.Build();

            // Aplica as migrations automaticamente
            using (var scope = app.Services.CreateScope())
            {
                if (app.Environment.IsDevelopment())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DefaultContext>();
                    await context.Database.MigrateAsync();

                    var services = scope.ServiceProvider;
                    var seedService = services.GetRequiredService<InitializerSeedService>();
                    await seedService.RunManuallyAsync();
                }
            }           

            // 2️⃣ Aplica o CORS antes de Authorization
            app.UseCors("AllowAngularDev");

            app.UseMiddleware<GlobalExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Sale API V1");
                });
            }
            else
            {

                app.UseHttpsRedirection();
            }

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
