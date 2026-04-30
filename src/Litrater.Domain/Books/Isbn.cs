using System.Text.RegularExpressions;
using Litrater.Domain.Common;

namespace Litrater.Domain.Books;

public sealed partial record Isbn : IValueObject
{
    public Isbn(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        if (!GetIsbnRegex().IsMatch(value))
        {
            throw new ArgumentException("ISBN must be exactly 13 digits", nameof(value));
        }

        Value = value;
    }

    public string Value { get; }

    public override string ToString()
    {
        return Value;
    }

    [GeneratedRegex(@"^\d{13}$")]
    private static partial Regex GetIsbnRegex();
}