using System;
using Xunit;

namespace SystemTools.SystemToolsShared.Tests;

public sealed class ProjectExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_SetsMessage()
    {
        // Arrange
        const string message = "Test error message";

        // Act
        var exception = new ProjectException(message);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Null(exception.InnerException);
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_SetsProperties()
    {
        // Arrange
        const string message = "Test error message";
        var innerException = new InvalidOperationException("Inner exception");

        // Act
        var exception = new ProjectException(message, innerException);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Same(innerException, exception.InnerException);
    }

    [Fact]
    public void Exception_InheritsFromException()
    {
        // Arrange
        var exception = new ProjectException();

        // Act & Assert
        Assert.IsAssignableFrom<Exception>(exception);
    }

    [Fact]
    public void Exception_IsSealed()
    {
        // Act & Assert
        Type type = typeof(ProjectException);
        Assert.True(type.IsSealed);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Error message")]
    [InlineData("Error\nwith\nmultiple\nlines")]
    public void Constructor_WithDifferentMessages_SetsMessageCorrectly(string message)
    {
        // Act
        var exception = new ProjectException(message);

        // Assert
        Assert.Equal(message, exception.Message);
    }
}
