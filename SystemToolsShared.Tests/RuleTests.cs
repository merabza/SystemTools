using System;
using System.Text.RegularExpressions;
using Xunit;

namespace SystemToolsShared.Tests;

public sealed class RuleTests
{
    [Fact]
    public void Constructor_WithValidParameters_CreatesRule()
    {
        // Arrange & Act
        var rule = new Rule("test", "replacement");

        // Assert
        Assert.NotNull(rule);
    }

    [Theory]
    [InlineData("test", "replacement", "test", "replacement")]
    [InlineData("t[e]st", "replacement", "test", "replacement")]
    [InlineData("TEST", "replacement", "test", "replacement")] // Testing case insensitivity
    [InlineData("t.st", "replacement", "test", "replacement")] // Testing regex pattern
    public void Apply_WithMatchingPattern_ReturnsReplacement(string pattern, string replacement, string input,
        string expected)
    {
        // Arrange
        var rule = new Rule(pattern, replacement);

        // Act
        var result = rule.Apply(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("test", "replacement", "nomatch")]
    [InlineData("t[e]st", "replacement", "tast")]
    [InlineData("^test$", "replacement", "testing")]
    public void Apply_WithNonMatchingPattern_ReturnsNull(string pattern, string replacement, string input)
    {
        // Arrange
        var rule = new Rule(pattern, replacement);

        // Act
        var result = rule.Apply(input);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData(@"(\w+)", "$1_suffix", "word", "word_suffix")] // Add suffix
    [InlineData(@"(\w+)", "prefix_$1", "word", "prefix_word")] // Add prefix
    [InlineData(@"(\w+)_(\w+)", "$2_$1", "first_second", "second_first")] // Swap parts
    public void Apply_WithRegexGroups_ReplacesCorrectly(string pattern, string replacement, string input,
        string expected)
    {
        // Arrange
        var rule = new Rule(pattern, replacement);

        // Act
        var result = rule.Apply(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("test", "replacement", "TEST")] // Upper case
    [InlineData("test", "replacement", "Test")] // Title case
    [InlineData("test", "replacement", "tEsT")] // Mixed case
    public void Apply_WithDifferentCasing_MatchesAndReplaces(string pattern, string replacement, string input)
    {
        // Arrange
        var rule = new Rule(pattern, replacement);

        // Act
        var result = rule.Apply(input);

        // Assert
        Assert.Equal(replacement, result);
    }

    [Fact]
    public void Apply_WithEmptyInput_ReturnsNull()
    {
        // Arrange
        var rule = new Rule("test", "replacement");

        // Act
        var result = rule.Apply(string.Empty);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Apply_WithNullInput_ThrowsArgumentNullException()
    {
        // Arrange
        var rule = new Rule("test", "replacement");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => rule.Apply(null!));
    }
}