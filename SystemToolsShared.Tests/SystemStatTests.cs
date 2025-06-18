using System.Runtime.InteropServices;
using Xunit;

namespace SystemToolsShared.Tests;

public sealed class SystemStatTests
{
    [Fact]
    public void IsWindows_ShouldReturnCorrectPlatform()
    {
        // Act
        var result = SystemStat.IsWindows();

        // Assert
        var expected = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void IsWindows_ShouldMatchRuntimeInformation()
    {
        // Arrange
        var platformWindows = OSPlatform.Windows;

        // Act
        var isWindowsResult = SystemStat.IsWindows();
        var runtimeResult = RuntimeInformation.IsOSPlatform(platformWindows);

        // Assert
        Assert.Equal(runtimeResult, isWindowsResult);
    }
}