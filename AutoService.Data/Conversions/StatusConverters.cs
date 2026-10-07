using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AutoService.Data.Conversions;

internal static class StatusConverters
{
    public static readonly ValueConverter<WorkOrderStatus, string> WorkOrderStatus = new(
        value => StatusText.WorkOrder(value),
        value => StatusText.WorkOrder(value));

    public static readonly ValueConverter<InvoiceStatus, string> InvoiceStatus = new(
        value => StatusText.Invoice(value),
        value => StatusText.Invoice(value));

    public static readonly ValueConverter<RepairBayKind, string> RepairBayKind = new(
        value => StatusText.RepairBay(value),
        value => StatusText.RepairBay(value));
}
