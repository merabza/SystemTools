using System.IO;
using Xunit;

namespace SystemTools.SystemToolsShared.Tests.StSharedTests;

public sealed class StSharedModuleTests
{
    [Fact]
    public void GetMainModulePath_ReturnsValidPath()
    {
        // Act
        string? path = StShared.GetMainModulePath();

        // Assert
        Assert.NotNull(path);
        Assert.True(Directory.Exists(path));
    }
}
