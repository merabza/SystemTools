using System;
using Xunit;

namespace SystemTools.SystemToolsShared.Tests.StSharedTests;

public sealed class StSharedTimeTests
{
    [Fact]
    public void TimeTakenMessage_WithLessThanOneMinute_ShowsOnlySeconds()
    {
        // Arrange
        DateTime startTime = DateTime.Now.AddSeconds(-30);

        // Act
        string result = StShared.TimeTakenMessage(startTime);

        // Assert
        Assert.Contains("30 seconds", result);
        Assert.DoesNotContain("minutes", result);
        Assert.DoesNotContain("hours", result);
    }

    [Fact]
    public void TimeTakenMessage_WithMinutes_ShowsMinutesAndSeconds()
    {
        // Arrange
        DateTime startTime = DateTime.Now.AddMinutes(-2).AddSeconds(-15);

        // Act
        string result = StShared.TimeTakenMessage(startTime);

        // Assert
        Assert.Contains("2 minutes", result);
        Assert.Contains("15 seconds", result);
        Assert.DoesNotContain("hours", result);
    }

    [Fact]
    public void TimeTakenMessage_WithHours_ShowsAllUnits()
    {
        // Arrange
        DateTime startTime = DateTime.Now.AddHours(-1).AddMinutes(-30).AddSeconds(-45);

        // Act
        string result = StShared.TimeTakenMessage(startTime);

        // Assert
        Assert.Contains("1 hours", result);
        Assert.Contains("30 minutes", result);
        Assert.Contains("45 seconds", result);
    }
}
