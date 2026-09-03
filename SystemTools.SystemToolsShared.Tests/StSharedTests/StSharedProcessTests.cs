using Microsoft.Extensions.Logging;
using Moq;
using SystemTools.SharedKernel;
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
        Result<(string, int)> result =
            StShared.RunProcessWithOutput(useConsole, _mockLogger.Object, "cmd", "/c echo test");

        // Assert
        Assert.True(result.IsSuccess);
        (string output, int exitCode) = result.Value;
        Assert.Equal(0, exitCode);
        Assert.Contains("test", output);
    }

    [Fact]
    public void RunProcess_WithValidCommand_ReturnsSuccess()
    {
        // Arrange
        bool useConsole = false;

        // Act
        Result result = StShared.RunProcess(useConsole, _mockLogger.Object, "cmd", "/c echo test");

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void IsAllowExitCode_WithZero_ReturnsTrue()
    {
        // Arrange & Act
        Result result = StShared.RunProcess(false, _mockLogger.Object, "cmd", "/c exit 0");

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void IsAllowExitCode_WithAllowedCode_ReturnsTrue()
    {
        // Arrange & Act
        Result result = StShared.RunProcess(false, _mockLogger.Object, "cmd", "/c exit 1", [1]);

        // Assert
        Assert.True(result.IsSuccess);
    }
}
