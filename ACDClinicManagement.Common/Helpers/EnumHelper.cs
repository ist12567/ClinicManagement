using System;
using System.ComponentModel;

namespace ACDClinicManagement.Common.Helpers
{
    public static class EnumHelper
    {
        // ••••••••••••
        // METHODS     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region public static string GetEnumDescription(this Enum value)
        public static string GetEnumDescription(this Enum value)
        {
            var type = value.GetType();

            var memberInfo = type.GetMember(value.ToString());

            if (memberInfo.Length <= 0) return value.ToString();
            var attributes = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? ((DescriptionAttribute)attributes[0]).Description : value.ToString();
        }

        #endregion
    }
}
