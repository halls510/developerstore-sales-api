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
    public static void Main(string[] args)
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
                    policy.WithOrigins("http://localhost:4200") // ✅ frontend Angular com HTTP
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                          //.AllowCredentials(); // se estiver usando cookies/autenticação
                });
            });

            // Habilita suporte para uploads de arquivos grandes
            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 104857600; // 100MB de limite
            });

            // Adicionando User Secrets
            if (builder.Environment.IsDevelopment())
            {
                builder.Configuration.AddUserSecrets<Program>();
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

            // Configurar inicialização do banco de dados em segundo plano
            builder.Services.AddHostedService<DbInitializerService>();

            // Registra o Publisher no container de DI
            builder.Services.AddTransient<IRabbitMqPublisher,RabbitMqPublisher>();


            // Adiciona UploadImageHandler
            builder.Services.AddScoped<UploadImageHandler>();

            // Aqui precisa ter:
            builder.Services.Configure<MinioSettings>(
                builder.Configuration.GetSection("MinioSettings"));

            // Adiciona MinioFileStorageService implementando IFileStorageService
            builder.Services.AddScoped<IFileStorageService, MinioFileStorageService>();

            var app = builder.Build();

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
