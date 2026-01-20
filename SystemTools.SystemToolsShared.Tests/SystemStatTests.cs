using System;
using System.Runtime.InteropServices;
using Xunit;

namespace SystemTools.SystemToolsShared.Tests;

public sealed class SystemStatTests
{
    [Fact]
    public void IsWindows_ShouldReturnCorrectPlatform()
    {
        // Act
        bool result = SystemStat.IsWindows();

        // Assert
        bool expected = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void IsWindows_ShouldMatchRuntimeInformation()
    {
        // Arrange
        OSPlatform platformWindows = OSPlatform.Windows;

        // Act
        bool isWindowsResult = SystemStat.IsWindows();
        bool runtimeResult = RuntimeInformation.IsOSPlatform(platformWindows);

        // Assert
        Assert.Equal(runtimeResult, isWindowsResult);
    }

    [Fact]
    public void IsWindows_ReturnsTrue_OnWindowsPlatform()
    {
        // Arrange & Act
        bool result = SystemStat.IsWindows();

        // Assert
        // This test will only pass on Windows. On other platforms, it will fail.
        // If you want platform-independent tests, consider using mocking frameworks.
        if (OperatingSystem.IsWindows())
        {
            Assert.True(result);
        }
        else
        {
            Assert.False(result);
        }
    }
}
