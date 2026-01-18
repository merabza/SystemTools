using System;
using System.Globalization;
using System.Threading;
using Xunit;

namespace SystemToolsShared.Tests;

public sealed class DateTimeExtendTests
{
    [Fact]
    public void StartOfPeriod_Year_ReturnsStartOfYear()
    {
        var date = new DateTime(2023, 5, 15, 10, 30, 45, DateTimeKind.Unspecified);
        var result = date.StartOfPeriod(EPeriodType.Year);
        Assert.Equal(new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified), result);
    }

    [Fact]
    public void StartOfPeriod_Quarter_ReturnsStartOfQuarter()
    {
        var date = new DateTime(2023, 5, 15, 0, 0, 0, DateTimeKind.Unspecified);
        var result = date.StartOfPeriod(EPeriodType.Quarter);
        Assert.Equal(new DateTime(2023, 4, 1, 0, 0, 0, DateTimeKind.Unspecified), result);
    }

    [Fact]
    public void StartOfPeriod_Month_ReturnsStartOfMonth()
    {
        var date = new DateTime(2023, 5, 15, 0, 0, 0, DateTimeKind.Unspecified);
        var result = date.StartOfPeriod(EPeriodType.Month);
        Assert.Equal(new DateTime(2023, 5, 1, 0, 0, 0, DateTimeKind.Unspecified), result);
    }

    [Fact]
    public void StartOfPeriod_Week_ReturnsStartOfWeek()
    {
        var date = new DateTime(2023, 5, 17, 0, 0, 0, DateTimeKind.Unspecified); // Wednesday
        var culture = new CultureInfo("en-US");
        Thread.CurrentThread.CurrentCulture = culture;
        var result = date.StartOfPeriod(EPeriodType.Week);
        Assert.Equal(new DateTime(2023, 5, 14, 0, 0, 0, DateTimeKind.Unspecified), result); // Sunday
    }

    [Fact]
    public void StartOfPeriod_Day_ReturnsStartOfDay()
    {
        var date = new DateTime(2023, 5, 15, 10, 30, 45, DateTimeKind.Unspecified);
        var result = date.StartOfPeriod(EPeriodType.Day);
        Assert.Equal(new DateTime(2023, 5, 15, 0, 0, 0, DateTimeKind.Unspecified), result);
    }

    [Fact]
    public void StartOfPeriod_Hour_ReturnsStartOfHour()
    {
        var date = new DateTime(2023, 5, 15, 10, 30, 45, DateTimeKind.Unspecified);
        var result = date.StartOfPeriod(EPeriodType.Hour);
        Assert.Equal(new DateTime(2023, 5, 15, 10, 0, 0, DateTimeKind.Unspecified), result);
    }

    [Fact]
    public void StartOfPeriod_Minute_ReturnsStartOfMinute()
    {
        var date = new DateTime(2023, 5, 15, 10, 30, 45, DateTimeKind.Unspecified);
        var result = date.StartOfPeriod(EPeriodType.Minute);
        Assert.Equal(new DateTime(2023, 5, 15, 10, 30, 0, DateTimeKind.Unspecified), result);
    }

    [Fact]
    public void StartOfPeriod_Second_ReturnsStartOfSecond()
    {
        var date = new DateTime(2023, 5, 15, 10, 30, 45, 123, DateTimeKind.Unspecified);
        var result = date.StartOfPeriod(EPeriodType.Second);
        Assert.Equal(new DateTime(2023, 5, 15, 10, 30, 45, 0, DateTimeKind.Unspecified), result);
    }

    [Fact]
    public void DateAdd_AddsCorrectly()
    {
        var date = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
        Assert.Equal(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified), date.DateAdd(EPeriodType.Year, 1));
        Assert.Equal(new DateTime(2023, 4, 1, 0, 0, 0, DateTimeKind.Unspecified), date.DateAdd(EPeriodType.Quarter, 1));
        Assert.Equal(new DateTime(2023, 2, 1, 0, 0, 0, DateTimeKind.Unspecified), date.DateAdd(EPeriodType.Month, 1));
        Assert.Equal(new DateTime(2023, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), date.DateAdd(EPeriodType.Week, 1));
        Assert.Equal(new DateTime(2023, 1, 2, 0, 0, 0, DateTimeKind.Unspecified), date.DateAdd(EPeriodType.Day, 1));
        Assert.Equal(new DateTime(2023, 1, 1, 1, 0, 0, DateTimeKind.Unspecified), date.DateAdd(EPeriodType.Hour, 1));
        Assert.Equal(new DateTime(2023, 1, 1, 0, 1, 0, DateTimeKind.Unspecified), date.DateAdd(EPeriodType.Minute, 1));
        Assert.Equal(new DateTime(2023, 1, 1, 0, 0, 1, DateTimeKind.Unspecified), date.DateAdd(EPeriodType.Second, 1));
    }

    [Fact]
    public void DateDiff_ReturnsCorrectDifference()
    {
        var d1 = new DateTime(2023, 5, 15, 10, 30, 45, DateTimeKind.Unspecified);
        var d2 = new DateTime(2020, 1, 1, 8, 15, 30, DateTimeKind.Unspecified);
        Assert.Equal(3, d1.DateDiff(EPeriodType.Year, d2));
        Assert.Equal(13, d1.DateDiff(EPeriodType.Quarter, d2));
        Assert.Equal(40, d1.DateDiff(EPeriodType.Month, d2));
        Assert.Equal(175, d1.DateDiff(EPeriodType.Week, d2));
        Assert.Equal((d1 - d2).Days, d1.DateDiff(EPeriodType.Day, d2));
        Assert.Equal((long)(d1 - d2).TotalHours, d1.DateDiff(EPeriodType.Hour, d2));
        Assert.Equal((long)(d1 - d2).TotalMinutes, d1.DateDiff(EPeriodType.Minute, d2));
        Assert.Equal((long)(d1 - d2).TotalSeconds, d1.DateDiff(EPeriodType.Second, d2));
    }

    [Fact]
    public void DifferenceMethods_ReturnCorrectValues()
    {
        var d1 = new DateTime(2023, 5, 15, 10, 30, 45, 500, DateTimeKind.Unspecified);
        var d2 = new DateTime(2020, 1, 1, 8, 15, 30, 250, DateTimeKind.Unspecified);
        Assert.Equal((long)(d1 - d2).TotalMilliseconds, d1.MillisecondsDifference(d2));
        Assert.Equal((long)(d1 - d2).TotalSeconds, d1.SecondsDifference(d2));
        Assert.Equal((long)(d1 - d2).TotalMinutes, d1.MinutesDifference(d2));
        Assert.Equal((long)(d1 - d2).TotalHours, d1.HoursDifference(d2));
        Assert.Equal((long)(d1 - d2).TotalDays, d1.DayDifference(d2));
        Assert.Equal((long)((d1 - d2).TotalDays / 7), d1.WeeksDifference(d2));
        Assert.Equal(40, d1.MonthDifference(d2));
        Assert.Equal(13, d1.QuarterDifference(d2));
        Assert.Equal(3, d1.YearsDifference(d2));
    }

    [Fact]
    public void StartOfPeriod_WithAllArgs_ComputesCorrectly()
    {
        var forDate = new DateTime(2023, 5, 15, 0, 0, 0, DateTimeKind.Unspecified);
        var startAtDate = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
        var result = forDate.StartOfPeriod(startAtDate, EPeriodType.Year, EPeriodType.Month, 2);
        // Should be 2023-03-01 (start at 2020, add 3 years, then add 2 months)
        Assert.Equal(new DateTime(2023, 3, 1, 0, 0, 0, DateTimeKind.Unspecified), result);
    }
}
