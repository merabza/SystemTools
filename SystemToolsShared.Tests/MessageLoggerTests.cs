using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using SystemToolsShared.Errors;
using Xunit;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace SystemToolsShared.Tests;

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

    [Fact]
    public async Task LogErrorAndSendMessageFromError_LogsErrorAndSends()
    {
        var logger = new TestMessageLogger(_mockLogger.Object, _mockMessagesDataManager.Object, UserName, false);

        var result = await logger.LogErrorAndSendMessageFromError("E1", "Error message");

        _mockMessagesDataManager.Verify(m => m.SendMessage(UserName, "Error message", It.IsAny<CancellationToken>()),
            Times.Once);
        var err = Assert.Single(result);
        Assert.Equal("E1", err.ErrorCode);
        Assert.Equal("Error message", err.ErrorMessage);
    }

    [Fact]
    public async Task LogErrorAndSendMessageFromError_ReturnsError_WhenNoDataManager()
    {
        var logger = new TestMessageLogger(_mockLogger.Object, null, UserName, false);

        var result = await logger.LogErrorAndSendMessageFromError("E2", "No manager");

        var err = Assert.Single(result);
        Assert.Equal("E2", err.ErrorCode);
        Assert.Equal("No manager", err.ErrorMessage);
    }

    [Fact]
    public async Task LogErrorAndSendMessageFromException_LogsAndSends()
    {
        var logger = new TestMessageLogger(_mockLogger.Object, _mockMessagesDataManager.Object, UserName, false);
        var ex = new InvalidOperationException("fail");

        var result = await logger.LogErrorAndSendMessageFromException(ex, "TestMethod");

        _mockMessagesDataManager.Verify(
            m => m.SendMessage(UserName, It.Is<string>(s => s.Contains("Error in TestMethod")),
                It.IsAny<CancellationToken>()), Times.Once);

        Assert.Equal("ErrorCaught", result.ErrorCode);
        Assert.Contains("Error in TestMethod", result.ErrorMessage);
    }

    private class TestMessageLogger : MessageLogger
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

        public new ValueTask LogInfoAndSendMessage(string message, object? arg1,
            CancellationToken cancellationToken = default)
        {
            return base.LogInfoAndSendMessage(message, arg1, cancellationToken);
        }

        public new ValueTask LogInfoAndSendMessage(string message, object? arg1, object? arg2,
            CancellationToken cancellationToken = default)
        {
            return base.LogInfoAndSendMessage(message, arg1, arg2, cancellationToken);
        }

        public new ValueTask LogInfoAndSendMessage(string message, object? arg1, object? arg2, object? arg3,
            CancellationToken cancellationToken = default)
        {
            return base.LogInfoAndSendMessage(message, arg1, arg2, arg3, cancellationToken);
        }

        public new ValueTask LogInfoAndSendMessage(string message, object? arg1, object? arg2, object? arg3,
            object? arg4, CancellationToken cancellationToken = default)
        {
            return base.LogInfoAndSendMessage(message, arg1, arg2, arg3, arg4, cancellationToken);
        }

        public new ValueTask LogWarningAndSendMessage(string message, CancellationToken cancellationToken = default)
        {
            return base.LogWarningAndSendMessage(message, cancellationToken);
        }

        public new ValueTask LogWarningAndSendMessage(string message, object? arg1,
            CancellationToken cancellationToken = default)
        {
            return base.LogWarningAndSendMessage(message, arg1, cancellationToken);
        }

        public new ValueTask LogWarningAndSendMessage(string message, object? arg1, object? arg2,
            CancellationToken cancellationToken = default)
        {
            return base.LogWarningAndSendMessage(message, arg1, arg2, cancellationToken);
        }

        public new ValueTask<Err[]> LogErrorAndSendMessageFromError(string errorCode, string message,
            CancellationToken cancellationToken = default)
        {
            return base.LogErrorAndSendMessageFromError(errorCode, message, cancellationToken);
        }

        public new ValueTask<Err[]> LogErrorAndSendMessageFromError(Err error,
            CancellationToken cancellationToken = default)
        {
            return base.LogErrorAndSendMessageFromError(error, cancellationToken);
        }

        public new ValueTask<Err> LogErrorAndSendMessageFromException(Exception ex, string methodName,
            CancellationToken cancellationToken = default)
        {
            return base.LogErrorAndSendMessageFromException(ex, methodName, cancellationToken);
        }
    }
}