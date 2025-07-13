using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Xunit;

namespace SystemToolsShared.Tests;

public sealed class FileStatTests : IDisposable
{
    private readonly string _tempDir;
    private readonly string _tempFile1;
    private readonly string _tempFile2;

    public FileStatTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDir);
        _tempFile1 = Path.Combine(_tempDir, "file1.txt");
        _tempFile2 = Path.Combine(_tempDir, "file2.txt");
        File.WriteAllText(_tempFile1, "abc");
        File.WriteAllText(_tempFile2, "abc");
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    [Theory]
    [InlineData("file:///C:/test.txt", true)]
    [InlineData("ftp://example.com/file.txt", false)]
    [InlineData(@"C:\test.txt", true)]
    [InlineData("/home/user/file.txt", true)]
    public void IsFileSchema_ReturnsExpected(string path, bool expected)
    {
        Assert.Equal(expected, FileStat.IsFileSchema(path));
    }

    [Theory]
    //[InlineData("/home/merab/ApAgentData/DatabaseFullBackups/", "/home/merab/ApAgentData/DatabaseFullBackups")]
    [InlineData("ftp://cyberia.ge:2150/MerinsonBU", "ftp://cyberia.ge:2150/MerinsonBU")]
    [InlineData("FTP://CYBERIA.ge:2150/MerinsonBU/", "ftp://cyberia.ge:2150/MerinsonBU")]
    [InlineData(@"D:\1WorkDotnetCore\ApAgent\SystemTools", @"D:\1WORKDOTNETCORE\APAGENT\SYSTEMTOOLS")]
    public void NormalizePathTest(string path, string result)
    {
        var normPath = FileStat.NormalizePath(path);
        Assert.Equal(result, normPath);
    }

    [Fact]
    public void CreatePrevFolderIfNotExists_CreatesDirectory()
    {
        var filePath = Path.Combine(_tempDir, "subdir", "file.txt");
        var logger = new TestLogger();
        var created = FileStat.CreatePrevFolderIfNotExists(filePath, false, logger);
        Assert.True(created);
        Assert.True(Directory.Exists(Path.GetDirectoryName(filePath)!));
    }

    [Fact]
    public void CreateFolderIfNotExists_CreatesAndReturnsPath()
    {
        var folder = Path.Combine(_tempDir, "newfolder");
        var logger = new TestLogger();
        var result = FileStat.CreateFolderIfNotExists(folder, false, logger);
        Assert.NotNull(result);
        Assert.True(Directory.Exists(folder));
    }

    [Fact]
    public void RemoveNotNeedLeadPart_RemovesLeadChar()
    {
        Assert.Equal("abc", "/abc".RemoveNotNeedLeadPart('/'));
        Assert.Equal("abc", "abc".RemoveNotNeedLeadPart('/'));
    }

    [Fact]
    public void RemoveNotNeedLastPart_RemovesLastChar()
    {
        Assert.Equal("abc", "abc/".RemoveNotNeedLastPart('/'));
        Assert.Equal("abc", "abc".RemoveNotNeedLastPart('/'));
    }

    [Fact]
    public void AddNeedLeadPart_AddsLeadString()
    {
        Assert.Equal("/abc", "abc".AddNeedLeadPart("/"));
        Assert.Equal("/abc", "/abc".AddNeedLeadPart("/"));
    }

    [Fact]
    public void AddNeedLastPart_AddsLastStringOrChar()
    {
        Assert.Equal("abc/", "abc".AddNeedLastPart("/"));
        Assert.Equal("abc/", "abc/".AddNeedLastPart("/"));
        Assert.Equal("abc/", "abc".AddNeedLastPart('/'));
        Assert.Equal("abc/", "abc/".AddNeedLastPart('/'));
    }

    [Theory]
    [InlineData("file.txt", "*.txt", true)]
    [InlineData("file.txt", "*.doc", false)]
    [InlineData("file.txt", "file.???", true)]
    [InlineData("file.txt", "", true)]
    public void FitsMask_WorksAsExpected(string fileName, string mask, bool expected)
    {
        Assert.Equal(expected, fileName.FitsMask(mask));
    }

    [Fact]
    public void GetDateTimeAndPatternByDigits_ValidPattern_ParsesDate()
    {
        var fileName = "backup_20230515123045.txt";
        var mask = "yyyyMMddHHmmss";
        var (dt, pattern) = fileName.GetDateTimeAndPatternByDigits(mask);
        Assert.NotEqual(DateTime.MinValue, dt);
        Assert.Contains(mask, pattern!);
    }

    [Fact]
    public void GetDateTimeAndPatternByDigits_InvalidPattern_ReturnsMinValue()
    {
        var fileName = "backup_file.txt";
        var mask = "yyyyMMddHHmmss";
        var (dt, pattern) = fileName.GetDateTimeAndPatternByDigits(mask);
        Assert.Equal(DateTime.MinValue, dt);
        Assert.Null(pattern);
    }

    [Fact]
    public void TryGetDate_ValidAndInvalid_ReturnsExpected()
    {
        var valid = FileStat.TryGetDate("20230515", "yyyyMMdd");
        Assert.Equal(new DateTime(2023, 5, 15), valid);

        var invalid = FileStat.TryGetDate("notadate", "yyyyMMdd");
        Assert.Equal(DateTime.MinValue, invalid);
    }

    [Fact]
    public void FileCompare_SameFiles_ReturnsTrue()
    {
        Assert.True(FileStat.FileCompare(_tempFile1, _tempFile2));
    }

    [Fact]
    public void FileCompare_DifferentFiles_ReturnsFalse()
    {
        File.WriteAllText(_tempFile2, "different");
        Assert.False(FileStat.FileCompare(_tempFile1, _tempFile2));
    }

    [Fact]
    public void DeleteDirectoryWithNormaliseAttributes_DeletesDirectory()
    {
        var dir = Path.Combine(_tempDir, "todelete");
        Directory.CreateDirectory(dir);
        var file = Path.Combine(dir, "file.txt");
        File.WriteAllText(file, "abc");
        File.SetAttributes(file, FileAttributes.ReadOnly);
        FileStat.DeleteDirectoryWithNormaliseAttributes(dir);
        Assert.False(Directory.Exists(dir));
    }

    [Fact]
    public void DeleteDirectoryIfExists_DeletesIfExists()
    {
        var dir = Path.Combine(_tempDir, "dir2");
        Directory.CreateDirectory(dir);
        FileStat.DeleteDirectoryIfExists(dir);
        Assert.False(Directory.Exists(dir));
    }

    [Fact]
    public void DeleteFileIfExists_DeletesIfExists()
    {
        var file = Path.Combine(_tempDir, "todelete.txt");
        File.WriteAllText(file, "abc");
        FileStat.DeleteFileIfExists(file);
        Assert.False(File.Exists(file));
    }

    [Fact]
    public void ClearFolder_DeletesFilesAndDirsExceptExcludes()
    {
        var folder = Path.Combine(_tempDir, "clearfolder");
        Directory.CreateDirectory(folder);
        var file1 = Path.Combine(folder, "keep.txt");
        var file2 = Path.Combine(folder, "remove.txt");
        File.WriteAllText(file1, "keep");
        File.WriteAllText(file2, "remove");
        var subdir = Path.Combine(folder, "subdir");
        Directory.CreateDirectory(subdir);
        File.WriteAllText(Path.Combine(subdir, "file.txt"), "subfile");

        FileStat.ClearFolder(folder, new[] { "keep" });

        Assert.True(File.Exists(file1));
        Assert.False(File.Exists(file2));
        Assert.False(Directory.Exists(subdir));
    }

    [Fact]
    public void CopyFilesAndFolders_CopiesAllExceptExcludes()
    {
        var src = Path.Combine(_tempDir, "src");
        var dst = Path.Combine(_tempDir, "dst");
        Directory.CreateDirectory(src);
        Directory.CreateDirectory(dst);
        var file1 = Path.Combine(src, "a.txt");
        var file2 = Path.Combine(src, "b.txt");
        File.WriteAllText(file1, "a");
        File.WriteAllText(file2, "b");
        var subdir = Path.Combine(src, "sub");
        Directory.CreateDirectory(subdir);
        var subfile = Path.Combine(subdir, "c.txt");
        File.WriteAllText(subfile, "c");

        var logger = new TestLogger();
        var result = FileStat.CopyFilesAndFolders(src, dst, new[] { "b.txt" }, false, logger);

        Assert.True(result);
        Assert.True(File.Exists(Path.Combine(dst, "a.txt")));
        Assert.False(File.Exists(Path.Combine(dst, "b.txt")));
        Assert.True(File.Exists(Path.Combine(dst, "sub", "c.txt")));
    }

    // Minimal ILogger implementation for testing
    private sealed class TestLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return NullScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
        }

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();

            public void Dispose()
            {
            }
        }
    }
}