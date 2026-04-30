using System.Globalization;
using Litrater.Domain.Common;

namespace Litrater.Domain.Books;

public sealed record Rating : IValueObject
{
    public Rating(int value)
    {
        if (value is < 1 or > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Rating must be between 1 and 5");
        }

        Value = value;
    }

    public int Value { get; }

    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }
}