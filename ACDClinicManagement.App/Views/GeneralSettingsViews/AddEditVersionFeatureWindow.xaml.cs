using ACDClinicManagement.App.Views.CommonViews;
using ACDClinicManagement.AppHelpers.CommonHelpers;
using ACDClinicManagement.Common.Enums;
using ACDClinicManagement.Common.Helpers;
using ACDClinicManagement.Common.SpecialHelpers;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace ACDClinicManagement.App.Views.GeneralSettingsViews
{
    /// <summary>
    /// Interaction logic for AddEditVersionFeatureWindow.xaml
    /// </summary>
    public partial class AddEditVersionFeatureWindow : Window
    {
        public AddEditVersionFeatureWindow()
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
            this.ShowWindow(CommonEnum.WindowStyleMode.MiniTool);
            TextBoxFeature.Focus();
            TextBlockVersion.Text = GeneralSettingsWindow.SelectedVersionTitle;
            switch (GeneralSettingsWindow.ChangeVersionsFeaturesMode)
            {
                case CommonEnum.ChangeDatabaseMode.InsertInToDatabase:
                    Title = "افزودن ویژگی جدید";
                    break;
                case CommonEnum.ChangeDatabaseMode.UpdateDatabase:
                    Title = "ویرایش مشخصات ویژگی";
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
            if (string.IsNullOrWhiteSpace(TextBoxFeature.Text))
            {
                status = false;
                "ویژگی".InputValidationMessage().ShowMessage();
                TextBoxFeature.Focus();
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
                var commandData = new SqlCommand("SELECT * FROM VersionsFeatures " +
                                                 "WHERE Id = @Id",
                    MainWindow.PublicConnection);
                commandData.Parameters.AddWithValue("@Id", GeneralSettingsWindow.SelectedVersionFeatureId);
                var dataReaderData = commandData.ExecuteReader();
                while (dataReaderData.Read())
                    Dispatcher.Invoke(() => TextBoxFeature.Text = dataReaderData["Feature"].ToString());
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
                switch (GeneralSettingsWindow.ChangeVersionsFeaturesMode)
                {
                    case CommonEnum.ChangeDatabaseMode.InsertInToDatabase:
                        var commandCheckInfo = new SqlCommand("SELECT COUNT(*) " +
                                                              "FROM VersionsFeatures " +
                                                              "WHERE VersionId = @VersionId AND " +
                                                              "Feature = @Feature",
                            MainWindow.PublicConnection);
                        commandCheckInfo.Parameters.AddWithValue("@VersionId", GeneralSettingsWindow.SelectedVersionId);
                        Dispatcher.Invoke(() => commandCheckInfo.Parameters.AddWithValue("@Feature", TextBoxFeature.Text.Trim().ToCorrectKeYe()));
                        if (Convert.ToInt16(commandCheckInfo.ExecuteScalar()) > 0)
                            Dispatcher.Invoke(() => "ویژگی".DuplicateMessage().ShowMessage());
                        else
                        {
                            var maxRecord = Convert.ToInt32(SqlHelper.MaxSqlRecord(MainWindow.PublicConnection, "VersionsFeatures")) + 1;
                            var data = new object[0];
                            Dispatcher.Invoke(() =>
                            {
                                data = new object[]
                                {
                                    maxRecord,
                                    GeneralSettingsWindow.SelectedVersionId,
                                    TextBoxFeature.Text.Trim().ToCorrectKeYe(),
                                    SpecialBaseHelper.UserId,
                                    DateTime.Now,
                                    SpecialBaseHelper.UserId,
                                    DateTime.Now
                                };
                            });
                            if (MainWindow.PublicConnection.InsertSqlData("VersionsFeatures", data))
                            {
                                GeneralSettingsWindow.SelectedVersionFeatureId = maxRecord;
                                _isClose = true;
                                Dispatcher.Invoke(() => "ویژگی".AddedMessage().ShowMessage());
                            }
                            else
                                Dispatcher.Invoke(() => WpfHelper.GetErrorMessage().ShowMessage());
                        }
                        break;
                    case CommonEnum.ChangeDatabaseMode.UpdateDatabase:
                        commandCheckInfo = new SqlCommand("SELECT COUNT(*) " +
                                                          "FROM VersionsFeatures " +
                                                          "WHERE VersionId = @VersionId AND " +
                                                          "Feature = @Feature AND " +
                                                          "Id <> @Id",
                            MainWindow.PublicConnection);
                        commandCheckInfo.Parameters.AddWithValue("@VersionId", GeneralSettingsWindow.SelectedVersionId);
                        Dispatcher.Invoke(() => commandCheckInfo.Parameters.AddWithValue("@Feature", TextBoxFeature.Text.Trim().ToCorrectKeYe()));
                        commandCheckInfo.Parameters.AddWithValue("@Id", GeneralSettingsWindow.SelectedVersionFeatureId);
                        if (Convert.ToInt16(commandCheckInfo.ExecuteScalar()) > 0)
                            Dispatcher.Invoke(() => "ویژگی".DuplicateMessage().ShowMessage());
                        else
                        {
                            var commandUpdateData = new SqlCommand
                            {
                                Connection = MainWindow.PublicConnection,
                                CommandText = "UPDATE VersionsFeatures " +
                                              "SET Feature = @Feature, " +
                                              "ModifiedBy = @ModifiedBy, " +
                                              "ModifiedAt = @ModifiedAt " +
                                              "WHERE Id = @Id"
                            };
                            Dispatcher.Invoke(() =>
                            {
                                commandUpdateData.Parameters.AddWithValue("@Feature", TextBoxFeature.Text.Trim().ToCorrectKeYe());
                                commandUpdateData.Parameters.AddWithValue("@ModifiedBy", SpecialBaseHelper.UserId);
                                commandUpdateData.Parameters.AddWithValue("@ModifiedAt", DateTime.Now);
                                commandUpdateData.Parameters.AddWithValue("@Id", GeneralSettingsWindow.SelectedVersionFeatureId);
                            });
                            if (commandUpdateData.ExecuteNonQuery() == 1)
                            {
                                _isClose = true;
                                Dispatcher.Invoke(() => "ویژگی".UpdatedMessage().ShowMessage());
                            }
                            else
                                Dispatcher.Invoke(() => WpfHelper.GetErrorMessage().ShowMessage());
                        }
                        break;
                }
                if (_isClose)
                {
                    var dataAdapterData = new SqlDataAdapter("SELECT * FROM VersionsFeatures " +
                                                             "WHERE VersionId = @VersionId " +
                                                             "ORDER BY Feature ASC",
                        MainWindow.PublicConnection);
                    dataAdapterData.SelectCommand.Parameters.AddWithValue("@VersionId", GeneralSettingsWindow.SelectedVersionId);
                    GeneralSettingsWindow.DataTableVersionFeatures = new DataTable();
                    dataAdapterData.Fill(GeneralSettingsWindow.DataTableVersionFeatures);
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
            if (!IsValidate()) return;
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
