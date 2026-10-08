namespace AutoService.Core.Enums;

// Русские подписи статусов заказа, счёта и ремонтного места.
// Используется в сервисах Workshop и в фильтрах экранов.
public static class StatusText
{
    public static string WorkOrder(WorkOrderStatus status) => status switch
    {
        WorkOrderStatus.ВРемонте => "В ремонте",
        _ => status.ToString()
    };

    public static WorkOrderStatus WorkOrder(string value) => value switch
    {
        "В ремонте" => WorkOrderStatus.ВРемонте,
        _ => Enum.Parse<WorkOrderStatus>(value)
    };

    public static string Invoice(InvoiceStatus status) => status switch
    {
        InvoiceStatus.ЧастичноОплачен => "Частично оплачен",
        _ => status.ToString()
    };

    public static InvoiceStatus Invoice(string value) => value switch
    {
        "Частично оплачен" => InvoiceStatus.ЧастичноОплачен,
        _ => Enum.Parse<InvoiceStatus>(value)
    };

    public static string RepairBay(RepairBayKind kind) => kind switch
    {
        RepairBayKind.ДиагностическийПост => "Диагностический пост",
        RepairBayKind.ШиномонтажнаяЗона => "Шиномонтажная зона",
        RepairBayKind.КузовнойУчасток => "Кузовной участок",
        _ => kind.ToString()
    };

    public static RepairBayKind RepairBay(string value) => value switch
    {
        "Диагностический пост" => RepairBayKind.ДиагностическийПост,
        "Шиномонтажная зона" => RepairBayKind.ШиномонтажнаяЗона,
        "Кузовной участок" => RepairBayKind.КузовнойУчасток,
        _ => Enum.Parse<RepairBayKind>(value)
    };
}
