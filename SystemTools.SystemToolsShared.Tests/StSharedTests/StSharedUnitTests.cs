using System;
using System.IO;
using System.Runtime.InteropServices;
using LanguageExt;
using OneOf;
using SystemTools.SystemToolsShared.Errors;
using Xunit;

namespace SystemTools.SystemToolsShared.Tests.StSharedTests;

public sealed class StSharedUnitTests
{
    [Fact]
    public void TimeTakenMessage_ReturnsExpectedFormat()
    {
        DateTime start = DateTime.Now.AddMinutes(-1).AddSeconds(-10);
        string msg = StShared.TimeTakenMessage(start);
        Assert.Contains("minutes", msg);
        Assert.Contains("seconds", msg);
    }

    [Fact]
    public void RunProcessWithOutput_ValidProcess_ReturnsOutput()
    {
        OneOf<(string, int), Err[]> result = StShared.RunProcessWithOutput(false, null, "dotnet", "--version");
        Assert.True(result.IsT0);
        (string output, int exitCode) = result.AsT0;
        Assert.False(string.IsNullOrWhiteSpace(output));
        Assert.Equal(0, exitCode);
    }

    //[Fact]
    //public void RunProcessWithOutput_InvalidProcess_ReturnsError()
    //{
    //    var result = StShared.RunProcessWithOutput(false, null, "notarealcommand", "");
    //    Assert.True(result.IsT1);
    //    var errors = result.AsT1;
    //    Assert.NotEmpty(errors);
    //}

    [Fact]
    public void RunProcess_ValidProcess_ReturnsNull()
    {
        Option<Err[]> result = StShared.RunProcess(false, null, "dotnet", "--version");
        Assert.True(result.IsNone);
    }

    //[Fact]
    //public void RunProcess_InvalidProcess_ReturnsError()
    //{
    //    var result = StShared.RunProcess(false, null, "notarealcommand", "");
    //    Assert.True(result.IsSome);
    //    result.IfSome(Assert.NotEmpty);
    //}

    [Fact]
    public void RunCmdProcess_ValidCommand_ReturnsTrue()
    {
        // Only run on Windows
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return;
        }

        bool result = StShared.RunCmdProcess("echo Hello");
        Assert.True(result);
    }

    [Fact]
    public void CreateFolder_CreatesAndReturnsTrue()
    {
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
            bool result = StShared.CreateFolder(tempDir, false);
            Assert.True(result);
            Assert.True(Directory.Exists(tempDir));
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    [Fact]
    public void CreateFolder_Fails_ReturnsFalse()
    {
        // Try to create a folder in an invalid location
        string invalidPath = Path.Combine("?:", "invalid", Guid.NewGuid().ToString());
        bool result = StShared.CreateFolder(invalidPath, false);
        Assert.False(result);
    }

    //[Fact]
    //public void ConsoleWriteInformationLine_LogsAndWritesToConsole()
    //{
    //    var logger = new Mock<ILogger>();
    //    using var sw = new StringWriter();
    //    Console.SetOut(sw);
    //    StShared.ConsoleWriteInformationLine(logger.Object, true, "Test {0}", "info");
    //    var output = sw.ToString();
    //    Assert.Contains("Test", output);
    //    logger.Verify(l => l.LogInformation("Test {0}", "info"), Times.Once);
    //}

    //[Fact]
    //public void WriteWarningLine_LogsAndWritesToConsole()
    //{
    //    var logger = new Mock<ILogger>();
    //    using var sw = new StringWriter();
    //    Console.SetOut(sw);
    //    StShared.WriteWarningLine("Warn!", true, logger.Object);
    //    var output = sw.ToString();
    //    Assert.Contains("[warning]", output);
    //    Assert.Contains("Warn!", output);
    //    logger.Verify(l => l.LogWarning("Warn!"), Times.Once);
    //}

    //[Fact]
    //public void WriteErrorLine_LogsAndWritesToConsole()
    //{
    //    var logger = new Mock<ILogger>();
    //    using var sw = new StringWriter();
    //    Console.SetOut(sw);
    //    StShared.WriteErrorLine("Error!", true, logger.Object, false);
    //    var output = sw.ToString();
    //    Assert.Contains("[ERROR]", output);
    //    Assert.Contains("Error!", output);
    //    logger.Verify(l => l.LogError("Error!"), Times.Once);
    //}

    [Fact]
    public void WriteSuccessMessage_WritesInGreen()
    {
        // ReSharper disable once using
        // ReSharper disable once DisposableConstructor
        using var sw = new StringWriter();
        Console.SetOut(sw);
        StShared.WriteSuccessMessage("Success!");
        string output = sw.ToString();
        Assert.Contains("Success!", output);
    }

    //[Fact]
    //public void WriteException_LogsAndWritesToConsole()
    //{
    //    var logger = new Mock<ILogger>();
    //    var ex = new InvalidOperationException("fail");
    //    using var sw = new StringWriter();
    //    Console.SetOut(sw);
    //    StShared.WriteException(ex, "Extra", true, logger.Object, false);
    //    var output = sw.ToString();
    //    Assert.Contains("[ERROR]", output);
    //    Assert.Contains("fail", output);
    //    logger.Verify(l => l.LogError(ex, "Extra"), Times.Once);
    //}

    //[Fact]
    //public void WriteException_WithoutAdditionalMessage_LogsAndWritesToConsole()
    //{
    //    var logger = new Mock<ILogger>();
    //    var ex = new InvalidOperationException("fail");
    //    using var sw = new StringWriter();
    //    Console.SetOut(sw);
    //    StShared.WriteException(ex, true, logger.Object, false);
    //    var output = sw.ToString();
    //    Assert.Contains("[ERROR]", output);
    //    Assert.Contains("fail", output);
    //    logger.Verify(l => l.LogError(ex, ""), Times.Once);
    //}

    [Fact]
    public void GetMainModulePath_ReturnsPath()
    {
        string? path = StShared.GetMainModulePath();
        Assert.False(string.IsNullOrWhiteSpace(path));
        Assert.True(Directory.Exists(path));
    }

    [Fact]
    public void GetMainModuleFileName_ReturnsFileName()
    {
        string? fileName = StShared.GetMainModuleFileName();
        Assert.False(string.IsNullOrWhiteSpace(fileName));
        Assert.True(fileName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) ||
                    fileName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase));
    }
}
