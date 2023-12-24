using ACDClinicManagement.Common.Helpers;
using ACDClinicManagement.Common.SpecialHelpers;
using System;
using System.Data.SqlClient;
using static ACDClinicManagement.Common.Enums.CommonEnum;

namespace ACDClinicManagement.AppHelpers.AppServices.NewsAnnouncement
{
    public class NewsAnnouncementStatus
    {
        // ••••••••••••
        // DEFINATIONS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region Classes

        #endregion

        #region Objects

        private object _lockerGetNewsAnnouncements;

        #endregion

        #region Variables

        public string ConnectionStringService;
        private string _errorMessage;

        #endregion

        // ••••••••••••
        // METHODS     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region public SunyStatus()
        public NewsAnnouncementStatus()
        {
            InitializeObjects();
        }
        #endregion

        #region private void InitializeObjects()
        private void InitializeObjects()
        {
            _lockerGetNewsAnnouncements = new object();
        }

        #endregion

        #region public void GetNewsAnnouncements()
        public void GetNewsAnnouncements()
        {
            lock (_lockerGetNewsAnnouncements)
            {
                using (var connectionnNewsAnnouncements = new SqlConnection(ConnectionStringService))
                {
                    try
                    {
                        connectionnNewsAnnouncements.Open();
                        var commandDateTime = new SqlCommand("SELECT GETDATE()", connectionnNewsAnnouncements);
                        SpecialBaseHelper.ServerDate = commandDateTime.ExecuteScalar().ToString();
                        var commandAnnouncement = new SqlCommand("SELECT TOP(1) Title, FullContent " +
                                                                 "FROM NewsAnnouncements " +
                                                                 "WHERE Type = @Type " +
                                                                 "ORDER BY Id DESC",
                            connectionnNewsAnnouncements);
                        commandAnnouncement.Parameters.AddWithValue("@Type", Convert.ToInt16(NewsAnnouncements.Announcement));
                        var dataReaderAnnouncement = commandAnnouncement.ExecuteReader();
                        while (dataReaderAnnouncement.Read())
                        {
                            SpecialBaseHelper.AnnouncementTitle = dataReaderAnnouncement["Title"]?.ToString();
                            SpecialBaseHelper.AnnouncementFullContent = dataReaderAnnouncement["FullContent"]?.ToString();
                        }
                        dataReaderAnnouncement.Close();
                    }
                    catch (Exception exception)
                    {
                        _errorMessage = _errorMessage + exception.Message;
                    }
                }
            }
        }
        #endregion
    }
}
