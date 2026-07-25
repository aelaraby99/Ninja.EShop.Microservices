namespace NINJA.EShop.Ordering.Domain.ValueObjects;

public record OrderName
{
    public string Value { get; }
    private const int DefaultLength = 5;
    private OrderName(string value) => Value = value;
    public static OrderName Of(string value)
    {
        ArgumentException.ThrowIfNullOrEmpty(value,nameof(value));
        ArgumentOutOfRangeException.ThrowIfNotEqual(value.Length,DefaultLength);
        return new OrderName(value);
    }
}