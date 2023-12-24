using ACDClinicManagement.App.Views.CommonViews;
using ACDClinicManagement.App.Views.GeneralSettingsViews;
using ACDClinicManagement.AppHelpers.CommonHelpers;
using ACDClinicManagement.Common.Enums;
using ACDClinicManagement.Common.Helpers;
using ACDClinicManagement.Common.SpecialHelpers;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ACDClinicManagement.App.Views.OrganizationsSubsetsViews
{
    /// <summary>
    /// Interaction logic for AddEditOrganizationWindow.xaml
    /// </summary>
    public partial class AddEditSubsetWindow
    {
        public AddEditSubsetWindow()
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
            TextBlockOrganizationTitle.Text = OrganizationsSubsetsWindow.SelectedOrganizationTitle;
            WpfHelper.FillComboBoxWithIdAndTitle(GeneralSettingsWindow.DataTableBanks.Copy(), "", null, ComboBoxBanks);
            // Fill ComboBoxPayMode by PaymentMethodMode enum
            var paymentMethodModes = Enum.GetValues(typeof(CommonEnum.PaymentMethodMode));
            foreach (var paymentMethodMode in paymentMethodModes)
            {
                var comboBoxItem = new ComboBoxItem
                {
                    Content = ((CommonEnum.PaymentMethodMode)paymentMethodMode).GetEnumDescription(),
                    Tag = (CommonEnum.PaymentMethodMode)paymentMethodMode
                };
                ComboBoxPaymentMethodMode.Items.Add(comboBoxItem);
            }
            TextBoxAccountNumber.IsEnabled = SpecialBaseHelper.CurrentUserType == CommonEnum.UserType.Administrator;
            TextBoxAccountId.IsEnabled = SpecialBaseHelper.CurrentUserType == CommonEnum.UserType.Administrator;
            ComboBoxPaymentMethodMode.IsEnabled = SpecialBaseHelper.CurrentUserType == CommonEnum.UserType.Administrator;
            switch (OrganizationsSubsetsWindow.ChangeSubsetsMode)
            {
                case CommonEnum.ChangeDatabaseMode.InsertInToDatabase:
                    Title = "افزودن زیرمجموعه‌ی جدید";
                    ComboBoxPaymentMethodMode.Text = CommonEnum.PaymentMethodMode.None.GetEnumDescription();
                    break;
                case CommonEnum.ChangeDatabaseMode.UpdateDatabase:
                    Title = "ویرایش مشخصات زیرمجموعه";
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
            else if (ComboBoxBanks.SelectedIndex == 0)
            {
                status = false;
                "بانک".SelectValidationMessage().ShowMessage();
                ComboBoxBanks.Focus();
            }
            else if(string.IsNullOrWhiteSpace(TextBoxAccountTitle.Text))
            {
                status = false;
                "عنوان حساب".InputValidationMessage().ShowMessage();
                TextBoxAccountTitle.Focus();
            }
            else if (!TextBoxAccountNumber.Text.IsValidNumeric())
            {
                status = false;
                "شماره‌ی حساب".NotValidMessage().ShowMessage();
                TextBoxAccountNumber.Focus();
            }
            else if (!TextBoxAccountId.Text.IsValidNumeric())
            {
                status = false;
                "شناسه‌ی حساب".NotValidMessage().ShowMessage();
                TextBoxAccountId.Focus();
            }
            else if (string.IsNullOrWhiteSpace(ComboBoxPaymentMethodMode.Text) || ComboBoxPaymentMethodMode.SelectedIndex == 0)
            {
                status = false;
                "روش پرداخت".SelectValidationMessage().ShowMessage();
                ComboBoxPaymentMethodMode.Focus();
            }
            else if (!TextBoxPosIp.Text.IsValidIpAddress())
            {
                status = false;
                "آی‌پی پایانه".NotValidMessage().ShowMessage();
                TextBoxPosIp.Focus();
            }
            else if (!TextBoxPosPort.Text.IsValidNumeric())
            {
                status = false;
                "پورت پایانه".NotValidMessage().ShowMessage();
                TextBoxPosPort.Focus();
            }
            else if (string.IsNullOrWhiteSpace(TextBoxTelNumber.Text))
            {
                status = false;
                "شماره‌ی تلفن".InputValidationMessage().ShowMessage();
                TextBoxTelNumber.Focus();
            }
            else if (!string.IsNullOrWhiteSpace(TextBoxEmailAddress.Text) && !TextBoxEmailAddress.Text.IsValidEmailAddress())
            {
                status = false;
                "ایمیل".NotValidMessage().ShowMessage();
                TextBoxEmailAddress.Focus();
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
                var commandData = new SqlCommand("SELECT * FROM Subsets " +
                                                 "WHERE Id = @Id",
                    MainWindow.PublicConnection);
                commandData.Parameters.AddWithValue("@Id", OrganizationsSubsetsWindow.SelectedSubsetId);
                var dataReaderData = commandData.ExecuteReader();
                while (dataReaderData.Read())
                {
                    Dispatcher.Invoke(() =>
                    {
                        TextBoxTitle.Text = dataReaderData["Title"].ToString();
                        TextBoxCode.Text = dataReaderData["Code"].ToString();
                        ComboBoxBanks.SelectedValue = dataReaderData["BankId"];
                        TextBoxAccountTitle.Text = dataReaderData["AccountTitle"].ToString();
                        TextBoxAccountNumber.Text = dataReaderData["AccountNumber"].ToString();
                        TextBoxAccountId.Text = dataReaderData["AccountId"].ToString();
                        ComboBoxPaymentMethodMode.Text = ((CommonEnum.PaymentMethodMode)Convert.ToInt16(dataReaderData["PaymentMethodMode"])).GetEnumDescription();
                        TextBoxPosIp.Text = dataReaderData["PosIp"].ToString();
                        TextBoxPosPort.Text = dataReaderData["PosPort"].ToString();
                        TextBoxTelNumber.Text = dataReaderData["TelNumber"].ToString();
                        TextBoxEmailAddress.Text = dataReaderData["EmailAddress"].ToString();
                        TextBoxAddress.Text = dataReaderData["Address"].ToString();
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
                switch (OrganizationsSubsetsWindow.ChangeSubsetsMode)
                {
                    case CommonEnum.ChangeDatabaseMode.InsertInToDatabase:
                        var commandCheckInfo = new SqlCommand("SELECT COUNT(*) " +
                                                              "FROM Subsets " +
                                                              "WHERE OrganizationId = @OrganizationId AND " +
                                                              "Title = @Title",
                            MainWindow.PublicConnection);
                        commandCheckInfo.Parameters.AddWithValue("@OrganizationId", OrganizationsSubsetsWindow.SelectedOrganizationId);
                        Dispatcher.Invoke(() => commandCheckInfo.Parameters.AddWithValue("@Title", TextBoxTitle.Text.Trim().ToCorrectKeYe()));
                        if (Convert.ToInt16(commandCheckInfo.ExecuteScalar()) > 0)
                            Dispatcher.Invoke(() => "زیرمجموعه‌ی".DuplicateMessage().ShowMessage());
                        else
                        {
                            var maxRecord = Convert.ToInt32(SqlHelper.MaxSqlRecord(MainWindow.PublicConnection, "Subsets")) + 1;
                            var data = new object[0];
                            Dispatcher.Invoke(() =>
                            {
                                data = new object[]
                                {
                                    maxRecord,
                                    OrganizationsSubsetsWindow.SelectedProvinceId,
                                    OrganizationsSubsetsWindow.SelectedCityId,
                                    OrganizationsSubsetsWindow.SelectedOrganizationId,
                                    TextBoxTitle.Text.Trim().ToCorrectKeYe(),
                                    TextBoxCode.Text.Trim(),
                                    ComboBoxBanks.SelectedValue,
                                    TextBoxAccountTitle.Text.Trim().ToCorrectKeYe(),
                                    TextBoxAccountNumber.Text.Trim(),
                                    TextBoxAccountId.Text.Trim(),
                                    Convert.ToInt16((CommonEnum.PaymentMethodMode) ((ComboBoxItem) ComboBoxPaymentMethodMode.SelectedItem).Tag),
                                    TextBoxPosIp.Text.Trim(),
                                    TextBoxPosPort.Text.Trim(),
                                    TextBoxTelNumber.Text.Trim(),
                                    TextBoxEmailAddress.Text.Trim(),
                                    TextBoxAddress.Text.Trim().ToCorrectKeYe(),
                                    CheckBoxActiveMode.IsChecked == true ? Convert.ToInt16(CommonEnum.ActiveType.Active) : Convert.ToInt16(CommonEnum.ActiveType.DeActive),
                                    SpecialBaseHelper.UserId,
                                    DateTime.Now,
                                    SpecialBaseHelper.UserId,
                                    DateTime.Now
                                };
                            });
                            if (MainWindow.PublicConnection.InsertSqlData("Subsets", data))
                            {
                                OrganizationsSubsetsWindow.SelectedSubsetId = maxRecord;
                                _isClose = true;
                                Dispatcher.Invoke(() => "زیرمجموعه‌ی".AddedMessage().ShowMessage());
                            }
                            else
                                Dispatcher.Invoke(() => WpfHelper.GetErrorMessage().ShowMessage());
                        }
                        break;
                    case CommonEnum.ChangeDatabaseMode.UpdateDatabase:
                        commandCheckInfo = new SqlCommand("SELECT COUNT(*) " +
                                                          "FROM Subsets " +
                                                          "WHERE OrganizationId = @OrganizationId AND " +
                                                          "Title = @Title AND " +
                                                          "Id <> @Id",
                            MainWindow.PublicConnection);
                        commandCheckInfo.Parameters.AddWithValue("@OrganizationId", OrganizationsSubsetsWindow.SelectedOrganizationId);
                        Dispatcher.Invoke(() => commandCheckInfo.Parameters.AddWithValue("@Title", TextBoxTitle.Text.Trim().ToCorrectKeYe()));
                        commandCheckInfo.Parameters.AddWithValue("@Id", OrganizationsSubsetsWindow.SelectedSubsetId);
                        if (Convert.ToInt16(commandCheckInfo.ExecuteScalar()) > 0)
                            Dispatcher.Invoke(() => "زیرمجموعه‌ی".DuplicateMessage().ShowMessage());
                        else
                        {
                            var commandUpdateData = new SqlCommand
                            {
                                Connection = MainWindow.PublicConnection,
                                CommandText = "UPDATE Subsets " +
                                              "SET Title = @Title, " +
                                              "Code = @Code, " +
                                              "BankId = @BankId, " +
                                              "AccountTitle = @AccountTitle, " +
                                              "AccountNumber = @AccountNumber, " +
                                              "AccountId = @AccountId, " +
                                              "PaymentMethodMode = @PaymentMethodMode, " +
                                              "PosIp = @PosIp, " +
                                              "PosPort = @PosPort, " +
                                              "TelNumber = @TelNumber, " +
                                              "EmailAddress = @EmailAddress, " +
                                              "Address = @Address, " +
                                              "ActiveMode = @ActiveMode, " +
                                              "ModifiedBy = @ModifiedBy, " +
                                              "ModifiedAt = @ModifiedAt " +
                                              "WHERE Id = @Id"
                            };
                            Dispatcher.Invoke(() =>
                            {
                                commandUpdateData.Parameters.AddWithValue("@Title", TextBoxTitle.Text.Trim().ToCorrectKeYe());
                                commandUpdateData.Parameters.AddWithValue("@Code", TextBoxCode.Text.Trim());
                                commandUpdateData.Parameters.AddWithValue("@BankId", ComboBoxBanks.SelectedValue);
                                commandUpdateData.Parameters.AddWithValue("@AccountTitle", TextBoxAccountTitle.Text.Trim().ToCorrectKeYe());
                                commandUpdateData.Parameters.AddWithValue("@AccountNumber", TextBoxAccountNumber.Text.Trim());
                                commandUpdateData.Parameters.AddWithValue("@AccountId", TextBoxAccountId.Text.Trim());
                                commandUpdateData.Parameters.AddWithValue("@PaymentMethodMode", Convert.ToInt16((CommonEnum.PaymentMethodMode)((ComboBoxItem)ComboBoxPaymentMethodMode.SelectedItem).Tag));
                                commandUpdateData.Parameters.AddWithValue("@PosIp", TextBoxPosIp.Text.Trim());
                                commandUpdateData.Parameters.AddWithValue("@PosPort", TextBoxPosPort.Text.Trim());
                                commandUpdateData.Parameters.AddWithValue("@TelNumber", TextBoxTelNumber.Text.Trim());
                                commandUpdateData.Parameters.AddWithValue("@EmailAddress", TextBoxEmailAddress.Text.Trim());
                                commandUpdateData.Parameters.AddWithValue("@Address", TextBoxAddress.Text.Trim().ToCorrectKeYe());
                                commandUpdateData.Parameters.AddWithValue("@ActiveMode", CheckBoxActiveMode.IsChecked == true ? Convert.ToInt16(CommonEnum.ActiveType.Active) : Convert.ToInt16(CommonEnum.ActiveType.DeActive));
                                commandUpdateData.Parameters.AddWithValue("@ModifiedBy", SpecialBaseHelper.UserId);
                                commandUpdateData.Parameters.AddWithValue("@ModifiedAt", DateTime.Now);
                                commandUpdateData.Parameters.AddWithValue("@Id", OrganizationsSubsetsWindow.SelectedSubsetId);
                            });
                            if (commandUpdateData.ExecuteNonQuery() == 1)
                            {
                                _isClose = true;
                                Dispatcher.Invoke(() => "زیرمجموعه‌ی".UpdatedMessage().ShowMessage());
                            }
                            else
                                Dispatcher.Invoke(() => WpfHelper.GetErrorMessage().ShowMessage());
                        }
                        break;
                }
                if (_isClose)
                {
                    var dataAdapterSubsets = new SqlDataAdapter("SELECT * FROM Subsets " +
                                                                "WHERE Id <> 0" +
                                                                (SpecialBaseHelper.SubsetId != 0
                                                                      ? " AND ActiveMode = @ActiveMode AND Id = @Id"
                                                                      : "") +
                                                                " ORDER BY Title ASC",
                    MainWindow.PublicConnection);
                    dataAdapterSubsets.SelectCommand.Parameters.AddWithValue("@ActiveMode", Convert.ToInt16(CommonEnum.ActiveType.Active));
                    dataAdapterSubsets.SelectCommand.Parameters.AddWithValue("@Id", SpecialBaseHelper.SubsetId);
                    OrganizationsSubsetsWindow.DataTableSubsets = new DataTable();
                    dataAdapterSubsets.Fill(OrganizationsSubsetsWindow.DataTableSubsets);
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

        #region ButtonBanksManagement_Click
        private void ButtonBanksManagement_Click(object sender, RoutedEventArgs e)
        {
            var banksWindow = new GeneralSettingsWindow { Owner = this };
            banksWindow.ShowDialog();
            ComboBoxBanks.ItemsSource = GeneralSettingsWindow.DataTableBanks.DefaultView;
            ComboBoxBanks.SelectedValuePath = GeneralSettingsWindow.DataTableBanks.Columns["Id"].ToString();
            ComboBoxBanks.DisplayMemberPath = GeneralSettingsWindow.DataTableBanks.Columns["Title"].ToString();
            ComboBoxBanks.SelectedIndex = 0;
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
