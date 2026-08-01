namespace NINJA.EShop.Ordering.Domain.ValueObjects;

public record OrderName
{
    public string Value { get; }
    private OrderName(string value) => Value = value;
    public static OrderName Of(string value)
    {
        ArgumentException.ThrowIfNullOrEmpty(value,nameof(value));
        return new OrderName(value);
    }
}