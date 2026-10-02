using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace SystemTools.ApiKeysManagement.Tests.TestDoubles;

//ინახავს ყოველი ჩანაწერის დონესა და დაფორმატებულ ტექსტს, რომ ტესტმა შეამოწმოს, რა ჩაიწერა ლოგში
internal sealed class CollectingLogger : ILogger
{
    private readonly bool _enabled;

    public CollectingLogger(bool enabled = true)
    {
        _enabled = enabled;
    }

    public List<(LogLevel Level, string Message)> Entries { get; } = [];

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return _enabled;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        Entries.Add((logLevel, formatter(state, exception)));
    }
}
