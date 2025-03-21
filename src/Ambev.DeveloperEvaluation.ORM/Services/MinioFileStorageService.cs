using Ambev.DeveloperEvaluation.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace Ambev.DeveloperEvaluation.ORM.Services;

public class MinioFileStorageService : IFileStorageService
{
    private readonly ILogger<MinioFileStorageService> _logger;
    private readonly IMinioClient _minioClient;
    private readonly string _bucketName;
    private readonly string _apiEndpoint;

    public MinioFileStorageService(ILogger<MinioFileStorageService> logger, IConfiguration configuration)
    {
        _logger = logger;

        _apiEndpoint = configuration["Minio:ApiEndpoint"] ?? throw new ArgumentNullException("ApiEndpoint não configurado.");
        _bucketName = configuration["Minio:BucketName"] ?? throw new ArgumentNullException("BucketName não configurado.");
        string accessKey = configuration["Minio:AccessKey"] ?? throw new ArgumentNullException("AccessKey não configurado.");
        string secretKey = configuration["Minio:SecretKey"] ?? throw new ArgumentNullException("SecretKey não configurado.");
        string region = configuration["Minio:Region"] ?? "us-east-1";

        _minioClient = new MinioClient()
            .WithEndpoint(_apiEndpoint)
            .WithCredentials(accessKey, secretKey)
            .WithRegion(region)
            .WithSSL()
            .Build();
    }

    public async Task<string?> UploadFileAsync(IFormFile file)
    {
        try
        {
            await EnsureBucketExistsAsync();

            using var stream = file.OpenReadStream();
            string objectName = GenerateObjectName(file.FileName);

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName)
                .WithStreamData(stream)
                .WithObjectSize(file.Length)
                .WithContentType(file.ContentType);

            await _minioClient.PutObjectAsync(putObjectArgs);

            string url = $"{_apiEndpoint}/{_bucketName}/{objectName}";
            return url;
        }
        catch (MinioException minioEx)
        {
            _logger.LogError($"Erro ao enviar arquivo para o MinIO: {minioEx.Message}");
            throw new Exception("Falha ao armazenar arquivo no MinIO.", minioEx);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro inesperado ao enviar arquivo: {ex.Message}");
            throw new Exception("Ocorreu um erro ao fazer upload do arquivo.", ex);
        }
    }

    private async Task EnsureBucketExistsAsync()
    {
        try
        {
            var bucketExistsArgs = new BucketExistsArgs().WithBucket(_bucketName);
            bool exists = await _minioClient.BucketExistsAsync(bucketExistsArgs);

            if (!exists)
            {
                _logger.LogInformation($"Bucket {_bucketName} não encontrado. Criando...");
                var makeBucketArgs = new MakeBucketArgs().WithBucket(_bucketName);
                await _minioClient.MakeBucketAsync(makeBucketArgs);
            }
        }
        catch (MinioException minioEx)
        {
            _logger.LogError($"Erro ao verificar ou criar bucket: {minioEx.Message}");
            throw new Exception("Erro ao verificar/criar bucket no MinIO.", minioEx);
        }
    }

    private string GenerateObjectName(string fileName)
    {       
        string fileExtension = Path.GetExtension(fileName).ToLower(); // Mantém a extensão original
        return $"{Guid.NewGuid()}{fileExtension}";
    }

}
