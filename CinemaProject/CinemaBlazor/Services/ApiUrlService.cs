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

    public string GetBaseUrl()
    {
        return _baseUrl;
    }

    public string GetImageUrl(string? imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
            return string.Empty;

        // Если URL начинается с "/", это локальное изображение на API сервере
        if (imageUrl.StartsWith("/"))
        {
            return $"{_baseUrl.TrimEnd('/')}{imageUrl}";
        }

        // Иначе это внешний URL - возвращаем как есть
        return imageUrl;
    }
}
