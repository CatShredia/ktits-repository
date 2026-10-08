using AutoService.Data.Conversions;

namespace AutoService.Data.Configurations;

// Правила таблицы счетов и формулы итога.
// Подключается в AutoServiceDbContext.OnModelCreating.
internal sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("invoices", table =>
        {
            table.HasComment("Счета");
            table.HasCheckConstraint(
                "ck_invoices_status",
                "status in ('Выставлен', 'Частично оплачен', 'Оплачен')");
            table.HasCheckConstraint("ck_invoices_amount", "amount_before_discount >= 0");
            table.HasCheckConstraint("ck_invoices_discount", "discount >= 0 and discount <= amount_before_discount");
            table.HasCheckConstraint("ck_invoices_tax_rate", "tax_rate >= 0 and tax_rate <= 100");
            table.HasCheckConstraint("ck_invoices_tax", "tax >= 0");
            table.HasCheckConstraint(
                "ck_invoices_tax_formula",
                "tax = round((amount_before_discount - discount) * tax_rate / 100, 2)");
            table.HasCheckConstraint(
                "ck_invoices_total",
                "total = amount_before_discount - discount + tax");
        });
        builder.Property(x => x.CreatedAt).HasColumnType("timestamp without time zone");
        builder.Property(x => x.AmountBeforeDiscount).HasPrecision(12, 2);
        builder.Property(x => x.Discount).HasPrecision(12, 2);
        builder.Property(x => x.TaxRate).HasPrecision(5, 2);
        builder.Property(x => x.Tax).HasPrecision(12, 2);
        builder.Property(x => x.Total).HasPrecision(12, 2);
        builder.Property(x => x.Status).HasConversion(StatusConverters.InvoiceStatus).HasMaxLength(FieldLimits.Status);
        builder.HasOne(x => x.WorkOrder).WithMany(x => x.Invoices).HasForeignKey(x => x.WorkOrderId);
    }
}

// Правила таблицы платежей.
// Подключается в AutoServiceDbContext.OnModelCreating.
internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments", table =>
        {
            table.HasComment("Платежи");
            table.HasCheckConstraint("ck_payments_amount", "amount > 0");
            table.HasCheckConstraint("ck_payments_method", "method in ('Наличные', 'Карта', 'Перевод')");
            table.HasCheckConstraint("ck_payments_status", "status in ('Проведён', 'Отменён')");
        });
        builder.Property(x => x.PaidAt).HasColumnType("timestamp without time zone");
        builder.Property(x => x.Amount).HasPrecision(12, 2);
        builder.Property(x => x.Method).HasConversion<string>().HasMaxLength(FieldLimits.Status);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(FieldLimits.Status);
        builder.Property(x => x.TransactionNumber).HasMaxLength(FieldLimits.TransactionNumber).IsRequired();
        builder.HasOne(x => x.Invoice).WithMany(x => x.Payments).HasForeignKey(x => x.InvoiceId);
    }
}

// Правила таблицы отзывов.
// Подключается в AutoServiceDbContext.OnModelCreating.
internal sealed class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("reviews", table =>
        {
            table.HasComment("Отзывы клиентов");
            table.HasCheckConstraint("ck_reviews_rating", "rating >= 1 and rating <= 5");
        });
        builder.Property(x => x.Comment).HasMaxLength(FieldLimits.Comment).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnType("timestamp without time zone");
        builder.HasIndex(x => x.WorkOrderId).IsUnique();
        builder.HasOne(x => x.Client).WithMany(x => x.Reviews).HasForeignKey(x => x.ClientId);
        builder.HasOne(x => x.WorkOrder).WithOne(x => x.Review).HasForeignKey<Review>(x => x.WorkOrderId);
    }
}
