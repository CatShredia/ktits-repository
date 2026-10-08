namespace AutoService.Core;

// Максимальные длины полей.
// Используются в конфигурациях таблиц и при проверке ввода.
public static class FieldLimits
{
    public const int Name = 200;
    public const int Login = 64;
    public const int PasswordHash = 200;
    public const int Phone = 32;
    public const int Email = 200;
    public const int Vin = 17;
    public const int LicensePlate = 16;
    public const int Model = 100;
    public const int Color = 50;
    public const int Notes = 1000;
    public const int Description = 2000;
    public const int Sku = 64;
    public const int Status = 32;
    public const int TransactionNumber = 64;
    public const int Inspection = 1000;
    public const int Comment = 2000;
    public const int Fault = 2000;
}
