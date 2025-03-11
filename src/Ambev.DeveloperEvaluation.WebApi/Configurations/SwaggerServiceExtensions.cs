using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Ambev.DeveloperEvaluation.WebApi.Configurations;

public static class SwaggerServiceExtensions
{
    public static void AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Sales API",
                Version = "v1",
                Description = "Sales API"
            });

            // Configurar o caminho do arquivo XML gerado pelo .NET
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);

            // Configuração para autenticação JWT no Swagger
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,  // Defina o Type como Http
                Scheme = "Bearer"  // Esquema precisa ser "Bearer" para que seja incluído no cabeçalho
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "Bearer", // Garantir que o esquema seja Bearer
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                       new string[] {}
                    }
                });
        });
    }
}
