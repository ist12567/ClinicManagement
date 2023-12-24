using static ACDClinicManagement.Common.Enums.CommonEnum;

namespace ACDClinicManagement.Common.SpecialHelpers
{
    public static class SpecialOphthalmologyHelper
    {
        // ••••••••••••
        // DEFINATIONS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region Properties

        public static string OrganizationTitle { get; set; }
        public static string OrganizationAccountNumber { get; set; }
        public static string OrganizationAccountId { get; set; }
        public static string OrganizationBankId { get; set; }
        public static PaymentMethodMode OrganizationPaymentMethodMode { get; set; }
        public static string OrganizationPosIp { get; set; }
        public static int OrganizationPosPort { get; set; }
        public static string OrganizationMessage { get; set; }
        public static byte[] OrganizationByteImage { get; set; }
        public static string SubsetTitle { get; set; }
        public static string SubsetAccountNumber { get; set; }
        public static string SubsetAccountId { get; set; }
        public static PaymentMethodMode SubsetPaymentMethodMode { get; set; }
        public static string SubsetPosIp { get; set; }
        public static int SubsetPosPort { get; set; }

        public static ShowReportMode ToShowReportMode { get; set; }

        #endregion
    }
}
