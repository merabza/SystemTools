using Xunit;

namespace SystemToolsShared.Tests;

public sealed class FileStatTests
{
    [Fact]
    public void PassingTest()
    {
        Assert.Equal(4, Add(2, 2));
    }

    private static int Add(int x, int y)
    {
        return x + y;
    }

    [Theory]
    [InlineData("/home/merab/ApAgentData/DatabaseFullBackups/", @"D:\HOME\MERAB\APAGENTDATA\DATABASEFULLBACKUPS")]
    [InlineData("ftp://cyberia.ge:2150/MerinsonBU", "ftp://cyberia.ge:2150/MerinsonBU")]
    [InlineData("FTP://CYBERIA.ge:2150/MerinsonBU/", "ftp://cyberia.ge:2150/MerinsonBU")]
    [InlineData(@"D:\1WorkDotnetCore\ApAgent\SystemTools", @"D:\1WORKDOTNETCORE\APAGENT\SYSTEMTOOLS")]
    public void NormalizePathTest(string path, string result)
    {
        //if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        var normPath = FileStat.NormalizePath(path);
        Assert.Equal(normPath, result);
    }
}