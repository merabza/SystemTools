using LanguageExt;
using Microsoft.Extensions.Logging;
using Moq;
using OneOf;
using SystemTools.SystemToolsShared.Errors;
using Xunit;

namespace SystemTools.SystemToolsShared.Tests.StSharedTests;

public sealed class StSharedProcessTests
{
    private readonly Mock<ILogger> _mockLogger;

    public StSharedProcessTests()
    {
        _mockLogger = new Mock<ILogger>();
    }

    [Fact]
    public void RunProcessWithOutput_WithValidCommand_ReturnsOutput()
    {
        // Arrange
        bool useConsole = false;

        // Act
        OneOf<(string, int), Err[]> result =
            StShared.RunProcessWithOutput(useConsole, _mockLogger.Object, "cmd", "/c echo test");

        // Assert
        Assert.True(result.IsT0);
        (string output, int exitCode) = result.AsT0;
        Assert.Equal(0, exitCode);
        Assert.Contains("test", output);
    }

    [Fact]
    public void RunProcess_WithValidCommand_ReturnsNone()
    {
        // Arrange
        bool useConsole = false;

        // Act
        Option<Err[]> result = StShared.RunProcess(useConsole, _mockLogger.Object, "cmd", "/c echo test");

        // Assert
        Assert.True(result.IsNone);
    }

    [Fact]
    public void IsAllowExitCode_WithZero_ReturnsTrue()
    {
        // Arrange & Act
        Option<Err[]> result = StShared.RunProcess(false, _mockLogger.Object, "cmd", "/c exit 0");

        // Assert
        Assert.True(result.IsNone);
    }

    [Fact]
    public void IsAllowExitCode_WithAllowedCode_ReturnsTrue()
    {
        // Arrange & Act
        Option<Err[]> result = StShared.RunProcess(false, _mockLogger.Object, "cmd", "/c exit 1", [1]);

        // Assert
        Assert.True(result.IsNone);
    }
}
