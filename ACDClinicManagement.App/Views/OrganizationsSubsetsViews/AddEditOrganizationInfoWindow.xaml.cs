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
using System.Windows.Media;

namespace ACDClinicManagement.App.Views.OrganizationsSubsetsViews
{
    /// <summary>
    /// Interaction logic for AddEditOrganizationInfoWindow.xaml
    /// </summary>
    public partial class AddEditOrganizationInfoWindow
    {
        public AddEditOrganizationInfoWindow()
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
            ComboBoxBanks.Focus();
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
            switch (OrganizationsSubsetsWindow.ChangeOrganizationsInformationMode)
            {
                case CommonEnum.ChangeDatabaseMode.InsertInToDatabase:
                    Title = "افزودن اطلاعات سازمان جدید";
                    ComboBoxPaymentMethodMode.Text = CommonEnum.PaymentMethodMode.None.GetEnumDescription();
                    break;
                case CommonEnum.ChangeDatabaseMode.UpdateDatabase:
                    Title = "ویرایش مشخصات اطلاعات سازمان ";
                    GridServices.IsEnabled = false;
                    var threadLoadData = new Thread(LoadData);
                    threadLoadData.Start();
                    _waitWindowLoadData = new WaitWindow { Owner = this };
                    _waitWindowLoadData.ShowDialog();
                    break;
            }
        }

        #endregion

        #region private int GetSelectedServiceMode()
        private int GetSelectedServiceMode()
        {
            var serviceModeId = 0;
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(GridServices); i++)
            {
                var childVisual = (Visual)VisualTreeHelper.GetChild(GridServices, i);
                if (childVisual is RadioButton && ((RadioButton)childVisual).IsChecked == true)
                    serviceModeId = (int)Enum.Parse(typeof(CommonEnum.ServiceMode), ((RadioButton)childVisual).Tag.ToString());
            }
            return serviceModeId;
        }

        #endregion

        #region private void SetServiceModeRadioButton(int serviceModeId)
        private void SetServiceModeRadioButton(int serviceModeId)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(GridServices); i++)
            {
                var childVisual = (Visual)VisualTreeHelper.GetChild(GridServices, i);
                if (childVisual is RadioButton && (int)Enum.Parse(typeof(CommonEnum.ServiceMode), ((RadioButton)childVisual).Tag.ToString()) == serviceModeId)
                    ((RadioButton)childVisual).IsChecked = true;
            }
        }

        #endregion

        #region private void IsValidate()
        private bool IsValidate()
        {
            var status = true;
            var serviceSelected = false;
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(GridServices); i++)
            {
                var childVisual = (Visual)VisualTreeHelper.GetChild(GridServices, i);
                if (childVisual is RadioButton && ((RadioButton)childVisual).IsChecked == true)
                    serviceSelected = true;
            }
            if (!serviceSelected)
            {
                status = false;
                "نوع سرویس".SelectValidationMessage().ShowMessage();
                GridServices.Focus();
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
            return status;
        }

        #endregion

        #region private void LoadData()

        private void LoadData()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                var commandData = new SqlCommand("SELECT * FROM OrganizationsInformation " +
                                                 "WHERE Id = @Id",
                    MainWindow.PublicConnection);
                commandData.Parameters.AddWithValue("Id", OrganizationsSubsetsWindow.SelectedOrganizationInfoId);
                var dataReaderData = commandData.ExecuteReader();
                while (dataReaderData.Read())
                {
                    Dispatcher.Invoke(() =>
                    {
                        SetServiceModeRadioButton(Convert.ToInt32(dataReaderData["ServiceMode"]));
                        ComboBoxBanks.SelectedValue = dataReaderData["BankId"];
                        TextBoxAccountTitle.Text = dataReaderData["AccountTitle"].ToString();
                        TextBoxAccountNumber.Text = dataReaderData["AccountNumber"].ToString();
                        TextBoxAccountId.Text = dataReaderData["AccountId"].ToString();
                        ComboBoxPaymentMethodMode.Text = ((CommonEnum.PaymentMethodMode)Convert.ToInt16(dataReaderData["PaymentMethodMode"])).GetEnumDescription();
                        TextBoxPosIp.Text = dataReaderData["PosIp"].ToString();
                        TextBoxPosPort.Text = dataReaderData["PosPort"].ToString();
                        TextBoxMessage.Text = dataReaderData["Message"].ToString();
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
                switch (OrganizationsSubsetsWindow.ChangeOrganizationsInformationMode)
                {
                    case CommonEnum.ChangeDatabaseMode.InsertInToDatabase:
                        var commandCheckInfo = new SqlCommand("SELECT COUNT(*) " +
                                                              "FROM OrganizationsInformation " +
                                                              "WHERE ServiceMode = @ServiceMode AND " +
                                                              "OrganizationId = @OrganizationId",
                            MainWindow.PublicConnection);
                        Dispatcher.Invoke(() => commandCheckInfo.Parameters.AddWithValue("@ServiceMode", GetSelectedServiceMode()));
                        commandCheckInfo.Parameters.AddWithValue("@OrganizationId", OrganizationsSubsetsWindow.SelectedOrganizationId);
                        if (Convert.ToInt16(commandCheckInfo.ExecuteScalar()) > 0)
                            Dispatcher.Invoke(() => "سرویس".DuplicateMessage().ShowMessage());
                        else
                        {
                            var maxRecord = Convert.ToInt32(SqlHelper.MaxSqlRecord(MainWindow.PublicConnection, "OrganizationsInformation")) + 1;
                            var data = new object[0];
                            Dispatcher.Invoke(() =>
                            {
                                data = new[]
                                {
                                    maxRecord,
                                    OrganizationsSubsetsWindow.SelectedOrganizationId,
                                    GetSelectedServiceMode(),
                                    ComboBoxBanks.SelectedValue,
                                    TextBoxAccountTitle.Text.Trim().ToCorrectKeYe(),
                                    TextBoxAccountNumber.Text.Trim(),
                                    TextBoxAccountId.Text.Trim(),
                                    Convert.ToInt16((CommonEnum.PaymentMethodMode) ((ComboBoxItem) ComboBoxPaymentMethodMode.SelectedItem).Tag),
                                    TextBoxPosIp.Text.Trim(),
                                    TextBoxPosPort.Text.Trim(),
                                    TextBoxMessage.Text.Trim().ToCorrectKeYe(),
                                    TextBoxComments.Text.Trim().ToCorrectKeYe(),
                                    SpecialBaseHelper.UserId,
                                    DateTime.Now,
                                    SpecialBaseHelper.UserId,
                                    DateTime.Now
                                };
                            });
                            if (MainWindow.PublicConnection.InsertSqlData("OrganizationsInformation", data))
                            {
                                OrganizationsSubsetsWindow.SelectedOrganizationInfoId = maxRecord;
                                _isClose = true;
                                Dispatcher.Invoke(() => "اطلاعات سازمان".AddedMessage().ShowMessage());
                            }
                            else
                                Dispatcher.Invoke(() => WpfHelper.GetErrorMessage().ShowMessage());
                        }
                        break;
                    case CommonEnum.ChangeDatabaseMode.UpdateDatabase:
                        commandCheckInfo = new SqlCommand("SELECT COUNT(*) " +
                                                          "FROM OrganizationsInformation " +
                                                          "WHERE ServiceMode = @ServiceMode AND " +
                                                          "OrganizationId = @OrganizationId AND " +
                                                          "Id <> @Id",
                            MainWindow.PublicConnection);
                        Dispatcher.Invoke(() => commandCheckInfo.Parameters.AddWithValue("@ServiceMode", GetSelectedServiceMode()));
                        commandCheckInfo.Parameters.AddWithValue("@OrganizationId", OrganizationsSubsetsWindow.SelectedOrganizationId);
                        commandCheckInfo.Parameters.AddWithValue("@Id", OrganizationsSubsetsWindow.SelectedOrganizationInfoId);
                        if (Convert.ToInt16(commandCheckInfo.ExecuteScalar()) > 0)
                            Dispatcher.Invoke(() => "سرویس".DuplicateMessage().ShowMessage());
                        else
                        {
                            var commandUpdateData = new SqlCommand
                            {
                                Connection = MainWindow.PublicConnection,
                                CommandText = "UPDATE OrganizationsInformation " +
                                              "SET BankId = @BankId, " +
                                              "AccountTitle = @AccountTitle, " +
                                              "AccountNumber = @AccountNumber, " +
                                              "AccountId = @AccountId, " +
                                              "PaymentMethodMode = @PaymentMethodMode, " +
                                              "PosIp = @PosIp, " +
                                              "PosPort = @PosPort, " +
                                              "Message = @Message, " +
                                              "Comments = @Comments, " +
                                              "ModifiedBy = @ModifiedBy, " +
                                              "ModifiedAt = @ModifiedAt " +
                                              "WHERE Id = @Id"
                            };
                            Dispatcher.Invoke(() =>
                            {
                                commandUpdateData.Parameters.AddWithValue("@BankId", ComboBoxBanks.SelectedValue);
                                commandUpdateData.Parameters.AddWithValue("@AccountTitle", TextBoxAccountTitle.Text.Trim().ToCorrectKeYe());
                                commandUpdateData.Parameters.AddWithValue("@AccountNumber", TextBoxAccountNumber.Text);
                                commandUpdateData.Parameters.AddWithValue("@AccountId", TextBoxAccountId.Text.Trim());
                                commandUpdateData.Parameters.AddWithValue("@PaymentMethodMode", Convert.ToInt16((CommonEnum.PaymentMethodMode)((ComboBoxItem)ComboBoxPaymentMethodMode.SelectedItem).Tag));
                                commandUpdateData.Parameters.AddWithValue("@PosIp", TextBoxPosIp.Text.Trim());
                                commandUpdateData.Parameters.AddWithValue("@PosPort", TextBoxPosPort.Text.Trim());
                                commandUpdateData.Parameters.AddWithValue("@Message", TextBoxMessage.Text.Trim().ToCorrectKeYe());
                                commandUpdateData.Parameters.AddWithValue("@Comments", TextBoxComments.Text.Trim().ToCorrectKeYe());
                                commandUpdateData.Parameters.AddWithValue("@ModifiedBy", SpecialBaseHelper.UserId);
                                commandUpdateData.Parameters.AddWithValue("@ModifiedAt", DateTime.Now);
                                commandUpdateData.Parameters.AddWithValue("@Id", OrganizationsSubsetsWindow.SelectedOrganizationInfoId);
                            });
                            if (commandUpdateData.ExecuteNonQuery() == 1)
                            {
                                _isClose = true;
                                Dispatcher.Invoke(() => "اطلاعات سازمان".UpdatedMessage().ShowMessage());
                            }
                            else
                                Dispatcher.Invoke(() => WpfHelper.GetErrorMessage().ShowMessage());
                        }
                        break;
                }
                if (_isClose)
                {
                    var dataAdapterOrganizationsInformaion = new SqlDataAdapter("SELECT * FROM OrganizationsInformation " +
                                                                                "WHERE Id <> 0" +
                                                                                (SpecialBaseHelper.SubsetId != 0
                                                                                      ? " AND OrganizationId = @OrganizationId"
                                                                                      : "") +
                                                                                " ORDER BY ServiceMode ASC",
                    MainWindow.PublicConnection);
                    dataAdapterOrganizationsInformaion.SelectCommand.Parameters.AddWithValue("@OrganizationId", SpecialBaseHelper.SubsetId);
                    OrganizationsSubsetsWindow.DataTableOrganizationsInformation = new DataTable();
                    dataAdapterOrganizationsInformaion.Fill(OrganizationsSubsetsWindow.DataTableOrganizationsInformation);
                }
            }
            catch (Exception exception)
            {
                Dispatcher.Invoke(() => WpfHelper.GetErrorMessage().ShowMessage());
                MainWindow.ErrorMessage = exception.Message;
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
            WpfHelper.FillComboBoxWithIdAndTitle(GeneralSettingsWindow.DataTableBanks.Copy(), "", null, ComboBoxBanks);
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
