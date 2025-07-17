using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace SystemToolsShared.Tests.StSharedTests;

public sealed class StSharedConsoleTests
{
    private readonly Mock<ILogger> _mockLogger;
    private readonly StringWriter _stringWriter;

    public StSharedConsoleTests()
    {
        _mockLogger = new Mock<ILogger>();
        _stringWriter = new StringWriter();
        Console.SetOut(_stringWriter);
    }

    //[Fact]
    //public void WriteWarningLine_WithConsole_DisplaysWarning()
    //{
    //    // Arrange
    //    const string warningText = "Test warning";
    //    const bool useConsole = true;

    //    // Act
    //    StShared.WriteWarningLine(warningText, useConsole, _mockLogger.Object);

    //    // Assert
    //    var output = _stringWriter.ToString();
    //    Assert.Contains("[warning]", output);
    //    Assert.Contains(warningText, output);
    //    _mockLogger.Verify(
    //        x => x.Log(LogLevel.Warning, It.IsAny<EventId>(),
    //            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(warningText)), null,
    //            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    //}

    //[Fact]
    //public void WriteErrorLine_WithConsole_DisplaysError()
    //{
    //    // Arrange
    //    const string errorText = "Test error";
    //    const bool useConsole = true;

    //    // Act
    //    StShared.WriteErrorLine(errorText, useConsole, _mockLogger.Object, false);

    //    // Assert
    //    var output = _stringWriter.ToString();
    //    Assert.Contains("[ERROR]", output);
    //    Assert.Contains(errorText, output);
    //    _mockLogger.Verify(
    //        x => x.Log(LogLevel.Error, It.IsAny<EventId>(),
    //            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(errorText)), null,
    //            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    //}

    [Fact]
    public void WriteSuccessMessage_DisplaysInGreen()
    {
        // Arrange
        const string message = "Success message";

        // Act
        StShared.WriteSuccessMessage(message);

        // Assert
        var output = _stringWriter.ToString();
        Assert.Contains(message, output);
    }
}