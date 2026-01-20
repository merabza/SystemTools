using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace SystemTools.SystemToolsShared;

public static class StringExtensions
{
    public static DateTime TryGetDate(string strDate, string mask)
    {
        try
        {
            return DateTime.ParseExact(strDate, mask, CultureInfo.InvariantCulture);
        }
        catch (Exception)
        {
            //Console.WriteLine(e);
        }

        return DateTime.MinValue;
    }

    extension(string dest)
    {
        //public string CountDtKey()
        //{
        //    return dest.ToLower().RemoveNotNeedLastPart("id");
        //}

        public string RemoveNotNeedLeadPart(char removeLead)
        {
            return dest.StartsWith(removeLead) ? dest[1..] : dest;
        }

        public string RemoveNotNeedLastPart(char removeLast)
        {
            return dest.EndsWith(removeLast) ? dest[..^1] : dest;
        }

        public string RemoveNotNeedLastPart(string removeLast)
        {
            return dest.EndsWith(removeLast, StringComparison.Ordinal) ? dest[..^removeLast.Length] : dest;
        }

        public string AddNeedLeadPart(string mustLead)
        {
            if (dest.StartsWith(mustLead, StringComparison.Ordinal))
            {
                return dest;
            }

            return mustLead + dest;
        }

        public string AddNeedLastPart(string mustLast)
        {
            if (dest.EndsWith(mustLast, StringComparison.Ordinal))
            {
                return dest;
            }

            return dest + mustLast;
        }

        public string AddNeedLastPart(char mustLast)
        {
            if (dest.EndsWith(mustLast))
            {
                return dest;
            }

            return dest + mustLast;
        }

        public bool FitsMask(string sFileMask)
        {
            if (string.IsNullOrWhiteSpace(sFileMask))
            {
                return true;
            }

            var regexFileMask = sFileMask.Replace(".", "[.]").Replace("*", ".*").Replace("?", ".").Replace("\\", @"\\");
            if (!sFileMask.EndsWith('*'))
            {
                regexFileMask += '$';
            }

            if (!sFileMask.StartsWith('*'))
            {
                regexFileMask = '^' + regexFileMask;
            }

            var mask = new Regex(regexFileMask);
            var toRet = mask.IsMatch(dest);
            return toRet;
        }

        public (DateTime, string?) GetDateTimeAndPatternByDigits(string maskFirstVersion)
        {
            var sbMask = new StringBuilder();
            var position = 0;
            var maskPosition = 0;
            var maskPositionInName = 0;
            foreach (var c in dest)
            {
                if (char.IsDigit(c))
                {
                    if (maskPosition == 0)
                    {
                        maskPositionInName = position;
                    }

                    sbMask.Append(maskFirstVersion[maskPosition]);
                    maskPosition++;
                    if (maskPosition == maskFirstVersion.Length)
                    {
                        break;
                    }
                }
                else if (c is '-' or '_' && maskPosition > 0 &&
                         maskFirstVersion[maskPosition] != maskFirstVersion[maskPosition - 1])
                {
                    sbMask.Append(c);
                }
                else if (maskPosition > 7)
                {
                    break;
                }
                else if (maskPosition > 0)
                {
                    maskPosition = 0;
                    sbMask.Clear();
                }

                position++;
            }

            //მინიმუმ 8 პოზიცია არის წელიწადი, თვე და დღე
            if (maskPosition < 8)
            {
                return (DateTime.MinValue, null);
            }

            var mask = sbMask.ToString();
            var strDate = dest.Substring(maskPositionInName, mask.Length);
            var pattern = dest[..maskPositionInName] + mask + dest[(maskPositionInName + mask.Length)..];

            var dt = TryGetDate(strDate, mask);
            return dt == DateTime.MinValue ? (DateTime.MinValue, null) : (dt, pattern);
        }
    }
}
