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

namespace ACDClinicManagement.App.Views.LocationsViews
{
    /// <summary>
    /// Interaction logic for AddEditCityWindow.xaml
    /// </summary>
    public partial class AddEditCityWindow
    {
        public AddEditCityWindow()
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
            TextBoxTitle.Focus();
            TextBlockProvinceTitle.Text = LocationsWindow.SelectedProvinceTitle;
            switch (LocationsWindow.ChangeCitiesMode)
            {
                case CommonEnum.ChangeDatabaseMode.InsertInToDatabase:
                    Title = "افزودن شهر جدید";
                    break;
                case CommonEnum.ChangeDatabaseMode.UpdateDatabase:
                    Title = "ویرایش مشخصات شهر";
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
            if (string.IsNullOrWhiteSpace(TextBoxTitle.Text))
            {
                status = false;
                "عنوان".InputValidationMessage().ShowMessage();
                TextBoxTitle.Focus();
            }
            else if (!TextBoxCode.Text.IsValidNumeric())
            {
                status = false;
                "کد".NotValidMessage().ShowMessage();
                TextBoxCode.Focus();
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
                var commandData = new SqlCommand("SELECT * FROM Cities " +
                                                 "WHERE Id = @Id",
                    MainWindow.PublicConnection);
                commandData.Parameters.AddWithValue("@Id", LocationsWindow.SelectedCityId);
                var dataReaderData = commandData.ExecuteReader();
                while (dataReaderData.Read())
                {
                    Dispatcher.Invoke(() =>
                    {
                        TextBoxTitle.Text = dataReaderData["Title"].ToString();
                        TextBoxCode.Text = dataReaderData["Code"].ToString();
                        CheckBoxActiveMode.IsChecked = ((CommonEnum.ActiveType)Convert.ToInt16(dataReaderData["ActiveMode"])) == CommonEnum.ActiveType.Active;
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
                switch (LocationsWindow.ChangeCitiesMode)
                {
                    case CommonEnum.ChangeDatabaseMode.InsertInToDatabase:
                        var commandCheckInfo = new SqlCommand("SELECT COUNT(*) " +
                                                              "FROM Cities " +
                                                              "WHERE ProvinceId = @ProvinceId AND " +
                                                              "Title = @Title",
                            MainWindow.PublicConnection);
                        commandCheckInfo.Parameters.AddWithValue("@ProvinceId", LocationsWindow.SelectedProvinceId);
                        Dispatcher.Invoke(() => commandCheckInfo.Parameters.AddWithValue("@Title", TextBoxTitle.Text.Trim().ToCorrectKeYe()));
                        if (Convert.ToInt16(commandCheckInfo.ExecuteScalar()) > 0)
                            Dispatcher.Invoke(() => "شهر".DuplicateMessage().ShowMessage());
                        else
                        {
                            var maxRecord = Convert.ToInt32(SqlHelper.MaxSqlRecord(MainWindow.PublicConnection, "Cities")) + 1;
                            var data = new object[0];
                            Dispatcher.Invoke(() =>
                            {
                                data = new object[]
                                {
                                    maxRecord,
                                    LocationsWindow.SelectedProvinceId,
                                    TextBoxTitle.Text.Trim().ToCorrectKeYe(),
                                    TextBoxCode.Text.Trim(),
                                    CheckBoxActiveMode.IsChecked == true ? Convert.ToInt16(CommonEnum.ActiveType.Active) : Convert.ToInt16(CommonEnum.ActiveType.DeActive),
                                    SpecialBaseHelper.UserId,
                                    DateTime.Now,
                                    SpecialBaseHelper.UserId,
                                    DateTime.Now
                                };
                            });
                            if (MainWindow.PublicConnection.InsertSqlData("Cities", data))
                            {
                                LocationsWindow.SelectedCityId = maxRecord;
                                _isClose = true;
                                Dispatcher.Invoke(() => "شهر".AddedMessage().ShowMessage());
                            }
                            else
                                Dispatcher.Invoke(() => WpfHelper.GetErrorMessage().ShowMessage());
                        }
                        break;
                    case CommonEnum.ChangeDatabaseMode.UpdateDatabase:
                        commandCheckInfo = new SqlCommand("SELECT COUNT(*) " +
                                                          "FROM Cities " +
                                                          "WHERE ProvinceId = @ProvinceId AND " +
                                                          "Title = @Title AND " +
                                                          "Id <> @Id",
                            MainWindow.PublicConnection);
                        commandCheckInfo.Parameters.AddWithValue("@ProvinceId", LocationsWindow.SelectedProvinceId);
                        Dispatcher.Invoke(() => commandCheckInfo.Parameters.AddWithValue("@Title", TextBoxTitle.Text.Trim().ToCorrectKeYe()));
                        commandCheckInfo.Parameters.AddWithValue("@Id", LocationsWindow.SelectedCityId);
                        if (Convert.ToInt16(commandCheckInfo.ExecuteScalar()) > 0)
                            Dispatcher.Invoke(() => "شهر".DuplicateMessage().ShowMessage());
                        else
                        {
                            var commandUpdateData = new SqlCommand
                            {
                                Connection = MainWindow.PublicConnection,
                                CommandText = "UPDATE Cities " +
                                              "SET Title = @Title, " +
                                              "Code = @Code, " +
                                              "ActiveMode = @ActiveMode, " +
                                              "ModifiedBy = @ModifiedBy, " +
                                              "ModifiedAt = @ModifiedAt " +
                                              "WHERE Id = @Id"
                            };
                            Dispatcher.Invoke(() =>
                            {
                                commandUpdateData.Parameters.AddWithValue("@Title", TextBoxTitle.Text.Trim().ToCorrectKeYe());
                                commandUpdateData.Parameters.AddWithValue("@Code", TextBoxCode.Text.Trim());
                                commandUpdateData.Parameters.AddWithValue("@ActiveMode", CheckBoxActiveMode.IsChecked == true ? Convert.ToInt16(CommonEnum.ActiveType.Active) : Convert.ToInt16(CommonEnum.ActiveType.DeActive));
                                commandUpdateData.Parameters.AddWithValue("@ModifiedBy", SpecialBaseHelper.UserId);
                                commandUpdateData.Parameters.AddWithValue("@ModifiedAt", DateTime.Now);
                                commandUpdateData.Parameters.AddWithValue("@Id", LocationsWindow.SelectedCityId);
                            });
                            if (commandUpdateData.ExecuteNonQuery() == 1)
                            {
                                _isClose = true;
                                Dispatcher.Invoke(() => "شهر".UpdatedMessage().ShowMessage());
                            }
                            else
                                Dispatcher.Invoke(() => WpfHelper.GetErrorMessage().ShowMessage());
                        }
                        break;
                }
                if (_isClose)
                {
                    var dataAdapterCities = new SqlDataAdapter("SELECT * FROM Cities " +
                                                               "WHERE Id <> 0" +
                                                               (SpecialBaseHelper.CityId != 0
                                                                  ? " AND ActiveMode = @ActiveMode AND Id = @Id"
                                                                  : "") +
                                                               " ORDER BY Title ASC",
                    MainWindow.PublicConnection);
                    dataAdapterCities.SelectCommand.Parameters.AddWithValue("@ActiveMode", Convert.ToInt16(CommonEnum.ActiveType.Active));
                    dataAdapterCities.SelectCommand.Parameters.AddWithValue("@Id", SpecialBaseHelper.CityId);
                    LocationsWindow.DataTableCities = new DataTable();
                    dataAdapterCities.Fill(LocationsWindow.DataTableCities);
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
