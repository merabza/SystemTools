using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace SystemToolsShared.Tests.StSharedTests;

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
        var useConsole = false;

        // Act
        var result = StShared.RunProcessWithOutput(useConsole, _mockLogger.Object, "cmd", "/c echo test");

        // Assert
        Assert.True(result.IsT0);
        var (output, exitCode) = result.AsT0;
        Assert.Equal(0, exitCode);
        Assert.Contains("test", output);
    }

    [Fact]
    public void RunProcess_WithValidCommand_ReturnsNone()
    {
        // Arrange
        var useConsole = false;

        // Act
        var result = StShared.RunProcess(useConsole, _mockLogger.Object, "cmd", "/c echo test");

        // Assert
        Assert.True(result.IsNone);
    }

    [Fact]
    public void IsAllowExitCode_WithZero_ReturnsTrue()
    {
        // Arrange & Act
        var result = StShared.RunProcess(false, _mockLogger.Object, "cmd", "/c exit 0");

        // Assert
        Assert.True(result.IsNone);
    }

    [Fact]
    public void IsAllowExitCode_WithAllowedCode_ReturnsTrue()
    {
        // Arrange & Act
        var result = StShared.RunProcess(false, _mockLogger.Object, "cmd", "/c exit 1", [1]);

        // Assert
        Assert.True(result.IsNone);
    }
}
