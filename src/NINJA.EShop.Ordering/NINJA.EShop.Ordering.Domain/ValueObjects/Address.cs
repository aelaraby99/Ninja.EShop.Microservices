namespace NINJA.EShop.Ordering.Domain.ValueObjects;

public record Address
{
    public string FirstName { get; } = default!;
    public string LastName { get; } = default!;
    public string? EmailAddress { get; } = default!;
    public string AddressLine { get; } = default!;
    public string Country { get; } = default!;
    public string State { get; } = default!;
    public string ZipCode { get; } = default!;
    protected Address() { }
    private Address(string firstName,string lastName,string? emailAddress,string addressLine,string country,string state,string zipCode)
    {
        FirstName = firstName;
        LastName = lastName;
        EmailAddress = emailAddress;
        AddressLine = addressLine;
        Country = country;
        State = state;
        ZipCode = zipCode;
    }
    public static Address Of(string firstName,string lastName,string? emailAddress,string addressLine,string country,string state,string zipCode)
    {
        ArgumentException.ThrowIfNullOrEmpty(firstName,nameof(firstName));
        ArgumentException.ThrowIfNullOrEmpty(lastName,nameof(lastName));
        ArgumentException.ThrowIfNullOrEmpty(addressLine,nameof(addressLine));
        ArgumentException.ThrowIfNullOrEmpty(country,nameof(country));
        ArgumentException.ThrowIfNullOrEmpty(state,nameof(state));
        ArgumentException.ThrowIfNullOrEmpty(zipCode,nameof(zipCode));
        return new Address(firstName,lastName,emailAddress,addressLine,country,state,zipCode);
    }
}