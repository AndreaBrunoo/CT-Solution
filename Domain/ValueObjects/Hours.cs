namespace Sln.Domain.ValueObjects;

public sealed class Hours : ValueObject
{
    public int Value { get; }

    public Hours(int value)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Hours must be greater than zero.");
        }

        Value = value;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}