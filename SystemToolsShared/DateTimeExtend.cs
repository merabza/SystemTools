using System;
using System.Threading;

namespace SystemToolsShared;

public static class DateTimeExtend
{
    public static DateTime StartOfPeriod(this DateTime forDate, DateTime startAtDate, EPeriodType periodType,
        EPeriodType lessPeriodType, int skipCount)
    {
        return startAtDate.DateAdd(periodType, forDate.DateDiff(periodType, startAtDate))
            .DateAdd(lessPeriodType, skipCount);
    }

    public static DateTime StartOfPeriod(this DateTime forDate, EPeriodType periodType)
    {
        switch (periodType)
        {
            case EPeriodType.Year:
                return new DateTime(forDate.Year, 1, 1);
            case EPeriodType.Quarter:
                var quarterNumber = (forDate.Month - 1) / 3 + 1;
                return new DateTime(forDate.Year, (quarterNumber - 1) * 3 + 1, 1);
            case EPeriodType.Month:
                return new DateTime(forDate.Year, forDate.Month, 1);
            case EPeriodType.Week:
                var ci =
                    Thread.CurrentThread.CurrentCulture;
                var firstDayOfWeek = ci.DateTimeFormat.FirstDayOfWeek;
                var dayOfWeek = forDate.DayOfWeek;
                return forDate.AddDays(firstDayOfWeek - dayOfWeek).Date;
            case EPeriodType.Day:
                return forDate.Date;
            case EPeriodType.Hour:
                return forDate.Date.AddHours(forDate.Hour);
            case EPeriodType.Minute:
                return forDate.Date.AddHours(forDate.Hour).AddMinutes(forDate.Minute);
            case EPeriodType.Second:
                return forDate.Date.AddHours(forDate.Hour).AddMinutes(forDate.Minute).AddSeconds(forDate.Second);
            default:
                return forDate;
        }
    }

    public static DateTime DateAdd(this DateTime lValue, EPeriodType periodType, long count)
    {
        return periodType switch
        {
            EPeriodType.Year => lValue.AddYears((int)count),
            EPeriodType.Quarter => lValue.AddMonths((int)count * 3),
            EPeriodType.Month => lValue.AddMonths((int)count),
            EPeriodType.Week => lValue.AddDays((int)count * 7),
            EPeriodType.Day => lValue.AddDays(count),
            EPeriodType.Hour => lValue.AddHours(count),
            EPeriodType.Minute => lValue.AddMinutes(count),
            EPeriodType.Second => lValue.AddSeconds(count),
            _ => default
        };
    }

    public static long DateDiff(this DateTime lValue, EPeriodType periodType, DateTime rValue)
    {
        return periodType switch
        {
            EPeriodType.Year => lValue.YearsDifference(rValue),
            EPeriodType.Quarter => lValue.QuarterDifference(rValue),
            EPeriodType.Month => lValue.MonthDifference(rValue),
            EPeriodType.Week => lValue.WeeksDifference(rValue),
            EPeriodType.Day => lValue.DayDifference(rValue),
            EPeriodType.Hour => lValue.HoursDifference(rValue),
            EPeriodType.Minute => lValue.MinutesDifference(rValue),
            EPeriodType.Second => lValue.SecondsDifference(rValue),
            _ => 0
        };
    }

    public static long MillisecondsDifference(this DateTime lValue, DateTime rValue)
    {
        return (long)(lValue - rValue).TotalMilliseconds;
    }

    public static long SecondsDifference(this DateTime lValue, DateTime rValue)
    {
        return (long)(lValue - rValue).TotalSeconds;
    }

    public static long MinutesDifference(this DateTime lValue, DateTime rValue)
    {
        return (long)(lValue - rValue).TotalMinutes;
    }

    public static long HoursDifference(this DateTime lValue, DateTime rValue)
    {
        return (long)(lValue - rValue).TotalHours;
    }

    public static long DayDifference(this DateTime lValue, DateTime rValue)
    {
        return (long)(lValue - rValue).TotalDays;
    }

    public static long WeeksDifference(this DateTime lValue, DateTime rValue)
    {
        return (long)((lValue - rValue).TotalDays / 7);
    }

    public static long MonthDifference(this DateTime lValue, DateTime rValue)
    {
        return lValue.Month - rValue.Month + 12 * (lValue.Year - rValue.Year);
    }

    public static long QuarterDifference(this DateTime lValue, DateTime rValue)
    {
        return lValue.MonthDifference(rValue) / 3;
    }

    public static long YearsDifference(this DateTime lValue, DateTime rValue)
    {
        var zeroTime = new DateTime(1, 1, 1);

        var span = lValue - rValue;
        return (zeroTime + span).Year - 1;
    }
}