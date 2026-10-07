namespace AutoService.Core.Enums;

public enum AppointmentStatus
{
    Запланирована,
    Подтверждена,
    Отменена,
    Выполнена
}

public enum WorkOrderStatus
{
    Создан,
    Диагностика,
    ВРемонте,
    Завершён,
    Отменён
}

public enum InvoiceStatus
{
    Выставлен,
    ЧастичноОплачен,
    Оплачен
}

public enum PaymentStatus
{
    Проведён,
    Отменён
}

public enum PaymentMethod
{
    Наличные,
    Карта,
    Перевод
}

public enum RepairBayKind
{
    Подъёмник,
    ДиагностическийПост,
    ШиномонтажнаяЗона,
    КузовнойУчасток
}
