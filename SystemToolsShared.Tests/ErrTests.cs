using System;
using System.IO;
using System.Linq;
using SystemToolsShared.Errors;
using Xunit;

namespace SystemToolsShared.Tests;

public sealed class ErrTests
{
    [Fact]
    public void Equals_ReturnsTrue_ForSameValues()
    {
        var err1 = new Err { ErrorCode = "A", ErrorMessage = "B" };
        var err2 = new Err { ErrorCode = "A", ErrorMessage = "B" };
        Assert.True(err1.Equals(err2));
        Assert.True(err1.Equals((object)err2));
        Assert.Equal(err1, err2);
    }

    [Fact]
    public void Equals_ReturnsFalse_ForDifferentValues()
    {
        var err1 = new Err { ErrorCode = "A", ErrorMessage = "B" };
        var err2 = new Err { ErrorCode = "A", ErrorMessage = "C" };
        var err3 = new Err { ErrorCode = "X", ErrorMessage = "B" };
        Assert.False(err1.Equals(err2));
        Assert.False(err1.Equals(err3));
        Assert.False(err1.Equals(null));
    }

    [Fact]
    public void GetHashCode_SameForEqualObjects()
    {
        var err1 = new Err { ErrorCode = "A", ErrorMessage = "B" };
        var err2 = new Err { ErrorCode = "A", ErrorMessage = "B" };
        Assert.Equal(err1.GetHashCode(), err2.GetHashCode());
    }

    [Fact]
    public void Create_ReturnsEnumerableWithErr()
    {
        var err = new Err { ErrorCode = "E", ErrorMessage = "M" };
        var result = Err.Create(err);
        var collection = result as Err[] ?? result.ToArray();
        Assert.Single(collection);
        Assert.Equal(err, collection.First());
    }

    [Fact]
    public void CreateArr_ReturnsArrayWithErr()
    {
        var err = new Err { ErrorCode = "E", ErrorMessage = "M" };
        var result = Err.CreateArr(err);
        Assert.Single(result);
        Assert.Equal(err, result[0]);
    }

    [Fact]
    public void RecreateErrors_AddsErrorToExisting()
    {
        var err1 = new Err { ErrorCode = "A", ErrorMessage = "B" };
        var err2 = new Err { ErrorCode = "C", ErrorMessage = "D" };
        var result = Err.RecreateErrors([err1], err2);
        Assert.Equal(2, result.Length);
        Assert.Contains(err1, result);
        Assert.Contains(err2, result);
    }

    [Fact]
    public void RecreateErrors_MergesTwoEnumerables()
    {
        var err1 = new Err { ErrorCode = "A", ErrorMessage = "B" };
        var err2 = new Err { ErrorCode = "C", ErrorMessage = "D" };
        var err3 = new Err { ErrorCode = "E", ErrorMessage = "F" };
        var result = Err.RecreateErrors([err1], [err2, err3]);
        Assert.Equal(3, result.Length);
        Assert.Contains(err1, result);
        Assert.Contains(err2, result);
        Assert.Contains(err3, result);
    }

    [Fact]
    public void PrintErrorsOnConsole_PrintsAllErrors()
    {
        var err1 = new Err { ErrorCode = "A", ErrorMessage = "B" };
        var err2 = new Err { ErrorCode = "C", ErrorMessage = "D" };
        // ReSharper disable once using
        using var sw = new StringWriter();
        using var originalOut = Console.Out;
        Console.SetOut(sw);
        try
        {
            Err.PrintErrorsOnConsole([err1, err2]);
            var output = sw.ToString();
            Assert.Contains("B", output);
            Assert.Contains("D", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}