namespace CinemaBlazor.Services;

public interface IApiUrlService
{
    string GetBaseUrl();
    string GetImageUrl(string? imageUrl);
}

public class ApiUrlService : IApiUrlService
{
    private readonly string _baseUrl;

    public ApiUrlService(IConfiguration configuration)
    {
        _baseUrl = configuration.GetValue<string>("ApiBaseUrl") ?? "http://localhost:5268";
    }

    // ! GetBaseUrl - returns API base URL from configuration
    // вызывается из компонентов для получения полного URL изображения
    public string GetBaseUrl()
    {
        return _baseUrl;
    }

    // ! GetImageUrl - converts relative image URL to absolute URL with base
    // вызывается из компонентов для отображения изображений фильмов
    public string GetImageUrl(string? imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
            return string.Empty;

        if (imageUrl.StartsWith("/"))
        {
            return $"{_baseUrl.TrimEnd('/')}{imageUrl}";
        }

        return imageUrl;
    }
}
