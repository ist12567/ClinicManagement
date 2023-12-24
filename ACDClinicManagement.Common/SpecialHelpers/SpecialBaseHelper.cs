using System;
using System.Collections.Generic;
using System.ComponentModel;
using static ACDClinicManagement.Common.Enums.CommonEnum;

namespace ACDClinicManagement.Common.SpecialHelpers
{
    public static class SpecialBaseHelper
    {
        // ••••••••••••
        // DEFINATIONS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region Objects

        public static readonly List<UserClaim> AdministratorRoles = new List<UserClaim>();
        public static readonly List<UserClaim> ProvinceAdminRoles = new List<UserClaim>();
        public static readonly List<UserClaim> CityAdminRoles = new List<UserClaim>();
        public static readonly List<UserClaim> OrganizationAdminRoles = new List<UserClaim>();
        public static readonly List<UserClaim> OrganizationUserRoles = new List<UserClaim>();
        public static readonly List<UserClaim> SubsetUserRoles = new List<UserClaim>();

        #endregion

        #region Properties

        public static bool ConnectionState { get; set; }
        public static string ServerDate { get; set; }


        public static string AnnouncementTitle { get; set; }
        public static string AnnouncementFullContent { get; set; }
        public static string ToDoList { get; set; }
        public static int ProvinceId { get; set; }
        public static int CityId { get; set; }
        public static int OrganizationId { get; set; }
        public static int SubsetId { get; set; }
        public static int UserId { get; set; }
        public static UserType CurrentUserType { get; set; }
        public static UserClaim Cliams { get; set; }
        public static ActiveType ActiveMode { get; set; }
        public static string FullName { get; set; }
        public static string UserName { get; set; }

        #endregion

        // ••••••••••••
        // METHODS     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region static SpecialHelper()
        static SpecialBaseHelper()
        {
            // Administrator
            AdministratorRoles.Add(UserClaim.ResetPassword);
            AdministratorRoles.Add(UserClaim.GeneralSettings);
            AdministratorRoles.Add(UserClaim.Locations);
            AdministratorRoles.Add(UserClaim.OrganizationsSubsets);
            AdministratorRoles.Add(UserClaim.BackupOperation);
            AdministratorRoles.Add(UserClaim.RestoreOperation);
            AdministratorRoles.Add(UserClaim.Users);
            AdministratorRoles.Add(UserClaim.People);
            AdministratorRoles.Add(UserClaim.DailyReferences);
            AdministratorRoles.Add(UserClaim.ReferencesReport);

            // Province Admin
            ProvinceAdminRoles.Add(UserClaim.ResetPassword);
            ProvinceAdminRoles.Add(UserClaim.GeneralSettings);
            ProvinceAdminRoles.Add(UserClaim.Locations);
            ProvinceAdminRoles.Add(UserClaim.OrganizationsSubsets);
            ProvinceAdminRoles.Add(UserClaim.BackupOperation);
            ProvinceAdminRoles.Add(UserClaim.RestoreOperation);
            ProvinceAdminRoles.Add(UserClaim.Users);
            ProvinceAdminRoles.Add(UserClaim.People);
            ProvinceAdminRoles.Add(UserClaim.DailyReferences);
            ProvinceAdminRoles.Add(UserClaim.ReferencesReport);

            // City Admin
            CityAdminRoles.Add(UserClaim.ResetPassword);
            CityAdminRoles.Add(UserClaim.GeneralSettings);
            CityAdminRoles.Add(UserClaim.Locations);
            CityAdminRoles.Add(UserClaim.OrganizationsSubsets);
            CityAdminRoles.Add(UserClaim.BackupOperation);
            CityAdminRoles.Add(UserClaim.RestoreOperation);
            CityAdminRoles.Add(UserClaim.Users);
            CityAdminRoles.Add(UserClaim.People);
            CityAdminRoles.Add(UserClaim.DailyReferences);
            CityAdminRoles.Add(UserClaim.ReferencesReport);

            // Organization Admin
            OrganizationAdminRoles.Add(UserClaim.ResetPassword);
            OrganizationAdminRoles.Add(UserClaim.GeneralSettings);
            OrganizationAdminRoles.Add(UserClaim.OrganizationsSubsets);
            OrganizationAdminRoles.Add(UserClaim.BackupOperation);
            OrganizationAdminRoles.Add(UserClaim.RestoreOperation);
            OrganizationAdminRoles.Add(UserClaim.Users);
            OrganizationAdminRoles.Add(UserClaim.People);
            OrganizationAdminRoles.Add(UserClaim.DailyReferences);
            OrganizationAdminRoles.Add(UserClaim.ReferencesReport);

            // Organization User
            OrganizationUserRoles.Add(UserClaim.BackupOperation);
            OrganizationUserRoles.Add(UserClaim.People);
            OrganizationUserRoles.Add(UserClaim.DailyReferences);

            // Subset User
            SubsetUserRoles.Add(UserClaim.BackupOperation);
            SubsetUserRoles.Add(UserClaim.People);
            SubsetUserRoles.Add(UserClaim.DailyReferences);
            SubsetUserRoles.Add(UserClaim.ReferencesReport);
        }

        #endregion

        #region public static bool IsAccessTo(this UserClaim userCliam)
        public static bool IsAccessTo(this UserClaim userCliam)
        {
            return (Cliams & userCliam) == userCliam;
        }

        #endregion
    }
}
