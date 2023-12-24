using ACDClinicManagement.App.Views.CommonViews;
using ACDClinicManagement.AppHelpers.CommonHelpers;
using ACDClinicManagement.Common.Enums;
using ACDClinicManagement.Common.Helpers;
using ACDClinicManagement.Common.SpecialHelpers;
using Microsoft.Win32;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ACDClinicManagement.App.Views.LocationsViews
{
    /// <summary>
    /// Interaction logic for AddEditOrganizationWindow.xaml
    /// </summary>
    public partial class AddEditOrganizationWindow
    {
        public AddEditOrganizationWindow()
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
            this.ShowWindow(CommonEnum.WindowStyleMode.Tool);
            TextBoxTitle.Focus();
            TextBlockCityTitle.Text = LocationsWindow.SelectedCityTitle;
            switch (LocationsWindow.ChangeOrganizationsMode)
            {
                case CommonEnum.ChangeDatabaseMode.InsertInToDatabase:
                    Title = "افزودن سازمان جدید";
                    ImageLogo.Source = new BitmapImage(new Uri("/Contents/Images/WarningLogo.jpg", UriKind.Relative));
                    break;
                case CommonEnum.ChangeDatabaseMode.UpdateDatabase:
                    Title = "ویرایش مشخصات سازمان";
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
            else if (string.IsNullOrWhiteSpace(TextBoxBossFirstName.Text))
            {
                status = false;
                "نام ریاست".InputValidationMessage().ShowMessage();
                TextBoxBossFirstName.Focus();
            }
            else if (string.IsNullOrWhiteSpace(TextBoxBossLastName.Text))
            {
                status = false;
                "نام خانوادگی ریاست".InputValidationMessage().ShowMessage();
                TextBoxBossLastName.Focus();
            }
            else if (!TextBoxBossCellPhoneNumber.Text.IsValidNumeric() || TextBoxBossCellPhoneNumber.Text.Length != 11)
            {
                "شماره‌ی تلفن همراه ریاست".NotValidMessage().ShowMessage();
                TextBoxBossCellPhoneNumber.Focus();
                status = false;
            }
            else if (string.IsNullOrWhiteSpace(TextBoxTelNumber.Text))
            {
                status = false;
                "شماره‌ی تلفن".InputValidationMessage().ShowMessage();
                TextBoxTelNumber.Focus();
            }
            else if (string.IsNullOrWhiteSpace(TextBoxFaxNumber.Text))
            {
                status = false;
                "شماره‌ی فکس".InputValidationMessage().ShowMessage();
                TextBoxFaxNumber.Focus();
            }
            else if (!string.IsNullOrWhiteSpace(TextBoxEmailAddress.Text) && !TextBoxEmailAddress.Text.IsValidEmailAddress())
            {
                status = false;
                "ایمیل".NotValidMessage().ShowMessage();
                TextBoxEmailAddress.Focus();
            }
            else if (!TextBoxPostalCode.Text.IsValidNumeric() || TextBoxPostalCode.Text.Length != 10)
            {
                "کد پستی".NotValidMessage().ShowMessage();
                TextBoxPostalCode.Focus();
                status = false;
            }
            else if (string.IsNullOrWhiteSpace(TextBoxAddress.Text))
            {
                status = false;
                "آدرس".InputValidationMessage().ShowMessage();
                TextBoxAddress.Focus();
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
                var commandData = new SqlCommand("SELECT * FROM Organizations " +
                                                 "WHERE Id = @Id",
                    MainWindow.PublicConnection);
                commandData.Parameters.AddWithValue("@Id", LocationsWindow.SelectedOrganizationId);
                var dataReaderData = commandData.ExecuteReader();
                while (dataReaderData.Read())
                {
                    Dispatcher.Invoke(() =>
                    {
                        TextBoxTitle.Text = dataReaderData["Title"].ToString();
                        TextBoxCode.Text = dataReaderData["Code"].ToString();
                        TextBoxBossFirstName.Text = dataReaderData["BossFirstName"].ToString();
                        TextBoxBossLastName.Text = dataReaderData["BossLastName"].ToString();
                        TextBoxBossCellPhoneNumber.Text = dataReaderData["BossCellPhoneNumber"].ToString();
                        TextBoxTelNumber.Text = dataReaderData["TelNumber"].ToString();
                        TextBoxFaxNumber.Text = dataReaderData["FaxNumber"].ToString();
                        TextBoxEmailAddress.Text = dataReaderData["EmailAddress"].ToString();
                        TextBoxPostalCode.Text = dataReaderData["PostalCode"].ToString();
                        TextBoxAddress.Text = dataReaderData["Address"].ToString();
                        CheckBoxActiveMode.IsChecked = ((CommonEnum.ActiveType)Convert.ToInt16(dataReaderData["ActiveMode"])) == CommonEnum.ActiveType.Active;
                        if (dataReaderData["Logo"] == DBNull.Value) return;
                        var memoryStream = new MemoryStream();
                        var data = (byte[])dataReaderData["Logo"];
                        memoryStream.Write(data, 0, data.Length);
                        memoryStream.Position = 0;
                        var img = Image.FromStream(memoryStream);
                        var bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        var ms = new MemoryStream();
                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                        ms.Seek(0, SeekOrigin.Begin);
                        bitmapImage.StreamSource = ms;
                        bitmapImage.EndInit();
                        ImageLogo.Source = bitmapImage;
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
                var bitmapImage = new BitmapImage();
                Dispatcher.Invoke(() =>
                {
                    bitmapImage = ImageLogo.Source as BitmapImage;
                });
                var logoMemoryStream = new MemoryStream();
                var encoder = new JpegBitmapEncoder();
                if (bitmapImage != null) encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(logoMemoryStream);
                switch (LocationsWindow.ChangeOrganizationsMode)
                {
                    case CommonEnum.ChangeDatabaseMode.InsertInToDatabase:
                        var commandCheckInfo = new SqlCommand("SELECT COUNT(*) " +
                                                              "FROM Organizations " +
                                                              "WHERE CityId = @CityId AND " +
                                                              "Title = @Title",
                            MainWindow.PublicConnection);
                        commandCheckInfo.Parameters.AddWithValue("@CityId", LocationsWindow.SelectedCityId);
                        Dispatcher.Invoke(() => commandCheckInfo.Parameters.AddWithValue("@Title", TextBoxTitle.Text.Trim().ToCorrectKeYe()));
                        if (Convert.ToInt16(commandCheckInfo.ExecuteScalar()) > 0)
                            Dispatcher.Invoke(() => "سازمان".DuplicateMessage().ShowMessage());
                        else
                        {
                            var maxRecord = Convert.ToInt32(SqlHelper.MaxSqlRecord(MainWindow.PublicConnection, "Organizations")) + 1;
                            var data = new object[0];
                            Dispatcher.Invoke(() =>
                            {
                                data = new object[]
                                {
                                    maxRecord,
                                    LocationsWindow.SelectedProvinceId,
                                    LocationsWindow.SelectedCityId,
                                    TextBoxTitle.Text.Trim().ToCorrectKeYe(),
                                    TextBoxCode.Text.Trim(),
                                    TextBoxBossFirstName.Text.Trim().ToCorrectKeYe(),
                                    TextBoxBossLastName.Text.Trim().ToCorrectKeYe(),
                                    TextBoxBossCellPhoneNumber.Text.Trim(),
                                    TextBoxTelNumber.Text.Trim(),
                                    TextBoxFaxNumber.Text.Trim(),
                                    TextBoxEmailAddress.Text.Trim(),
                                    TextBoxPostalCode.Text.Trim(),
                                    TextBoxAddress.Text.Trim().ToCorrectKeYe(),
                                    logoMemoryStream.GetBuffer(),
                                    CheckBoxActiveMode.IsChecked == true ? Convert.ToInt16(CommonEnum.ActiveType.Active) : Convert.ToInt16(CommonEnum.ActiveType.DeActive),
                                    SpecialBaseHelper.UserId,
                                    DateTime.Now,
                                    SpecialBaseHelper.UserId,
                                    DateTime.Now
                                };
                            });
                            if (MainWindow.PublicConnection.InsertSqlData("Organizations", data))
                            {
                                LocationsWindow.SelectedOrganizationId = maxRecord;
                                _isClose = true;
                                Dispatcher.Invoke(() => "سازمان".AddedMessage().ShowMessage());
                            }
                            else
                                Dispatcher.Invoke(() => WpfHelper.GetErrorMessage().ShowMessage());
                        }
                        break;
                    case CommonEnum.ChangeDatabaseMode.UpdateDatabase:
                        commandCheckInfo = new SqlCommand("SELECT COUNT(*) " +
                                                          "FROM Organizations " +
                                                          "WHERE CityId = @CityId AND " +
                                                          "Title = @Title AND " +
                                                          "Id <> @Id",
                            MainWindow.PublicConnection);
                        commandCheckInfo.Parameters.AddWithValue("@CityId", LocationsWindow.SelectedCityId);
                        Dispatcher.Invoke(() => commandCheckInfo.Parameters.AddWithValue("@Title", TextBoxTitle.Text.Trim().ToCorrectKeYe()));
                        commandCheckInfo.Parameters.AddWithValue("@Id", LocationsWindow.SelectedOrganizationId);
                        if (Convert.ToInt16(commandCheckInfo.ExecuteScalar()) > 0)
                            Dispatcher.Invoke(() => "سازمان".DuplicateMessage().ShowMessage());
                        else
                        {
                            var commandUpdateData = new SqlCommand
                            {
                                Connection = MainWindow.PublicConnection,
                                CommandText = "UPDATE Organizations " +
                                              "SET Title = @Title, " +
                                              "Code = @Code, " +
                                              "BossFirstName = @BossFirstName, " +
                                              "BossLastName = @BossLastName, " +
                                              "BossCellPhoneNumber = @BossCellPhoneNumber, " +
                                              "TelNumber = @TelNumber, " +
                                              "FaxNumber = @FaxNumber, " +
                                              "EmailAddress = @EmailAddress, " +
                                              "PostalCode = @PostalCode, " +
                                              "Address = @Address, " +
                                              "Logo = @Logo, " +
                                              "ActiveMode = @ActiveMode, " +
                                              "ModifiedBy = @ModifiedBy, " +
                                              "ModifiedAt = @ModifiedAt " +
                                              "WHERE Id = @Id"
                            };
                            Dispatcher.Invoke(() =>
                            {
                                commandUpdateData.Parameters.AddWithValue("@Title", TextBoxTitle.Text.Trim().ToCorrectKeYe());
                                commandUpdateData.Parameters.AddWithValue("@Code", TextBoxCode.Text.Trim());
                                commandUpdateData.Parameters.AddWithValue("@BossFirstName", TextBoxBossFirstName.Text.Trim().ToCorrectKeYe());
                                commandUpdateData.Parameters.AddWithValue("@BossLastName", TextBoxBossLastName.Text.Trim().ToCorrectKeYe());
                                commandUpdateData.Parameters.AddWithValue("@BossCellPhoneNumber", TextBoxBossCellPhoneNumber.Text.Trim());
                                commandUpdateData.Parameters.AddWithValue("@TelNumber", TextBoxTelNumber.Text.Trim());
                                commandUpdateData.Parameters.AddWithValue("@FaxNumber", TextBoxFaxNumber.Text.Trim());
                                commandUpdateData.Parameters.AddWithValue("@EmailAddress", TextBoxEmailAddress.Text.Trim());
                                commandUpdateData.Parameters.AddWithValue("@PostalCode", TextBoxPostalCode.Text.Trim());
                                commandUpdateData.Parameters.AddWithValue("@Address", TextBoxAddress.Text.Trim().ToCorrectKeYe());
                                commandUpdateData.Parameters.AddWithValue("@Logo", logoMemoryStream.GetBuffer());
                                commandUpdateData.Parameters.AddWithValue("@ActiveMode", CheckBoxActiveMode.IsChecked == true ? Convert.ToInt16(CommonEnum.ActiveType.Active) : Convert.ToInt16(CommonEnum.ActiveType.DeActive));
                                commandUpdateData.Parameters.AddWithValue("@ModifiedBy", SpecialBaseHelper.UserId);
                                commandUpdateData.Parameters.AddWithValue("@ModifiedAt", DateTime.Now);
                                commandUpdateData.Parameters.AddWithValue("@Id", LocationsWindow.SelectedOrganizationId);
                            });
                            if (commandUpdateData.ExecuteNonQuery() == 1)
                            {
                                _isClose = true;
                                Dispatcher.Invoke(() => "سازمان".UpdatedMessage().ShowMessage());
                            }
                            else
                                Dispatcher.Invoke(() => WpfHelper.GetErrorMessage().ShowMessage());
                        }
                        break;
                }
                if (_isClose)
                {
                    var dataAdapterOrganizations = new SqlDataAdapter("SELECT * FROM Organizations " +
                                                                      "WHERE Id <> 0" +
                                                                      (SpecialBaseHelper.OrganizationId != 0
                                                                          ? " AND ActiveMode = @ActiveMode AND Id = @Id"
                                                                          : "") +
                                                                      " ORDER BY Title ASC",
                    MainWindow.PublicConnection);
                    dataAdapterOrganizations.SelectCommand.Parameters.AddWithValue("@ActiveMode", Convert.ToInt16(CommonEnum.ActiveType.Active));
                    dataAdapterOrganizations.SelectCommand.Parameters.AddWithValue("@Id", SpecialBaseHelper.OrganizationId);
                    LocationsWindow.DataTableOrganizations = new DataTable();
                    dataAdapterOrganizations.Fill(LocationsWindow.DataTableOrganizations);
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

        #region ButtonSelectLogo_Click

        private void ButtonSelectLogo_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = @"JPEG File (*.jpg) |*.jpg|All Files (*.*) |*.*",
                FilterIndex = 1,
                Title = "Image Files"
            };
            var resultOpenFileDialog = openFileDialog.ShowDialog();
            if (resultOpenFileDialog != true) return;
            ImageLogo.Source = new BitmapImage(new Uri(openFileDialog.FileName));
        }

        #endregion

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
