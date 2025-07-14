using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using SystemToolsShared.Errors;

namespace SystemToolsShared.Tests;

public sealed class MessageLoggerTests
{
    private readonly Mock<ILogger> _mockLogger = new();
    private readonly Mock<IMessagesDataManager> _mockMessagesDataManager = new();
    private readonly string _userName = "testUser";

    private class TestMessageLogger : MessageLogger
    {
        public TestMessageLogger(ILogger? logger, IMessagesDataManager? messagesDataManager, string? userName,
            bool useConsole) : base(logger, messagesDataManager, userName, useConsole)
        {
        }

        // Expose protected methods for testing
        public new ValueTask LogInfoAndSendMessage(string message, CancellationToken cancellationToken = default) =>
            base.LogInfoAndSendMessage(message, cancellationToken);

        public new ValueTask LogInfoAndSendMessage(string message, object? arg1,
            CancellationToken cancellationToken = default) =>
            base.LogInfoAndSendMessage(message, arg1, cancellationToken);

        public new ValueTask LogInfoAndSendMessage(string message, object? arg1, object? arg2,
            CancellationToken cancellationToken = default) =>
            base.LogInfoAndSendMessage(message, arg1, arg2, cancellationToken);

        public new ValueTask LogInfoAndSendMessage(string message, object? arg1, object? arg2, object? arg3,
            CancellationToken cancellationToken = default) =>
            base.LogInfoAndSendMessage(message, arg1, arg2, arg3, cancellationToken);

        public new ValueTask LogInfoAndSendMessage(string message, object? arg1, object? arg2, object? arg3,
            object? arg4, CancellationToken cancellationToken = default) =>
            base.LogInfoAndSendMessage(message, arg1, arg2, arg3, arg4, cancellationToken);

        public new ValueTask LogWarningAndSendMessage(string message, CancellationToken cancellationToken = default) =>
            base.LogWarningAndSendMessage(message, cancellationToken);

        public new ValueTask LogWarningAndSendMessage(string message, object? arg1,
            CancellationToken cancellationToken = default) =>
            base.LogWarningAndSendMessage(message, arg1, cancellationToken);

        public new ValueTask LogWarningAndSendMessage(string message, object? arg1, object? arg2,
            CancellationToken cancellationToken = default) =>
            base.LogWarningAndSendMessage(message, arg1, arg2, cancellationToken);

        public new ValueTask<IEnumerable<Err>> LogErrorAndSendMessageFromError(string errorCode, string message,
            CancellationToken cancellationToken = default) =>
            base.LogErrorAndSendMessageFromError(errorCode, message, cancellationToken);

        public new ValueTask<IEnumerable<Err>> LogErrorAndSendMessageFromError(Err error,
            CancellationToken cancellationToken = default) =>
            base.LogErrorAndSendMessageFromError(error, cancellationToken);

        public new ValueTask<IEnumerable<Err>> LogErrorAndSendMessageFromException(Exception ex, string methodName,
            CancellationToken cancellationToken = default) =>
            base.LogErrorAndSendMessageFromException(ex, methodName, cancellationToken);
    }

    [Fact]
    public async Task LogInfoAndSendMessage_Console_WritesToConsole()
    {
        var logger = new TestMessageLogger(null, null, null, true);
        using var sw = new StringWriter();
        Console.SetOut(sw);

        await logger.LogInfoAndSendMessage("Hello Console");

        Assert.Contains("Hello Console", sw.ToString());
    }

    [Fact]
    public async Task LogInfoAndSendMessage_SendsMessage()
    {
        var logger = new TestMessageLogger(null, _mockMessagesDataManager.Object, _userName, false);

        await logger.LogInfoAndSendMessage("Test message");

        _mockMessagesDataManager.Verify(m => m.SendMessage(_userName, "Test message", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task LogWarningAndSendMessage_LogsWarningAndSends()
    {
        var logger = new TestMessageLogger(_mockLogger.Object, _mockMessagesDataManager.Object, _userName, false);

        await logger.LogWarningAndSendMessage("Warn!");

        _mockMessagesDataManager.Verify(m => m.SendMessage(_userName, "Warn!", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task LogErrorAndSendMessageFromError_LogsErrorAndSends()
    {
        var logger = new TestMessageLogger(_mockLogger.Object, _mockMessagesDataManager.Object, _userName, false);

        var result = await logger.LogErrorAndSendMessageFromError("E1", "Error message");

        _mockMessagesDataManager.Verify(m => m.SendMessage(_userName, "Error message", It.IsAny<CancellationToken>()),
            Times.Once);
        Assert.Collection(result, err =>
        {
            Assert.Equal("E1", err.ErrorCode);
            Assert.Equal("Error message", err.ErrorMessage);
        });
    }

    [Fact]
    public async Task LogErrorAndSendMessageFromError_ReturnsError_WhenNoDataManager()
    {
        var logger = new TestMessageLogger(_mockLogger.Object, null, _userName, false);

        var result = await logger.LogErrorAndSendMessageFromError("E2", "No manager");

        Assert.Collection(result, err =>
        {
            Assert.Equal("E2", err.ErrorCode);
            Assert.Equal("No manager", err.ErrorMessage);
        });
    }

    [Fact]
    public async Task LogErrorAndSendMessageFromException_LogsAndSends()
    {
        var logger = new TestMessageLogger(_mockLogger.Object, _mockMessagesDataManager.Object, _userName, false);
        var ex = new InvalidOperationException("fail");

        var result = await logger.LogErrorAndSendMessageFromException(ex, "TestMethod");

        _mockMessagesDataManager.Verify(
            m => m.SendMessage(_userName, It.Is<string>(s => s.Contains("Error in TestMethod")),
                It.IsAny<CancellationToken>()), Times.Once);
        Assert.Collection(result, err =>
        {
            Assert.Equal("ErrorCaught", err.ErrorCode);
            Assert.Contains("Error in TestMethod", err.ErrorMessage);
        });
    }
}