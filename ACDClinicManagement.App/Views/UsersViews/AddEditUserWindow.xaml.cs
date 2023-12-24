using ACDClinicManagement.App.Views.CommonViews;
using ACDClinicManagement.App.Views.LocationsViews;
using ACDClinicManagement.App.Views.OrganizationsSubsetsViews;
using ACDClinicManagement.AppHelpers.CommonHelpers;
using ACDClinicManagement.Common.Enums;
using ACDClinicManagement.Common.Helpers;
using ACDClinicManagement.Common.SpecialHelpers;
using Microsoft.Win32;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ACDClinicManagement.App.Views.UsersViews
{
    /// <summary>
    /// Interaction logic for AddEditUserWindow.xaml
    /// </summary>
    public partial class AddEditUserWindow
    {
        public AddEditUserWindow()
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

        private long _cliams = 0;

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
            TextBoxFirstName.Focus();
            WpfHelper.FillComboBoxWithIdAndTitle(LocationsWindow.DataTableProvinces.Copy(), "", null, ComboBoxProvinces);
            // Fill ComboBoxUserType by UserType enum
            var userTypes = Enum.GetValues(typeof(CommonEnum.UserType));
            foreach (var userType in userTypes)
            {
                if ((CommonEnum.UserType)userType == 0 ||
                    Convert.ToInt16((CommonEnum.UserType)userType) > Convert.ToInt16(SpecialBaseHelper.CurrentUserType))
                {
                    var comboBoxItem = new ComboBoxItem
                    {
                        Content = ((CommonEnum.UserType)userType).GetEnumDescription(),
                        Tag = (CommonEnum.UserType)userType
                    };
                    ComboBoxUserType.Items.Add(comboBoxItem);
                }
            }
            // Fill WrapPanelRoles by UserCliam enum
            var cliams = Enum.GetValues(typeof(CommonEnum.UserClaim));
            foreach (var cliam in cliams)
            {
                if (Convert.ToInt64(cliam) == 0) continue;
                var checkBox = new CheckBox
                {
                    Name = cliam.ToString(),
                    Content = ((CommonEnum.UserClaim)cliam).GetEnumDescription(),
                    FlowDirection = FlowDirection.LeftToRight,
                    Margin = new Thickness { Left = 5, Right = 5, Top = 5, Bottom = 5 }
                };
                WrapPanelCliams.Children.Add(checkBox);
            }
            TextBoxUserName.IsEnabled = UsersWindow.ChangeUsersMode == CommonEnum.ChangeDatabaseMode.InsertInToDatabase;
            TextBoxUserName.IsEnabled = SpecialBaseHelper.CurrentUserType == CommonEnum.UserType.Administrator;
            PasswordBoxPassword.IsEnabled = UsersWindow.ChangeUsersMode == CommonEnum.ChangeDatabaseMode.InsertInToDatabase;
            PasswordBoxRePasssword.IsEnabled = UsersWindow.ChangeUsersMode == CommonEnum.ChangeDatabaseMode.InsertInToDatabase;
            ComboBoxUserType.Text = CommonEnum.UserType.None.GetEnumDescription();

            switch (UsersWindow.ChangeUsersMode)
            {
                case CommonEnum.ChangeDatabaseMode.InsertInToDatabase:
                    Title = "افزودن کاربر جدید";
                    ImageUser.Source = new BitmapImage(new Uri("/Contents/Images/businessman-xxl.png", UriKind.Relative));
                    break;
                case CommonEnum.ChangeDatabaseMode.UpdateDatabase:
                    Title = "ویرایش مشخصات کاربر";
                    var threadLoadData = new Thread(LoadData);
                    threadLoadData.Start();
                    _waitWindowLoadData = new WaitWindow { Owner = this };
                    _waitWindowLoadData.ShowDialog();
                    break;
            }
            foreach (var child in WrapPanelCliams.Children)
            {
                if (((CommonEnum.UserClaim)_cliams & (CommonEnum.UserClaim)Enum.Parse(typeof(CommonEnum.UserClaim), ((CheckBox)child).Name)) == (CommonEnum.UserClaim)Enum.Parse(typeof(CommonEnum.UserClaim), ((CheckBox)child).Name))
                    ((CheckBox)child).IsChecked = true;
            }
        }

        #endregion

        #region private void IsValidate()

        private bool IsValidate()
        {
            var status = true;
            if (string.IsNullOrWhiteSpace(TextBoxFirstName.Text))
            {
                status = false;
                "نام".InputValidationMessage().ShowMessage();
                TextBoxFirstName.Focus();
            }
            else if (string.IsNullOrWhiteSpace(TextBoxFirstName.Text))
            {
                status = false;
                "نام خانوادگی".InputValidationMessage().ShowMessage();
                TextBoxFirstName.Focus();
            }
            else if (ComboBoxUserType.SelectedIndex == 0)
            {
                status = false;
                "نوع کاربری".SelectValidationMessage().ShowMessage();
                ComboBoxUserType.Focus();
            }
            else if (!TextBoxUserName.Text.IsValidNumeric() || !TextBoxUserName.Text.IsValidNationalCode())
            {
                status = false;
                "نام کاربری".NotValidMessage().ShowMessage();
                TextBoxUserName.Focus();
            }
            if (string.IsNullOrWhiteSpace(PasswordBoxPassword.Password))
            {
                status = false;
                "کلمه‌ی عبور".InputValidationMessage().ShowMessage();
                PasswordBoxPassword.Focus();
            }

            if (string.IsNullOrWhiteSpace(PasswordBoxPassword.Password))
            {
                status = false;
                "تکرار کلمه‌ی عبور".InputValidationMessage().ShowMessage();
                PasswordBoxPassword.Focus();
            }
            else if (PasswordBoxPassword.Password != PasswordBoxRePasssword.Password)
            {
                status = false;
                "کلمه‌ی عبور".ValidationPasswordMessage();
                PasswordBoxPassword.Password = "";
                PasswordBoxRePasssword.Password = "";
                PasswordBoxPassword.Focus();
            }
            else if (!TextBoxCellPhoneNumber.Text.IsValidNumeric() || TextBoxCellPhoneNumber.Text.Length != 11)
            {
                status = false;
                "شماره‌ی تلفن همراه".NotValidMessage().ShowMessage();
                TextBoxCellPhoneNumber.Focus();
            }
            else if ((CommonEnum.UserType)((ComboBoxItem)ComboBoxUserType.SelectedItem).Tag == CommonEnum.UserType.ProvinceAdmin &&
                     ComboBoxProvinces.SelectedValue.ToString() == "0")
            {
                status = false;
                "استان".SelectValidationMessage().ShowMessage();
                ComboBoxProvinces.Focus();
            }
            else if ((CommonEnum.UserType)((ComboBoxItem)ComboBoxUserType.SelectedItem).Tag == CommonEnum.UserType.CityAdmin &&
                     ComboBoxCities.SelectedValue.ToString() == "0")
            {
                status = false;
                "شهر".SelectValidationMessage().ShowMessage();
                ComboBoxCities.Focus();
            }
            else if (((CommonEnum.UserType)((ComboBoxItem)ComboBoxUserType.SelectedItem).Tag == CommonEnum.UserType.OrganizationAdmin ||
                (CommonEnum.UserType)((ComboBoxItem)ComboBoxUserType.SelectedItem).Tag == CommonEnum.UserType.OrganizationAdmin) &&
                     ComboBoxOrganizations.SelectedValue.ToString() == "0")
            {
                status = false;
                "سازمان".SelectValidationMessage().ShowMessage();
                ComboBoxOrganizations.Focus();
            }
            else if ((CommonEnum.UserType)((ComboBoxItem)ComboBoxUserType.SelectedItem).Tag == CommonEnum.UserType.SubsetUser &&
                     ComboBoxSubsets.SelectedValue.ToString() == "0")
            {
                status = false;
                "زیرمجموعه".SelectValidationMessage().ShowMessage();
                ComboBoxSubsets.Focus();
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
                var commandData = new SqlCommand("SELECT * FROM Users " +
                                                 "WHERE Users.Id = @Id",
                    MainWindow.PublicConnection);
                commandData.Parameters.AddWithValue("Id", UsersWindow.SelectedUserId);
                var dataReaderData = commandData.ExecuteReader();
                while (dataReaderData.Read())
                {
                    Dispatcher.Invoke(() =>
                    {
                        ComboBoxProvinces.SelectedValue = dataReaderData["ProvinceId"];
                        ComboBoxCities.SelectedValue = dataReaderData["CityId"];
                        ComboBoxOrganizations.SelectedValue = dataReaderData["OrganizationId"];
                        ComboBoxSubsets.SelectedValue = dataReaderData["SubsetId"];
                        TextBoxFirstName.Text = dataReaderData["FirstName"].ToString();
                        TextBoxLastName.Text = dataReaderData["LastName"].ToString();
                        TextBoxUserName.Text = dataReaderData["UserName"].ToString();
                        PasswordBoxPassword.Password = "******";
                        PasswordBoxRePasssword.Password = "******";
                        TextBoxTellNumber.Text = dataReaderData["TellNumber"].ToString().ToDecryptData();
                        TextBoxCellPhoneNumber.Text = dataReaderData["CellPhoneNumber"].ToString().ToDecryptData();
                        TextBoxAddress.Text = dataReaderData["Address"].ToString().ToDecryptData();
                        CheckBoxActiveMode.IsChecked = ((CommonEnum.ActiveType)Convert.ToInt16(dataReaderData["ActiveMode"])) == CommonEnum.ActiveType.Active;
                        ComboBoxUserType.Text = ((CommonEnum.UserType)Convert.ToInt16(dataReaderData["TypeMode"])).GetEnumDescription();
                        _cliams = Convert.ToInt64(dataReaderData["Cliams"]);
                        if (dataReaderData["Image"] == DBNull.Value)
                        {
                            ImageUser.Source = new BitmapImage(new Uri("/Contents/Images/businessman-xxl.png", UriKind.Relative));
                            return;
                        }
                        var memoryStream = new MemoryStream();
                        var data = (byte[])dataReaderData["Image"];
                        memoryStream.Write(data, 0, data.Length);
                        memoryStream.Position = 0;
                        var img = System.Drawing.Image.FromStream(memoryStream);
                        var bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        var ms = new MemoryStream();
                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                        ms.Seek(0, SeekOrigin.Begin);
                        bitmapImage.StreamSource = ms;
                        bitmapImage.EndInit();
                        ImageUser.Source = bitmapImage;
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
            using (var transactionScope = new TransactionScope())
            {
                try
                {
                    MainWindow.PublicConnection.LoadConnection();
                    var bitmapImage = new BitmapImage();
                    Dispatcher.Invoke(() =>
                    {
                        bitmapImage = ImageUser.Source as BitmapImage;
                    });
                    var memStream = new MemoryStream();
                    var encoder = new JpegBitmapEncoder();
                    if (bitmapImage != null) encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                    encoder.Save(memStream);
                    switch (UsersWindow.ChangeUsersMode)
                    {
                        case CommonEnum.ChangeDatabaseMode.InsertInToDatabase:
                            var commandCheckInfo = new SqlCommand("SELECT COUNT(*) " +
                                                                  "FROM Users " +
                                                                  "WHERE UserName = @UserName ",
                                MainWindow.PublicConnection);
                            Dispatcher.Invoke(() => commandCheckInfo.Parameters.AddWithValue("@UserName", TextBoxUserName.Text.Trim().ToCorrectKeYe()));
                            if (Convert.ToInt16(commandCheckInfo.ExecuteScalar()) > 0)
                                Dispatcher.Invoke(() => "حساب کاربری".DuplicateMessage().ShowMessage());
                            else
                            {
                                var maxRecord = Convert.ToInt32(SqlHelper.MaxSqlRecord(MainWindow.PublicConnection, "Users")) + 1;
                                var data = new object[0];
                                Dispatcher.Invoke(() =>
                                {
                                    data = new object[]
                                    {
                                        maxRecord,
                                        Convert.ToInt16((CommonEnum.UserType) ((ComboBoxItem) ComboBoxUserType.SelectedItem).Tag),
                                        ComboBoxProvinces.SelectedValue.ToString(),
                                        ComboBoxCities.SelectedValue.ToString(),
                                        ComboBoxOrganizations.SelectedValue.ToString(),
                                        ComboBoxSubsets.SelectedValue.ToString(),
                                        TextBoxFirstName.Text.Trim().ToCorrectKeYe(),
                                        TextBoxLastName.Text.Trim().ToCorrectKeYe(),
                                        TextBoxUserName.Text.Trim().ToCorrectKeYe(),
                                        PasswordBoxPassword.Password.ToHashPassword(),
                                        TextBoxTellNumber.Text.Trim().ToCorrectKeYe().ToEncryptData(),
                                        TextBoxCellPhoneNumber.Text.ToEncryptData(),
                                        TextBoxAddress.Text.Trim().ToCorrectKeYe().ToEncryptData(),
                                        CheckBoxActiveMode.IsChecked == true ? Convert.ToInt16(CommonEnum.ActiveType.Active) : Convert.ToInt16(CommonEnum.ActiveType.DeActive),
                                        _cliams,
                                        memStream.GetBuffer(),
                                        "".ToEncryptData(),
                                        SpecialBaseHelper.UserId,
                                        DateTime.Now,
                                        SpecialBaseHelper.UserId,
                                        DateTime.Now
                                    };
                                });
                                if (MainWindow.PublicConnection.InsertSqlData("Users", data))
                                {
                                    UsersWindow.SelectedUserId = maxRecord;
                                    _isClose = true;
                                    Dispatcher.Invoke(() => "حساب کاربری".AddedMessage());
                                }
                                else
                                    Dispatcher.Invoke(() => WpfHelper.GetErrorMessage().ShowMessage());
                            }
                            break;
                        case CommonEnum.ChangeDatabaseMode.UpdateDatabase:
                            commandCheckInfo = new SqlCommand("SELECT COUNT(*) " +
                                                              "FROM Users " +
                                                              "WHERE UserName = @UserName AND " +
                                                              "Id <> @Id",
                                MainWindow.PublicConnection);
                            commandCheckInfo.Parameters.AddWithValue("@Id", UsersWindow.SelectedUserId);
                            Dispatcher.Invoke(() => commandCheckInfo.Parameters.AddWithValue("@UserName", TextBoxUserName.Text.Trim().ToCorrectKeYe()));
                            if (Convert.ToInt16(commandCheckInfo.ExecuteScalar()) > 0)
                                Dispatcher.Invoke(() => "حساب کاربری".DuplicateMessage().ShowMessage());
                            else
                            {
                                var commandUpdateData = new SqlCommand { Connection = MainWindow.PublicConnection };
                                Dispatcher.Invoke(() =>
                                {
                                    commandUpdateData.CommandText = "UPDATE Users " +
                                                                    "SET TypeMode = @TypeMode, " +
                                                                    "ProvinceId = @ProvinceId, " +
                                                                    "CityId = @CityId, " +
                                                                    "OrganizationId = @OrganizationId, " +
                                                                    "SubsetId = @SubsetId, " +
                                                                    "FirstName = @FirstName, " +
                                                                    "LastName = @LastName, " +
                                                                    "UserName = @UserName, " +
                                                                    "TellNumber = @TellNumber, " +
                                                                    "CellPhoneNumber = @CellPhoneNumber, " +
                                                                    "Address = @Address, " +
                                                                    "ActiveMode = @ActiveMode, " +
                                                                    "Cliams = @Cliams, " +
                                                                    "Image = @Image," +
                                                                    "ModifiedBy = @ModifiedBy, " +
                                                                    "ModifiedAt = @ModifiedAt " +
                                                                    "WHERE Id = @Id";
                                    commandUpdateData.Parameters.AddWithValue("@TypeMode", Convert.ToInt16((CommonEnum.UserType)((ComboBoxItem)ComboBoxUserType.SelectedItem).Tag));
                                    commandUpdateData.Parameters.AddWithValue("@ProvinceId", ComboBoxProvinces.SelectedValue.ToString());
                                    commandUpdateData.Parameters.AddWithValue("@CityId", ComboBoxCities.SelectedValue.ToString());
                                    commandUpdateData.Parameters.AddWithValue("@OrganizationId", ComboBoxOrganizations.SelectedValue.ToString());
                                    commandUpdateData.Parameters.AddWithValue("@SubsetId", ComboBoxSubsets.SelectedValue.ToString());
                                    commandUpdateData.Parameters.AddWithValue("@FirstName", TextBoxFirstName.Text.Trim().ToCorrectKeYe());
                                    commandUpdateData.Parameters.AddWithValue("@LastName", TextBoxLastName.Text.Trim().ToCorrectKeYe());
                                    commandUpdateData.Parameters.AddWithValue("@UserName", TextBoxUserName.Text.Trim().ToCorrectKeYe());
                                    commandUpdateData.Parameters.AddWithValue("@TellNumber", TextBoxTellNumber.Text.Trim().ToCorrectKeYe().ToEncryptData());
                                    commandUpdateData.Parameters.AddWithValue("@CellPhoneNumber", TextBoxCellPhoneNumber.Text.ToEncryptData());
                                    commandUpdateData.Parameters.AddWithValue("@Address", TextBoxAddress.Text.Trim().ToCorrectKeYe().ToEncryptData());
                                    commandUpdateData.Parameters.AddWithValue("@ActiveMode", CheckBoxActiveMode.IsChecked == true ? Convert.ToInt16(CommonEnum.ActiveType.Active) : Convert.ToInt16(CommonEnum.ActiveType.DeActive));
                                    commandUpdateData.Parameters.AddWithValue("@Cliams", _cliams);
                                    commandUpdateData.Parameters.AddWithValue("@Image", memStream.GetBuffer());
                                    commandUpdateData.Parameters.AddWithValue("@ModifiedBy", SpecialBaseHelper.UserId);
                                    commandUpdateData.Parameters.AddWithValue("@ModifiedAt", DateTime.Now);
                                    commandUpdateData.Parameters.AddWithValue("@Id", UsersWindow.SelectedUserId);
                                });
                                if (commandUpdateData.ExecuteNonQuery() == 1)
                                {
                                    _isClose = true;
                                    Dispatcher.Invoke(() => "حساب کاربری".UpdatedMessage());
                                }
                                else
                                    Dispatcher.Invoke(() => WpfHelper.GetErrorMessage().ShowMessage());
                            }
                            break;
                    }
                    if (_isClose)
                    {
                        var dataAdapterData = new SqlDataAdapter("SELECT Users.Id, TypeMode, " +
                                                                 "FirstName, LastName, UserName, " +
                                                                 "Provinces.Title AS Province, " +
                                                                 "Cities.Title AS City, " +
                                                                 "Organizations.Title AS Organization, " +
                                                                 "Subsets.Title AS Subset, " +
                                                                 "Users.ActiveMode, " +
                                                                 "Users.CreatedAt, Users.ModifiedAt " +
                                                                 "FROM Users, Provinces, Cities, " +
                                                                 "Organizations, Subsets " +
                                                                 "WHERE Users.Id <> 1 AND " +
                                                                 "Provinces.Id = Users.ProvinceId AND " +
                                                                 "Cities.Id = Users.CityId AND " +
                                                                 "Organizations.Id = Users.OrganizationId AND " +
                                                                 "Subsets.Id = Users.SubsetId AND " +
                                                                 (SpecialBaseHelper.ProvinceId == 0
                                                                      ? ""
                                                                      : "Users.ProvinceId = @ProvinceId AND " +
                                                                        (SpecialBaseHelper.CityId == 0
                                                                            ? ""
                                                                            : "Users.CityId = @CityId AND " +
                                                                              (SpecialBaseHelper.OrganizationId == 0
                                                                                  ? ""
                                                                                  : "Users.OrganizationId = @OrganizationId AND " +
                                                                                    (SpecialBaseHelper.SubsetId == 0
                                                                                        ? ""
                                                                                        : "Users.SubsetId = @SubsetId AND ")))) +
                                                                 "Users.Id <> @Id AND " +
                                                                 "Users.TypeMode > @TypeMode",
                        MainWindow.PublicConnection);
                        dataAdapterData.SelectCommand.Parameters.AddWithValue("@ProvinceId", SpecialBaseHelper.ProvinceId);
                        dataAdapterData.SelectCommand.Parameters.AddWithValue("@CityId", SpecialBaseHelper.CityId);
                        dataAdapterData.SelectCommand.Parameters.AddWithValue("@OrganizationId", SpecialBaseHelper.OrganizationId);
                        dataAdapterData.SelectCommand.Parameters.AddWithValue("@SubsetId", SpecialBaseHelper.SubsetId);
                        dataAdapterData.SelectCommand.Parameters.AddWithValue("@Id", SpecialBaseHelper.UserId);
                        dataAdapterData.SelectCommand.Parameters.AddWithValue("@TypeMode", Convert.ToInt16(SpecialBaseHelper.CurrentUserType));

                        UsersWindow.DataTableUsers = new DataTable();
                        dataAdapterData.Fill(UsersWindow.DataTableUsers);

                        transactionScope.Complete();
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

        #region RadioButton_Events

        #region RadioButtonMale_Checked

        private void RadioButtonMale_Checked(object sender, RoutedEventArgs e)
        {
            ImageUser.Source = new BitmapImage(new Uri("/Contents/Images/Man.png", UriKind.Relative));
        }

        #endregion

        #region RadioButtonFemale_Checked

        private void RadioButtonFemale_Checked(object sender, RoutedEventArgs e)
        {
            ImageUser.Source = new BitmapImage(new Uri("/Contents/Images/Woman.png", UriKind.Relative));
        }

        #endregion

        #endregion

        #region ComboBox_Events

        #region ComboBoxUserType_SelectionChanged

        private void ComboBoxUserType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxProvinces.IsEnabled = false;
            ComboBoxCities.IsEnabled = false;
            ComboBoxOrganizations.IsEnabled = false;
            ComboBoxSubsets.IsEnabled = false;

            foreach (var child in WrapPanelCliams.Children)
                ((CheckBox)child).IsChecked = false;
            switch ((CommonEnum.UserType)((ComboBoxItem)ComboBoxUserType.SelectedItem).Tag)
            {
                case CommonEnum.UserType.None:
                    foreach (var child in WrapPanelCliams.Children)
                        ((CheckBox)child).IsEnabled = false;

                    ComboBoxProvinces.SelectedValue = SpecialBaseHelper.ProvinceId;
                    ComboBoxCities.SelectedValue = SpecialBaseHelper.CityId;
                    ComboBoxOrganizations.SelectedValue = SpecialBaseHelper.OrganizationId;
                    ComboBoxSubsets.SelectedValue = SpecialBaseHelper.SubsetId;
                    break;
                case CommonEnum.UserType.Administrator:
                    foreach (var child in WrapPanelCliams.Children)
                        ((CheckBox)child).IsEnabled = SpecialBaseHelper.AdministratorRoles.Contains((CommonEnum.UserClaim)Enum.Parse(typeof(CommonEnum.UserClaim), ((CheckBox)child).Name));
                    break;
                case CommonEnum.UserType.ProvinceAdmin:
                    foreach (var child in WrapPanelCliams.Children)
                        ((CheckBox)child).IsEnabled = SpecialBaseHelper.ProvinceAdminRoles.Contains((CommonEnum.UserClaim)Enum.Parse(typeof(CommonEnum.UserClaim), ((CheckBox)child).Name));

                    ComboBoxProvinces.IsEnabled = SpecialBaseHelper.ProvinceId == 0;
                    break;
                case CommonEnum.UserType.CityAdmin:
                    foreach (var child in WrapPanelCliams.Children)
                        ((CheckBox)child).IsEnabled = SpecialBaseHelper.CityAdminRoles.Contains((CommonEnum.UserClaim)Enum.Parse(typeof(CommonEnum.UserClaim), ((CheckBox)child).Name));

                    ComboBoxProvinces.IsEnabled = SpecialBaseHelper.ProvinceId == 0;
                    ComboBoxCities.IsEnabled = SpecialBaseHelper.CityId == 0;
                    break;
                case CommonEnum.UserType.OrganizationAdmin:
                    foreach (var child in WrapPanelCliams.Children)
                        ((CheckBox)child).IsEnabled = SpecialBaseHelper.OrganizationAdminRoles.Contains((CommonEnum.UserClaim)Enum.Parse(typeof(CommonEnum.UserClaim), ((CheckBox)child).Name));

                    ComboBoxProvinces.IsEnabled = SpecialBaseHelper.ProvinceId == 0;
                    ComboBoxCities.IsEnabled = SpecialBaseHelper.CityId == 0;
                    ComboBoxOrganizations.IsEnabled = SpecialBaseHelper.OrganizationId == 0;
                    break;
                case CommonEnum.UserType.OrganizationUser:
                    foreach (var child in WrapPanelCliams.Children)
                        ((CheckBox)child).IsEnabled = SpecialBaseHelper.OrganizationUserRoles.Contains((CommonEnum.UserClaim)Enum.Parse(typeof(CommonEnum.UserClaim), ((CheckBox)child).Name));

                    ComboBoxProvinces.IsEnabled = SpecialBaseHelper.ProvinceId == 0;
                    ComboBoxCities.IsEnabled = SpecialBaseHelper.CityId == 0;
                    ComboBoxOrganizations.IsEnabled = SpecialBaseHelper.OrganizationId == 0;
                    break;
                case CommonEnum.UserType.SubsetUser:
                    foreach (var child in WrapPanelCliams.Children)
                        ((CheckBox)child).IsEnabled = SpecialBaseHelper.SubsetUserRoles.Contains((CommonEnum.UserClaim)Enum.Parse(typeof(CommonEnum.UserClaim), ((CheckBox)child).Name));

                    ComboBoxProvinces.IsEnabled = SpecialBaseHelper.ProvinceId == 0;
                    ComboBoxCities.IsEnabled = SpecialBaseHelper.CityId == 0;
                    ComboBoxOrganizations.IsEnabled = SpecialBaseHelper.OrganizationId == 0;
                    ComboBoxSubsets.IsEnabled = SpecialBaseHelper.SubsetId == 0;
                    break;
            }
        }

        #endregion

        #region ComboBoxProvinces_SelectionChanged
        private void ComboBoxProvinces_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WpfHelper.FillComboBoxWithIdAndTitle(LocationsWindow.DataTableCities.Copy(), "ProvinceId", e, ComboBoxCities);
        }

        #endregion

        #region ComboBoxCities_SelectionChanged
        private void ComboBoxCities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WpfHelper.FillComboBoxWithIdAndTitle(LocationsWindow.DataTableOrganizations.Copy(), "CityId", e, ComboBoxOrganizations);
        }

        #endregion

        #region ComboBoxOrganizations_SelectionChanged
        private void ComboBoxOrganizations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WpfHelper.FillComboBoxWithIdAndTitle(OrganizationsSubsetsWindow.DataTableSubsets.Copy(), "OrganizationId", e, ComboBoxSubsets);
        }

        #endregion

        #endregion

        #region Button_Events

        #region ButtonSelectImageUser_Click

        private void ButtonSelectImageUser_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = @"JPEG File (*.jpg) |*.jpg|All Files (*.*) |*.*",
                FilterIndex = 1,
                Title = "Image Files"
            };
            var resultOpenFileDialog = openFileDialog.ShowDialog();
            if (resultOpenFileDialog != true) return;
            ImageUser.Source = new BitmapImage(new Uri(openFileDialog.FileName));
        }

        #endregion

        #region ButtonSave_Click

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            _cliams = 0;
            foreach (var child in WrapPanelCliams.Children)
                if (((CheckBox)child).IsChecked == true)
                    _cliams |= Convert.ToInt64(Enum.Parse(typeof(CommonEnum.UserClaim), ((CheckBox)child).Name));
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
