using ACDClinicManagement.App.Views.CommonViews;
using ACDClinicManagement.AppHelpers.CommonHelpers;
using ACDClinicManagement.Common.Enums;
using ACDClinicManagement.Common.Helpers;
using ACDClinicManagement.Common.SpecialHelpers;
using ACDClinicManagement.Helpers;
using MhclassLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ACDClinicManagement.App.Views.LocationsViews
{
    /// <summary>
    /// Interaction logic for LocationsWindow.xaml
    /// </summary>
    public partial class LocationsWindow : Window
    {
        public LocationsWindow()
        {
            InitializeComponent();
            InitializeObjects();
        }

        // ••••••••••••
        // DEFINATIONS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region Windows

        private WaitWindow _waitWindowDeleteProvince, _waitWindowDeleteCity, _waitWindowDeleteOrganization;

        #endregion

        #region Classes


        #endregion

        #region Objects

        public static CommonEnum.ChangeDatabaseMode ChangeProvincesMode { get; private set; }
        public static CommonEnum.ChangeDatabaseMode ChangeCitiesMode { get; private set; }
        public static CommonEnum.ChangeDatabaseMode ChangeOrganizationsMode { get; private set; }
        public static CommonEnum.ChangeDatabaseMode ChangeZonesMode { get; private set; }
        public static CommonEnum.ChangeDatabaseMode ChangeAreasMode { get; private set; }
        public static CommonEnum.ChangeDatabaseMode ChangeBlocksMode { get; private set; }
        public static CommonEnum.ChangeDatabaseMode ChangePiecesMode { get; private set; }
        public static CommonEnum.ChangeDatabaseMode ChangeBuildingsMode { get; private set; }
        public static CommonEnum.ChangeDatabaseMode ChangeApartmentsMode { get; private set; }

        public static DataTable DataTableProvinces, DataTableCities, DataTableOrganizations, DataTableZones,
            DataTableAreas, DataTableBlocks, DataTablePieces, DataTableBuildings, DataTableApartments;
        private DataTable _dataTableProvinces, _dataTableCities, _dataTableOrganizations;

        #endregion

        #region Variables

        public static int SelectedProvinceId, SelectedCityId, SelectedOrganizationId, SelectedZoneId, SelectedAreaId, SelectedBlockId, SelectedPieceId, SelectedBuildingId, SelectedApartmentId;
        public static string SelectedProvinceTitle, SelectedCityTitle, SelectedOrganizationTitle, SelectedZoneTitle, SelectedAreaTitle, SelectedBlockTitle, SelectedPieceTitle, SelectedBuildingTitle, SelectedApartmentTitle;
        private static string _searchTextProvince, _searchTextCity, _searchTextOrganization;

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
            _dataTableOrganizations.Columns.Add("نام و نام خانوادگی ریاست", typeof(string));
            _dataTableOrganizations.Columns.Add("شماره‌ی تلفن", typeof(string));
            _dataTableOrganizations.Columns.Add("آدرس", typeof(string));
            _dataTableOrganizations.Columns.Add("وضعیت", typeof(string));
            _dataTableOrganizations.Columns.Add("تاریخ و زمان ایجاد", typeof(string));
            _dataTableOrganizations.Columns.Add("تاریخ و زمان ویرایش", typeof(string));
        }

        #endregion

        #region private void LoadDefaults()

        private void LoadDefaults()
        {
            this.ShowWindow(CommonEnum.WindowStyleMode.Normal);

            _searchTextProvince = "";
            _searchTextCity = "";
            _searchTextOrganization = "";

            DataGridProvinces.IsEnabled = SpecialBaseHelper.ProvinceId == 0;
            DataGridCities.IsEnabled = SpecialBaseHelper.CityId == 0;
            DataGridOrganizations.IsEnabled = SpecialBaseHelper.OrganizationId == 0;

            ButtonNewProvince.IsEnabled = SpecialBaseHelper.ProvinceId == 0;
            ButtonEditProvince.IsEnabled = SpecialBaseHelper.ProvinceId == 0;
            ButtonDeleteProvince.IsEnabled = SpecialBaseHelper.ProvinceId == 0;
            ButtonNewCity.IsEnabled = SpecialBaseHelper.CityId == 0;
            ButtonEditCity.IsEnabled = SpecialBaseHelper.CityId == 0;
            ButtonDeleteCity.IsEnabled = SpecialBaseHelper.CityId == 0;
            ButtonNewOrganization.IsEnabled = SpecialBaseHelper.OrganizationId == 0;
            ButtonEditOrganization.IsEnabled = SpecialBaseHelper.OrganizationId == 0;
            ButtonDeleteOrganization.IsEnabled = SpecialBaseHelper.OrganizationId == 0;

            TextBoxSearchProvince.IsEnabled = SpecialBaseHelper.ProvinceId == 0;
            TextBoxSearchCity.IsEnabled = SpecialBaseHelper.CityId == 0;
            TextBoxSearchOrganization.IsEnabled = SpecialBaseHelper.OrganizationId == 0;

            ShowProvinces(DataTableProvinces.Copy(), _searchTextProvince, DataGridProvinces);
            WpfHelper.SetDataGridSelectedValue(DataGridProvinces, SpecialBaseHelper.ProvinceId);
            WpfHelper.SetDataGridSelectedValue(DataGridCities, SpecialBaseHelper.CityId);
            WpfHelper.SetDataGridSelectedValue(DataGridOrganizations, SpecialBaseHelper.OrganizationId);
        }

        #endregion

        #region public static bool LoadProvinces()
        public static bool LoadProvinces()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                var dataAdapterProvinces = new SqlDataAdapter("SELECT * FROM Provinces " +
                                                              "WHERE Id <> 0" +
                                                              (SpecialBaseHelper.ProvinceId != 0
                                                                  ? " AND ActiveMode = @ActiveMode AND Id = @Id"
                                                                  : "") +
                                                              " ORDER BY Title ASC",
                    MainWindow.PublicConnection);
                dataAdapterProvinces.SelectCommand.Parameters.AddWithValue("@ActiveMode", Convert.ToInt16(CommonEnum.ActiveType.Active));
                dataAdapterProvinces.SelectCommand.Parameters.AddWithValue("@Id", SpecialBaseHelper.ProvinceId);
                DataTableProvinces = new DataTable();
                dataAdapterProvinces.Fill(DataTableProvinces);
                return true;
            }
            catch (Exception exception)
            {
                MainWindow.ErrorMessage = exception.Message;
                return false;
            }
        }

        #endregion

        #region private void DeleteProvince()

        private void DeleteProvince()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                var cityIdList = new List<int>();
                var organizationIdList = new List<int>();
                var organizationInfoIdList = new List<int>();
                var subsetIdList = new List<int>();
                // Get Cities
                var commandGetCityIdList = new SqlCommand("SELECT Id " +
                                                          "FROM Cities " +
                                                          "WHERE ProvinceId = @ProvinceId",
                    MainWindow.PublicConnection);
                commandGetCityIdList.Parameters.AddWithValue("@ProvinceId", SelectedProvinceId);
                var dataReaderCityId = commandGetCityIdList.ExecuteReader();
                while (dataReaderCityId.Read())
                {
                    if (!cityIdList.Contains(Convert.ToInt32(dataReaderCityId["Id"])))
                        cityIdList.Add(Convert.ToInt32(dataReaderCityId["Id"]));
                }
                dataReaderCityId.Close();

                foreach (var cityId in cityIdList)
                {
                    // Get Organizations
                    var commandGetOrganizationIdList = new SqlCommand("SELECT Id " +
                                                                      "FROM Organizations " +
                                                                      "WHERE CityId = @CityId",
                        MainWindow.PublicConnection);
                    commandGetOrganizationIdList.Parameters.AddWithValue("@CityId", cityId);
                    var dataReaderOrganizationId = commandGetOrganizationIdList.ExecuteReader();
                    while (dataReaderOrganizationId.Read())
                    {
                        if (!organizationIdList.Contains(Convert.ToInt32(dataReaderOrganizationId["Id"])))
                            organizationIdList.Add(Convert.ToInt32(dataReaderOrganizationId["Id"]));
                    }
                    dataReaderOrganizationId.Close();
                    foreach (var organizationId in organizationIdList)
                    {
                        // Get OrganizationInformation
                        var commandGetOrganizationInfoIdList = new SqlCommand("SELECT Id " +
                                                                              "FROM OrganizationsInformation " +
                                                                              "WHERE OrganizationId = @OrganizationId",
                            MainWindow.PublicConnection);
                        commandGetOrganizationInfoIdList.Parameters.AddWithValue("@OrganizationId", organizationId);
                        var dataReaderOrganizationInfoId = commandGetOrganizationInfoIdList.ExecuteReader();
                        while (dataReaderOrganizationInfoId.Read())
                        {
                            if (!organizationInfoIdList.Contains(Convert.ToInt32(dataReaderOrganizationInfoId["Id"])))
                                organizationInfoIdList.Add(Convert.ToInt32(dataReaderOrganizationInfoId["Id"]));
                        }
                        dataReaderOrganizationInfoId.Close();
                        //// Get Subsets
                        var commandGetSubsetIdList = new SqlCommand("SELECT Id " +
                                                                    "FROM Subsets " +
                                                                    "WHERE OrganizationId = @OrganizationId",
                            MainWindow.PublicConnection);
                        commandGetSubsetIdList.Parameters.AddWithValue("@OrganizationId", organizationId);
                        var dataReaderSubsetId = commandGetSubsetIdList.ExecuteReader();
                        while (dataReaderSubsetId.Read())
                        {
                            if (!subsetIdList.Contains(Convert.ToInt32(dataReaderSubsetId["Id"])))
                                subsetIdList.Add(Convert.ToInt32(dataReaderSubsetId["Id"]));
                        }
                        dataReaderSubsetId.Close();
                    }
                }
                foreach (var subsetId in subsetIdList)
                    MainWindow.PublicConnection.DeleteSqlData("Subsets", "Id = " + subsetId);
                foreach (var organizationInfoId in organizationInfoIdList)
                    MainWindow.PublicConnection.DeleteSqlData("OrganizationsInformation", "Id = " + organizationInfoId);
                foreach (var organizationId in organizationIdList)
                    MainWindow.PublicConnection.DeleteSqlData("Organizations", "Id = " + organizationId);
                foreach (var cityId in cityIdList)
                    MainWindow.PublicConnection.DeleteSqlData("Cities", "Id = " + cityId);
                MainWindow.PublicConnection.DeleteSqlData("Provinces", "Id = " + SelectedProvinceId);
                // Load data again
                LoadProvinces();
                LoadCities();
                Dispatcher.Invoke(() => "استان".DeletedMessage().ShowMessage());
            }
            catch (Exception exception)
            {
                MainWindow.ErrorMessage = exception.Message;
                Dispatcher.Invoke(() => WpfHelper.GetErrorMessage().ShowMessage());
            }
            finally
            {
                Dispatcher.Invoke(() => _waitWindowDeleteProvince.Close());
            }
        }

        #endregion

        #region public static bool LoadCities()
        public static bool LoadCities()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                var dataAdapterCities = new SqlDataAdapter("SELECT * FROM Cities " +
                                                           "WHERE Id <> 0" +
                                                           (SpecialBaseHelper.CityId != 0
                                                              ? " AND ActiveMode = @ActiveMode AND Id = @Id"
                                                              : "") +
                                                           " ORDER BY Title ASC",
                    MainWindow.PublicConnection);
                dataAdapterCities.SelectCommand.Parameters.AddWithValue("@ActiveMode", Convert.ToInt16(CommonEnum.ActiveType.Active));
                dataAdapterCities.SelectCommand.Parameters.AddWithValue("@Id", SpecialBaseHelper.CityId);
                DataTableCities = new DataTable();
                dataAdapterCities.Fill(DataTableCities);
                return true;
            }
            catch (Exception exception)
            {
                MainWindow.ErrorMessage = exception.Message;
                return false;
            }
        }

        #endregion

        #region private void DeleteCity()

        private void DeleteCity()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                var organizationIdList = new List<int>();
                var organizationInfoIdList = new List<int>();
                var subsetIdList = new List<int>();
                // Get Organizations
                var commandGetOrganizationIdList = new SqlCommand("SELECT Id " +
                                                                  "FROM Organizations " +
                                                                  "WHERE CityId = @CityId",
                    MainWindow.PublicConnection);
                commandGetOrganizationIdList.Parameters.AddWithValue("@CityId", SelectedCityId);
                var dataReaderOrganizationId = commandGetOrganizationIdList.ExecuteReader();
                while (dataReaderOrganizationId.Read())
                {
                    if (!organizationIdList.Contains(Convert.ToInt32(dataReaderOrganizationId["Id"])))
                        organizationIdList.Add(Convert.ToInt32(dataReaderOrganizationId["Id"]));
                }
                dataReaderOrganizationId.Close();
                foreach (var organizationId in organizationIdList)
                {
                    // Get OrganizationInformation
                    var commandGetOrganizationInfoIdList = new SqlCommand("SELECT Id " +
                                                                          "FROM OrganizationsInformation " +
                                                                          "WHERE OrganizationId = @OrganizationId",
                        MainWindow.PublicConnection);
                    commandGetOrganizationInfoIdList.Parameters.AddWithValue("@OrganizationId", organizationId);
                    var dataReaderOrganizationInfoId = commandGetOrganizationInfoIdList.ExecuteReader();
                    while (dataReaderOrganizationInfoId.Read())
                    {
                        if (!organizationInfoIdList.Contains(Convert.ToInt32(dataReaderOrganizationInfoId["Id"])))
                            organizationInfoIdList.Add(Convert.ToInt32(dataReaderOrganizationInfoId["Id"]));
                    }
                    dataReaderOrganizationInfoId.Close();
                    // Get Subsets
                    var commandGetSubsetIdList = new SqlCommand("SELECT Id " +
                                                                "FROM Subsets " +
                                                                "WHERE OrganizationId = @OrganizationId",
                        MainWindow.PublicConnection);
                    commandGetSubsetIdList.Parameters.AddWithValue("@OrganizationId", organizationId);
                    var dataReaderSubsetId = commandGetSubsetIdList.ExecuteReader();
                    while (dataReaderSubsetId.Read())
                    {
                        if (!subsetIdList.Contains(Convert.ToInt32(dataReaderSubsetId["Id"])))
                            subsetIdList.Add(Convert.ToInt32(dataReaderSubsetId["Id"]));
                    }
                    dataReaderSubsetId.Close();
                }
                foreach (var subsetId in subsetIdList)
                    MainWindow.PublicConnection.DeleteSqlData("Subsets", "Id = " + subsetId);
                foreach (var organizationInfoId in organizationInfoIdList)
                    MainWindow.PublicConnection.DeleteSqlData("OrganizationsInformation", "Id = " + organizationInfoId);
                foreach (var organizationId in organizationIdList)
                MainWindow.PublicConnection.DeleteSqlData("Cities", "Id = " + SelectedCityId);
                // Load data again
                LoadCities();
                LoadOrganizations();
                Dispatcher.Invoke(() => "شهر".DeletedMessage().ShowMessage());
            }
            catch (Exception exception)
            {
                MainWindow.ErrorMessage = exception.Message;
                Dispatcher.Invoke(() => WpfHelper.GetErrorMessage().ShowMessage());
            }
            finally
            {
                Dispatcher.Invoke(() => _waitWindowDeleteCity.Close());
            }
        }

        #endregion

        #region public static bool LoadOrganizations()
        public static bool LoadOrganizations()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                var dataAdapterOrganizations = new SqlDataAdapter("SELECT * FROM Organizations " +
                                                                  "WHERE Id <> 0" +
                                                                  (SpecialBaseHelper.OrganizationId != 0
                                                                      ? " AND ActiveMode = @ActiveMode AND Id = @Id"
                                                                      : "") +
                                                                  " ORDER BY Title ASC",
                    MainWindow.PublicConnection);
                dataAdapterOrganizations.SelectCommand.Parameters.AddWithValue("@ActiveMode", Convert.ToInt16(CommonEnum.ActiveType.Active));
                dataAdapterOrganizations.SelectCommand.Parameters.AddWithValue("@Id", SpecialBaseHelper.OrganizationId);
                DataTableOrganizations = new DataTable();
                dataAdapterOrganizations.Fill(DataTableOrganizations);
                return true;
            }
            catch (Exception exception)
            {
                MainWindow.ErrorMessage = exception.Message;
                return false;
            }
        }

        #endregion

        #region private void DeleteOrganization()

        private void DeleteOrganization()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                var organizationInfoIdList = new List<int>();
                var subsetIdList = new List<int>();
                // Get OrganizationInformation
                var commandGetOrganizationInfoIdList = new SqlCommand("SELECT Id " +
                                                                      "FROM OrganizationsInformation " +
                                                                      "WHERE OrganizationId = @OrganizationId",
                    MainWindow.PublicConnection);
                commandGetOrganizationInfoIdList.Parameters.AddWithValue("@OrganizationId", SelectedOrganizationId);
                var dataReaderOrganizationInfoId = commandGetOrganizationInfoIdList.ExecuteReader();
                while (dataReaderOrganizationInfoId.Read())
                {
                    if (!organizationInfoIdList.Contains(Convert.ToInt32(dataReaderOrganizationInfoId["Id"])))
                        organizationInfoIdList.Add(Convert.ToInt32(dataReaderOrganizationInfoId["Id"]));
                }
                dataReaderOrganizationInfoId.Close();
                // Get Subsets
                var commandGetSubsetIdList = new SqlCommand("SELECT Id " +
                                                            "FROM Subsets " +
                                                            "WHERE OrganizationId = @OrganizationId",
                    MainWindow.PublicConnection);
                commandGetSubsetIdList.Parameters.AddWithValue("@OrganizationId", SelectedOrganizationId);
                var dataReaderSubsetId = commandGetSubsetIdList.ExecuteReader();
                while (dataReaderSubsetId.Read())
                {
                    if (!subsetIdList.Contains(Convert.ToInt32(dataReaderSubsetId["Id"])))
                        subsetIdList.Add(Convert.ToInt32(dataReaderSubsetId["Id"]));
                }
                dataReaderSubsetId.Close();
                foreach (var subsetId in subsetIdList)
                    MainWindow.PublicConnection.DeleteSqlData("Subsets", "Id = " + subsetId);
                foreach (var organizationInfoId in organizationInfoIdList)
                    MainWindow.PublicConnection.DeleteSqlData("OrganizationsInformation", "Id = " + organizationInfoId);
                MainWindow.PublicConnection.DeleteSqlData("Organizations", "Id = " + SelectedOrganizationId);
                // Load data again
                LoadOrganizations();
                Dispatcher.Invoke(() => "سازمان".DeletedMessage().ShowMessage());
            }
            catch (Exception exception)
            {
                MainWindow.ErrorMessage = exception.Message;
                Dispatcher.Invoke(() => WpfHelper.GetErrorMessage().ShowMessage());
            }
            finally
            {
                Dispatcher.Invoke(() => _waitWindowDeleteOrganization.Close());
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
            DataGridCities.ItemsSource = null;
            DataGridOrganizations.ItemsSource = null;
            if (DataGridProvinces.SelectedItem == null || DataGridProvinces.SelectedItem.ToString().Contains("NewItemPlaceholder")) return;
            var row = (DataRowView)DataGridProvinces.SelectedItem;
            SelectedProvinceId = Convert.ToInt32(row["Id"]);
            SelectedProvinceTitle = row["عنوان"].ToString();
            ShowCities(DataTableCities.Copy(), _searchTextCity, DataGridCities);
        }

        #endregion

        #region DataGridCities_SelectionChanged

        private void DataGridCities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBoxSearchOrganization.Text = "";
            DataGridOrganizations.ItemsSource = null;
            if (DataGridCities.SelectedItem == null || DataGridCities.SelectedItem.ToString().Contains("NewItemPlaceholder")) return;
            var row = (DataRowView)DataGridCities.SelectedItem;
            SelectedCityId = Convert.ToInt32(row["Id"]);
            SelectedCityTitle = row["عنوان"].ToString();
            ShowOrganizations(DataTableOrganizations, _searchTextOrganization, DataGridOrganizations);
        }

        #endregion

        #region DataGridOrganizations_SelectionChanged

        private void DataGridOrganizations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridOrganizations.SelectedItem == null || DataGridOrganizations.SelectedItem.ToString().Contains("NewItemPlaceholder")) return;
            var row = (DataRowView)DataGridOrganizations.SelectedItem;
            SelectedOrganizationId = Convert.ToInt32(row["Id"]);
        }

        #endregion

        #endregion

        #region TextBox_Events

        #region TextBoxSearchProvince_TextChanged

        private void TextBoxSearchProvince_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchTextProvince = TextBoxSearchProvince.Text.Trim().ToCorrectKeYe();
            ShowProvinces(DataTableProvinces.Copy(), _searchTextProvince, DataGridProvinces);
        }

        #endregion

        #region TextBoxSearchCity_TextChanged

        private void TextBoxSearchCity_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchTextCity = TextBoxSearchCity.Text.Trim().ToCorrectKeYe();
            ShowCities(DataTableCities.Copy(), _searchTextCity, DataGridCities);
        }

        #endregion

        #region TextBoxSearchOrganization_TextChanged

        private void TextBoxSearchOrganization_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchTextOrganization = TextBoxSearchOrganization.Text.Trim().ToCorrectKeYe();
            ShowOrganizations(LocationsWindow.DataTableOrganizations.Copy(), _searchTextOrganization, DataGridOrganizations);
        }

        #endregion

        #endregion

        #region Button_Events

        #region ButtonNewProvince_Click

        private void ButtonNewProvince_Click(object sender, RoutedEventArgs e)
        {
            ChangeProvincesMode = CommonEnum.ChangeDatabaseMode.InsertInToDatabase;
            var addEditProvinceWindow = new AddEditProvinceWindow { Owner = this };
            addEditProvinceWindow.ShowDialog();
            ShowProvinces(DataTableProvinces.Copy(), _searchTextProvince, DataGridProvinces);
            DataGridProvinces.SelectDataGridRow(SelectedProvinceId);
        }

        #endregion

        #region ButtonEditProvince_Click

        private void ButtonEditProvince_Click(object sender, RoutedEventArgs e)
        {
            if (!Convert.ToBoolean(WpfHelper.GetSelectedDataGridRowInfo(DataGridProvinces, ref SelectedProvinceId, "استانی"))) return;
            ChangeProvincesMode = CommonEnum.ChangeDatabaseMode.UpdateDatabase;
            var addEditProvinceWindow = new AddEditProvinceWindow { Owner = this };
            addEditProvinceWindow.ShowDialog();
            ShowProvinces(DataTableProvinces.Copy(), _searchTextProvince, DataGridProvinces);
            DataGridProvinces.SelectDataGridRow(SelectedProvinceId);
        }

        #endregion

        #region ButtonDeleteProvince_Click

        private void ButtonDeleteProvince_Click(object sender, RoutedEventArgs e)
        {
            if (!Convert.ToBoolean(WpfHelper.GetSelectedDataGridRowInfo(DataGridProvinces, ref SelectedProvinceId, "استانی"))) return;
            var dialogResult = PersianMessageBox_Mhclass.Show("آیا از حذف استان انتخاب‌شده اطمینان دارید؟",
                "حذف استان", PersianMessageBox_Mhclass.MsgMhButton.YesNo, PersianMessageBox_Mhclass.MsgMhIcon.Question);
            if (dialogResult == MessageBoxResult.No) return;
            dialogResult = PersianMessageBox_Mhclass.Show("با حذف این استان تمامی شهرها، سازمان‌ها و زیرمجموعه‌ها آن حذف خواهند شد؟\n" +
                                 "آیا آین مورد را تأیید می‌کنید؟", "حذف استان", PersianMessageBox_Mhclass.MsgMhButton.YesNo, PersianMessageBox_Mhclass.MsgMhIcon.Question);
            if (dialogResult == MessageBoxResult.No) return;
            var threadDeleteProvince = new Thread(DeleteProvince);
            threadDeleteProvince.Start();
            _waitWindowDeleteProvince = new WaitWindow { Owner = this };
            _waitWindowDeleteProvince.ShowDialog();
            ShowProvinces(DataTableProvinces.Copy(), _searchTextProvince, DataGridProvinces);
        }

        #endregion

        #region ButtonNewCity_Click

        private void ButtonNewCity_Click(object sender, RoutedEventArgs e)
        {
            if (!Convert.ToBoolean(WpfHelper.GetSelectedDataGridRowInfo(DataGridProvinces, ref SelectedProvinceId, ref SelectedProvinceTitle, "استانی"))) return;
            ChangeCitiesMode = CommonEnum.ChangeDatabaseMode.InsertInToDatabase;
            var addEditCityWindow = new AddEditCityWindow { Owner = this };
            addEditCityWindow.ShowDialog();
            ShowCities(DataTableCities.Copy(), _searchTextCity, DataGridCities);
            DataGridCities.SelectDataGridRow(SelectedCityId);

        }

        #endregion

        #region ButtonEditCity_Click

        private void ButtonEditCity_Click(object sender, RoutedEventArgs e)
        {
            if (!Convert.ToBoolean(WpfHelper.GetSelectedDataGridRowInfo(DataGridCities, ref SelectedCityId, "شهری"))) return;
            ChangeCitiesMode = CommonEnum.ChangeDatabaseMode.UpdateDatabase;
            var addEditCityWindow = new AddEditCityWindow { Owner = this };
            addEditCityWindow.ShowDialog();
            ShowCities(DataTableCities.Copy(), _searchTextCity, DataGridCities);
            DataGridCities.SelectDataGridRow(SelectedCityId);
        }

        #endregion

        #region ButtonDeleteCity_Click

        private void ButtonDeleteCity_Click(object sender, RoutedEventArgs e)
        {
            if (!Convert.ToBoolean(WpfHelper.GetSelectedDataGridRowInfo(DataGridCities, ref SelectedCityId, "شهری"))) return;
            var dialogResult = PersianMessageBox_Mhclass.Show("آیا از حذف شهر انتخاب‌شده اطمینان دارید؟",
                "حذف شهر", PersianMessageBox_Mhclass.MsgMhButton.YesNo, PersianMessageBox_Mhclass.MsgMhIcon.Question);
            if (dialogResult == MessageBoxResult.No) return;
            dialogResult = PersianMessageBox_Mhclass.Show("با حذف این شهر تمامی سازمان‌ها و زیرمجموعه‌های آن حذف خواهند شد؟\n" +
                                             "آیا آین مورد را تأیید می‌کنید؟",
                "حذف شهر", PersianMessageBox_Mhclass.MsgMhButton.YesNo, PersianMessageBox_Mhclass.MsgMhIcon.Question);
            if (dialogResult == MessageBoxResult.No) return;
            var threadDeleteCity = new Thread(DeleteCity);
            threadDeleteCity.Start();
            _waitWindowDeleteCity = new WaitWindow { Owner = this };
            _waitWindowDeleteCity.ShowDialog();
            ShowCities(DataTableCities.Copy(), _searchTextCity, DataGridCities);
        }

        #endregion

        #region ButtonNewOrganization_Click

        private void ButtonNewOrganization_Click(object sender, RoutedEventArgs e)
        {
            if (!Convert.ToBoolean(WpfHelper.GetSelectedDataGridRowInfo(DataGridCities, ref SelectedCityId, ref SelectedCityTitle, "شهری"))) return;
            ChangeOrganizationsMode = CommonEnum.ChangeDatabaseMode.InsertInToDatabase;
            var addEditOrganizationWindow = new AddEditOrganizationWindow { Owner = this };
            addEditOrganizationWindow.ShowDialog();
            ShowOrganizations(DataTableOrganizations.Copy(), _searchTextOrganization, DataGridOrganizations);
            DataGridOrganizations.SelectDataGridRow(SelectedOrganizationId);
        }

        #endregion

        #region ButtonEditOrganization_Click

        private void ButtonEditOrganization_Click(object sender, RoutedEventArgs e)
        {
            if (!Convert.ToBoolean(WpfHelper.GetSelectedDataGridRowInfo(DataGridOrganizations, ref SelectedOrganizationId, "سازمانی"))) return;
            ChangeOrganizationsMode = CommonEnum.ChangeDatabaseMode.UpdateDatabase;
            var addEditOrganizationWindow = new AddEditOrganizationWindow { Owner = this };
            addEditOrganizationWindow.ShowDialog();
            ShowOrganizations(DataTableOrganizations.Copy(), _searchTextOrganization, DataGridOrganizations);
            DataGridOrganizations.SelectDataGridRow(SelectedOrganizationId);
        }

        #endregion

        #region ButtonDeleteOrganization_Click

        private void ButtonDeleteOrganization_Click(object sender, RoutedEventArgs e)
        {
            if (!Convert.ToBoolean(WpfHelper.GetSelectedDataGridRowInfo(DataGridOrganizations, ref SelectedOrganizationId, "سازمانی"))) return;
            var dialogResult = PersianMessageBox_Mhclass.Show("آیا از حذف سازمان انتخاب‌شده اطمینان دارید؟",
                "حذف سازمان", PersianMessageBox_Mhclass.MsgMhButton.YesNo, PersianMessageBox_Mhclass.MsgMhIcon.Question);
            if (dialogResult == MessageBoxResult.No) return;
            var threadDeleteOrganization = new Thread(DeleteOrganization);
            threadDeleteOrganization.Start();
            _waitWindowDeleteOrganization = new WaitWindow { Owner = this };
            _waitWindowDeleteOrganization.ShowDialog();
            ShowOrganizations(DataTableOrganizations.Copy(), _searchTextOrganization, DataGridOrganizations);
        }

        #endregion

        #endregion
    }
}
