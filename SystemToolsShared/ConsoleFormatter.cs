using System;

namespace SystemToolsShared;

public sealed class ConsoleFormatter
{
    private int _lastClearLength;
    private int _lastLineLength;

    public void WriteFirstLine(string text)
    {
        _lastLineLength = text.Length;
        Console.WriteLine(text);
    }

    public void Clear()
    {
        var linesUp = (_lastLineLength + _lastClearLength - 1) / Console.WindowWidth + 1;
        var currentLine = Console.CursorTop;
        Console.SetCursorPosition(0, currentLine - linesUp);
    }

    public void WriteInSameLine(string text)
    {
        Clear();
        var forClear = "";
        _lastClearLength = 0;
        if (_lastLineLength > text.Length)
        {
            _lastClearLength = _lastLineLength - text.Length;
            forClear = new string(' ', _lastClearLength);
        }

        _lastLineLength = text.Length;
        Console.WriteLine(text + forClear);
    }
}