using ACDClinicManagement.Helpers;
using System;
using System.Data;
using System.Data.SqlClient;
using ACDClinicManagement.Common.Enums;

namespace ACDClinicManagement.AppHelpers.AppServices.References
{
    public class ReferencesStatus
    {
        // ••••••••••••
        // DEFINATIONS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region Classes

        #endregion

        #region Objects

        private object _lockerGetReferencesStatus;

        #endregion

        #region Variables

        public string ConnectionStringService;
        private string _errorMessage;

        #endregion

        // ••••••••••••
        // METHODS     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region public SunyStatus()
        public ReferencesStatus()
        {
            InitializeObjects();
        }
        #endregion

        #region private void InitializeObjects()
        private void InitializeObjects()
        {
            _lockerGetReferencesStatus = new object();
        }

        #endregion

        #region public (int ReferencesCount, int VisitedReferencesCount) GetReferencesStatus()
        public (int ReferencesCount, int VisitedReferencesCount) GetReferencesStatus()
        {
            lock (_lockerGetReferencesStatus)
            {
                using (var connectionReferencesStatus = new SqlConnection(ConnectionStringService))
                {
                    try
                    {
                        int referencesCount = 0;
                        int visitedReferencesCount = 0;
                        connectionReferencesStatus.Open();
                        var commandReferencesStatus = new SqlCommand("SELECT COUNT(*) AS CountOfReferences " +
                                                                     "FROM DailyReferences " +
                                                                     "WHERE Date = @Date",
                            connectionReferencesStatus);
                        commandReferencesStatus.Parameters.AddWithValue("@Date", SpecialAppHelper.CurrentDateTime.ToOnlyDateFormat());
                        var commandVisitedReferencesStatus = new SqlCommand("SELECT COUNT(*) AS CountOfVisited " +
                                                                            "FROM DailyReferences " +
                                                                            "WHERE Date= @Date AND " +
                                                                            "VisitStatusMode = @VisitStatusMode",
                            connectionReferencesStatus);
                        commandVisitedReferencesStatus.Parameters.AddWithValue("@Date", SpecialAppHelper.CurrentDateTime.ToOnlyDateFormat());
                        commandVisitedReferencesStatus.Parameters.AddWithValue("@VisitStatusMode", Convert.ToInt16(CommonEnum.VisitStatusMode.Visited));
                        referencesCount = Convert.ToInt32(commandReferencesStatus.ExecuteScalar());
                        visitedReferencesCount = Convert.ToInt32(commandVisitedReferencesStatus.ExecuteScalar());
                        return (referencesCount, visitedReferencesCount);
                    }
                    catch (Exception exception)
                    {
                        _errorMessage = _errorMessage + exception.Message;
                        return (0, 0);
                    }
                }
            }
        }

        #endregion
    }
}
