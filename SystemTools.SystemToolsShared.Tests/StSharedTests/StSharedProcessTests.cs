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

    [Fact]
    public void RunProcessWithOutput_WithFailingCommand_ReturnsErrorOutputInMessage()
    {
        // Act
        Result<(string, int)> result = StShared.RunProcessWithOutput(false, _mockLogger.Object, "cmd",
            "/c \"echo boom 1>&2 & exit 3\"");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("ExitCode=3", result.Error.Description);
        Assert.Contains("boom", result.Error.Description);
    }

    [Fact]
    public void RunProcessWithOutput_WithErrorOutputAndZeroExitCode_ReturnsSuccess()
    {
        // Act
        Result<(string, int)> result = StShared.RunProcessWithOutput(false, _mockLogger.Object, "cmd",
            "/c \"echo warn 1>&2 & echo out\"");

        // Assert
        Assert.True(result.IsSuccess);
        (string output, int exitCode) = result.Value;
        Assert.Equal(0, exitCode);
        Assert.Contains("out", output);
        Assert.DoesNotContain("warn", output);
    }

    //pipe-ის ბუფერზე მეტი stderr არ უნდა გაჭედოს პროცესი
    [Fact]
    public void RunProcessWithOutput_WithLargeErrorOutput_DoesNotHang()
    {
        // Act
        Result<(string, int)> result = StShared.RunProcessWithOutput(false, _mockLogger.Object, "cmd",
            "/c \"for /L %i in (1,1,4000) do @echo 0123456789012345678901234567890123456789012345678901234567890123 1>&2\"");

        // Assert
        Assert.True(result.IsSuccess);
    }
}
