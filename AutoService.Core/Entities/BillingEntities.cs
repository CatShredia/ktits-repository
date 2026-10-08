using AutoService.Core.Enums;

namespace AutoService.Core.Entities;

// Счёт по завершённому заказ-наряду.
// Хранится в AutoServiceDbContext, ведётся в AdminService.
public sealed class Invoice
{
    public int Id { get; set; }
    public int WorkOrderId { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal AmountBeforeDiscount { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxRate { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
    public InvoiceStatus Status { get; set; }
    public WorkOrder WorkOrder { get; set; } = null!;
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

// Платёж по счёту.
// Хранится в AutoServiceDbContext, регистрируется в AdminService.
public sealed class Payment
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public DateTime PaidAt { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; }
    public required string TransactionNumber { get; set; }
    public Invoice Invoice { get; set; } = null!;
}

// Отзыв клиента по заказ-наряду.
// Хранится в AutoServiceDbContext, ведётся в ReviewService.
public sealed class Review
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int WorkOrderId { get; set; }
    public int Rating { get; set; }
    public required string Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public Client Client { get; set; } = null!;
    public WorkOrder WorkOrder { get; set; } = null!;
}
