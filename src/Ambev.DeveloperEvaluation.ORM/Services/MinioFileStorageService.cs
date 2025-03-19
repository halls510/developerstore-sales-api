using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Ambev.DeveloperEvaluation.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.ORM.Services;

public class MinioFileStorageService : IFileStorageService
{
    private readonly ILogger<MinioFileStorageService> _logger;
    private const string BucketName = "teste11";
    private const string ApiEndpoint = "https://minio-api.hallison.com.br";
    private const string AccessKey = "admin";
    private const string SecretKey = "admin123";
    private const string Region = "us-east-1";
    private const string ServiceName = "s3";

    public MinioFileStorageService(ILogger<MinioFileStorageService> logger)
    {
        _logger = logger;
    }

    public async Task<string?> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        try
        {
            if (!IsValidImage(contentType))
                throw new InvalidOperationException("Formato de arquivo não suportado.");

            await EnsureBucketExistsAsync();
            string objectName = $"{Guid.NewGuid()}-{fileName}";
            string url = $"{ApiEndpoint}/{BucketName}/{objectName}";

            var request = new HttpRequestMessage(HttpMethod.Put, url)
            {
                Content = new StreamContent(fileStream)
            };
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            SignRequest(request, "PUT", BucketName, objectName);

            using var client = new HttpClient();
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            _logger.LogInformation($"[UPLOAD] Sucesso: {objectName} enviado para {BucketName}");
            return url;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao enviar arquivo: {ex.Message}");
            return null;
        }
    }

    //public async Task<string?> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    //{
    //    try
    //    {
    //        if (!IsValidImage(contentType))
    //            throw new InvalidOperationException("Formato de arquivo não suportado.");

    //        await EnsureBucketExistsAsync();
    //        return "";
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError($"Erro ao enviar arquivo: {ex.Message}");
    //        return null;
    //    }
    //}

    private async Task EnsureBucketExistsAsync()
    {
        try
        {
            string url = $"{ApiEndpoint}/{BucketName}";
            var request = new HttpRequestMessage(HttpMethod.Put, url);
            var content = new StringContent(string.Empty);
            request.Content = content;

            SignRequest(request, "PUT", BucketName, null);

            using var client = new HttpClient();
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            _logger.LogInformation($"[BUCKET] Verificação concluída: {BucketName} está pronto para uso");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao verificar/criar bucket: {ex.Message}");
        }
    }

    private void SignRequest(HttpRequestMessage request, string httpMethod, string bucket, string? objectName)
    {
        string amzDate = DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture);
        string dateStamp = amzDate.Substring(0, 8);
        string canonicalUri = objectName != null ? $"/{bucket}/{objectName}" : $"/{bucket}";
        string canonicalQueryString = "";
        string payloadHash = request.Content != null
            ? Hash(Encoding.UTF8.GetString(request.Content.ReadAsByteArrayAsync().Result))
            : "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";

        string contentTypeHeader = request.Content?.Headers.ContentType?.ToString() ?? "application/octet-stream";

        string canonicalHeaders = $"content-type:{contentTypeHeader}\n" +
                                   $"host:{new Uri(ApiEndpoint).Host}\n" +
                                   $"x-amz-content-sha256:{payloadHash}\n" +
                                   $"x-amz-date:{amzDate}\n";

        string signedHeaders = "content-type;host;x-amz-content-sha256;x-amz-date";

        string canonicalRequest = $"{httpMethod}\n{canonicalUri}\n{canonicalQueryString}\n{canonicalHeaders}\n{signedHeaders}\n{payloadHash}";
        string credentialScope = $"{dateStamp}/{Region}/{ServiceName}/aws4_request";
        string stringToSign = $"AWS4-HMAC-SHA256\n{amzDate}\n{credentialScope}\n{Hash(canonicalRequest)}";

        byte[] signingKey = GetSignatureKey(SecretKey, dateStamp, Region, ServiceName);
        string signature = HexEncode(HmacSHA256(stringToSign, signingKey));

        string authorizationHeader = $"AWS4-HMAC-SHA256 Credential={AccessKey}/{credentialScope}, SignedHeaders={signedHeaders}, Signature={signature}";

        request.Headers.TryAddWithoutValidation("Authorization", authorizationHeader);
        request.Headers.TryAddWithoutValidation("x-amz-date", amzDate);
        request.Headers.TryAddWithoutValidation("x-amz-content-sha256", payloadHash);
    }

    private static string Hash(string data)
    {
        using var sha256 = SHA256.Create();
        return BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(data))).Replace("-", "").ToLower();
    }

    private static byte[] HmacSHA256(string data, byte[] key)
    {
        using var hmac = new HMACSHA256(key);
        return hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
    }

    private static byte[] GetSignatureKey(string key, string dateStamp, string regionName, string serviceName)
    {
        byte[] kDate = HmacSHA256(dateStamp, Encoding.UTF8.GetBytes("AWS4" + key));
        byte[] kRegion = HmacSHA256(regionName, kDate);
        byte[] kService = HmacSHA256(serviceName, kRegion);
        return HmacSHA256("aws4_request", kService);
    }

    private static string HexEncode(byte[] data) => BitConverter.ToString(data).Replace("-", "").ToLower();

    private bool IsValidImage(string contentType)
    {
        var allowedTypes = new HashSet<string> { "image/png", "image/jpeg", "image/jpg", "image/gif", "image/webp" };
        return allowedTypes.Contains(contentType);
    }
}



////using System.Globalization;
////using System.Net.Http.Headers;
////using System.Security.Cryptography;
////using System.Text;
////using Ambev.DeveloperEvaluation.Domain.Services;
////using Microsoft.Extensions.Logging;

////namespace Ambev.DeveloperEvaluation.ORM.Services;

////public class MinioFileStorageService : IFileStorageService
////{
////    private readonly ILogger<MinioFileStorageService> _logger;
////    private const string BucketName = "teste3";
////    private const string ApiEndpoint = "https://minio-api.hallison.com.br";
////    private const string AccessKey = "admin";
////    private const string SecretKey = "admin123";
////    private const string Region = "us-east-1";
////    private const string ServiceName = "s3";

////    public MinioFileStorageService(ILogger<MinioFileStorageService> logger)
////    {
////        _logger = logger;
////    }

////    public async Task<string?> UploadFileAsync(Stream fileStream, string fileName, string contentType)
////    {
////        try
////        {
////            if (!IsValidImage(contentType))
////                throw new InvalidOperationException("Formato de arquivo não suportado.");

////            await EnsureBucketExistsAsync();
////            //string objectName = $"{Guid.NewGuid()}-{fileName}";
////            //string url = $"{ApiEndpoint}/{BucketName}/{objectName}";

////            //var request = new HttpRequestMessage(HttpMethod.Put, url)
////            //{
////            //    Content = new StreamContent(fileStream)
////            //};
////            //request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

////            //SignRequest(request, "PUT", BucketName, objectName);

////            //using var client = new HttpClient();
////            //var response = await client.SendAsync(request);
////            //response.EnsureSuccessStatusCode();

////            //_logger.LogInformation($"[UPLOAD] Sucesso: {objectName} enviado para {BucketName}");
////            //return url;
////            return "";
////        }
////        catch (Exception ex)
////        {
////            _logger.LogError($"Erro ao enviar arquivo: {ex.Message}");
////            return null;
////        }
////    }

////    private async Task EnsureBucketExistsAsync()
////    {
////        try
////        {
////            string url = $"{ApiEndpoint}/{BucketName}";
////            var request = new HttpRequestMessage(HttpMethod.Put, url);
////            SignRequest(request, "PUT", BucketName, null);

////            using var client = new HttpClient();
////            var response = await client.SendAsync(request);
////            response.EnsureSuccessStatusCode();

////            _logger.LogInformation($"[BUCKET] Verificação concluída: {BucketName} está pronto para uso");
////        }
////        catch (Exception ex)
////        {
////            _logger.LogError($"Erro ao verificar/criar bucket: {ex.Message}");
////        }
////    }

////    private void SignRequest(HttpRequestMessage request, string httpMethod, string bucket, string? objectName)
////    {
////        string amzDate = DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture);
////        string dateStamp = amzDate.Substring(0, 8);
////        string canonicalUri = objectName != null ? $"/{bucket}/{objectName}" : $"/{bucket}";
////        string canonicalQueryString = "";
////        string payloadHash = request.Content != null
////            ? Hash(Encoding.UTF8.GetString(request.Content.ReadAsByteArrayAsync().Result))
////            : "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";

////        string contentTypeHeader = request.Content?.Headers.ContentType?.ToString() ?? "application/octet-stream";

////        string canonicalHeaders = $"content-type:{contentTypeHeader}\n" +
////                                   $"host:{new Uri(ApiEndpoint).Host}\n" +
////                                   $"x-amz-content-sha256:{payloadHash}\n" +
////                                   $"x-amz-date:{amzDate}\n";

////        string signedHeaders = "content-type;host;x-amz-content-sha256;x-amz-date";

////        string canonicalRequest = $"{httpMethod}\n{canonicalUri}\n{canonicalQueryString}\n{canonicalHeaders}\n{signedHeaders}\n{payloadHash}";
////        string credentialScope = $"{dateStamp}/{Region}/{ServiceName}/aws4_request";
////        string stringToSign = $"AWS4-HMAC-SHA256\n{amzDate}\n{credentialScope}\n{Hash(canonicalRequest)}";

////        byte[] signingKey = GetSignatureKey(SecretKey, dateStamp, Region, ServiceName);
////        string signature = HexEncode(HmacSHA256(stringToSign, signingKey));

////        string authorizationHeader = $"AWS4-HMAC-SHA256 Credential={AccessKey}/{credentialScope}, SignedHeaders={signedHeaders}, Signature={signature}";

////        request.Headers.TryAddWithoutValidation("Authorization", authorizationHeader);
////        request.Headers.TryAddWithoutValidation("x-amz-date", amzDate);
////        request.Headers.TryAddWithoutValidation("x-amz-content-sha256", payloadHash);
////    }


////    private static string Hash(string data)
////    {
////        using var sha256 = SHA256.Create();
////        return BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(data))).Replace("-", "").ToLower();
////    }

////    private static byte[] HmacSHA256(string data, byte[] key)
////    {
////        using var hmac = new HMACSHA256(key);
////        return hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
////    }

////    private static byte[] GetSignatureKey(string key, string dateStamp, string regionName, string serviceName)
////    {
////        byte[] kDate = HmacSHA256(dateStamp, Encoding.UTF8.GetBytes("AWS4" + key));
////        byte[] kRegion = HmacSHA256(regionName, kDate);
////        byte[] kService = HmacSHA256(serviceName, kRegion);
////        return HmacSHA256("aws4_request", kService);
////    }

////    private static string HexEncode(byte[] data) => BitConverter.ToString(data).Replace("-", "").ToLower();

////    private bool IsValidImage(string contentType)
////    {
////        var allowedTypes = new HashSet<string> { "image/png", "image/jpeg", "image/jpg", "image/gif", "image/webp" };
////        return allowedTypes.Contains(contentType);
////    }
////}
