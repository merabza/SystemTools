﻿//using System.Collections.Generic;
//using System.Diagnostics.CodeAnalysis;
//using System.Text;
//using System;
//using System.Collections;

//namespace SystemToolsShared.ConsoleColorFormat;

///// <summary>
///// Formatter to convert the named format items like {NamedformatItem} to <see cref="string.Format(IFormatProvider, string, object)"/> format.
///// </summary>
//internal sealed class LogValuesFormatter
//{
//    private const string NullValue = "(null)";
//    private static readonly char[] FormatDelimiters = [',', ':'];
//    private readonly CompositeFormat _format;

//    // NOTE: If this assembly ever builds for netcoreapp, the below code should change to:
//    // - Be annotated as [SkipLocalsInit] to avoid zero'ing the stackalloc'd char span
//    // - Format _valueNames.Count directly into a span

//    public LogValuesFormatter(string? format)
//    {
//        if (format is null)
//            return;

//        OriginalFormat = format;

//        var vsb = new StringBuilder(256);
//        var scanIndex = 0;
//        var endIndex = format.Length;

//        while (scanIndex < endIndex)
//        {
//            var openBraceIndex = FindBraceIndex(format, '{', scanIndex, endIndex);
//            if (scanIndex == 0 && openBraceIndex == endIndex)
//            {
//                // No holes found.
//                _format = CompositeFormat.Parse(format);
//                //_format = format;
//                return;
//            }

//            var closeBraceIndex = FindBraceIndex(format, '}', openBraceIndex, endIndex);

//            if (closeBraceIndex == endIndex)
//            {
//                vsb.Append(format.AsSpan(scanIndex, endIndex - scanIndex));
//                scanIndex = endIndex;
//            }
//            else
//            {
//                // Format item syntax : { index[,alignment][ :formatString] }.
//                var formatDelimiterIndex = FindIndexOfAny(format, FormatDelimiters, openBraceIndex, closeBraceIndex);

//                vsb.Append(format.AsSpan(scanIndex, openBraceIndex - scanIndex + 1));
//                vsb.Append(ValueNames.Count);
//                ValueNames.Add(format.Substring(openBraceIndex + 1, formatDelimiterIndex - openBraceIndex - 1));
//                vsb.Append(format.AsSpan(formatDelimiterIndex, closeBraceIndex - formatDelimiterIndex + 1));

//                scanIndex = closeBraceIndex + 1;
//            }
//        }

//        _format = CompositeFormat.Parse(vsb.ToString());
//        //_format = vsb.ToString();
//    }

//    private string OriginalFormat { get; set; }
//    public List<string> ValueNames { get; } = [];

//    private static int FindBraceIndex(string format, char brace, int startIndex, int endIndex)
//    {
//        // Example: {{prefix{{{Argument}}}suffix}}.
//        var braceIndex = endIndex;
//        var scanIndex = startIndex;
//        var braceOccurrenceCount = 0;

//        while (scanIndex < endIndex)
//        {
//            if (braceOccurrenceCount > 0 && format[scanIndex] != brace)
//            {
//                if (braceOccurrenceCount % 2 == 0)
//                {
//                    // Even number of '{' or '}' found. Proceed search with next occurrence of '{' or '}'.
//                    braceOccurrenceCount = 0;
//                    braceIndex = endIndex;
//                }
//                else
//                {
//                    // An unescaped '{' or '}' found.
//                    break;
//                }
//            }
//            else if (format[scanIndex] == brace)
//            {
//                if (brace == '}')
//                {
//                    if (braceOccurrenceCount == 0)
//                    {
//                        // For '}' pick the first occurrence.
//                        braceIndex = scanIndex;
//                    }
//                }
//                else
//                {
//                    // For '{' pick the last occurrence.
//                    braceIndex = scanIndex;
//                }

//                braceOccurrenceCount++;
//            }

//            scanIndex++;
//        }

//        return braceIndex;
//    }

//    private static int FindIndexOfAny(string format, char[] chars, int startIndex, int endIndex)
//    {
//        var findIndex = format.IndexOfAny(chars, startIndex, endIndex - startIndex);
//        return findIndex == -1 ? endIndex : findIndex;
//    }

//    //public string Format(object?[]? values)
//    //{
//    //    var formattedValues = values;

//    //    if (values == null)
//    //        return string.Format(CultureInfo.InvariantCulture, _format, formattedValues ?? []);

//    //    for (var i = 0; i < values.Length; i++)
//    //    {
//    //        var formattedValue = FormatArgument(values[i]);
//    //        // If the formatted value is changed, we allocate and copy items to a new array to avoid mutating the array passed in to this method
//    //        if (ReferenceEquals(formattedValue, values[i]))
//    //            continue;

//    //        formattedValues = new object[values.Length];
//    //        Array.Copy(values, formattedValues, i);
//    //        formattedValues[i++] = formattedValue;
//    //        for (; i < values.Length; i++)
//    //            formattedValues[i] = FormatArgument(values[i]);
//    //        break;
//    //    }

//    //    return string.Format(CultureInfo.InvariantCulture, _format, formattedValues ?? []);
//    //}

//    //// NOTE: This method mutates the items in the array if needed to avoid extra allocations, and should only be used when caller expects this to happen
//    //internal string FormatWithOverwrite(object?[]? values)
//    //{
//    //    if (values != null)
//    //    {
//    //        for (int i = 0; i < values.Length; i++)
//    //        {
//    //            values[i] = FormatArgument(values[i]);
//    //        }
//    //    }

//    //    return string.Format(CultureInfo.InvariantCulture, _format, values ?? Array.Empty<object>());
//    //}

////        internal string Format()
////        {
////#if NET8_0_OR_GREATER
////            return _format.Format;
////#else
////            return _format;
////#endif
////        }

////#if NET8_0_OR_GREATER
////        internal string Format<TArg0>(TArg0 arg0)
////        {
////            object? arg0String = null;
////            return
////                !TryFormatArgumentIfNullOrEnumerable(arg0, ref arg0String) ?
////                string.Format(CultureInfo.InvariantCulture, _format, arg0) :
////                string.Format(CultureInfo.InvariantCulture, _format, arg0String);
////        }

////        internal string Format<TArg0, TArg1>(TArg0 arg0, TArg1 arg1)
////        {
////            object? arg0String = null, arg1String = null;
////            return
////                !TryFormatArgumentIfNullOrEnumerable(arg0, ref arg0String) &&
////                !TryFormatArgumentIfNullOrEnumerable(arg1, ref arg1String) ?
////                string.Format(CultureInfo.InvariantCulture, _format, arg0, arg1) :
////                string.Format(CultureInfo.InvariantCulture, _format, arg0String ?? arg0, arg1String ?? arg1);
////        }

////        internal string Format<TArg0, TArg1, TArg2>(TArg0 arg0, TArg1 arg1, TArg2 arg2)
////        {
////            object? arg0String = null, arg1String = null, arg2String = null;
////            return
////                !TryFormatArgumentIfNullOrEnumerable(arg0, ref arg0String) &&
////                !TryFormatArgumentIfNullOrEnumerable(arg1, ref arg1String) &&
////                !TryFormatArgumentIfNullOrEnumerable(arg2, ref arg2String) ?
////                string.Format(CultureInfo.InvariantCulture, _format, arg0, arg1, arg2) :
////                string.Format(CultureInfo.InvariantCulture, _format, arg0String ?? arg0, arg1String ?? arg1, arg2String ?? arg2);
////        }
////#else
////        internal string Format(object? arg0) =>
////            string.Format(CultureInfo.InvariantCulture, _format, FormatArgument(arg0));

////        internal string Format(object? arg0, object? arg1) =>
////            string.Format(CultureInfo.InvariantCulture, _format, FormatArgument(arg0), FormatArgument(arg1));

////        internal string Format(object? arg0, object? arg1, object? arg2) =>
////            string.Format(CultureInfo.InvariantCulture, _format, FormatArgument(arg0), FormatArgument(arg1), FormatArgument(arg2));
////#endif

//    public KeyValuePair<string, object?> GetValue(object?[] values, int index)
//    {
//        if (index < 0 || index > ValueNames.Count)
//        {
//            throw new IndexOutOfRangeException(nameof(index));
//        }

//        return ValueNames.Count > index
//            ? new KeyValuePair<string, object?>(ValueNames[index], values[index])
//            : new KeyValuePair<string, object?>("{OriginalFormat}", OriginalFormat);
//    }

//    //public IEnumerable<KeyValuePair<string, object?>> GetValues(object[] values)
//    //{
//    //    var valueArray = new KeyValuePair<string, object?>[values.Length + 1];
//    //    for (int index = 0; index != ValueNames.Count; ++index)
//    //    {
//    //        valueArray[index] = new KeyValuePair<string, object?>(ValueNames[index], values[index]);
//    //    }

//    //    valueArray[valueArray.Length - 1] = new KeyValuePair<string, object?>("{OriginalFormat}", OriginalFormat);
//    //    return valueArray;
//    //}

//    private static object FormatArgument(object? value)
//    {
//        object? stringValue = null;
//        return TryFormatArgumentIfNullOrEnumerable(value, ref stringValue) ? stringValue : value!;
//    }

//    private static bool TryFormatArgumentIfNullOrEnumerable<T>(T? value, [NotNullWhen(true)] ref object? stringValue)
//    {
//        if (value == null)
//        {
//            stringValue = NullValue;
//            return true;
//        }

//        // if the value implements IEnumerable but isn't itself a string, build a comma separated string.
//        if (value is string || value is not IEnumerable enumerable)
//            return false;
//        var vsb = new StringBuilder(256);
//        var first = true;
//        foreach (var e in enumerable)
//        {
//            if (!first)
//                vsb.Append(", ");

//            vsb.Append(e != null ? e.ToString() : NullValue);
//            first = false;
//        }

//        stringValue = vsb.ToString();
//        return true;

//    }
//}

