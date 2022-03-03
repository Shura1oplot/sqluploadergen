using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace bcpstream
{
    public class Helpers
    {
        public static string ConvertStringToString(string value,
                                                   bool trim = true,
                                                   bool trimQuotes = true,
                                                   bool replaceSpecialChars = true,
                                                   bool replaceDoubleSpaces = true,
                                                   bool upperCase = false,
                                                   bool defaultNull = true)
        {
            string defaultString = string.Empty;

            if (defaultNull)
                defaultString = null;

            if (value == null)
                return defaultString;

            if (trim)
                value = value.Trim();

            if (value == string.Empty)
                return defaultString;
           
            if (trimQuotes && value.StartsWith("\"") && value.EndsWith("\""))
            {
                value = value[1..^1];

                if (trim)
                    value = value.Trim();

                if (value == string.Empty)
                    return defaultString;
            }

            if (replaceSpecialChars)
            {
                StringBuilder sb = new(value.Length);

                for (int i = 0; i < value.Length; i++)
                {
                    char c = value[i];

                    if (Char.IsWhiteSpace(c))
                        c = ' ';

                    sb.Append(c);
                }
                
                value = sb.ToString();
            }

            if (replaceDoubleSpaces)
            {
                StringBuilder sb = new(value.Length);

                for (int i = 0; i < value.Length; i++)
                {
                    char c = value[i];

                    if (i == 0 || c != ' ' || value[i-1] != ' ')
                        sb.Append(c);
                }

                value = sb.ToString();
            }

            if (value == string.Empty)
                return defaultString;

            if (upperCase)
                value = value.ToUpper();

            return value;
        }

        private static readonly NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
        private static readonly NumberStyles intStyle = NumberStyles.Integer
            | NumberStyles.AllowLeadingSign
            | NumberStyles.AllowTrailingSign
            | NumberStyles.AllowParentheses;
        private static readonly NumberStyles floatStyle = NumberStyles.Float
            | NumberStyles.AllowLeadingSign
            | NumberStyles.AllowTrailingSign
            | NumberStyles.AllowParentheses;

        private static string ParseNumber(string value,
                                          bool replaceNullWithZero = false,
                                          bool dropDot = true,
                                          bool dropComma = true)
        {
            if (value == null || value == "нет данных")
            {
                if (replaceNullWithZero)
                    return "0";

                return null;
            }
            
            value = value.Trim();

            if (value == String.Empty)
            {
                if (replaceNullWithZero)
                    return "0";

                return null;
            }

            if (value == "-" || value == "()" || value == ".")
                return "0";

            value = value.Replace(" ", "");

            if (dropDot)
                value = value.Replace(".", "");

            if (dropComma)
                value = value.Replace(",", "");

            return value;
        }

        public static Int64? ConvertStringToLong(string value,
                                                 bool replaceNullWithZero = false)
        {
            value = ParseNumber(value, replaceNullWithZero);

            if (value == null)
                return null;

            return Int64.Parse(value, intStyle, nfi);
        }

        public static Int32? ConvertStringToInt(string value,
                                                bool replaceNullWithZero = false)
        {
            value = ParseNumber(value, replaceNullWithZero);

            if (value == null)
                return null;

            return Int32.Parse(value, intStyle, nfi);
        }

        public static Int16? ConvertStringToShort(string value,
                                                  bool replaceNullWithZero = false)
        {
            value = ParseNumber(value, replaceNullWithZero);

            if (value == null)
                return null;

            return Int16.Parse(value, intStyle, nfi);
        }

        public static byte? ConvertStringToByte(string value,
                                                bool replaceNullWithZero = false)
        {
            value = ParseNumber(value, replaceNullWithZero);

            if (value == null)
                return null;

            return byte.Parse(value, intStyle, nfi);
        }

        private static string ParseDecimal(string value,
                                           bool replaceNullWithZero = false)
        {
            value = ParseNumber(value, replaceNullWithZero, false, false);

            if (value == null || value == "нет данных")
            {
                if (replaceNullWithZero)
                    return "0";

                return null;
            }

            if (value == ",")
                return "0";

            if (value.Contains('e'))  // 6.626e-34
                return value;

            value = value.Replace("..", ".");

            if (value.Contains(','))
            {
                if (value.Contains('.'))
                {
                    if (value.IndexOf('.') > value.IndexOf(','))
                    {
                        // 1,000.00
                        value = value.Replace(",", "");
                    }
                    else
                    {
                        // 1.000,00
                        value = value.Replace(".", "").Replace(',', '.');
                    }
                }
                else
                {
                    // 1000,00
                    value = value.Replace(',', '.');
                }
            }

            if (value.StartsWith('.'))
                value = "0" + value;

            return value;
        }

        public static float? ConvertStringToFloat(string value,
                                                  bool replaceNullWithZero = false)
        {
            value = ParseDecimal(value, replaceNullWithZero);

            if (value == null)
                return null;

            return float.Parse(value, floatStyle, nfi);
        }

        public static double? ConvertStringToDouble(string value,
                                                    bool replaceNullWithZero = false)
        {
            value = ParseDecimal(value, replaceNullWithZero);

            if (value == null)
                return null;

            return double.Parse(value, floatStyle, nfi);
        }

        public static decimal? ConvertStringToDecimal(string value,
                                                      bool replaceNullWithZero = false)
        {
            value = ParseDecimal(value, replaceNullWithZero);

            if (value == null)
                return null;

            return decimal.Parse(value, floatStyle, nfi);
        }

        public static bool? ConvertStringToBool(string value)
        {
            value = value.Trim();

            if (value == string.Empty)
                return null;

            value = value.ToLower();

            if (value == "1" || value == "-1" || value == "t" || value == "true" || value == "yes")
                return true;

            if (value == "0" || value == "f" || value == "false" || value == "no")
                return false;

            throw (new FormatException("incorrect bool value"));
        }

        public static DateTime? ConvertStringToDateTime(string value,
                                                        string format = "yyyy-MM-dd")
        {
            if (value == null)
                return null;

            if (value == "\\N")
                return null;

            value = value.Trim();

            if (value == String.Empty)
                return null;

            return DateTime.ParseExact(value, format, CultureInfo.InvariantCulture);
        }

        public static TimeSpan? ConvertStringToTimeSpan(string value,
                                                        string format = "g")  // h\\:mm
        {
            if (value == null)
                return null;

            value = value.Trim();

            if (value == String.Empty)
                return null;

            return TimeSpan.ParseExact(value, format, CultureInfo.InvariantCulture);
        }

        public static void SelfTest()
        {
            Console.Out.WriteLine("ConvertStringToString:");

            Console.Out.Write("ConvertStringToString(\" foo bar \") = \"");
            Console.Out.Write(ConvertStringToString(" foo bar "));
            Console.Out.WriteLine("\"");

            Console.Out.Write("ConvertStringToString(\" foo bar \", trim: false) = \"");
            Console.Out.Write(ConvertStringToString(" foo bar ", trim: false));
            Console.Out.WriteLine("\"");

            Console.Out.Write("ConvertStringToString(\"\\\" foo bar \\\"\", trimQuotes: true) = \"");
            Console.Out.Write(ConvertStringToString("\" foo bar \"", trimQuotes: true));
            Console.Out.WriteLine("\"");

            Console.Out.Write("ConvertStringToString(\" foo\\nbar \", replaceSpecialChars: true) = \"");
            Console.Out.Write(ConvertStringToString(" foo\nbar ", replaceSpecialChars: true));
            Console.Out.WriteLine("\"");

            Console.Out.Write("ConvertStringToString(\" foo  bar \", replaceDoubleSpaces: true) = \"");
            Console.Out.Write(ConvertStringToString(" foo  bar ", replaceDoubleSpaces: true));
            Console.Out.WriteLine("\"");

            Console.Out.Write("ConvertStringToString(\" foo bar \", upperCase: true) = \"");
            Console.Out.Write(ConvertStringToString(" foo  bar ", upperCase: true));
            Console.Out.WriteLine("\"");

            Console.Out.WriteLine();
            Console.Out.WriteLine("ConvertStringToLong:");

            Console.Out.Write("ConvertStringToLong(\"1234567\") = ");
            Console.Out.WriteLine(ConvertStringToLong("1234567").ToString());

            Console.Out.Write("ConvertStringToLong(\"-1234567\") = ");
            Console.Out.WriteLine(ConvertStringToLong("-1234567").ToString());

            Console.Out.Write("ConvertStringToLong(\"1234567-\") = ");
            Console.Out.WriteLine(ConvertStringToLong("1234567-").ToString());

            Console.Out.Write("ConvertStringToLong(\"(1234567)\") = ");
            Console.Out.WriteLine(ConvertStringToLong("(1234567)").ToString());

            Console.Out.Write("ConvertStringToLong(\"1 234 567\") = ");
            Console.Out.WriteLine(ConvertStringToLong("1 234 567").ToString());

            Console.Out.Write("ConvertStringToLong(\"1,234,567\") = ");
            Console.Out.WriteLine(ConvertStringToLong("1,234,567").ToString());

            Console.Out.Write("ConvertStringToLong(\"\") = ");
            Console.Out.WriteLine(ConvertStringToLong("").ToString());

            Console.Out.Write("ConvertStringToLong(\"\", replaceNullWithZero: true) = ");
            Console.Out.WriteLine(ConvertStringToLong("", replaceNullWithZero: true).ToString());

            Console.Out.WriteLine();
            Console.Out.WriteLine("ConvertStringToDouble:");

            Console.Out.Write("ConvertStringToDouble(\"12345.67\") = ");
            Console.Out.WriteLine(ConvertStringToDouble("12345.67").ToString());

            Console.Out.Write("ConvertStringToDouble(\"12345,67\") = ");
            Console.Out.WriteLine(ConvertStringToDouble("12345,67").ToString());

            Console.Out.Write("ConvertStringToDouble(\"12345.67\") = ");
            Console.Out.WriteLine(ConvertStringToDouble("12345.67").ToString());

            Console.Out.Write("ConvertStringToDouble(\"12 345.67\") = ");
            Console.Out.WriteLine(ConvertStringToDouble("12 345.67").ToString());

            Console.Out.Write("ConvertStringToDouble(\"12,345.67\") = ");
            Console.Out.WriteLine(ConvertStringToDouble("12,345.67").ToString());

            Console.Out.Write("ConvertStringToDouble(\"12.345,67\") = ");
            Console.Out.WriteLine(ConvertStringToDouble("12.345,67").ToString());

            Console.Out.Write("ConvertStringToDouble(\"12.345,67\") = ");
            Console.Out.WriteLine(ConvertStringToDouble("12.345,67").ToString());

            Console.Out.Write("ConvertStringToDouble(\"-12345.67\") = ");
            Console.Out.WriteLine(ConvertStringToDouble("-12345.67").ToString());

            Console.Out.Write("ConvertStringToDouble(\"12345.67-\") = ");
            Console.Out.WriteLine(ConvertStringToDouble("12345.67-").ToString());

            Console.Out.Write("ConvertStringToDouble(\"(12 345,67)\") = ");
            Console.Out.WriteLine(ConvertStringToDouble("(12 345,67)").ToString());

            Console.Out.Write("ConvertStringToDouble(\"-\") = ");
            Console.Out.WriteLine(ConvertStringToDouble("-").ToString());

            Console.Out.Write("ConvertStringToDouble(\"6.626e-34\") = ");
            Console.Out.WriteLine(ConvertStringToDouble("6.626e-34").ToString());

            Console.Out.Write("ConvertStringToDouble(\"\") = ");
            Console.Out.WriteLine(ConvertStringToDouble("").ToString());

            Console.Out.Write("ConvertStringToDouble(\"\", replaceNullWithZero: true) = ");
            Console.Out.WriteLine(ConvertStringToDouble("", replaceNullWithZero: true).ToString());

            Console.Out.WriteLine();
            Console.Out.WriteLine("ConvertStringToBool:");

            Console.Out.Write("ConvertStringToBool(\"true\") = ");
            Console.Out.WriteLine(ConvertStringToBool("true").ToString());

            Console.Out.Write("ConvertStringToBool(\"0\") = ");
            Console.Out.WriteLine(ConvertStringToBool("0").ToString());

            Console.Out.Write("ConvertStringToBool(\"-1\") = ");
            Console.Out.WriteLine(ConvertStringToBool("-1").ToString());

            Console.Out.WriteLine();
            Console.Out.WriteLine("ConvertStringToDateTime:");

            Console.Out.Write("ConvertStringToDateTime(\"2020-12-31\") = ");
            Console.Out.WriteLine(ConvertStringToDateTime("2020-12-31").ToString());

            Console.Out.Write("ConvertStringToDateTime(\"31122020\", \"ddMMyyyy\") = ");
            Console.Out.WriteLine(ConvertStringToDateTime("31122020", "ddMMyyyy").ToString());
        }
    }
}
