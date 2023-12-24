using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using ACDClinicManagement.Common.Helpers;

namespace ACDClinicManagement.AppHelpers.CommonHelpers
{
    public static class TransactionHelper
    {
        // ••••••••••••
        // DEFINATIONS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region Enums

        #region public enum ChannelTypes
        public enum ChannelTypes
        {
            [Description("دستی")]
            Present = 1,
            [Description("خودپرداز (ATM)")]
            Atm =2,
            [Description("شعبه")]
            Branch = 3,
            [Description("اینترانت")]
            Intranet = 5,
            [Description("پیامک")]
            Sms = 6,
            [Description("تلفنبانک")]
            TelBank = 7,
            [Description("خوددریافت")]
            SelfPay = 8,
            [Description("بانک پرداخت")]
            BankPay = 9,
            [Description("Web Kiosk")]
            WebKiosk = 13,
            [Description("پایانه فروش (POS)")]
            Pos = 14,
            [Description("اینترنت")]
            Internet = 59,
        }

        #endregion

        #region public enum Banks
        public enum Banks
        {
            [Description("بانک انصار")]
            Ansar = 1,
            [Description("بانک اقتصاد نوین")]
            EghtesadNovin = 2,
            [Description("بانک کشاورزی")]
            Keshavarzi = 3,
            [Description("بانک کارآفرین")]
            KarAfarin = 4,
            [Description("بانک مسکن")]
            Maskan = 5,
            [Description("بانک ملت")]
            Mellat = 6,
            [Description("بانک ملی")]
            Melli = 7,
            [Description("بانک پارسیان")]
            Parsian = 8,
            [Description("بانک رفاه")]
            Refah = 9,
            [Description("بانک صادرات")]
            Saderat = 10,
            [Description("صادرات")]
            Saderat2 = 11,
            [Description("بانک سامان")]
            Saman = 12,
            [Description("بانک سرمایه")]
            Sarmayeh = 13,
            [Description("بانک سپه")]
            Sepah = 14,
            [Description("بانک سینا")]
            Sina = 15,
            [Description("بانک توسعه")]
            Toseh = 16
        }

        #endregion

        #region public enum InstallmentType
        public enum InstallmentType
        {
            [Description("همه")]
            All = 0,
            [Description("چک")]
            Cheque = 1,
            [Description("قسط")]
            Installment = 2
        }

        #endregion

        #region public enum ChequeStatus
        public enum InstallmentStatus
        {
            [Description("همه")]
            All = 0,
            [Description("واگذار به بانک")]
            Issued = 1,
            [Description("در جریان وصول")]
            Collecting = 2,
            [Description("وصول شده")]
            Collected = 3,
            [Description("در جریان عودت")]
            Returning = 4,
            [Description("عودت داده شده")]
            Returned = 5,
            [Description("برگشتی")]
            Dishonoured = 6
        }

        #endregion

        #endregion

        #region Classes

        #endregion

        #region Objects

        #endregion

        #region Variables

        private static readonly int[] Formula = { 2, 3, 4, 5, 6, 7 };
        private static int _sum;

        #endregion

        // ••••••••••••
        // METHODS     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region private static int CalculateSum(string str)
        private static void CalculateSum(string str)
        {
            var integerList = new List<long>();
            for (var i = 0; i < str.Count(); i++)
            {
                integerList.Add(long.Parse(str.Substring(i, 1)));
            }
            for (var i = 0; i < integerList.Count; i++)
            {
                _sum += Convert.ToInt32(integerList[integerList.Count - 1 - i] * Formula[i]);
            }
        }
        #endregion

        #region private int CalculateRadix11(string digit)
        public static int CalculateRadix11(string digit)
        {
            _sum = 0;

            if (digit.Length <= 6)
            {
                CalculateSum(digit);
            }
            else
            {
                do
                {
                    if (digit.Length > 6)
                    {
                        CalculateSum(digit.Substring(digit.Length - 6));
                        digit = digit.Substring(0, digit.Length - 6);
                    }
                    else
                    {
                        CalculateSum(digit);
                        break;
                    }



                } while (digit.Any());
            }

            if ((_sum % 11 == 0) || _sum % 11 == 1)
            {
                return 0;
            }
            return 11 - _sum % 11;
        }
        #endregion

        #region public static string ReceiptNumber(this string digit8, string payCode)
        public static string ReceiptNumber(this string digit8, string payCode)
        {
            if (payCode == "") return "";
            var controlDigit = CalculateRadix11(digit8 + payCode);
            return Convert.ToInt64(digit8 + payCode + controlDigit).ToString("D13");
        }

        #endregion

        #region public static string PaymentNumber(this string receiptNumber, string price1000, string year1, string period2)

        public static string PaymentNumber(this string receiptNumber, string price1000, string year1, string period2)
        {
            if (receiptNumber == "") return "";
            long receiptNo;

            long.TryParse(receiptNumber, out receiptNo);

            var controlDigit1 = CalculateRadix11(price1000 + year1 + period2);
            var controlDigit2 =
                CalculateRadix11(receiptNo.ToString(CultureInfo.InvariantCulture) + price1000 + year1 + period2 +
                                 controlDigit1.ToString(CultureInfo.InvariantCulture));
            return
                Convert.ToInt64(price1000 + year1 + period2 + controlDigit1 + controlDigit2).ToString("D13");
        }

        #endregion

        #region public static string ReceiptNumber4Saderat(this string profileNumber, string payCode)

        public static string ReceiptNumber4Saderat(this string profileNumber, string payCode)
        {
            if (payCode == "") return "";
            var controlDigit1 = CalculateRadix11(profileNumber);
            var controlDigit2 = CalculateRadix11(payCode + controlDigit1 + profileNumber);
            return
                Convert.ToInt64((controlDigit2 == 0 || controlDigit2 == 1 ? 9 : controlDigit2) + payCode +
                                controlDigit1 + profileNumber).ToString("D18");
        }

        #endregion

        #region public static string PaymentNumber4Saderat(this string receiptNumber, string price1000)

        public static string PaymentNumber4Saderat(this string receiptNumber, string price1000)
        {
            if (receiptNumber == "") return "";
            var controlDigit1 = CalculateRadix11("1" + "0001" + price1000).ToString();
            var controlDigit2 = CalculateRadix11(receiptNumber + controlDigit1 + "1" + "0001" + price1000).ToString();
            return
                Convert.ToInt64((controlDigit2 == "0" || controlDigit2 == "1" ? "9" : controlDigit2) + controlDigit1 +
                                "1" + "0001" + price1000).ToString("D18");
        }

        #endregion

        #region public static string[] GetBillsInfo(this string text)

        public static string[] GetBillsInfo(this string text)
        {
            return new[]
            {
                text.Substring(4, 2),
                "تعداد فیش های فایل بانکی: " +
                    Convert.ToInt64(text.Substring(22)).ToThousandsPlaceFarsiFormat() + " فیش " +
                    "به مبلغ " + (Convert.ToInt64(text.Substring(12, 10))*1000).ToThousandsPlaceFarsiFormat() + " ریال"
            };
        }

        #endregion

        #region public static string GetBillsInfo4Saderat(this string text)

        public static string GetBillsInfo4Saderat(this string text)
        {
            return "تعداد فیش های فایل بانکی: " +
                   Convert.ToInt64(text.Substring(28)).ToThousandsPlaceFarsiFormat() + " فیش " +
                   "به مبلغ " + Convert.ToInt64(text.Substring(12, 16)).ToThousandsPlaceFarsiFormat() + " ریال";
        }

        #endregion

        #region public static string[] GetBillInfo4Confirm(this string text)
        public static string[] GetBillInfo4Confirm(this string text)
        {
            return new[]
            {
                text.Substring(0, 6),                                                   //Branch Code
                text.Substring(6, 2),                                                   //Pay Channel
                text.Substring(8, 6),                                                   //Pay Date
                text.Substring(14, 8),                                                  //Local Profile
                text.Substring(14, 13),                                                 //Receipt Number
                text.Substring(27, 13),                                                 //Payment Number
                text.Substring(40, 6),                                                  //Ref Code
                Convert.ToInt64(text.Substring(27, 13)
                .Substring(0, text.Substring(27, 13).Length - 5)) + "000"               //All Amount
            };
        }

        #endregion

        #region public static string[] GetSaderatBillInfo4Confirm(this string text)
        public static string[] GetSaderatBillInfo4Confirm(this string text)
        {
            return new[]
            {
                text.Substring(6, 2),                                                   //Pay Channel
                text.Substring(8, 6),                                                   //Pay Date
                Convert.ToInt64(text.Substring(14, 18)).ToString().Substring(6),        //Local Profile 
                text.Substring(14, 18),                                                 //Receipt Number
                text.Substring(32, 18),                                                 //Payment Number
                text.Substring(50, 6),                                                  //Ref Code
                Convert.ToInt64(text.Substring(32, 18))
                .ToString().Substring(7) + "000"                                        //All Amount
            };
        }

        #endregion
    }
}
