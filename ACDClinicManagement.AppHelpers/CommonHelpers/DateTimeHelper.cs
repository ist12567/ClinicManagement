using System;
using System.Data;
using System.Globalization;
using ACDClinicManagement.Common.Helpers;

namespace ACDClinicManagement.Helpers
{
    public static class DateTimeHelper
    {
        // ••••••••••••
        // DEFINATIONS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region Variable

        public static readonly string ShamsiMonthFarsi;

        #endregion

        #region Properties

        private static int MiladiYear { get; set; }

        private static int MiladiMonth { get; set; }

        private static int MiladiDay { get; set; }

        private static int ShamsiBeginYear { get; set; }

        public static int ShamsiYear { get; private set; }

        public static int ShamsiMonth { get; private set; }

        private static int ShamsiDay { get; set; }

        private static int MiladiPassDay { get; set; }

        private static int ShamsiPassDay { get; set; }

        private static System.DateTime DateTime { get; set; }

        #endregion

        #region Array

        private static readonly int[] Year = { 0, 31, 31, 31, 31, 31, 31, 30, 30, 30, 30, 30, 29 };
        private static readonly int[] LeapYear = { 0, 31, 31, 31, 31, 31, 31, 30, 30, 30, 30, 30, 30 };

        #endregion

        // ••••••••••••
        // METHODS     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region public static bool IsShamsiYearLeap(this int value)

        public static bool IsShamsiYearLeap(this int value)
        {
            var remaning = value % 33;
            return (remaning == 1) || (remaning == 5) || (remaning == 9) || (remaning == 13) || (remaning == 17) ||
                   (remaning == 22) || (remaning == 26) || (remaning == 30);
        }

        #endregion

        #region public static bool IsPersianDate(this string value)
        public static bool IsPersianDate(this string value)
        {
            try
            {
                var year = Convert.ToInt32(value.Substring(0, 4));
                var month = Convert.ToInt32(value.Substring(5, 2));
                var day = Convert.ToInt32(value.Substring(8, 2));

                new PersianDateTime(year, month, day);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region public static PersianDateTime ToPersianDateTime(this DateTime value)

        public static PersianDateTime ToPersianDateTime(this DateTime value)
        {
            return new PersianDateTime(value);
        }

        #endregion

        #region public static PersianDateTime ToPersianDateTime(this string value)
        public static PersianDateTime ToPersianDateTime(this string value)
        {
            var year = Convert.ToInt32(value.Substring(0, 4));
            var month = Convert.ToInt32(value.Substring(5, 2));
            var day = Convert.ToInt32(value.Substring(8, 2));

            return new PersianDateTime(year, month, day);
        }

        #endregion

        #region public static int RemaindDaysOfYear(this PersianDateTime dateTime)
        public static int RemaindDaysOfYear(this PersianDateTime dateTime)
        {
            return dateTime.IsLeapYear() ? 366 - dateTime.DayOfYear : 365 - dateTime.DayOfYear;
        }
        #endregion

        #region public static string ToFarsiFormatDate(this PersianDateTime persianDateTime)
        public static string ToFarsiFormatDate(this PersianDateTime persianDateTime)
        {
            return $"{persianDateTime.DayName} {persianDateTime.Day.ToString().ToFarsiFormat()} {persianDateTime.MonthName} سال {persianDateTime.Year.ToString().ToFarsiFormat()}";
        }

        #endregion

        #region public static string ToFarsiFormatTime(this PersianDateTime persianDateTime)
        public static string ToFarsiFormatTime(this PersianDateTime persianDateTime)
        {
            return $"ساعت {persianDateTime.Hour.ToString("D2").ToFarsiFormat()}:{persianDateTime.Minute.ToString("D2").ToFarsiFormat()}:{persianDateTime.Second.ToString("D2").ToFarsiFormat()}";
        }

        #endregion

        #region public static string ToFarsiFormatDateTime(this PersianDateTime persianDateTime)
        public static string ToFarsiFormatDateTime(this PersianDateTime persianDateTime)
        {
            return $"{persianDateTime.ToFarsiFormatDate()} {persianDateTime.ToFarsiFormatTime()}";
        }

        #endregion

        #region public static string ToFarsiFormatDateTimeFromSql(this object at)
        public static string ToFarsiFormatDateTimeFromSql(this object at)
        {
            return $"{Convert.ToDateTime(at).ToPersianDateTime().ToFarsiFormatDate()} {Convert.ToDateTime(at).ToPersianDateTime().ToFarsiFormatTime()}";
        }

        #endregion

        #region public static string ToOnlyDateFormat(this PersianDateTime persianDateTime)
        public static string ToOnlyDateFormat(this PersianDateTime persianDateTime)
        {
            return $"{persianDateTime.Year:D4}/{persianDateTime.Month:D2}/{persianDateTime.Day:D2}";
        }

        #endregion

        #region public static string ToOnlyTimeFormat(this PersianDateTime persianDateTime)
        public static string ToOnlyTimeFormat(this PersianDateTime persianDateTime)
        {
            return $"{persianDateTime.Hour:D2}:{persianDateTime.Minute:D2}:{persianDateTime.Second:D2}";
        }

        #endregion
    }
}
