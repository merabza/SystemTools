//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System;
//using System.Collections;
//using System.Threading;

//namespace SystemToolsShared.ConsoleColorFormat;

//    /// <summary>
//    /// LogValues to enable formatting options supported by <see cref="string.Format(IFormatProvider, string, object?)"/>.
//    /// This also enables using {NamedformatItem} in the format string.
//    /// </summary>
//    internal readonly struct FormattedLogValues : IReadOnlyList<KeyValuePair<string, object?>>
//    {
//        private const int MaxCachedFormatters = 1024;
//        private const string NullFormat = "[null]";

//        private static int _sCount;
//        private static readonly ConcurrentDictionary<string, LogValuesFormatter> SFormatters = new();

//        private readonly LogValuesFormatter? _formatter;
//        private readonly object?[]? _values;
//        private readonly string _originalMessage;

//        //// for testing purposes
//        //internal LogValuesFormatter? Formatter => _formatter;

//        public FormattedLogValues(string? format, params object?[]? values)
//        {
//            if (values != null && values.Length != 0 && format != null)
//            {
//                if (_sCount >= MaxCachedFormatters)
//                {
//                    if (!SFormatters.TryGetValue(format, out _formatter)) 
//                        _formatter = new LogValuesFormatter(format);
//                }
//                else
//                {
//                    _formatter = SFormatters.GetOrAdd(format, f =>
//                    {
//                        Interlocked.Increment(ref _sCount);
//                        return new LogValuesFormatter(f);
//                    });
//                }
//            }
//            else
//                _formatter = null;

//            _originalMessage = format ?? NullFormat;
//            _values = values;
//        }

//        public KeyValuePair<string, object?> this[int index]
//        {
//            get
//            {
//                if (index < 0 || index >= Count)
//                    throw new IndexOutOfRangeException(nameof(index));

//                return index == Count - 1
//                    ? new KeyValuePair<string, object?>("{OriginalFormat}", _originalMessage)
//                    : _formatter!.GetValue(_values!, index);
//            }
//        }

//        public int Count
//        {
//            get
//            {
//                if (_formatter == null)
//                    return 1;

//                return _formatter.ValueNames.Count + 1;
//            }
//        }

//        public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
//        {
//            for (var i = 0; i < Count; ++i)
//                yield return this[i];
//        }

//        public override string ToString()
//        {
//            return _formatter == null ? _originalMessage : _formatter.Format(_values);
//        }

//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            return GetEnumerator();
//        }
//    }
