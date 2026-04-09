namespace CinemaAPI.Services;

public interface IImageService
{
    Task<string> SaveImageAsync(IFormFile file);
    Task<string> SaveImageToFolderAsync(IFormFile file, string folderName);
    void DeleteImage(string imageUrl);
    bool IsValidImage(IFormFile file);
}

public class ImageService : IImageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly string _filmsFolder = "images/films";

    private static readonly string[] AllowedMimeTypes =
    {
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/webp",
        "image/gif"
    };

    private const int MaxFileSize = 5 * 1024 * 1024; // 5 МБ

    public ImageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public bool IsValidImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return false;

        if (file.Length > MaxFileSize)
            return false;

        if (!AllowedMimeTypes.Contains(file.ContentType.ToLower()))
            return false;

        var extension = Path.GetExtension(file.FileName).ToLower();
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        if (!allowedExtensions.Contains(extension))
            return false;

        return true;
    }

    public async Task<string> SaveImageAsync(IFormFile file)
    {
        return await SaveImageToFolderAsync(file, _filmsFolder);
    }

    public async Task<string> SaveImageToFolderAsync(IFormFile file, string folderName)
    {
        if (!IsValidImage(file))
        {
            throw new ArgumentException("Invalid image file. Allowed types: JPEG, PNG, WebP, GIF. Max size: 5MB");
        }

        var extension = Path.GetExtension(file.FileName).ToLower();
        var fileName = $"{Guid.NewGuid()}{extension}";

        var webRootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        var uploadsFolder = Path.Combine(webRootPath, folderName);
        Directory.CreateDirectory(uploadsFolder);

        var filePath = Path.Combine(uploadsFolder, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/{folderName}/{fileName}";
    }

    public void DeleteImage(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
            return;

        if (!imageUrl.StartsWith("/images/"))
            return;

        try
        {
            var relativePath = imageUrl.TrimStart('/');
            var webRootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var filePath = Path.Combine(webRootPath, relativePath);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        catch
        {
        }
    }
}
