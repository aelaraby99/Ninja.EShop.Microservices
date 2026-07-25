namespace NINJA.EShop.Ordering.Domain.ValueObjects;

public record Payment
{
    public string? CardName { get; } = default!;
    public string CardNumber { get; } = default!;
    public string Expiration { get; } = default!;
    public string CVV { get; } = default!;
    public int PaymentMethod { get; } = default!;
    protected Payment() { }
    private Payment(string? cardName,string cardNumber,string expiration,string cvv,int paymentMethod)
    {
        CardName = cardName;
        CardNumber = cardNumber;
        Expiration = expiration;
        CVV = cvv;
        PaymentMethod = paymentMethod;
    }
    public static Payment Create(string? cardName,string cardNumber,string expiration,string cvv,int paymentMethod)
    {
        ArgumentException.ThrowIfNullOrEmpty(cardNumber,nameof(cardNumber));
        ArgumentException.ThrowIfNullOrEmpty(expiration,nameof(expiration));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(cvv.Length,3);
        if (paymentMethod < 0)
            throw new ArgumentOutOfRangeException(nameof(paymentMethod),"Payment method cannot be negative.");
        return new Payment(cardName,cardNumber,expiration,cvv,paymentMethod);
    }
}