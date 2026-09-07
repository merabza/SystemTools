using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace SystemTools.SystemToolsShared.Tests;

public sealed class MessageLoggerTests
{
    private const string UserName = "testUser";
    private readonly Mock<ILogger> _mockLogger = new();
    private readonly Mock<IMessagesDataManager> _mockMessagesDataManager = new();

    [Fact]
    public async Task LogInfoAndSendMessage_Console_WritesToConsole()
    {
        var logger = new TestMessageLogger(null, null, null, true);

        // ReSharper disable once using
        // ReSharper disable once DisposableConstructor
        await using var sw = new StringWriter();
        Console.SetOut(sw);

        await logger.LogInfoAndSendMessage("Hello Console");

        Assert.Contains("Hello Console", sw.ToString());
    }

    [Fact]
    public async Task LogInfoAndSendMessage_SendsMessage()
    {
        var logger = new TestMessageLogger(null, _mockMessagesDataManager.Object, UserName, false);

        await logger.LogInfoAndSendMessage("Test message");

        _mockMessagesDataManager.Verify(m => m.SendMessage(UserName, "Test message", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task LogWarningAndSendMessage_LogsWarningAndSends()
    {
        var logger = new TestMessageLogger(_mockLogger.Object, _mockMessagesDataManager.Object, UserName, false);

        await logger.LogWarningAndSendMessage("Warn!");

        _mockMessagesDataManager.Verify(m => m.SendMessage(UserName, "Warn!", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private sealed class TestMessageLogger : MessageLogger
    {
        public TestMessageLogger(ILogger? logger, IMessagesDataManager? messagesDataManager, string? userName,
            bool useConsole) : base(logger, messagesDataManager, userName, useConsole)
        {
        }

        // Expose protected methods for testing
        public new ValueTask LogInfoAndSendMessage(string message, CancellationToken cancellationToken = default)
        {
            return base.LogInfoAndSendMessage(message, cancellationToken);
        }

        public new ValueTask LogWarningAndSendMessage(string message, CancellationToken cancellationToken = default)
        {
            return base.LogWarningAndSendMessage(message, cancellationToken);
        }
    }
}
