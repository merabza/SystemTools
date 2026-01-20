using System;
using System.IO;
using SystemTools.SystemToolsShared.LinuxFileSecurity;
using Xunit;

namespace SystemTools.SystemToolsShared.Tests.LinuxFileSecurity;

public sealed class FilePermissionTests : IDisposable
{
    private readonly string _testFilePath;

    public FilePermissionTests()
    {
        // Create a temporary file for testing
        _testFilePath = Path.GetTempFileName();
    }

    public void Dispose()
    {
        if (File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath);
        }
    }

    [Fact]
    public void Constructor_SetsFilePath()
    {
        var perm = new FilePermission(_testFilePath);
        Assert.Equal(_testFilePath, perm.FilePath);
    }

    [Fact]
    public void ToString_ReturnsExpectedFormat()
    {
        var perm = new FilePermission(_testFilePath);
        perm.Apply(LinuxFileAccess.Read, LinuxFileAccess.Write, LinuxFileAccess.Execute);
        string str = perm.ToString();
        // Should be like "421 <filePath>"
        Assert.Matches(@"^\d{3} .+$", str);
        Assert.EndsWith(_testFilePath, str);
    }

    [Fact]
    public void Apply_UpdatesFlagsAndReturnsSelf()
    {
        var perm = new FilePermission(_testFilePath);
        perm.Apply(LinuxFileAccess.Read, LinuxFileAccess.Write, LinuxFileAccess.Execute);
        Assert.Equal(LinuxFileAccess.Read, perm.Flags.User);
        Assert.Equal(LinuxFileAccess.Write, perm.Flags.Group);
        Assert.Equal(LinuxFileAccess.Execute, perm.Flags.Others);
    }
}
