namespace ProductionSystem.Client;

/// <summary>
/// Avalonia 11 DatePicker использует DateTimeOffset?, API и доменная модель — DateOnly.
/// </summary>
public static class DatePickerValue
{
    public static DateTimeOffset? FromDateOnly(DateOnly? date)
    {
        if (date is null)
            return null;

        var dt = date.Value.ToDateTime(TimeOnly.MinValue);
        return new DateTimeOffset(dt, TimeZoneInfo.Local.GetUtcOffset(dt));
    }

    public static DateOnly? ToDateOnly(DateTimeOffset? value)
    {
        if (value is null)
            return null;

        return DateOnly.FromDateTime(value.Value.LocalDateTime.Date);
    }

    public static DateTimeOffset? Today(int addDays = 0)
        => FromDateOnly(DateOnly.FromDateTime(DateTime.Today.AddDays(addDays)));
}
