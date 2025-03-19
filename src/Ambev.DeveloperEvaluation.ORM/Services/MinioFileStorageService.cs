using Ambev.DeveloperEvaluation.Domain.Services;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using System.Globalization;
using System.Text;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using Amazon;

namespace Ambev.DeveloperEvaluation.ORM.Services;

public class MinioFileStorageService : IFileStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly ILogger<MinioFileStorageService> _logger;
    private const string BucketName = "productssalesapi";
    private const string AccessKey = "admin";
    private const string SecretKey = "admin123";

    public MinioFileStorageService(ILogger<MinioFileStorageService> logger)
    {
        _logger = logger;
        _minioClient = new MinioClient()
            .WithEndpoint("minio-api.hallison.com.br")  // 🔹 Sem http:// ou https://
            .WithCredentials(AccessKey, SecretKey)
            .WithRegion("us-east-1")
            .WithSSL(false)  // 🔹 Se MinIO estiver em HTTPS, mude para true                        
            .Build();
    }

    public async Task<string?> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        string? uploadedFileUrl = null; // Variável para armazenar a URL do arquivo

        try
        {
            // 🔹 Garante que apenas arquivos de imagem sejam enviados
            if (!IsValidImage(contentType))
                throw new InvalidOperationException("Formato de arquivo não suportado.");

            var objectName = $"{Guid.NewGuid()}-{fileName}";

            // 🔹 Verifica e cria o bucket se necessário
            try
            {
                // 🔹 Gera `x-amz-date` com a data atual
                string amzDateBucket = DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture);

                // 🔹 Configura os headers adicionais
                var headersBucket = new Dictionary<string, string>
                     {
                         { "x-amz-date", amzDateBucket },
                         { "Authorization", $"AWS {AccessKey}:{SecretKey}" },
                         { "Content-Type", "application/xml" }
                     };


                //bool bucketExists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(BucketName).WithHeaders(headersBucket));
                //if (!bucketExists)
                //{

                _logger.LogInformation($"[MinIO] Criando bucket: {BucketName}");
                await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(BucketName).WithHeaders(headersBucket));
                //  }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao verificar/criar bucket: {ex.Message}");
                return "Erro ao criar bucket no MinIO.";
            }

            // 🔹 Gera `x-amz-date` com a data atual
            string amzDate = DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture);

            // 🔹 Gera token de autenticação Base64 para `Authorization: AWS`
            string authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{AccessKey}:{SecretKey}"));

            // 🔹 Configura os headers adicionais
            var headers = new Dictionary<string, string>
            {
                { "x-amz-date", amzDate },
                { "Authorization", $"AWS {authToken}" },
                { "Content-Type", contentType }
            };

            // 🔹 Faz o upload do arquivo no MinIO
            try
            {
                var response = await _minioClient.PutObjectAsync(new PutObjectArgs()
                      .WithBucket(BucketName)
                      .WithObject(objectName)
                      .WithStreamData(fileStream)
                      .WithObjectSize(fileStream.Length)
                      .WithHeaders(headers)
                      .WithContentType(contentType));

                uploadedFileUrl = $"https://minio-api.hallison.com.br/{BucketName}/{objectName}";

                _logger.LogInformation($"[UPLOAD] Sucesso: {objectName} enviado para {BucketName} às {amzDate}");
            }
            catch (MinioException ex)
            {
                if (ex.Message.Contains("403"))
                {
                    _logger.LogError("[FORBIDDEN] Erro de permissão no MinIO. Verifique as credenciais e a política do bucket.");
                    return "Permissão negada. Verifique as credenciais e a política do bucket.";
                }
                else
                {
                    _logger.LogError($"Erro ao enviar arquivo para MinIO: {ex.Message}");
                    return "Erro ao enviar arquivo para MinIO.";
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro inesperado ao enviar arquivo para MinIO: {ex.Message}");
            return "Erro inesperado ao processar o upload.";
        }

        // 🔹 Retorna a URL do arquivo se o upload foi bem-sucedido
        return uploadedFileUrl;
    }

    private bool IsValidImage(string contentType)
    {
        var allowedTypes = new HashSet<string>
        {
            "image/png",
            "image/jpeg",
            "image/jpg",
            "image/gif",
            "image/webp"
        };

        return allowedTypes.Contains(contentType);
    }

    
}
