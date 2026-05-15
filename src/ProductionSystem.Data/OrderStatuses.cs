namespace ProductionSystem.Data;

/// <summary>Статусы заказа (сессия 2, WSR C2 §2.2).</summary>
public static class OrderStatuses
{
    public const string New = "Новый";
    public const string Cancelled = "Отменен";
    public const string Specification = "Составление спецификации";
    public const string Confirmation = "Подтверждение";
    public const string Procurement = "Закупка";
    public const string Production = "Производство";
    public const string QualityControl = "Контроль";
    public const string Ready = "Готов";
    public const string Closed = "Закрыт";

    /// <summary>Отклонение на этапе подтверждения (WSR).</summary>
    public const string Rejected = "Отклонен";

    public static readonly string[] MainFlow =
    {
        New, Specification, Confirmation, Procurement, Production, QualityControl, Ready, Closed,
    };

    public static readonly string[] All =
    {
        New, Cancelled, Rejected, Specification, Confirmation, Procurement, Production, QualityControl, Ready, Closed,
    };

    public static bool IsTerminal(string status) =>
        status is Cancelled or Rejected or Closed;

    public static bool CanEditOrder(string status) => status == New;
}
