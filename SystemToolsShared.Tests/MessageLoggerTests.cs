using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using SystemToolsShared;
using SystemToolsShared.Errors;

namespace SystemToolsShared.Tests;

public sealed class MessageLoggerTests
{
    private sealed class TestableMessageLogger : MessageLogger
    {
        public TestableMessageLogger(ILogger? logger, IMessagesDataManager? messagesDataManager, string? userName, bool useConsole)
            : base(logger, messagesDataManager, userName, useConsole) { }

        public new async ValueTask LogInfoAndSendMessage(string message, CancellationToken cancellationToken = default) =>
            await base.LogInfoAndSendMessage(message, cancellationToken);
        public new async ValueTask LogInfoAndSendMessage(string message, object? arg1, CancellationToken cancellationToken = default) =>
            await base.LogInfoAndSendMessage(message, arg1, cancellationToken);
        public new async ValueTask LogInfoAndSendMessage(string message, object? arg1, object? arg2, CancellationToken cancellationToken = default) =>
            await base.LogInfoAndSendMessage(message, arg1, arg2, cancellationToken);
        public new async ValueTask LogInfoAndSendMessage(string message, object? arg1, object? arg2, object? arg3, CancellationToken cancellationToken = default) =>
            await base.LogInfoAndSendMessage(message, arg1, arg2, arg3, cancellationToken);
        public new async ValueTask LogInfoAndSendMessage(string message, object? arg1, object? arg2, object? arg3, object? arg4, CancellationToken cancellationToken = default) =>
            await base.LogInfoAndSendMessage(message, arg1, arg2, arg3, arg4, cancellationToken);
        public new async ValueTask LogWarningAndSendMessage(string message, CancellationToken cancellationToken = default) =>
            await base.LogWarningAndSendMessage(message, cancellationToken);
        public new async ValueTask LogWarningAndSendMessage(string message, object? arg1, CancellationToken cancellationToken = default) =>
            await base.LogWarningAndSendMessage(message, arg1, cancellationToken);
        public new async ValueTask LogWarningAndSendMessage(string message, object? arg1, object? arg2, CancellationToken cancellationToken = default) =>
            await base.LogWarningAndSendMessage(message, arg1, arg2, cancellationToken);
        public new async ValueTask<IEnumerable<Err>> LogErrorAndSendMessageFromError(string errorCode, string message, CancellationToken cancellationToken = default) =>
            await base.LogErrorAndSendMessageFromError(errorCode, message, cancellationToken);
        public new async ValueTask<IEnumerable<Err>> LogErrorAndSendMessageFromError(Err error, CancellationToken cancellationToken = default) =>
            await base.LogErrorAndSendMessageFromError(error, cancellationToken);
        public new async ValueTask<IEnumerable<Err>> LogErrorAndSendMessageFromException(Exception ex, string methodName, CancellationToken cancellationToken = default) =>
            await base.LogErrorAndSendMessageFromException(ex, methodName, cancellationToken);
    }

    private readonly Mock<ILogger> _mockLogger = new();
    private readonly Mock<IMessagesDataManager> _mockMessages = new();
    private readonly StringWriter _consoleOut = new();
    private readonly string _userName = "testUser";

    public MessageLoggerTests()
    {
        Console.SetOut(_consoleOut);
    }

    [Fact]
    public async Task LogInfoAndSendMessage_ConsoleTrue_WritesToConsoleAndSendsMessage()
    {
        var logger = new TestableMessageLogger(null, _mockMessages.Object, _userName, true);
        await logger.LogInfoAndSendMessage("info message");
        Assert.Contains("info message", _consoleOut.ToString());
        _mockMessages.Verify(m => m.SendMessage(_userName, "info message", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LogWarningAndSendMessage_LogsWarningAndSends()
    {
        var logger = new TestableMessageLogger(_mockLogger.Object, _mockMessages.Object, _userName, false);
        await logger.LogWarningAndSendMessage("warn");
        _mockMessages.Verify(m => m.SendMessage(_userName, "warn", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LogErrorAndSendMessageFromError_ReturnsErrAndSends()
    {
        var logger = new TestableMessageLogger(_mockLogger.Object, _mockMessages.Object, _userName, false);
        var result = await logger.LogErrorAndSendMessageFromError("E1", "errormsg");
        Assert.Contains(result, e => e.ErrorCode == "E1" && e.ErrorMessage == "errormsg");
        _mockMessages.Verify(m => m.SendMessage(_userName, "errormsg", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LogErrorAndSendMessageFromException_ReturnsErrAndSends()
    {
        var logger = new TestableMessageLogger(_mockLogger.Object, _mockMessages.Object, _userName, false);
        var ex = new InvalidOperationException("fail");
        var result = await logger.LogErrorAndSendMessageFromException(ex, "TestMethod");
        Assert.Contains(result, e => e.ErrorCode == "ErrorCaught" && e.ErrorMessage.Contains("TestMethod"));
        _mockMessages.Verify(m => m.SendMessage(_userName, It.Is<string>(s => s.Contains("TestMethod")), It.IsAny<CancellationToken>()), Times.Once);
    }
}
