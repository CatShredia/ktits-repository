namespace AutoService.Data.Workshop;

// Ошибка предметной области с текстом для пользователя.
// Выбрасывается сервисами Workshop и показывается на экранах.
public sealed class WorkshopException : Exception
{
    public WorkshopException(string message) : base(message)
    {
    }
}

// Названия ролей, как они записаны в базе.
// Сравниваются в ShellViewModel и ReviewsViewModel.
public static class RoleNames
{
    public const string Admin = "Администратор";
    public const string Mechanic = "Механик";
    public const string Chief = "Руководитель";
}

// Налог и итог счёта.
// Вызывается из AdminService при сохранении счёта.
public static class InvoiceMath
{
    public static (decimal Tax, decimal Total) Calculate(decimal amountBeforeDiscount, decimal discount, decimal taxRate)
    {
        if (amountBeforeDiscount < 0)
            throw new WorkshopException("Сумма до скидки не может быть отрицательной.");
        if (discount < 0 || discount > amountBeforeDiscount)
            throw new WorkshopException("Скидка должна быть от нуля до суммы до скидки.");
        if (taxRate < 0 || taxRate > 100)
            throw new WorkshopException("Ставка налога должна быть от 0 до 100.");

        var tax = Math.Round((amountBeforeDiscount - discount) * taxRate / 100m, 2, MidpointRounding.AwayFromZero);
        return (tax, amountBeforeDiscount - discount + tax);
    }
}
