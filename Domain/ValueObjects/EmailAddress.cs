using System.ComponentModel.DataAnnotations;

namespace Sln.Domain.ValueObjects;

public sealed class EmailAddress : ValueObject
{
    public string Value { get; }

    public EmailAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Email address is required.", nameof(value));
        }

        var normalized = value.Trim();
        if (!new EmailAddressAttribute().IsValid(normalized))
        {
            throw new ArgumentException("Email address is invalid.", nameof(value));
        }

        Value = normalized.ToLowerInvariant();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}