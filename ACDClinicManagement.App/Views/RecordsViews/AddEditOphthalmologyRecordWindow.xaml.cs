using ACDClinicManagement.App.Views.CommonViews;
using ACDClinicManagement.App.Views.DailyReferencesViews;
using ACDClinicManagement.AppHelpers.CommonHelpers;
using ACDClinicManagement.Common.Enums;
using ACDClinicManagement.Common.Helpers;
using ACDClinicManagement.Common.SpecialHelpers;
using ACDClinicManagement.Helpers;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace ACDClinicManagement.App.Views.RecordsViews
{
    /// <summary>
    /// Interaction logic for AddEditOphthalmologyRecordWindow.xaml
    /// </summary>
    public partial class AddEditOphthalmologyRecordWindow : Window
    {
        public AddEditOphthalmologyRecordWindow()
        {
            InitializeComponent();
            InitializeObjects();
        }

        // ••••••••••••
        // DEFINATIONS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region Windows

        private WaitWindow _waitWindowLoadData, _waitWindowSave;

        #endregion

        #region Classes


        #endregion

        #region Objects


        #endregion

        #region Variables

        private bool _isClose;

        #endregion

        // ••••••••••••
        // METHODS     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region private void InitializeObjects()
        private void InitializeObjects()
        {

        }

        #endregion

        #region private void LoadDefaults()

        private void LoadDefaults()
        {
            if (!(Owner is OphthalmologyRecordsWindow))
                InputLanguageHelper.LoadEnglishKeyboardLayout();
            this.ShowWindow(CommonEnum.WindowStyleMode.MiniTool);
            TextBoxREOD.Focus();
            switch (OphthalmologyRecordsWindow.ChangeRecordMode)
            {
                case CommonEnum.ChangeDatabaseMode.InsertInToDatabase:
                    Title = "افزودن سابقه‌ی مراجعه‌ی جدید";
                    break;
                case CommonEnum.ChangeDatabaseMode.UpdateDatabase:
                    Title = "ویرایش سابقه‌ی مراجعه";
                    var threadLoadData = new Thread(LoadData);
                    threadLoadData.Start();
                    _waitWindowLoadData = new WaitWindow { Owner = this };
                    _waitWindowLoadData.ShowDialog();
                    break;
            }
        }

        #endregion

        #region private void IsValidate()
        private bool IsValidate()
        {
            var status = true;
            if (Owner.Owner is OphthalmologyRecordsWindow && (string.IsNullOrWhiteSpace(TextBoxREOD.Text) ||
                string.IsNullOrWhiteSpace(TextBoxTOOD.Text) || string.IsNullOrWhiteSpace(TextBoxTOOS.Text) ||
                string.IsNullOrWhiteSpace(TextBoxREOS.Text) || string.IsNullOrWhiteSpace(TextBoxCLOD.Text) ||
                string.IsNullOrWhiteSpace(TextBoxOPERATION.Text) || string.IsNullOrWhiteSpace(TextBoxNOTE.Text)))
            {
                status = false;
                "حداقل یکی از آیتم‌های این بخش باید تکمیل شود".ShowMessage();
                TextBoxNOTE.Focus();
            }
            return status;
        }

        #endregion

        #region private void LoadData()

        private void LoadData()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                var commandData = new SqlCommand("SELECT * FROM OphthalmologyRecords " +
                                                 "WHERE Id = @Id",
                    MainWindow.PublicConnection);
                commandData.Parameters.AddWithValue("@Id", OphthalmologyRecordsWindow.SelectedRecordId);
                var dataReaderData = commandData.ExecuteReader();
                while (dataReaderData.Read())
                {
                    Dispatcher.Invoke(() =>
                    {
                        TextBoxREOD.Text = dataReaderData["REOD"].ToString();
                        TextBoxREOS.Text = dataReaderData["REOS"].ToString();
                        TextBoxTOOD.Text = dataReaderData["TOOD"].ToString();
                        TextBoxTOOS.Text = dataReaderData["TOOS"].ToString();
                        TextBoxCLOD.Text = dataReaderData["CLOD"].ToString();
                        TextBoxCLOS.Text = dataReaderData["CLOS"].ToString();
                        TextBoxOPERATION.Text = dataReaderData["OPERATION"].ToString();
                        TextBoxNOTE.Text = dataReaderData["NOTE"].ToString();
                    });
                }
                dataReaderData.Close();
            }
            catch (Exception exception)
            {
                MainWindow.ErrorMessage = exception.Message;
            }
            finally
            {
                Dispatcher.Invoke(() => _waitWindowLoadData.Close());
            }
        }

        #endregion

        #region private void Save()

        private void Save()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                switch (OphthalmologyRecordsWindow.ChangeRecordMode)
                {
                    case CommonEnum.ChangeDatabaseMode.InsertInToDatabase:
                        var commandCheckInfo = new SqlCommand("SELECT COUNT(*) " +
                                                              "FROM OphthalmologyRecords " +
                                                              "WHERE ReferenceId = @ReferenceId",
                            MainWindow.PublicConnection);
                        commandCheckInfo.Parameters.AddWithValue("@ReferenceId", DailyReferencesWindow.SelectedDailyReferenceId);
                        if (Convert.ToInt16(commandCheckInfo.ExecuteScalar()) > 0)
                            Dispatcher.Invoke(() => "سابقه‌ی مراجعه‌ی".DuplicateMessage().ShowMessage());
                        else
                        {
                            var maxRecord = Convert.ToInt32(SqlHelper.MaxSqlRecord(MainWindow.PublicConnection, "OphthalmologyRecords")) + 1;
                            var data = new object[0];
                            Dispatcher.Invoke(() =>
                            {
                                data = new object[]
                                {
                                    maxRecord,
                                    DailyReferencesWindow.SelectedPersonId,
                                    DailyReferencesWindow.SelectedDailyReferenceId,
                                    TextBoxREOD.Text.Trim().ToCorrectKeYe(),
                                    TextBoxREOS.Text.Trim().ToCorrectKeYe(),
                                    TextBoxTOOD.Text.Trim().ToCorrectKeYe(),
                                    TextBoxTOOS.Text.Trim().ToCorrectKeYe(),
                                    TextBoxCLOD.Text.Trim().ToCorrectKeYe(),
                                    TextBoxCLOS.Text.Trim().ToCorrectKeYe(),
                                    TextBoxOPERATION.Text.Trim().ToCorrectKeYe(),
                                    TextBoxNOTE.Text.Trim().ToCorrectKeYe(),
                                    SpecialBaseHelper.UserId,
                                    DateTime.Now,
                                    SpecialBaseHelper.UserId,
                                    DateTime.Now
                                };
                            });
                            if (MainWindow.PublicConnection.InsertSqlData("OphthalmologyRecords", data))
                            {
                                var permitToVisit = false;
                                Dispatcher.Invoke(() => permitToVisit = Owner is DailyReferencesWindow);
                                if (permitToVisit)
                                {
                                    var commandUpdateDailyReference = new SqlCommand("UPDATE DailyReferences " +
                                                                                     "SET VisitStatusMode = @VisitStatusMode, " +
                                                                                     "ModifiedBy = @ModifiedBy, " +
                                                                                     "ModifiedAt = @ModifiedAt " +
                                                                                     "WHERE Id = @Id",
                                        MainWindow.PublicConnection);
                                    commandUpdateDailyReference.Parameters.AddWithValue("@VisitStatusMode", Convert.ToInt16(CommonEnum.VisitStatusMode.Visited));
                                    commandUpdateDailyReference.Parameters.AddWithValue("@Id", DailyReferencesWindow.SelectedDailyReferenceId);
                                    commandUpdateDailyReference.Parameters.AddWithValue("@ModifiedBy", SpecialBaseHelper.UserId);
                                    commandUpdateDailyReference.Parameters.AddWithValue("@ModifiedAt", DateTime.Now);
                                    commandUpdateDailyReference.ExecuteNonQuery();
                                    var dataAdapterReferences = new SqlDataAdapter("SELECT DailyReferences.Id, People.Id AS PersonId, " +
                                                                                   "People.Code, People.FirstName, People.LastName, " +
                                                                                   "People.BirthDate, DailyReferences.VisitStatusMode, " +
                                                                                   "(SELECT COUNT(*) FROM DailyReferences WHERE PersonId = People.Id) AS ReferencesCount, " +
                                                                                   "DailyReferences.CreatedAt, DailyReferences.ModifiedAt " +
                                                                                   "FROM People, DailyReferences " +
                                                                                   "WHERE People.Id = DailyReferences.PersonId AND " +
                                                                                   "DailyReferences.Date = @Date " +
                                                                                   "ORDER BY DailyReferences.VisitStatusMode ASC, DailyReferences.ModifiedAt ASC",
                                        MainWindow.PublicConnection);
                                    dataAdapterReferences.SelectCommand.Parameters.AddWithValue("@Date", DailyReferencesWindow.SelectedDateTime.ToOnlyDateFormat());
                                    DailyReferencesWindow.DataTableDailyReferences = new DataTable();
                                    dataAdapterReferences.Fill(DailyReferencesWindow.DataTableDailyReferences);
                                }
                                OphthalmologyRecordsWindow.SelectedRecordId = maxRecord;
                                _isClose = true;
                                Dispatcher.Invoke(() => "سابقه‌ی مراجعه‌ی".AddedMessage().ShowMessage());
                            }
                            else
                                Dispatcher.Invoke(() => WpfHelper.GetErrorMessage().ShowMessage());
                        }
                        break;
                    case CommonEnum.ChangeDatabaseMode.UpdateDatabase:
                        commandCheckInfo = new SqlCommand("SELECT COUNT(*) " +
                                                          "FROM OphthalmologyRecords " +
                                                          "WHERE ReferenceId = @ReferenceId AND " +
                                                          "Id <> @Id",
                            MainWindow.PublicConnection);
                        commandCheckInfo.Parameters.AddWithValue("@ReferenceId", DailyReferencesWindow.SelectedDailyReferenceId);
                        commandCheckInfo.Parameters.AddWithValue("@Id", OphthalmologyRecordsWindow.SelectedRecordId);
                        if (Convert.ToInt16(commandCheckInfo.ExecuteScalar()) > 0)
                            Dispatcher.Invoke(() => "سابقه‌ی مراجعه‌ی".DuplicateMessage().ShowMessage());
                        else
                        {
                            var commandUpdateData = new SqlCommand
                            {
                                Connection = MainWindow.PublicConnection,
                                CommandText = "UPDATE OphthalmologyRecords " +
                                              "SET REOD = @REOD, " +
                                              "REOS = @REOS, " +
                                              "TOOD = @TOOD, " +
                                              "TOOS = @TOOS, " +
                                              "CLOD = @CLOD, " +
                                              "CLOS = @CLOS, " +
                                              "OPERATION = @OPERATION, " +
                                              "NOTE = @NOTE, " +
                                              "ModifiedBy = @ModifiedBy, " +
                                              "ModifiedAt = @ModifiedAt " +
                                              "WHERE Id = @Id"
                            };
                            Dispatcher.Invoke(() =>
                            {
                                commandUpdateData.Parameters.AddWithValue("@REOD", TextBoxREOD.Text.Trim().ToCorrectKeYe());
                                commandUpdateData.Parameters.AddWithValue("@REOS", TextBoxREOS.Text.Trim().ToCorrectKeYe());
                                commandUpdateData.Parameters.AddWithValue("@TOOD", TextBoxTOOD.Text.Trim().ToCorrectKeYe());
                                commandUpdateData.Parameters.AddWithValue("@TOOS", TextBoxTOOS.Text.Trim().ToCorrectKeYe());
                                commandUpdateData.Parameters.AddWithValue("@CLOD", TextBoxCLOD.Text.Trim().ToCorrectKeYe());
                                commandUpdateData.Parameters.AddWithValue("@CLOS", TextBoxCLOS.Text.Trim().ToCorrectKeYe());
                                commandUpdateData.Parameters.AddWithValue("@OPERATION", TextBoxOPERATION.Text.Trim().ToCorrectKeYe());
                                commandUpdateData.Parameters.AddWithValue("@NOTE", TextBoxNOTE.Text.Trim().ToCorrectKeYe());
                                commandUpdateData.Parameters.AddWithValue("@ModifiedBy", SpecialBaseHelper.UserId);
                                commandUpdateData.Parameters.AddWithValue("@ModifiedAt", DateTime.Now);
                                commandUpdateData.Parameters.AddWithValue("@Id", OphthalmologyRecordsWindow.SelectedRecordId);
                            });
                            if (commandUpdateData.ExecuteNonQuery() == 1)
                            {
                                _isClose = true;
                                Dispatcher.Invoke(() => "سابقه‌ی مراجعه‌ی".UpdatedMessage().ShowMessage());
                            }
                            else
                                Dispatcher.Invoke(() => WpfHelper.GetErrorMessage().ShowMessage());
                        }
                        break;
                }
                if (_isClose)
                {
                    var dataAdapterRecords = new SqlDataAdapter("SELECT OphthalmologyRecords.Id, OphthalmologyRecords.REOD, OphthalmologyRecords.REOS, " +
                                                                "OphthalmologyRecords.TOOD, OphthalmologyRecords.TOOS, " +
                                                                "OphthalmologyRecords.CLOD, OphthalmologyRecords.CLOS, " +
                                                                "OphthalmologyRecords.OPERATION, OphthalmologyRecords.NOTE, " +
                                                                "OphthalmologyRecords.CreatedAt, OphthalmologyRecords.ModifiedAt " +
                                                                "FROM DailyReferences, OphthalmologyRecords " +
                                                                "WHERE DailyReferences.Id = OphthalmologyRecords.ReferenceId AND " +
                                                                "DailyReferences.PersonId = @PersonId " +
                                                                "ORDER BY DailyReferences.CreatedAt ASC",
                    MainWindow.PublicConnection);
                    dataAdapterRecords.SelectCommand.Parameters.AddWithValue("@PersonId", OphthalmologyRecordsWindow.SelectedPersonId);
                    OphthalmologyRecordsWindow.DataTableRecords = new DataTable();
                    dataAdapterRecords.Fill(OphthalmologyRecordsWindow.DataTableRecords);
                }
            }
            catch (Exception exception)
            {
                Dispatcher.Invoke(() => WpfHelper.GetErrorMessage().ShowMessage());
                MainWindow.ErrorMessage = exception.Message;
                _isClose = false;
            }
            finally
            {
                Dispatcher.Invoke(() => _waitWindowSave.Close());
            }
        }

        #endregion

        // ••••••••••••
        // EVENTS       ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region Window_Events

        #region Window_Loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDefaults();
        }

        #endregion

        #region Window_Closed
        private void Window_Closed(object sender, EventArgs e)
        {
            if (!(Owner is OphthalmologyRecordsWindow))
                InputLanguageHelper.LoadPersianKeyboardLayout();
        }

        #endregion

        #region Window_KeyDown
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        #endregion

        #endregion

        #region Button_Events

        #region ButtonSave_Click

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            //if (!IsValidate()) return;
            var threadSave = new Thread(Save);
            threadSave.Start();
            _waitWindowSave = new WaitWindow { Owner = this };
            _waitWindowSave.ShowDialog();
            if (_isClose) Close();
        }

        #endregion

        #endregion
    }
}
