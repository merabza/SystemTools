//using System;

//namespace SystemToolsShared;

//public sealed class ConsoleFormatter
//{
//    private static int _lastPrefixMaxLength;
//    private int _lastClearLength;
//    private int _lastLineLength;
//    private bool _useCurrentLine;

//    public void WriteFirstLine(string text)
//    {
//        _lastLineLength = text.Length;
//        Console.WriteLine(text);
//    }

//    private void Clear()
//    {
//        int linesUp = (_lastLineLength + _lastClearLength - 1) / Console.WindowWidth + 1;
//        int currentLine = Console.CursorTop;
//        Console.SetCursorPosition(0, currentLine - linesUp);
//    }

//    public void UseCurrentLine()
//    {
//        _useCurrentLine = true;
//    }

//    public void WriteInSameLine(string prefix, string text)
//    {
//        int prefixLength = prefix.Length + 1;
//        if (prefixLength > _lastPrefixMaxLength)
//        {
//            _lastPrefixMaxLength = prefixLength;
//        }

//        string allText = $"{prefix}{new string(' ', _lastPrefixMaxLength - prefix.Length)}{text}";
//        if (_useCurrentLine)
//        {
//            _useCurrentLine = false;
//        }
//        else
//        {
//            Clear();
//        }

//        string forClear = string.Empty;
//        _lastClearLength = 0;
//        if (_lastLineLength > allText.Length)
//        {
//            _lastClearLength = _lastLineLength - allText.Length;
//            forClear = new string(' ', _lastClearLength);
//        }

//        _lastLineLength = allText.Length;
//        Console.WriteLine(allText + forClear);
//    }

//    public void WriteInSameLine(string text)
//    {
//        Clear();
//        string forClear = string.Empty;
//        _lastClearLength = 0;
//        if (_lastLineLength > text.Length)
//        {
//            _lastClearLength = _lastLineLength - text.Length;
//            forClear = new string(' ', _lastClearLength);
//        }

//        _lastLineLength = text.Length;
//        Console.WriteLine(text + forClear);
//    }
//}


