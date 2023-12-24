using ACDClinicManagement.App.Views.CommonViews;
using ACDClinicManagement.App.Views.GeneralSettingsViews;
using ACDClinicManagement.App.Views.LocationsViews;
using ACDClinicManagement.AppHelpers.CommonHelpers;
using ACDClinicManagement.Common.Enums;
using ACDClinicManagement.Common.Helpers;
using ACDClinicManagement.Common.SpecialHelpers;
using ACDClinicManagement.Helpers;
using MhclassLib;
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
    /// Interaction logic for OrganizationsSubsetsWindow.xaml
    /// </summary>
    public partial class OrganizationsSubsetsWindow
    {
        public OrganizationsSubsetsWindow()
        {
            InitializeComponent();
            InitializeObjects();
        }

        // ••••••••••••
        // DEFINATIONS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region Windows

        private WaitWindow _waitWindowDeleteOrganizationInfo, _waitWindowDeleteSubset;

        #endregion

        #region Classes


        #endregion

        #region Objects

        public static CommonEnum.ChangeDatabaseMode ChangeOrganizationsInformationMode { get; private set; }
        public static CommonEnum.ChangeDatabaseMode ChangeSubsetsMode { get; private set; }

        public static DataTable DataTableOrganizationsInformation, DataTableSubsets;
        private DataTable _dataTableProvinces, _dataTableCities, _dataTableOrganizations, _dataTableOrganizationsInformation, _dataTableSubsets;

        #endregion

        #region Variables

        public static int SelectedProvinceId, SelectedCityId, SelectedOrganizationId, SelectedOrganizationInfoId, SelectedSubsetId;
        public static string SelectedProvinceTitle, SelectedCityTitle, SelectedOrganizationTitle;
        private static string _searchTextProvince, _searchTextCity, _searchTextOrganization, _searchTextOrganizationInfo, _searchTextSubset;

        #endregion

        // ••••••••••••
        // METHODS     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region private void InitializeObjects()

        private void InitializeObjects()
        {
            _dataTableProvinces = new DataTable();
            _dataTableProvinces.Columns.Add("Id", typeof(int));
            _dataTableProvinces.Columns.Add("عنوان", typeof(string));
            _dataTableProvinces.Columns.Add("وضعیت", typeof(string));
            _dataTableProvinces.Columns.Add("تاریخ و زمان ایجاد", typeof(string));
            _dataTableProvinces.Columns.Add("تاریخ و زمان ویرایش", typeof(string));

            _dataTableCities = new DataTable();
            _dataTableCities.Columns.Add("Id", typeof(int));
            _dataTableCities.Columns.Add("عنوان", typeof(string));
            _dataTableCities.Columns.Add("وضعیت", typeof(string));
            _dataTableCities.Columns.Add("تاریخ و زمان ایجاد", typeof(string));
            _dataTableCities.Columns.Add("تاریخ و زمان ویرایش", typeof(string));

            _dataTableOrganizations = new DataTable();
            _dataTableOrganizations.Columns.Add("Id", typeof(int));
            _dataTableOrganizations.Columns.Add("عنوان", typeof(string));
            _dataTableOrganizations.Columns.Add("شناسه‌ی ملی", typeof(string));
            _dataTableOrganizations.Columns.Add("نام و نام خانوادگی ریاست", typeof(string));
            _dataTableOrganizations.Columns.Add("شماره‌ی تلفن", typeof(string));
            _dataTableOrganizations.Columns.Add("آدرس", typeof(string));
            _dataTableOrganizations.Columns.Add("وضعیت", typeof(string));
            _dataTableOrganizations.Columns.Add("تاریخ و زمان ایجاد", typeof(string));
            _dataTableOrganizations.Columns.Add("تاریخ و زمان ویرایش", typeof(string));

            _dataTableOrganizationsInformation = new DataTable();
            _dataTableOrganizationsInformation.Columns.Add("Id", typeof(int));
            _dataTableOrganizationsInformation.Columns.Add("سرویس", typeof(string));
            _dataTableOrganizationsInformation.Columns.Add("بانک عامل", typeof(string));
            _dataTableOrganizationsInformation.Columns.Add("عنوان حساب", typeof(string));
            _dataTableOrganizationsInformation.Columns.Add("شماره‌ی حساب", typeof(string));
            _dataTableOrganizationsInformation.Columns.Add("شناسه‌ی حساب", typeof(string));
            _dataTableOrganizationsInformation.Columns.Add("حالت پرداخت", typeof(string));
            _dataTableOrganizationsInformation.Columns.Add("پیام", typeof(string));
            _dataTableOrganizationsInformation.Columns.Add("تاریخ و زمان ایجاد", typeof(string));
            _dataTableOrganizationsInformation.Columns.Add("تاریخ و زمان ویرایش", typeof(string));

            _dataTableSubsets = new DataTable();
            _dataTableSubsets.Columns.Add("Id", typeof(int));
            _dataTableSubsets.Columns.Add("عنوان", typeof(string));
            _dataTableSubsets.Columns.Add("بانک", typeof(string));
            _dataTableSubsets.Columns.Add("عنوان حساب", typeof(string));
            _dataTableSubsets.Columns.Add("شماره‌ی حساب", typeof(string));
            _dataTableSubsets.Columns.Add("شناسه‌ی حساب", typeof(string));
            _dataTableSubsets.Columns.Add("شماره‌ی تلفن", typeof(string));
            _dataTableSubsets.Columns.Add("آدرس", typeof(string));
            _dataTableSubsets.Columns.Add("وضعیت", typeof(string));
            _dataTableSubsets.Columns.Add("تاریخ و زمان ایجاد", typeof(string));
            _dataTableSubsets.Columns.Add("تاریخ و زمان ویرایش", typeof(string));
        }

        #endregion

        #region private void LoadDefaults()

        private void LoadDefaults()
        {
            this.ShowWindow(CommonEnum.WindowStyleMode.Normal);

            _searchTextProvince = "";
            _searchTextCity = "";
            _searchTextOrganization = "";
            _searchTextOrganizationInfo = "";
            _searchTextSubset = "";

            DataGridProvinces.IsEnabled = SpecialBaseHelper.ProvinceId == 0;
            DataGridCities.IsEnabled = SpecialBaseHelper.CityId == 0;
            DataGridOrganizationInformation.IsEnabled = SpecialBaseHelper.SubsetId == 0;
            DataGridSubsets.IsEnabled = SpecialBaseHelper.SubsetId == 0;

            ButtonNewOrganizationInfo.IsEnabled = SpecialBaseHelper.SubsetId == 0;
            ButtonEditOrganizationInfo.IsEnabled = SpecialBaseHelper.SubsetId == 0;
            ButtonDeleteOrganizationInfo.IsEnabled = SpecialBaseHelper.SubsetId == 0;
            ButtonNewSubset.IsEnabled = SpecialBaseHelper.SubsetId == 0;
            ButtonEditSubset.IsEnabled = SpecialBaseHelper.SubsetId == 0;
            ButtonDeleteSubset.IsEnabled = SpecialBaseHelper.SubsetId == 0;

            TextBoxSearchProvince.IsEnabled = SpecialBaseHelper.ProvinceId == 0;
            TextBoxSearchCity.IsEnabled = SpecialBaseHelper.CityId == 0;
            TextBoxSearchOrganizationInfo.IsEnabled = SpecialBaseHelper.SubsetId == 0;
            TextBoxSearchSubset.IsEnabled = SpecialBaseHelper.SubsetId == 0;

            ShowProvinces(LocationsWindow.DataTableProvinces.Copy(), _searchTextProvince, DataGridProvinces);
            WpfHelper.SetDataGridSelectedValue(DataGridProvinces, SpecialBaseHelper.ProvinceId);
            WpfHelper.SetDataGridSelectedValue(DataGridCities, SpecialBaseHelper.CityId);
            WpfHelper.SetDataGridSelectedValue(DataGridOrganizations, SpecialBaseHelper.OrganizationId);
        }

        #endregion

        #region public static bool LoadOrganizationsInformation()
        public static bool LoadOrganizationsInformation()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                var dataAdapterOrganizationsInformaion = new SqlDataAdapter("SELECT * FROM OrganizationsInformation " +
                                                                            "WHERE Id <> 0" +
                                                                            (SpecialBaseHelper.SubsetId != 0
                                                                                  ? " AND OrganizationId = @OrganizationId"
                                                                                  : "") +
                                                                            " ORDER BY ServiceMode ASC",
                    MainWindow.PublicConnection);
                dataAdapterOrganizationsInformaion.SelectCommand.Parameters.AddWithValue("@OrganizationId", SpecialBaseHelper.SubsetId);
                DataTableOrganizationsInformation = new DataTable();
                dataAdapterOrganizationsInformaion.Fill(DataTableOrganizationsInformation);
                return true;
            }
            catch (Exception exception)
            {
                MainWindow.ErrorMessage = exception.Message;
                return false;
            }
        }

        #endregion

        #region private void DeleteOrganizationInfo()
        private void DeleteOrganizationInfo()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                if (MainWindow.PublicConnection.DeleteSqlData("OrganizationsInformation", "Id = " + SelectedOrganizationInfoId))
                {
                    var dataAdapterOrganizationsInformation = new SqlDataAdapter("SELECT * FROM OrganizationsInformation " +
                                                                                 "ORDER BY ServiceMode ASC",
                    MainWindow.PublicConnection);
                    DataTableOrganizationsInformation = new DataTable();
                    dataAdapterOrganizationsInformation.Fill(DataTableOrganizationsInformation);
                    Dispatcher.Invoke(() => "اطلاعات سازمان".DeletedMessage().ShowMessage());
                }
                else
                    Dispatcher.Invoke(() => WpfHelper.GetErrorMessage().ShowMessage());
            }
            catch (Exception exception)
            {
                MainWindow.ErrorMessage = exception.Message;
            }
            finally
            {
                Dispatcher.Invoke(() => _waitWindowDeleteOrganizationInfo.Close());
            }
        }
        #endregion

        #region public static bool LoadSubsets()
        public static bool LoadSubsets()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                var dataAdapterSubsets = new SqlDataAdapter("SELECT * FROM Subsets " +
                                                            "WHERE Id <> 0" +
                                                            (SpecialBaseHelper.SubsetId != 0
                                                                  ? " AND ActiveMode = @ActiveMode AND Id = @Id"
                                                                  : "") +
                                                            " ORDER BY Title ASC",
                    MainWindow.PublicConnection);
                dataAdapterSubsets.SelectCommand.Parameters.AddWithValue("@ActiveMode", Convert.ToInt16(CommonEnum.ActiveType.Active));
                dataAdapterSubsets.SelectCommand.Parameters.AddWithValue("@Id", SpecialBaseHelper.SubsetId);
                DataTableSubsets = new DataTable();
                dataAdapterSubsets.Fill(DataTableSubsets);
                return true;
            }
            catch (Exception exception)
            {
                MainWindow.ErrorMessage = exception.Message;
                return false;
            }
        }

        #endregion

        #region private void DeleteSubset()

        private void DeleteSubset()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                if (MainWindow.PublicConnection.DeleteSqlData("Subsets", "Id = " + SelectedSubsetId))
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
                    DataTableSubsets = new DataTable();
                    dataAdapterSubsets.Fill(DataTableSubsets);
                    Dispatcher.Invoke(() => "زیرمجموعه".DeletedMessage().ShowMessage());
                }
                else
                    Dispatcher.Invoke(() => WpfHelper.GetErrorMessage().ShowMessage());
            }
            catch (Exception exception)
            {
                MainWindow.ErrorMessage = exception.Message;
            }
            finally
            {
                Dispatcher.Invoke(() => _waitWindowDeleteSubset.Close());
            }
        }

        #endregion

        #region private void ShowProvinces(DataTable dataTable, string searchTerm, DataGrid dataGrid)
        private void ShowProvinces(DataTable dataTable, string searchTerm, DataGrid dataGrid)
        {
            _dataTableProvinces.Clear();
            foreach (DataRow data in dataTable.Rows)
                if (data["Title"].ToString().ToCorrectKeYe().ToLower().Contains(searchTerm))
                    _dataTableProvinces.Rows.Add(data["Id"],
                        data["Title"].ToString().ToFarsiFormat(),
                        ((CommonEnum.ActiveType)Convert.ToInt16(data["ActiveMode"])).GetEnumDescription(),
                        data["CreatedAt"].ToFarsiFormatDateTimeFromSql(),
                        data["ModifiedAt"].ToFarsiFormatDateTimeFromSql());
            dataGrid.ItemsSource = _dataTableProvinces.Copy().DefaultView;
        }

        #endregion

        #region private void ShowCities(DataTable dataTable, string searchTerm, DataGrid dataGrid)
        private void ShowCities(DataTable dataTable, string searchTerm, DataGrid dataGrid)
        {
            _dataTableCities.Clear();
            foreach (DataRow data in dataTable.Rows)
                if (SelectedProvinceId == Convert.ToInt32(data["ProvinceId"]) && data["Title"].ToString().ToCorrectKeYe().ToLower().Contains(searchTerm.ToLower()))
                    _dataTableCities.Rows.Add(data["Id"],
                        data["Title"].ToString().ToFarsiFormat(),
                        ((CommonEnum.ActiveType)Convert.ToInt16(data["ActiveMode"])).GetEnumDescription(),
                        data["CreatedAt"].ToFarsiFormatDateTimeFromSql(),
                        data["ModifiedAt"].ToFarsiFormatDateTimeFromSql());
            dataGrid.ItemsSource = _dataTableCities.Copy().DefaultView;
        }

        #endregion

        #region private void ShowOrganizations(DataTable dataTable, string searchTerm, DataGrid dataGrid)
        private void ShowOrganizations(DataTable dataTable, string searchTerm, DataGrid dataGrid)
        {
            _dataTableOrganizations.Clear();
            foreach (DataRow data in dataTable.Rows)
                if (SelectedCityId == Convert.ToInt32(data["CityId"]) && data["Title"].ToString().ToCorrectKeYe().ToLower().Contains(searchTerm.ToLower()))
                    _dataTableOrganizations.Rows.Add(data["Id"],
                        data["Title"].ToString().ToFarsiFormat(),
                        $"{data["BossFirstName"]} {data["BossLastName"]}",
                        data["TelNumber"].ToString().ToFarsiFormat(),
                        data["Address"].ToString().ToFarsiFormat(),
                        ((CommonEnum.ActiveType)Convert.ToInt16(data["ActiveMode"])).GetEnumDescription(),
                        data["CreatedAt"].ToFarsiFormatDateTimeFromSql(),
                        data["ModifiedAt"].ToFarsiFormatDateTimeFromSql());
            dataGrid.ItemsSource = _dataTableOrganizations.Copy().DefaultView;
        }

        #endregion

        #region private void ShowOrganizationInformation(DataTable dataTable, string searchTerm, DataGrid dataGrid)
        private void ShowOrganizationInformation(DataTable dataTable, string searchTerm, DataGrid dataGrid)
        {
            _dataTableOrganizationsInformation.Clear();
            foreach (DataRow data in dataTable.Rows)
                if (SelectedOrganizationId == Convert.ToInt32(data["OrganizationId"]))
                {
                    _dataTableOrganizationsInformation.Rows.Add(data["Id"],
                        ((CommonEnum.ServiceMode)Convert.ToInt16(data["ServiceMode"])).GetEnumDescription(),
                        GeneralSettingsWindow.GetBankTitle(data["BankId"]),
                        data["AccountTitle"].ToString().ToFarsiFormat(),
                        data["AccountNumber"].ToString().ToFarsiFormat(),
                        data["AccountId"].ToString().ToFarsiFormat(),
                        ((CommonEnum.PaymentMethodMode)Convert.ToInt16(data["PaymentMethodMode"])).GetEnumDescription(),
                        data["Message"].ToString().ToFarsiFormat(),
                        data["CreatedAt"].ToFarsiFormatDateTimeFromSql(),
                        data["ModifiedAt"].ToFarsiFormatDateTimeFromSql());
                }
            dataGrid.ItemsSource = _dataTableOrganizationsInformation.Copy().DefaultView;
        }
        #endregion

        #region private void ShowSubsets(DataTable dataTable, string searchTerm, DataGrid dataGrid)
        private void ShowSubsets(DataTable dataTable, string searchTerm, DataGrid dataGrid)
        {
            _dataTableSubsets.Clear();
            foreach (DataRow data in dataTable.Rows)
                if (SelectedOrganizationId == Convert.ToInt32(data["OrganizationId"]) && data["Title"].ToString().ToCorrectKeYe().ToLower().Contains(searchTerm.ToLower()))
                    _dataTableSubsets.Rows.Add(data["Id"],
                        data["Title"].ToString().ToFarsiFormat(),
                        GeneralSettingsWindow.GetBankTitle(data["BankId"]),
                        data["AccountTitle"].ToString().ToFarsiFormat(),
                        data["AccountNumber"].ToString().ToFarsiFormat(),
                        data["AccountId"].ToString().ToFarsiFormat(),
                        data["TelNumber"].ToString().ToFarsiFormat(),
                        data["Address"].ToString().ToFarsiFormat(),
                        ((CommonEnum.ActiveType)Convert.ToInt16(data["ActiveMode"])).GetEnumDescription(),
                        data["CreatedAt"].ToFarsiFormatDateTimeFromSql(),
                        data["ModifiedAt"].ToFarsiFormatDateTimeFromSql());
            dataGrid.ItemsSource = _dataTableSubsets.Copy().DefaultView;
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

        #region DataGrid_Events

        #region DataGrid_AutoGeneratingColumn

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString() == "Id")
                e.Column.Visibility = Visibility.Hidden;
        }

        #endregion

        #region DataGridProvinces_SelectionChanged

        private void DataGridProvinces_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBoxSearchCity.Text = "";
            TextBoxSearchOrganization.Text = "";
            TextBoxSearchSubset.Text = "";
            DataGridCities.ItemsSource = null;
            DataGridOrganizations.ItemsSource = null;
            DataGridOrganizationInformation.ItemsSource = null;
            DataGridSubsets.ItemsSource = null;
            if (DataGridProvinces.SelectedItem == null || DataGridProvinces.SelectedItem.ToString().Contains("NewItemPlaceholder")) return;
            var row = (DataRowView)DataGridProvinces.SelectedItem;
            SelectedProvinceId = Convert.ToInt32(row["Id"]);
            SelectedProvinceTitle = row["عنوان"].ToString();
            ShowCities(LocationsWindow.DataTableCities.Copy(), _searchTextCity, DataGridCities);
        }

        #endregion

        #region DataGridCities_SelectionChanged

        private void DataGridCities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBoxSearchOrganization.Text = "";
            TextBoxSearchSubset.Text = "";
            DataGridOrganizations.ItemsSource = null;
            DataGridOrganizationInformation.ItemsSource = null;
            DataGridSubsets.ItemsSource = null;
            if (DataGridCities.SelectedItem == null || DataGridCities.SelectedItem.ToString().Contains("NewItemPlaceholder")) return;
            var row = (DataRowView)DataGridCities.SelectedItem;
            SelectedCityId = Convert.ToInt32(row["Id"]);
            SelectedCityTitle = row["عنوان"].ToString();
            ShowOrganizations(LocationsWindow.DataTableOrganizations.Copy(), _searchTextOrganization, DataGridOrganizations);
        }

        #endregion

        #region DataGridOrganizations_SelectionChanged

        private void DataGridOrganizations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBoxSearchSubset.Text = "";
            DataGridOrganizationInformation.ItemsSource = null;
            DataGridSubsets.ItemsSource = null;
            if (DataGridOrganizations.SelectedItem == null || DataGridOrganizations.SelectedItem.ToString().Contains("NewItemPlaceholder")) return;
            var row = (DataRowView)DataGridOrganizations.SelectedItem;
            SelectedOrganizationId = Convert.ToInt32(row["Id"]);
            ShowOrganizationInformation(DataTableOrganizationsInformation.Copy(), "", DataGridOrganizationInformation);
            ShowSubsets(DataTableSubsets.Copy(), _searchTextSubset, DataGridSubsets);
        }

        #endregion

        #region DataGridOrganizationInformation_SelectionChanged

        private void DataGridOrganizationInformation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridOrganizationInformation.SelectedItem == null || DataGridOrganizationInformation.SelectedItem.ToString().Contains("NewItemPlaceholder")) return;
            var row = (DataRowView)DataGridOrganizationInformation.SelectedItem;
            SelectedOrganizationInfoId = Convert.ToInt32(row["Id"]);
        }

        #endregion

        #region DataGridSubsets_SelectionChanged

        private void DataGridSubsets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridSubsets.SelectedItem == null || DataGridSubsets.SelectedItem.ToString().Contains("NewItemPlaceholder")) return;
            var row = (DataRowView)DataGridSubsets.SelectedItem;
            SelectedSubsetId = Convert.ToInt32(row["Id"]);
        }

        #endregion

        #endregion

        #region TextBox_Events

        #region TextBoxSearchProvince_TextChanged

        private void TextBoxSearchProvince_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchTextProvince = TextBoxSearchProvince.Text.Trim().ToCorrectKeYe();
            ShowProvinces(LocationsWindow.DataTableProvinces.Copy(), _searchTextProvince, DataGridProvinces);
        }

        #endregion

        #region TextBoxSearchCity_TextChanged

        private void TextBoxSearchCity_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchTextCity = TextBoxSearchCity.Text.Trim().ToCorrectKeYe();
            ShowCities(LocationsWindow.DataTableCities.Copy(), _searchTextCity, DataGridCities);
        }

        #endregion

        #region TextBoxSearchOrganization_TextChanged

        private void TextBoxSearchOrganization_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchTextOrganization = TextBoxSearchOrganization.Text.Trim().ToCorrectKeYe();
            ShowOrganizations(LocationsWindow.DataTableOrganizations.Copy(), _searchTextOrganization, DataGridOrganizations);
        }

        #endregion

        #region TextBoxSearchOrganizationInfo_TextChanged

        private void TextBoxSearchOrganizationInfo_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchTextOrganizationInfo = TextBoxSearchOrganizationInfo.Text.Trim().ToCorrectKeYe();
            ShowOrganizationInformation(DataTableOrganizationsInformation.Copy(), "", DataGridOrganizationInformation);
        }

        #endregion

        #region TextBoxSearchSubset_TextChanged

        private void TextBoxSearchSubset_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchTextSubset = TextBoxSearchSubset.Text.Trim().ToCorrectKeYe();
            ShowSubsets(DataTableSubsets.Copy(), _searchTextSubset, DataGridSubsets);
        }

        #endregion

        #endregion

        #region Button_Events

        #region ButtonNewOrganizationInfo_Click

        private void ButtonNewOrganizationInfo_Click(object sender, RoutedEventArgs e)
        {
            if (!Convert.ToBoolean(WpfHelper.GetSelectedDataGridRowInfo(DataGridOrganizations, ref SelectedOrganizationId, ref SelectedOrganizationTitle, "سازمانی"))) return;
            ChangeOrganizationsInformationMode = CommonEnum.ChangeDatabaseMode.InsertInToDatabase;
            var addEditOrganizationInfoWindow = new AddEditOrganizationInfoWindow { Owner = this };
            addEditOrganizationInfoWindow.ShowDialog();
            ShowOrganizationInformation(DataTableOrganizationsInformation.Copy(), "", DataGridOrganizationInformation);
            DataGridOrganizationInformation.SelectDataGridRow(SelectedOrganizationInfoId);
        }

        #endregion

        #region ButtonEditOrganizationInfo_Click

        private void ButtonEditOrganizationInfo_Click(object sender, RoutedEventArgs e)
        {
            if (!Convert.ToBoolean(WpfHelper.GetSelectedDataGridRowInfo(DataGridOrganizationInformation, ref SelectedOrganizationInfoId, "جزئیات سازمانی"))) return;
            ChangeOrganizationsInformationMode = CommonEnum.ChangeDatabaseMode.UpdateDatabase;
            var addEditOrganizationInfoWindow = new AddEditOrganizationInfoWindow { Owner = this };
            addEditOrganizationInfoWindow.ShowDialog();
            ShowOrganizationInformation(DataTableOrganizationsInformation.Copy(), "", DataGridOrganizationInformation);
            DataGridOrganizationInformation.SelectDataGridRow(SelectedOrganizationInfoId);
        }

        #endregion

        #region ButtonDeleteOrganizationInfo_Click

        private void ButtonDeleteOrganizationInfo_Click(object sender, RoutedEventArgs e)
        {
            if (!Convert.ToBoolean(WpfHelper.GetSelectedDataGridRowInfo(DataGridOrganizationInformation, ref SelectedOrganizationInfoId, "جزئیات سازمانی"))) return;
            var dialogResult = PersianMessageBox_Mhclass.Show("آیا از حذف جزئیات سازمان انتخاب‌شده اطمینان دارید؟",
                "حذف جزئیات سازمان", PersianMessageBox_Mhclass.MsgMhButton.YesNo, PersianMessageBox_Mhclass.MsgMhIcon.Question, PersianMessageBox_Mhclass.DefaultSelectedButton.No);
            if (dialogResult == MessageBoxResult.No) return;
            var threadDeleteOrganizationInfo = new Thread(DeleteOrganizationInfo);
            threadDeleteOrganizationInfo.Start();
            _waitWindowDeleteOrganizationInfo = new WaitWindow { Owner = this };
            _waitWindowDeleteOrganizationInfo.ShowDialog();
            ShowOrganizationInformation(DataTableOrganizationsInformation.Copy(), "", DataGridOrganizationInformation);
        }

        #endregion

        #region ButtonNewSubset_Click

        private void ButtonNewSubset_Click(object sender, RoutedEventArgs e)
        {
            if (!Convert.ToBoolean(WpfHelper.GetSelectedDataGridRowInfo(DataGridOrganizations, ref SelectedOrganizationId, ref SelectedOrganizationTitle, "سازمانی"))) return;
            ChangeSubsetsMode = CommonEnum.ChangeDatabaseMode.InsertInToDatabase;
            var addEditSubsetWindow = new AddEditSubsetWindow { Owner = this };
            addEditSubsetWindow.ShowDialog();
            ShowSubsets(DataTableSubsets.Copy(), _searchTextSubset, DataGridSubsets);
            DataGridSubsets.SelectDataGridRow(SelectedSubsetId);
        }

        #endregion

        #region ButtonEditSubset_Click

        private void ButtonEditSubset_Click(object sender, RoutedEventArgs e)
        {
            if (!Convert.ToBoolean(WpfHelper.GetSelectedDataGridRowInfo(DataGridSubsets, ref SelectedSubsetId, "زیرمجموعه‌ای"))) return;
            ChangeSubsetsMode = CommonEnum.ChangeDatabaseMode.UpdateDatabase;
            var addEditSubsetWindow = new AddEditSubsetWindow { Owner = this };
            addEditSubsetWindow.ShowDialog();
            ShowSubsets(DataTableSubsets.Copy(), _searchTextSubset, DataGridSubsets);
            DataGridSubsets.SelectDataGridRow(SelectedSubsetId);
        }

        #endregion

        #region ButtonDeleteSubset_Click

        private void ButtonDeleteSubset_Click(object sender, RoutedEventArgs e)
        {
            if (!Convert.ToBoolean(WpfHelper.GetSelectedDataGridRowInfo(DataGridSubsets, ref SelectedSubsetId, "زیرمجموعه‌ای"))) return;
            var dialogResult = PersianMessageBox_Mhclass.Show("آیا از حذف زیرمجموعه‌ی انتخاب‌شده اطمینان دارید؟",
                "حذف زیرمجموعه", PersianMessageBox_Mhclass.MsgMhButton.YesNo, PersianMessageBox_Mhclass.MsgMhIcon.Question);
            if (dialogResult == MessageBoxResult.No) return;
            var threadDeleteSubset = new Thread(DeleteSubset);
            threadDeleteSubset.Start();
            _waitWindowDeleteSubset = new WaitWindow { Owner = this };
            _waitWindowDeleteSubset.ShowDialog();
            ShowSubsets(DataTableSubsets.Copy(), _searchTextSubset, DataGridSubsets);
        }

        #endregion

        #endregion
    }
}
