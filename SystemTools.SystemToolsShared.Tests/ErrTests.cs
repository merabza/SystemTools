using System;
using System.IO;
using SystemTools.SystemToolsShared.Errors;
using Xunit;

namespace SystemTools.SystemToolsShared.Tests;

public sealed class ErrTests
{
    [Fact]
    public void Equals_ReturnsTrue_ForSameValues()
    {
        var err1 = new ErrorOmd { Code = "A", Name = "B" };
        var err2 = new ErrorOmd { Code = "A", Name = "B" };
        Assert.True(err1.Equals(err2));
        Assert.True(err1.Equals((object)err2));
        Assert.Equal(err1, err2);
    }

    [Fact]
    public void Equals_ReturnsFalse_ForDifferentValues()
    {
        var err1 = new ErrorOmd { Code = "A", Name = "B" };
        var err2 = new ErrorOmd { Code = "A", Name = "C" };
        var err3 = new ErrorOmd { Code = "X", Name = "B" };
        Assert.False(err1.Equals(err2));
        Assert.False(err1.Equals(err3));
    }

    [Fact]
    public void GetHashCode_SameForEqualObjects()
    {
        var err1 = new ErrorOmd { Code = "A", Name = "B" };
        var err2 = new ErrorOmd { Code = "A", Name = "B" };
        Assert.Equal(err1.GetHashCode(), err2.GetHashCode());
    }

    [Fact]
    public void Create_ReturnsEnumerableWithErr()
    {
        var err = new ErrorOmd { Code = "E", Name = "M" };
        ErrorOmd[] result = ErrorOmd.Create(err);
        Assert.Single(result);
        Assert.Equal(err, result[0]);
    }

    [Fact]
    public void CreateArr_ReturnsArrayWithErr()
    {
        var err = new ErrorOmd { Code = "E", Name = "M" };
        ErrorOmd[] result = ErrorOmd.CreateArr(err);
        Assert.Single(result);
        Assert.Equal(err, result[0]);
    }

    [Fact]
    public void RecreateErrors_AddsErrorToExisting()
    {
        var err1 = new ErrorOmd { Code = "A", Name = "B" };
        var err2 = new ErrorOmd { Code = "C", Name = "D" };
        ErrorOmd[] result = ErrorOmd.RecreateErrors([err1], err2);
        Assert.Equal(2, result.Length);
        Assert.Contains(err1, result);
        Assert.Contains(err2, result);
    }

    [Fact]
    public void RecreateErrors_MergesTwoEnumerables()
    {
        var err1 = new ErrorOmd { Code = "A", Name = "B" };
        var err2 = new ErrorOmd { Code = "C", Name = "D" };
        var err3 = new ErrorOmd { Code = "E", Name = "F" };
        ErrorOmd[] result = ErrorOmd.RecreateErrors([err1], [err2, err3]);
        Assert.Equal(3, result.Length);
        Assert.Contains(err1, result);
        Assert.Contains(err2, result);
        Assert.Contains(err3, result);
    }

    [Fact]
    public void PrintErrorsOnConsole_PrintsAllErrors()
    {
        var err1 = new ErrorOmd { Code = "A", Name = "B" };
        var err2 = new ErrorOmd { Code = "C", Name = "D" };
        // ReSharper disable once using
        // ReSharper disable once DisposableConstructor
        using var sw = new StringWriter();
        // ReSharper disable once using
        using TextWriter originalOut = Console.Out;
        Console.SetOut(sw);
        try
        {
            ErrorOmd.PrintErrorsOnConsole([err1, err2]);
            string output = sw.ToString();
            Assert.Contains("B", output);
            Assert.Contains("D", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}
