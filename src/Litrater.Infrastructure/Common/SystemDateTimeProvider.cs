using Litrater.Application.Abstractions.Common;

namespace Litrater.Infrastructure.Common;

public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}