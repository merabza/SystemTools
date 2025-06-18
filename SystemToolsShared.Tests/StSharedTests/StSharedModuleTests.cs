using System;
using System.IO;
using Xunit;

namespace SystemToolsShared.Tests.StSharedTests;

public sealed class StSharedModuleTests
{
    [Fact]
    public void GetMainModulePath_ReturnsValidPath()
    {
        // Act
        var path = StShared.GetMainModulePath();

        // Assert
        Assert.NotNull(path);
        Assert.True(Directory.Exists(path));
    }
}