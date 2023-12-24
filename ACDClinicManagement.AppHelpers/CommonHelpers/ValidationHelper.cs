using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ACDClinicManagement.AppHelpers.CommonHelpers
{
    public static class ValidationHelper
    {
        #region public static bool IsValidNumeric(this string text)
        public static bool IsValidNumeric(this string text)
        {
            Regex regex = new Regex(@"^\-?[0-9]*\.?[0-9]+$");
            return regex.IsMatch(text);
        }

        #endregion

        #region public static bool IsValidIpAddress(this string text)
        public static bool IsValidIpAddress(this string text)
        {
            Regex regex = new Regex(@"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$");
            return regex.IsMatch(text);
        }

        #endregion

        #region public static Boolean IsValidNationalCode(this string nationalCode)

        public static bool IsValidNationalCode(this string nationalCode)
        {
            if (string.IsNullOrEmpty(nationalCode))
                return false;

            if (nationalCode.Length != 10)
                return false;

            var regex = new Regex(@"\d{10}");
            if (!regex.IsMatch(nationalCode))
                return false;

            var allDigitEqual = new[]
            {
                "1111111111", "2222222222",
                "3333333333", "4444444444", "5555555555", "6666666666",
                "7777777777", "8888888888", "9999999999"
            };
            if (allDigitEqual.Contains(nationalCode)) return false;


            var chArray = nationalCode.ToCharArray();
            var num0 = Convert.ToInt32(chArray[0].ToString()) * 10;
            var num2 = Convert.ToInt32(chArray[1].ToString()) * 9;
            var num3 = Convert.ToInt32(chArray[2].ToString()) * 8;
            var num4 = Convert.ToInt32(chArray[3].ToString()) * 7;
            var num5 = Convert.ToInt32(chArray[4].ToString()) * 6;
            var num6 = Convert.ToInt32(chArray[5].ToString()) * 5;
            var num7 = Convert.ToInt32(chArray[6].ToString()) * 4;
            var num8 = Convert.ToInt32(chArray[7].ToString()) * 3;
            var num9 = Convert.ToInt32(chArray[8].ToString()) * 2;
            var a = Convert.ToInt32(chArray[9].ToString());

            var b = num0 + num2 + num3 + num4 + num5 + num6 + num7 + num8 + num9;
            var c = b % 11;

            return c < 2 && a == c || c >= 2 && 11 - c == a;
        }

        #endregion

        #region public static bool IsValidEmailAddress(this string text)
        public static bool IsValidEmailAddress(this string text)
        {
            Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            return regex.IsMatch(text);
        }

        #endregion
    }
}
