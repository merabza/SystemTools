using System;

namespace SystemTools.SharedKernel;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
