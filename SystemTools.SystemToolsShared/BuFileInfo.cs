using System;
using System.IO;

namespace SystemTools.SystemToolsShared;

public sealed class BuFileInfo
{
    public BuFileInfo(string fileName, bool isOriginalFileName, DateTime fileDateTime, int fileManagerId)
    {
        FileName = fileName;
        OriginalFileName = isOriginalFileName ? fileName : Path.GetFileNameWithoutExtension(fileName);
        FileDateTime = fileDateTime;
        FileManagerId = fileManagerId;
    }

    public BuFileInfo(string fileName, DateTime fileDateTime)
    {
        FileName = fileName;
        FileDateTime = fileDateTime;
    }

    public BuFileInfo(string fileName, long fileLength, DateTime fileDateTime)
    {
        FileName = fileName;
        FileLength = fileLength;
        FileDateTime = fileDateTime;
    }

    public string? OriginalFileName { get; }
    public string FileName { get; }
    public long FileLength { get; }

    public DateTime FileDateTime { get; }

    public int FileManagerId { get; }
}