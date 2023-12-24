using ACDClinicManagement.App.Views.CommonViews;
using ACDClinicManagement.App.Views.LocationsViews;
using ACDClinicManagement.AppHelpers;
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
using System.Windows.Controls;
using System.Windows.Input;

namespace ACDClinicManagement.App.Views.PeopleViews
{
    /// <summary>
    /// Interaction logic for AddEditPersonWindow.xaml
    /// </summary>
    public partial class AddEditPersonWindow : Window
    {
        public AddEditPersonWindow()
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
            WpfHelper.FillComboBoxWithIdAndTitle(LocationsWindow.DataTableProvinces.Copy(), "", null, ComboBoxProvinces);
            // Fill ComboBoxGender by GenderMode enum
            var genders = Enum.GetValues(typeof(CommonEnum.GenderMode));
            foreach (var gender in genders)
            {
                var comboBoxItem = new ComboBoxItem
                {
                    Content = ((CommonEnum.GenderMode)gender).GetEnumDescription(),
                    Tag = (CommonEnum.GenderMode)gender
                };
                ComboBoxGender.Items.Add(comboBoxItem);
            }
            // Fill ComboBoxBloodTypes by BloodTypeMode enum
            var bloodTypes = Enum.GetValues(typeof(CommonEnum.BloodTypeMode));
            foreach (var bloodType in bloodTypes)
            {
                var comboBoxItem = new ComboBoxItem
                {
                    Content = ((CommonEnum.BloodTypeMode)bloodType).GetEnumDescription(),
                    Tag = (CommonEnum.BloodTypeMode)bloodType
                };
                ComboBoxBloodTypes.Items.Add(comboBoxItem);
            }
            // Fill ComboBoxMaritalStatus by MaritalStatusMode enum
            var MaritalsStatus = Enum.GetValues(typeof(CommonEnum.MaritalStatusMode));
            foreach (var MaritalStatus in MaritalsStatus)
            {
                var comboBoxItem = new ComboBoxItem
                {
                    Content = ((CommonEnum.MaritalStatusMode)MaritalStatus).GetEnumDescription(),
                    Tag = (CommonEnum.MaritalStatusMode)MaritalStatus
                };
                ComboBoxMaritalStatus.Items.Add(comboBoxItem);
            }
            switch (PeopleWindow.ChangePeopleMode)
            {
                case CommonEnum.ChangeDatabaseMode.InsertInToDatabase:
                    Title = "افزودن شخص جدید";
                    ComboBoxGender.Focus();
                    ComboBoxGender.Text = CommonEnum.GenderMode.None.GetEnumDescription();
                    ComboBoxBloodTypes.Text = CommonEnum.BloodTypeMode.None.GetEnumDescription();
                    ComboBoxMaritalStatus.Text = CommonEnum.MaritalStatusMode.None.GetEnumDescription();
                    TextBlockBirthDate.Text = "... انتخاب کنید ...";
                    ComboBoxProvinces.SelectedValue = SpecialBaseHelper.ProvinceId;
                    ComboBoxCities.SelectedValue = SpecialBaseHelper.CityId;
                    break;
                case CommonEnum.ChangeDatabaseMode.UpdateDatabase:
                    Title = "ویرایش مشخصات شخص";
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
            if (ComboBoxGender.SelectedIndex == 0)
            {
                "جنسیت".SelectValidationMessage().ShowMessage();
                ComboBoxGender.Focus();
                status = false;
            }
            else if (string.IsNullOrWhiteSpace(TextBoxFirstName.Text))
            {
                "نام".InputValidationMessage().ShowMessage();
                TextBoxFirstName.Focus();
                status = false;
            }
            else if (string.IsNullOrWhiteSpace(TextBoxLastName.Text))
            {
                "نام خانوادگی".InputValidationMessage().ShowMessage();
                TextBoxLastName.Focus();
                status = false;
            }
            else if (!string.IsNullOrWhiteSpace(TextBoxNationalCode.Text) && (!TextBoxNationalCode.Text.IsValidNumeric() || !TextBoxNationalCode.Text.IsValidNationalCode()))
            {
                "کد ملی".NotValidMessage().ShowMessage();
                TextBoxNationalCode.Focus();
                status = false;
            }
            else if (TextBlockBirthDate.Tag is null)
            {
                status = false;
                "تاریخ تولد".NotValidMessage().ShowMessage();
                TextBlockBirthDate.Focus();
            }
            else if (!TextBoxCellPhoneNumber.Text.IsValidNumeric() || TextBoxCellPhoneNumber.Text.Length != 11)
            {
                "شماره‌ی تلفن همراه".NotValidMessage().ShowMessage();
                TextBoxCellPhoneNumber.Focus();
                status = false;
            }
            else if (ComboBoxProvinces.SelectedIndex == 0)
            {
                "استان".SelectValidationMessage().ShowMessage();
                ComboBoxProvinces.Focus();
                status = false;
            }
            else if (ComboBoxCities.SelectedIndex == 0)
            {
                "شهر".SelectValidationMessage().ShowMessage();
                ComboBoxCities.Focus();
                status = false;
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
                var commandData = new SqlCommand("SELECT * FROM People " +
                                                 "WHERE Id = @Id",
                    MainWindow.PublicConnection);
                commandData.Parameters.AddWithValue("Id", PeopleWindow.SelectedPersonId);
                var dataReaderData = commandData.ExecuteReader();
                while (dataReaderData.Read())
                {
                    Dispatcher.Invoke(() =>
                    {
                        TextBoxCode.Text = dataReaderData["Code"].ToString();
                        ComboBoxGender.Text = ((CommonEnum.GenderMode)Convert.ToInt16(dataReaderData["GenderMode"])).GetEnumDescription();
                        TextBoxFirstName.Text = dataReaderData["FirstName"].ToString();
                        TextBoxLastName.Text = dataReaderData["LastName"].ToString();
                        TextBoxNationalCode.Text = dataReaderData["NationalCode"].ToString();
                        TextBlockBirthDate.Text = dataReaderData["BirthDate"].ToString() == "" ? SpecialAppHelper.CurrentDateTime.ToFarsiFormatDate() : dataReaderData["BirthDate"].ToString().ToPersianDateTime().ToFarsiFormatDate();
                        TextBlockBirthDate.Tag = dataReaderData["BirthDate"].ToString() == "" ? SpecialAppHelper.CurrentDateTime.ToOnlyDateFormat() : dataReaderData["BirthDate"].ToString();
                        ComboBoxBloodTypes.Text = ((CommonEnum.BloodTypeMode)Convert.ToInt16(dataReaderData["BloodTypeMode"])).GetEnumDescription();
                        ComboBoxMaritalStatus.Text = ((CommonEnum.MaritalStatusMode)Convert.ToInt16(dataReaderData["MaritalStatusMode"])).GetEnumDescription();
                        TextBoxCellPhoneNumber.Text = dataReaderData["CellPhoneNumber"].ToString();
                        TextBoxPostalCode.Text = dataReaderData["PostalCode"].ToString();
                        ComboBoxProvinces.SelectedValue = dataReaderData["ProvinceId"];
                        ComboBoxCities.SelectedValue = dataReaderData["CityId"];
                        TextBoxAddress.Text = dataReaderData["Address"].ToString();
                        TextBoxComments.Text = dataReaderData["Comments"].ToString();
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
                switch (PeopleWindow.ChangePeopleMode)
                {
                    case CommonEnum.ChangeDatabaseMode.InsertInToDatabase:
                        var commandCheckInfo = new SqlCommand("SELECT COUNT(*) " +
                                                              "FROM People " +
                                                              "WHERE FirstName = @FirstName AND " +
                                                              "LastName = @LastName AND " +
                                                              "BirthDate = @BirthDate",
                            MainWindow.PublicConnection);
                        Dispatcher.Invoke(() =>
                        {
                            commandCheckInfo.Parameters.AddWithValue("@FirstName", TextBoxFirstName.Text.Trim().ToCorrectKeYe());
                            commandCheckInfo.Parameters.AddWithValue("@LastName", TextBoxLastName.Text.Trim().ToCorrectKeYe());
                            commandCheckInfo.Parameters.AddWithValue("@BirthDate", TextBlockBirthDate.Tag);
                        });
                        if (Convert.ToInt16(commandCheckInfo.ExecuteScalar()) > 0)
                            Dispatcher.Invoke(() => "شخص".DuplicateMessage().ShowMessage());
                        else
                        {
                            var maxRecord = Convert.ToInt32(SqlHelper.MaxSqlRecord(MainWindow.PublicConnection, "People")) + 1;
                            var commandInsertData = new SqlCommand("INSERT INTO People VALUES " +
                                                                   "(@Id, @OldCode, @GenderMode, @FirstName, @LastName, @NationalCode, " +
                                                                   "@BirthDate, @BloodTypeMode, @MaritalStatusMode, @CellPhoneNumber, @PostalCode, " +
                                                                   "@ProvinceId, @CityId, @Address, @Comments, @CreatedBy, @CreatedAt, @ModifiedBy, @ModifiedAt)",
                                MainWindow.PublicConnection);
                            var data = new object[0];
                            Dispatcher.Invoke(() =>
                            {
                                commandInsertData.Parameters.AddWithValue("@Id", maxRecord);
                                commandInsertData.Parameters.AddWithValue("@OldCode", 0);
                                commandInsertData.Parameters.AddWithValue("@GenderMode", Convert.ToInt16((CommonEnum.GenderMode)((ComboBoxItem)ComboBoxGender.SelectedItem).Tag));
                                commandInsertData.Parameters.AddWithValue("@FirstName", TextBoxFirstName.Text.Trim().ToCorrectKeYe());
                                commandInsertData.Parameters.AddWithValue("@LastName", TextBoxLastName.Text.Trim().ToCorrectKeYe());
                                commandInsertData.Parameters.AddWithValue("@NationalCode", TextBoxNationalCode.Text);
                                commandInsertData.Parameters.AddWithValue("@BirthDate", TextBlockBirthDate.Tag);
                                commandInsertData.Parameters.AddWithValue("@BloodTypeMode", Convert.ToInt16((CommonEnum.BloodTypeMode)((ComboBoxItem)ComboBoxBloodTypes.SelectedItem).Tag));
                                commandInsertData.Parameters.AddWithValue("@MaritalStatusMode", Convert.ToInt16((CommonEnum.MaritalStatusMode)((ComboBoxItem)ComboBoxMaritalStatus.SelectedItem).Tag));
                                commandInsertData.Parameters.AddWithValue("@CellPhoneNumber", TextBoxCellPhoneNumber.Text.Trim());
                                commandInsertData.Parameters.AddWithValue("@PostalCode", TextBoxPostalCode.Text.Trim());
                                commandInsertData.Parameters.AddWithValue("@ProvinceId", ComboBoxProvinces.SelectedValue);
                                commandInsertData.Parameters.AddWithValue("@CityId", ComboBoxCities.SelectedValue);
                                commandInsertData.Parameters.AddWithValue("@Address", TextBoxAddress.Text.Trim().ToCorrectKeYe());
                                commandInsertData.Parameters.AddWithValue("@Comments", TextBoxComments.Text.Trim().ToCorrectKeYe());
                                commandInsertData.Parameters.AddWithValue("@CreatedBy", SpecialBaseHelper.UserId);
                                commandInsertData.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                                commandInsertData.Parameters.AddWithValue("@ModifiedBy", SpecialBaseHelper.UserId);
                                commandInsertData.Parameters.AddWithValue("@ModifiedAt", DateTime.Now);
                            });
                            if (commandInsertData.ExecuteNonQuery() > 0)
                            {
                                PeopleWindow.SelectedPersonId = maxRecord;
                                _isClose = true;
                                Dispatcher.Invoke(() => "شخص".AddedMessage().ShowMessage());
                            }
                            else
                                Dispatcher.Invoke(() => WpfHelper.GetErrorMessage().ShowMessage());
                        }
                        break;
                    case CommonEnum.ChangeDatabaseMode.UpdateDatabase:
                        commandCheckInfo = new SqlCommand("SELECT COUNT(*) " +
                                                          "FROM People " +
                                                          "WHERE FirstName = @FirstName AND " +
                                                          "LastName = @LastName AND " +
                                                          "BirthDate = @BirthDate AND " +
                                                          "Id <> @Id",
                            MainWindow.PublicConnection);
                        Dispatcher.Invoke(() =>
                        {
                            commandCheckInfo.Parameters.AddWithValue("@FirstName", TextBoxFirstName.Text.Trim().ToCorrectKeYe());
                            commandCheckInfo.Parameters.AddWithValue("@LastName", TextBoxLastName.Text.Trim().ToCorrectKeYe());
                            commandCheckInfo.Parameters.AddWithValue("@BirthDate", TextBlockBirthDate.Tag);
                        });
                        commandCheckInfo.Parameters.AddWithValue("@Id", PeopleWindow.SelectedPersonId);
                        if (Convert.ToInt16(commandCheckInfo.ExecuteScalar()) > 0)
                            Dispatcher.Invoke(() => "شخص".DuplicateMessage().ShowMessage());
                        else
                        {
                            var commandUpdateData = new SqlCommand
                            {
                                Connection = MainWindow.PublicConnection,
                                CommandText = "UPDATE People " +
                                              "SET GenderMode = @GenderMode, " +
                                              "FirstName = @FirstName, " +
                                              "LastName = @LastName, " +
                                              "NationalCode = @NationalCode, " +
                                              "BirthDate = @BirthDate, " +
                                              "BloodTypeMode = @BloodTypeMode, " +
                                              "MaritalStatusMode = @MaritalStatusMode, " +
                                              "CellPhoneNumber = @CellPhoneNumber, " +
                                              "PostalCode = @PostalCode, " +
                                              "ProvinceId = @ProvinceId, " +
                                              "CityId = @CityId, " +
                                              "Address = @Address, " +
                                              "Comments = @Comments, " +
                                              "ModifiedBy = @ModifiedBy, " +
                                              "ModifiedAt = @ModifiedAt " +
                                              "WHERE Id = @Id"
                            };
                            Dispatcher.Invoke(() =>
                            {
                                commandUpdateData.Parameters.AddWithValue("@GenderMode", Convert.ToInt16((CommonEnum.GenderMode)((ComboBoxItem)ComboBoxGender.SelectedItem).Tag));
                                commandUpdateData.Parameters.AddWithValue("@FirstName", TextBoxFirstName.Text.Trim().ToCorrectKeYe());
                                commandUpdateData.Parameters.AddWithValue("@LastName", TextBoxLastName.Text.Trim().ToCorrectKeYe());
                                commandUpdateData.Parameters.AddWithValue("@NationalCode", TextBoxNationalCode.Text.Trim());
                                commandUpdateData.Parameters.AddWithValue("@BirthDate", TextBlockBirthDate.Tag);
                                commandUpdateData.Parameters.AddWithValue("@BloodTypeMode", Convert.ToInt16((CommonEnum.BloodTypeMode)((ComboBoxItem)ComboBoxBloodTypes.SelectedItem).Tag));
                                commandUpdateData.Parameters.AddWithValue("@MaritalStatusMode", Convert.ToInt16((CommonEnum.MaritalStatusMode)((ComboBoxItem)ComboBoxMaritalStatus.SelectedItem).Tag));
                                commandUpdateData.Parameters.AddWithValue("@CellPhoneNumber", TextBoxCellPhoneNumber.Text.Trim());
                                commandUpdateData.Parameters.AddWithValue("@PostalCode", TextBoxPostalCode.Text.Trim());
                                commandUpdateData.Parameters.AddWithValue("@ProvinceId", ComboBoxProvinces.SelectedValue);
                                commandUpdateData.Parameters.AddWithValue("@CityId", ComboBoxCities.SelectedValue);
                                commandUpdateData.Parameters.AddWithValue("@Address", TextBoxAddress.Text.Trim().ToCorrectKeYe());
                                commandUpdateData.Parameters.AddWithValue("@Comments", TextBoxComments.Text.Trim().ToCorrectKeYe());
                                commandUpdateData.Parameters.AddWithValue("@ModifiedBy", SpecialBaseHelper.UserId);
                                commandUpdateData.Parameters.AddWithValue("@ModifiedAt", DateTime.Now);
                                commandUpdateData.Parameters.AddWithValue("@Id", PeopleWindow.SelectedPersonId);
                            });
                            if (commandUpdateData.ExecuteNonQuery() == 1)
                            {
                                _isClose = true;
                                Dispatcher.Invoke(() => "شخص".UpdatedMessage().ShowMessage());
                            }
                            else
                                Dispatcher.Invoke(() => WpfHelper.GetErrorMessage().ShowMessage());
                        }
                        break;
                }
                if (_isClose)
                {
                    var dataAdapterData = new SqlDataAdapter("SELECT TOP(500) * FROM People " +
                                                             "WHERE Id <> 0 AND " +
                                                             "(CAST(Code AS NVARCHAR) = @Code OR " +
                                                             "CAST(OldCode AS NVARCHAR) = @OldCode OR " +
                                                             "FirstName LIKE Concat('%', @FirstName, '%') OR " +
                                                             "LastName LIKE Concat('%', @LastName, '%') OR " +
                                                             "(FirstName + ' ' + LastName) LIKE Concat('%', @FirstNameLastName, '%') OR " +
                                                             "NationalCode = @NationalCode) " +
                                                             "ORDER BY ModifiedAt DESC",
                    MainWindow.PublicConnection);
                    dataAdapterData.SelectCommand.Parameters.AddWithValue("@Code", PeopleWindow.SearchTextPerson);
                    dataAdapterData.SelectCommand.Parameters.AddWithValue("@OldCode", PeopleWindow.SearchTextPerson);
                    dataAdapterData.SelectCommand.Parameters.AddWithValue("@FirstName", PeopleWindow.SearchTextPerson);
                    dataAdapterData.SelectCommand.Parameters.AddWithValue("@LastName", PeopleWindow.SearchTextPerson);
                    dataAdapterData.SelectCommand.Parameters.AddWithValue("@FirstNameLastName", PeopleWindow.SearchTextPerson);
                    dataAdapterData.SelectCommand.Parameters.AddWithValue("@NationalCode", PeopleWindow.SearchTextPerson);
                    PeopleWindow.DataTablePeople = new DataTable();
                    dataAdapterData.Fill(PeopleWindow.DataTablePeople);
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

        #region TextBlock_Events

        #region TextBlockBirthDate_MouseLeftButtonDown
        private void TextBlockBirthDate_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dateTimePickerWindow = new DateTimePickerWindow(TextBlockBirthDate.Tag) { Owner = this };
            dateTimePickerWindow.ShowDialog();
            if (DateTimePickerWindow.Result)
            {
                TextBlockBirthDate.Text = DateTimePickerWindow.PickedDateTime.ToFarsiFormatDate();
                TextBlockBirthDate.Tag = DateTimePickerWindow.PickedDateTime.ToOnlyDateFormat();
            }
        }

        #endregion

        #endregion

        #region ComboBox_Events

        #region ComboBoxProvinces_SelectionChanged
        private void ComboBoxProvinces_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WpfHelper.FillComboBoxWithIdAndTitle(LocationsWindow.DataTableCities.Copy(), "ProvinceId", e, ComboBoxCities);
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
