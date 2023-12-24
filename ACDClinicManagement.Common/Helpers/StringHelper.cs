using ACDClinicManagement.Common.SpecialHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;

namespace ACDClinicManagement.Common.Helpers
{
    public static class StringHelper
    {
        // ••••••••••••
        // DEFINATIONS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region Array

        private static readonly string[] Yakan = { "صفر", "یک", "دو", "سه", "چهار", "پنج", "شش", "هفت", "هشت", "نه" };
        private static readonly string[] Dahgan = { "", "", "بیست", "سی", "چهل", "پنجاه", "شصت", "هفتاد", "هشتاد", "نود" };
        private static readonly string[] Dahyek = { "ده", "یازده", "دوازده", "سیزده", "چهارده", "پانزده", "شانزده", "هفده", "هجده", "نوزده" };
        private static readonly string[] Sadgan = { "", "یکصد", "دویست", "سیصد", "چهارصد", "پانصد", "ششصد", "هفتصد", "هشتصد", "نهصد" };
        private static readonly string[] Basex = { "", "هزار", "میلیون", "میلیارد", "تریلیون" };

        #endregion

        #region Const

        public const char ArabicKeChar = (char)1603; // "ك"
        public const char PersianKeChar = (char)1705; // "ک"
        public const char ArabicYeChar = (char)1610; // "ي"
        public const char PersianYeChar = (char)1740; // "ی"

        #endregion

        #region Variable

        private static string _errorMessage = "";

        #endregion

        // ••••••••••••
        // METHODS     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region public static string ToGroupedString(this string value)
        public static string ToGroupedString(this string value)
        {
            //if (groupSize <= 0)
            //    throw new ArgumentOutOfRangeException("groupSize", "groupSize must be greater than zero");

            //if (string.IsNullOrWhiteSpace(str))
            //    return str;

            //str = str.ToUnGroupedString();
            //if (groupSize >= str.Length) return str;
            //var groupedString = "";
            //var head = str.Length;
            //do
            //{
            //    head -= groupSize;
            //    groupedString = str.Substring(head, groupSize) + GroupSeperator + groupedString;
            //} while (head >= groupSize);

            //if (head > 0) groupedString = str.Substring(0, head) + GroupSeperator + groupedString;
            //return groupedString.TrimEnd(GroupSeperator.ToCharArray());

            decimal number;
            return decimal.TryParse(value, out number) ? number.ToString("N0") : value;
        }

        #endregion

        #region public static string ToUnGroupedString(this string value)
        public static string ToUnGroupedString(this string value)
        {
            decimal number;
            return decimal.TryParse(value, out number) ? number.ToString(CultureInfo.InvariantCulture) : value;
        }

        #endregion

        #region public static string GetFullName(string firstName, string lastName)
        public static string GetFullName(string firstName, string lastName)
        {
            var fullName = string.Format("{0} {1}", firstName, lastName);
            return fullName.Trim();
        }

        #endregion

        #region public static string GetYearMonthDayString(int? years, int? months, int? days)
        public static string GetYearMonthDayString(int? years, int? months, int? days)
        {
            const char va = 'و';
            var separator = " " + va + " ";
            var yearMonthDayString = new StringBuilder();

            if (years.HasValue && years != 0) yearMonthDayString.Append(years + " سال" + separator);
            if (months.HasValue && months != 0) yearMonthDayString.Append(months + " ماه" + separator);
            if (days.HasValue && days != 0) yearMonthDayString.Append(days + " روز");

            return yearMonthDayString
                .ToString()
                .Trim()
                .Trim(va)
                .Trim();
        }
        #endregion

        #region public static decimal? ToNullableDecimal(this string value)
        public static decimal? ToNullableDecimal(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            decimal decimalNumber;
            if (!decimal.TryParse(value, out decimalNumber))
                throw new FormatException("This can not be convert to decimal.");

            return decimalNumber;
        }

        #endregion

        #region public static decimal ToDecimal(this string value)
        public static decimal ToDecimal(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0m;

            decimal decimalNumber;
            if (!decimal.TryParse(value, out decimalNumber))
                throw new FormatException("This can not be convert to decimal.");

            return decimalNumber;
        }

        #endregion

        #region public static double? ToNullableDouble(this string value)
        public static double? ToNullableDouble(this string value)
        {
            return !string.IsNullOrWhiteSpace(value)
                ? Convert.ToDouble(value)
                : (double?)null;
        }

        #endregion

        #region public static string ToCurrencyString(this decimal value)
        public static string ToCurrencyString(this decimal value)
        {
            return value.ToString("F0");
        }

        #endregion

        #region public static string ToSignedCurrencyString(this decimal value)
        public static string ToSignedCurrencyString(this decimal value)
        {
            var groupedCurrencyString = Math.Abs(value).ToCurrencyString().ToGroupedString();
            var signedCurrencyString = string.Format(value < 0 ? "{0}-" : "{0}+", groupedCurrencyString);
            return signedCurrencyString;
        }

        #endregion

        #region public static decimal ToSignedDecimal(this string value)
        public static decimal ToSignedDecimal(this string value)
        {
            var signedChars = new[] { '+', '-' };
            var unsignedDecimalString = value.Trim(signedChars);
            var decimalValue = unsignedDecimalString.ToUnGroupedString().ToDecimal();

            if (value.Contains(signedChars[1].ToString(CultureInfo.InvariantCulture)))
                decimalValue *= -1;
            return decimalValue;
        }

        #endregion

        #region public static string ToCurrencyString(this decimal? value)
        public static string ToCurrencyString(this decimal? value)
        {
            return value.HasValue ? value.Value.ToCurrencyString() : null;
        }

        #endregion

        #region public static string ToDoubleString(this double value)
        public static string ToDoubleString(this double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        #endregion

        #region public static string ToDoubleString(this double? value)
        public static string ToDoubleString(this double? value)
        {
            return value.HasValue ? value.Value.ToDoubleString() : null;
        }

        #endregion

        #region public static string ToCorrectKeYe(this string value)
        public static string ToCorrectKeYe(this string value)
        {
            return value.TrimEnd().Replace(ArabicKeChar, PersianKeChar).Replace(ArabicYeChar, PersianYeChar);
        }

        #endregion

        #region public static double ToDoubleP(this string value)
        public static double ToDoubleP(this string value)
        {
            return double.Parse(value.Replace("/", ".").Replace(",", "."));
        }

        #endregion

        #region  public static string ToFarsiFormat(this string value)

        public static string ToFarsiFormat(this string value)
        {
            const string vInt = "1234567890";
            var mystring = value.ToCharArray(0, value.Length);
            var newStr = string.Empty;
            for (var i = 0; i <= mystring.Length - 1; i++)
                if (vInt.IndexOf(mystring[i]) == -1)
                    newStr += mystring[i];
                else
                    newStr += (char)(mystring[i] + 1728);
            return newStr;
        }

        #endregion

        #region public int ToSmsCount(this string message)

        public static int ToSmsCount(this string message)
        {
            if (message.Length <= 70) return 1;
            if (message.Length % 67 == 0) return message.Length / 67;
            return message.Length / 67 + 1;
        }

        #endregion

        #region public static string ToRandomItem(this List<string> list)
        public static string ToRandomItem(this List<string> list)
        {
            var randomNumber = new Random();
            return list.Count == 0 ? "" : list[randomNumber.Next(list.Count - 1)];
        }

        #endregion

        #region public static string GetFarsiString(this string value)

        private static string Getnum3(this int num3)
        {
            var s = "";
            var d12 = num3 % 100;
            var d3 = num3 / 100;
            if (d3 != 0)
                s = Sadgan[d3] + " و ";
            if ((d12 >= 10) && (d12 <= 19))
            {
                s = s + Dahyek[d12 - 10];
            }
            else
            {
                var d2 = d12 / 10;
                if (d2 != 0)
                    s = s + Dahgan[d2] + " و ";
                var d1 = d12 % 10;
                if (d1 != 0)
                    s = s + Yakan[d1] + " و ";
                s = s.Substring(0, s.Length - 3);
            }
            return s;
        }

        public static string GetFarsiString(this string value)
        {
            try
            {
                var stotal = "";
                if (value == "0")
                {
                    return Yakan[0];
                }
                value = value.PadLeft(((value.Length - 1) / 3 + 1) * 3, '0');
                var l = value.Length / 3 - 1;
                for (var i = 0; i <= l; i++)
                {
                    var b = int.Parse(value.Substring(i * 3, 3));
                    if (b != 0)
                        stotal = stotal + Getnum3(b) + " " + Basex[l - i] + " و ";
                }
                stotal = stotal.Substring(0, stotal.Length - 3);
                return stotal;
            }
            catch (Exception exception)
            {
                _errorMessage = exception.Message;
                return _errorMessage;
            }
        }

        #endregion

        #region public static string ToThousandsPlaceFarsiFormat(this decimal value)
        public static string ToThousandsPlaceFarsiFormat(this decimal value)
        {
            return ToFarsiFormat(string.Format("{0:n0}", value) == ""
                ? "0"
                : string.Format("{0:n0}", value));
        }

        #endregion

        #region public static string ToThousandsPlaceFarsiFormat(this long value)
        public static string ToThousandsPlaceFarsiFormat(this long value)
        {
            return ToFarsiFormat(string.Format("{0:n0}", value) == ""
                ? "0"
                : string.Format("{0:n0}", value));
        }

        #endregion

        #region public static string ToThousandsPlaceFarsiFormat(this int value)
        public static string ToThousandsPlaceFarsiFormat(this int value)
        {
            return ToFarsiFormat(string.Format("{0:n0}", value) == ""
                ? "0"
                : string.Format("{0:n0}", value));
        }

        #endregion

        #region public static string ToThousandsPlaceFarsiFormat(this short value)
        public static string ToThousandsPlaceFarsiFormat(this short value)
        {
            return ToFarsiFormat(string.Format("{0:n0}", value) == ""
                ? "0"
                : string.Format("{0:n0}", value));
        }

        #endregion

        #region public static string ToThousandsPlaceFormat(this int value)
        public static string ToThousandsPlaceFormat(this int value)
        {
            return string.Format("{0:n0}", value);
        }

        #endregion

        #region public static decimal ToRoundFloor(this decimal amount)
        public static decimal ToRoundFloor(this decimal amount)
        {
            return Convert.ToDecimal(Math.Floor(Convert.ToDouble(amount) / 1000.0d) * 1000);
        }

        #endregion
    }
}
