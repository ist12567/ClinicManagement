using ACDClinicManagement.App.Views.CommonViews;
using ACDClinicManagement.AppHelpers.AppServices.Calculation;
using ACDClinicManagement.AppHelpers.CommonHelpers;
using ACDClinicManagement.Common.Enums;
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
    /// Interaction logic for AddEditValueAddedWindow.xaml
    /// </summary>
    public partial class AddEditValueAddedWindow : Window
    {
        public AddEditValueAddedWindow()
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
            ComboBoxYear.Focus();
            // Fill ComboBoxYear
            for (var year = 1387; year <= new PersianDateTime(DateTime.Now).Year + 1; year++)
                ComboBoxYear.Items.Add(year);

            switch (GeneralSettingsWindow.ChangeValuesAddedMode)
            {
                case CommonEnum.ChangeDatabaseMode.InsertInToDatabase:
                    Title = "افزودن ارزش افزوده‌ی جدید";
                    ComboBoxYear.SelectedItem = new PersianDateTime(DateTime.Now).Year;
                    break;
                case CommonEnum.ChangeDatabaseMode.UpdateDatabase:
                    Title = "ویرایش مشخصات ارزش افزوده";
                    ComboBoxYear.SelectedItem = new PersianDateTime(DateTime.Now).Year + 1;
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
            if (!TextBoxValueAdded.Text.IsValidNumeric())
            {
                status = false;
                "ارزش افزوده‌ی".NotValidMessage().ShowMessage();
                TextBoxValueAdded.Focus();
            }
            else if (Convert.ToDouble(TextBoxValueAdded.Text) < 0 || Convert.ToDouble(TextBoxValueAdded.Text) > 100)
            {
                status = false;
                "ارزش افزوده‌ی".NotValidMessage().ShowMessage();
                TextBoxValueAdded.Focus();
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
                var commandData = new SqlCommand("SELECT * FROM ValuesAdded " +
                                                 "WHERE Id = @Id",
                    MainWindow.PublicConnection);
                commandData.Parameters.AddWithValue("@Id", GeneralSettingsWindow.SelectedValueAddedId);
                var dataReaderData = commandData.ExecuteReader();
                while (dataReaderData.Read())
                {
                    Dispatcher.Invoke(() =>
                    {
                        ComboBoxYear.Text = dataReaderData["Year"].ToString();
                        TextBoxValueAdded.Text = dataReaderData["ValueAdded"].ToString();
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
                switch (GeneralSettingsWindow.ChangeValuesAddedMode)
                {
                    case CommonEnum.ChangeDatabaseMode.InsertInToDatabase:
                        var commandCheckInfo = new SqlCommand("SELECT COUNT(*) " +
                                                              "FROM ValuesAdded " +
                                                              "WHERE Year = @Year",
                            MainWindow.PublicConnection);
                        Dispatcher.Invoke(() => commandCheckInfo.Parameters.AddWithValue("@Year", ComboBoxYear.SelectedItem));
                        if (Convert.ToInt16(commandCheckInfo.ExecuteScalar()) > 0)
                            Dispatcher.Invoke(() => "ارزش افزوده برای این سال".DuplicateMessage().ShowMessage());
                        else
                        {
                            var maxRecord = Convert.ToInt32(SqlHelper.MaxSqlRecord(MainWindow.PublicConnection, "ValuesAdded")) + 1;
                            var data = new object[0];
                            Dispatcher.Invoke(() =>
                            {
                                data = new object[]
                                {
                                    maxRecord,
                                    ComboBoxYear.SelectedItem,
                                    TextBoxValueAdded.Text.Trim(),
                                    SpecialBaseHelper.UserId,
                                    DateTime.Now,
                                    SpecialBaseHelper.UserId,
                                    DateTime.Now
                                };
                            });
                            if (MainWindow.PublicConnection.InsertSqlData("ValuesAdded", data))
                            {
                                GeneralSettingsWindow.SelectedValueAddedId = maxRecord;
                                _isClose = true;
                                Dispatcher.Invoke(() => "ارزش افزوده‌ی".AddedMessage().ShowMessage());
                            }
                            else
                                Dispatcher.Invoke(() => WpfHelper.GetErrorMessage().ShowMessage());
                        }
                        break;
                    case CommonEnum.ChangeDatabaseMode.UpdateDatabase:
                        commandCheckInfo = new SqlCommand("SELECT COUNT(*) " +
                                                          "FROM ValuesAdded " +
                                                          "WHERE Year = @Year AND " +
                                                          "Id <> @Id",
                            MainWindow.PublicConnection);
                        Dispatcher.Invoke(() => commandCheckInfo.Parameters.AddWithValue("@Year", ComboBoxYear.SelectedItem));
                        commandCheckInfo.Parameters.AddWithValue("@Id", GeneralSettingsWindow.SelectedValueAddedId);
                        if (Convert.ToInt16(commandCheckInfo.ExecuteScalar()) > 0)
                            Dispatcher.Invoke(() => "ارزش افزوده برای این سال".DuplicateMessage().ShowMessage());
                        else
                        {
                            var commandUpdateData = new SqlCommand
                            {
                                Connection = MainWindow.PublicConnection,
                                CommandText = "UPDATE ValuesAdded " +
                                              "SET Year = @Year, " +
                                              "ValueAdded = @ValueAdded, " +
                                              "ModifiedAt = @ModifiedAt, " +
                                              "ModifiedBy = @ModifiedBy " +
                                              "WHERE Id = @Id"
                            };
                            Dispatcher.Invoke(() =>
                            {
                                commandUpdateData.Parameters.AddWithValue("@Year", ComboBoxYear.SelectedItem);
                                commandUpdateData.Parameters.AddWithValue("@ValueAdded", TextBoxValueAdded.Text.Trim());
                                commandUpdateData.Parameters.AddWithValue("@ModifiedBy", SpecialBaseHelper.UserId);
                                commandUpdateData.Parameters.AddWithValue("@ModifiedAt", DateTime.Now);
                                commandUpdateData.Parameters.AddWithValue("@Id", GeneralSettingsWindow.SelectedValueAddedId);
                            });
                            if (commandUpdateData.ExecuteNonQuery() == 1)
                            {
                                _isClose = true;
                                Dispatcher.Invoke(() => "ارزش افزوده‌ی".UpdatedMessage().ShowMessage());
                            }
                            else
                                Dispatcher.Invoke(() => WpfHelper.GetErrorMessage().ShowMessage());
                        }
                        break;
                }
                if (_isClose)
                {
                    var dataAdapterData = new SqlDataAdapter("SELECT * FROM ValuesAdded " +
                                                             "WHERE Id <> 0 " +
                                                             "ORDER BY Year ASC",
                        MainWindow.PublicConnection);
                    CalculateBaseHelper.DataTableValuesAdded = new DataTable();
                    dataAdapterData.Fill(CalculateBaseHelper.DataTableValuesAdded);
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
