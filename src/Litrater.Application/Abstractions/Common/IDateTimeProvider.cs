namespace Litrater.Application.Abstractions.Common;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}