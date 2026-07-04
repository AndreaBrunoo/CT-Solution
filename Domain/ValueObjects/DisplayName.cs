namespace Sln.Domain.ValueObjects;

public sealed class DisplayName : ValueObject
{
    public string Value { get; }

    public DisplayName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Name is required.", nameof(value));
        }

        Value = value.Trim();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}