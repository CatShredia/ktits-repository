namespace ProductManagement;

public class ProductService
{
    /// <summary>Общая стоимость = цена * количество.</summary>
    public decimal CalculateTotalPrice(decimal price, int quantity)
    {
        if (price < 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Цена не может быть отрицательной.");
        if (quantity < 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Количество не может быть отрицательным.");

        return price * quantity;
    }

    /// <summary>Стоимость с учётом скидки в процентах (0..100).</summary>
    public decimal CalculateDiscount(decimal totalPrice, decimal discountPercent)
    {
        if (totalPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(totalPrice), "Стоимость не может быть отрицательной.");
        if (discountPercent < 0 || discountPercent > 100)
            throw new ArgumentOutOfRangeException(nameof(discountPercent), "Процент скидки должен быть в диапазоне от 0 до 100.");

        return totalPrice - totalPrice * discountPercent / 100m;
    }

    /// <summary>Проверяет данные товара. true — данные корректны.</summary>
    public bool ValidateProduct(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        return !string.IsNullOrWhiteSpace(product.Name)
            && product.Price > 0
            && product.Quantity >= 0
            && !string.IsNullOrWhiteSpace(product.Category);
    }
}
