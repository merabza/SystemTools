using System;
using System.IO;

namespace SystemTools.SystemToolsShared;

public static class FileNameValidator
{
    public static bool IsValidFileName(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return false;
        }

        // Check for invalid characters
        if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            return false;
        }

        // Check for reserved Windows names (like CON, PRN, AUX, etc.)
        string[] reservedNames =
        [
            "CON", "PRN", "AUX", "NUL",
            "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
            "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
        ];

        string upperName = Path.GetFileNameWithoutExtension(fileName).ToUpperInvariant();
        if (Array.Exists(reservedNames, rn => rn == upperName))
        {
            return false;
        }

        // Optional: check length
        return fileName.Length <= 255;
    }
}
