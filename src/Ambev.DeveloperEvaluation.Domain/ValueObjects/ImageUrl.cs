namespace Ambev.DeveloperEvaluation.Domain.ValueObjects;

public class ImageUrl
{
    public string Url { get; private set; }

    public ImageUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url) || !url.StartsWith("http"))
            throw new ArgumentException("A URL da imagem deve ser válida.");

        Url = url;
    }
}