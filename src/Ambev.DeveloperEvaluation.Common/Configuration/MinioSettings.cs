namespace Ambev.DeveloperEvaluation.Common.Configuration;

public class MinioSettings
{
    public string ApiEndpoint { get; set; } = string.Empty;
    public string PublicUrl { get; set; } = string.Empty;
    public string BucketName { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public bool UseHttps { get; set; } = false;
}
