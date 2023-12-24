using System;
using System.ComponentModel;

namespace ACDClinicManagement.Common.Enums
{
    public class CommonEnum
    {
        // ••••••••••••
        // DEFINATIONS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region WindowStyleMode
        public enum WindowStyleMode
        {
            [Description("Normal Window")] Normal,
            [Description("Tool Owned Window")] Owned,
            [Description("Tool Window")] Tool,
            [Description("Tool Window")] MiniTool,
            [Description("ExtraMiniTool")] ExtraMiniTool
        }

        #endregion

        #region ChangeDatabaseMode
        public enum ChangeDatabaseMode
        {
            [Description("هیچ‌کدام")] None,
            [Description("افزوده رکورد به دیتابیس")] InsertInToDatabase,
            [Description("به‌روزرسانی رکورد در دیتابیس")] UpdateDatabase
        }

        #endregion

        #region ShowReportMode
        public enum ShowReportMode
        {
            None,
            Bill,
            Recoupment
        }

        #endregion

        #region ActiveType
        public enum ActiveType
        {
            [Description("... انتخاب کنید ...")] None = 0,
            [Description("فعال")] Active = 1,
            [Description("غیرفعال")] DeActive = 2
        }

        #endregion

        #region ShamsiMonth
        public enum ShamsiMonth
        {
            [Description("فروردین")] Farvardin = 1,
            [Description("اردیبهشت")] Ordibehesht = 2,
            [Description("خرداد")] Khordad = 3,
            [Description("تیر")] Tir = 4,
            [Description("مرداد")] Mordad = 5,
            [Description("شهریور")] Shahrivar = 6,
            [Description("مهر")] Mehr = 7,
            [Description("آبان")] Aban = 8,
            [Description("آذر")] Azar = 9,
            [Description("دی")] Dey = 10,
            [Description("یهمن")] Bahman = 11,
            [Description("اسفند")] Esfand = 12
        }

        #endregion

        #region UserType
        public enum UserType
        {
            [Description("... انتخاب کنید ...")] None = 0,
            [Description("مدیریت سیستم")] Administrator = 1,
            [Description("مدیریت استانی")] ProvinceAdmin = 2,
            [Description("مدیریت شهری")] CityAdmin = 3,
            [Description("مدیریت سازمانی")] OrganizationAdmin = 4,
            [Description("کاربری سازمانی")] OrganizationUser = 5,
            [Description("کاربری زیرمجموعه")] SubsetUser = 6
        }

        #endregion

        #region UserClaim
        [Flags]
        public enum UserClaim : ulong
        {
            [Description("None")] None = 0,
            [Description("بازنشانی کلمه‌ی عبور")] ResetPassword = 1,
            [Description("تنظیمات عمومی")] GeneralSettings = 2,
            [Description("موقعیت‌ها")] Locations = 4,
            [Description("سازمان‌ها و زیرمجموعه‌ها")] OrganizationsSubsets = 8,
            [Description("پشتیبان‌گیری از پایگاه داده")] BackupOperation = 16,
            [Description("بازیابی پایگاه داده")] RestoreOperation = 32,
            [Description("کاربران")] Users = 64,
            [Description("اشخاص")] People = 128,
            [Description("مراجعات روزانه")] DailyReferences = 256,
            [Description("گزارشات مراجعات")] ReferencesReport = 512,
        }

        #endregion

        #region PersonType
        public enum PersonType
        {
            [Description("هیچ‌کدام")] None = 0,
            [Description("حقیقی")] Real = 1,
            [Description("حقوقی")] Legal = 2
        }

        #endregion

        #region FuelType
        public enum FuelType
        {
            [Description("... انتخاب کنید ...")] None = 0,
            [Description("بنزین")] Petrol = 1,
            [Description("گازوئیل")] Gasoline = 2,
            [Description("گاز")] Gas = 3,
            [Description("دوگانه")] Hybrid = 4
        }

        #endregion

        #region ServiceMode
        public enum ServiceMode
        {
            [Description("هیچ‌کدام")] None = 0,
            [Description("چشم‌پزشکی")] Ophthalmology = 1
        }

        #endregion

        #region VisitStatusMode
        public enum VisitStatusMode
        {
            [Description("... انتخاب کنید ...")] None = 0,
            [Description("در انتظار ویزیت")] WaitingForVisit = 1,
            [Description("ویزیت‌شده")] Visited = 2,
            [Description("ویزیت‌نشده")] NotVisited = 3
        }

        #endregion

        #region PaymentMethodMode
        public enum PaymentMethodMode
        {
            [Description("هیچ‌کدام")] None = 0,
            [Description("بانکی و الکترونیکی")] PayByBankandPos = 1,
            [Description("مراجعه‌ی بانکی")] PayByBank = 2,
            [Description("الکترونیکی")] PayByPos = 3
        }

        #endregion

        #region PaymentStateMode
        public enum PaymentStateMode
        {
            [Description("هیچ‌کدام")] None = 0,
            [Description("در ارنتظار تأیید")] WaitingForConfirm = 1,
            [Description("تأییدشده")] Confirmed = 2
        }

        #endregion

        #region FarsiLetters
        public enum FarsiLetters
        {
            [Description("---")] None = 0,
            [Description("الف")] Alef = 1,
            [Description("ب")] Be = 2,
            [Description("پ")] Pe = 3,
            [Description("ت")] Te = 4,
            [Description("ث")] Se = 5,
            [Description("ج")] Jim = 6,
            [Description("چ")] Che = 7,
            [Description("ح")] He = 8,
            [Description("خ")] Khe = 9,
            [Description("د")] Dal = 10,
            [Description("ذ")] Zal = 11,
            [Description("ر")] Re = 12,
            [Description("ز")] Ze = 13,
            [Description("ژ")] Zhe = 14,
            [Description("س")] Sin = 15,
            [Description("ش")] Shin = 16,
            [Description("ص")] Sad = 17,
            [Description("ض")] Zad = 18,
            [Description("ط")] Ta = 19,
            [Description("ظ")] Za = 20,
            [Description("ع")] Eyn = 21,
            [Description("غ")] Gheyn = 22,
            [Description("ف")] Fe = 23,
            [Description("ق")] Ghaf = 24,
            [Description("ک")] Kaf = 25,
            [Description("گ")] Gaf = 26,
            [Description("ل")] Lam = 27,
            [Description("م")] Mim = 28,
            [Description("ن")] Noon = 29,
            [Description("و")] Wav = 30,
            [Description("ه")] HHe = 31,
            [Description("ی")] Ye = 32
        }
        #endregion

        #region BloodTypeMode
        public enum BloodTypeMode
        {
            [Description("... انتخاب کنید ...")] None = 0,
            [Description("O-")] ONegative = 1,
            [Description("O+")] OPositive = 2,
            [Description("A-")] ANegative = 3,
            [Description("A+")] APositive = 4,
            [Description("B-")] BNegative = 5,
            [Description("B+")] BPositive = 6,
            [Description("AB-")] ABNegative = 7,
            [Description("AB+")] ABPositive = 8
        }

        #endregion

        #region GenderMode
        public enum GenderMode
        {
            [Description("... انتخاب کنید ...")] None = 0,
            [Description("مرد")] Male = 1,
            [Description("زن")] Female = 2
        }

        #endregion

        #region MaritalStatusMode
        public enum MaritalStatusMode
        {
            [Description("... انتخاب کنید ...")] None = 0,
            [Description("مجرد")] Bachelor = 1,
            [Description("متأهل")] Married = 2
        }

        #endregion

        #region NewsAnnouncements
        public enum NewsAnnouncements
        {
            [Description("... انتخاب کنید ...")] None = 0,
            [Description("اعلان")] Announcement = 1,
            [Description("خبر")] News = 2
        }

        #endregion

        #region ReportLevel
        public enum ReportLevel
        {
            NoneLevel = 0,
            ProvincesLevel = 1,
            CitiesLevel = 2,
            OrganizationsLevel = 3,
            SubsetsLevel = 4,
            DetailsLevel = 5
        }

        #endregion

        #region GeneralMode
        public enum GeneralMode
        {
            [Description("None")] None = 0,
            [Description("Succeed")] Succeed = 1,
            [Description("Failed")] Failed = 2
        }

        #endregion
    }
}
