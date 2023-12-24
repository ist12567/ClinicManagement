using ACDClinicManagement.App.Views.CommonViews;
using ACDClinicManagement.AppHelpers.AppServices.Calculation;
using ACDClinicManagement.AppHelpers.CommonHelpers;
using ACDClinicManagement.Common.Enums;
using ACDClinicManagement.Common.Helpers;
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

namespace ACDClinicManagement.App.Views.GeneralSettingsViews
{
    /// <summary>
    /// Interaction logic for GeneralSettingsWindow.xaml
    /// </summary>
    public partial class GeneralSettingsWindow : Window
    {
        public GeneralSettingsWindow()
        {
            InitializeComponent();
            InitializeObjects();
        }

        // ••••••••••••
        // DEFINATIONS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region Windows

        private WaitWindow _waitWindowDeleteBank, _waitWindowDeleteValueAdded, _waitWindowDeleteVersion, _waitWindowDeleteVersionFeature,
            _waitWindowLoadVersions, _waitWindowLoadVersionFeatures;

        #endregion

        #region Classes


        #endregion

        #region Objects

        public static CommonEnum.ChangeDatabaseMode ChangeBanksMode { get; private set; }
        public static CommonEnum.ChangeDatabaseMode ChangeValuesAddedMode { get; private set; }
        public static CommonEnum.ChangeDatabaseMode ChangeVehicleTollsInformationMode { get; private set; }
        public static CommonEnum.ChangeDatabaseMode ChangeVersionsMode { get; private set; }
        public static CommonEnum.ChangeDatabaseMode ChangeVersionsFeaturesMode { get; private set; }

        private DataTable _dataTableBanks, _dataTableValuesAdded, _dataTableVehicleTollsInformation, _dataTableVersions, _dataTableVersionFeatures;
        public static DataTable DataTableBanks, DataTableVersions, DataTableVersionFeatures;

        private string _searchTextBank, _searchTextValueAdded, _searchTextVersion, _searchTextVersionFeature;

        #endregion

        #region Variables

        public static int SelectedBankId, SelectedValueAddedId, SelectedVehicleTollInfoId, SelectedVersionId, SelectedVersionFeatureId;
        public static string SelectedVersionTitle;

        #endregion

        // ••••••••••••
        // METHODS     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region private void InitializeObjects()

        private void InitializeObjects()
        {
            _dataTableBanks = new DataTable();
            _dataTableBanks.Columns.Add("Id", typeof(int));
            _dataTableBanks.Columns.Add("عنوان", typeof(string));
            _dataTableBanks.Columns.Add("تاریخ و زمان ایجاد", typeof(string));
            _dataTableBanks.Columns.Add("تاریخ و زمان ویرایش", typeof(string));

            _dataTableValuesAdded = new DataTable();
            _dataTableValuesAdded.Columns.Add("Id", typeof(int));
            _dataTableValuesAdded.Columns.Add("سال", typeof(string));
            _dataTableValuesAdded.Columns.Add("ارزش افزوده", typeof(string));
            _dataTableValuesAdded.Columns.Add("تاریخ و زمان ایجاد", typeof(string));
            _dataTableValuesAdded.Columns.Add("تاریخ و زمان ویرایش", typeof(string));

            _dataTableVehicleTollsInformation = new DataTable();
            _dataTableVehicleTollsInformation.Columns.Add("Id", typeof(int));
            _dataTableVehicleTollsInformation.Columns.Add("سال", typeof(string));
            _dataTableVehicleTollsInformation.Columns.Add("نرخ عوارض", typeof(string));
            _dataTableVehicleTollsInformation.Columns.Add("نرخ جریمه", typeof(string));
            _dataTableVehicleTollsInformation.Columns.Add("تاریخ و زمان ایجاد", typeof(string));
            _dataTableVehicleTollsInformation.Columns.Add("تاریخ و زمان ویرایش", typeof(string));

            _dataTableVersions = new DataTable();
            _dataTableVersions.Columns.Add("Id", typeof(int));
            _dataTableVersions.Columns.Add("نسخه", typeof(string));
            _dataTableVersions.Columns.Add("تاریخ و زمان ایجاد", typeof(string));
            _dataTableVersions.Columns.Add("تاریخ و زمان ویرایش", typeof(string));

            _dataTableVersionFeatures = new DataTable();
            _dataTableVersionFeatures.Columns.Add("Id", typeof(int));
            _dataTableVersionFeatures.Columns.Add("ویژگی", typeof(string));
            _dataTableVersionFeatures.Columns.Add("تاریخ و زمان ایجاد", typeof(string));
            _dataTableVersionFeatures.Columns.Add("تاریخ و زمان ویرایش", typeof(string));
        }

        #endregion

        #region private void LoadDefaults()

        private void LoadDefaults()
        {
            this.ShowWindow(CommonEnum.WindowStyleMode.Normal);

            _searchTextBank = "";
            _searchTextValueAdded = "";
            _searchTextVersion = "";
            _searchTextVersionFeature = "";
            ShowBanks(DataTableBanks.Copy(), _searchTextBank, DataGridBanks);
            ShowValuesAdded(CalculateBaseHelper.DataTableValuesAdded.Copy(), _searchTextValueAdded, DataGridValuesAdded);
            var threadLoadVersions = new Thread(LoadVersions);
            threadLoadVersions.Start();
            _waitWindowLoadVersions = new WaitWindow { Owner = this };
            _waitWindowLoadVersions.ShowDialog();
            ShowVersions(DataTableVersions.Copy(), _searchTextVersion, DataGridVersions);
        }

        #endregion

        #region private void LoadVersions()

        private void LoadVersions()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                var dataAdapterVersions = new SqlDataAdapter("SELECT * FROM Versions " +
                                                             "WHERE Id <> 0 " +
                                                             "ORDER BY Id ASC",
                    MainWindow.PublicConnection);
                DataTableVersions = new DataTable();
                dataAdapterVersions.Fill(DataTableVersions);
            }
            catch (Exception exception)
            {
                MainWindow.ErrorMessage = exception.Message;
            }
            finally
            {
                Dispatcher.Invoke(() => _waitWindowLoadVersions.Close());
            }
        }

        #endregion

        #region private void LoadVersionFeatures()

        private void LoadVersionFeatures()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                var dataAdapterVersionsFeatures = new SqlDataAdapter("SELECT * FROM VersionsFeatures " +
                                                                     "WHERE VersionId = @VersionId " +
                                                                     "ORDER BY Feature ASC",
                    MainWindow.PublicConnection);
                dataAdapterVersionsFeatures.SelectCommand.Parameters.AddWithValue("@VersionId", SelectedVersionId);
                DataTableVersionFeatures = new DataTable();
                dataAdapterVersionsFeatures.Fill(DataTableVersionFeatures);
            }
            catch (Exception exception)
            {
                MainWindow.ErrorMessage = exception.Message;
            }
            finally
            {
                Dispatcher.Invoke(() => _waitWindowLoadVersionFeatures.Close());
            }
        }

        #endregion

        #region public static bool LoadBanks()
        public static bool LoadBanks()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                var dataAdapterBanks = new SqlDataAdapter("SELECT * FROM Banks " +
                                                          "WHERE Id <> 0 " +
                                                          "ORDER BY Title ASC",
                    MainWindow.PublicConnection);
                DataTableBanks = new DataTable();
                dataAdapterBanks.Fill(DataTableBanks);
                return true;
            }
            catch (Exception exception)
            {
                MainWindow.ErrorMessage = exception.Message;
                return false;
            }
        }

        #endregion

        #region private void DeleteBank()

        private void DeleteBank()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                if (MainWindow.PublicConnection.DeleteSqlData("Banks", "Id = " + SelectedBankId))
                {
                    if (LoadBanks())
                        Dispatcher.Invoke(() => "بانک".DeletedMessage().ShowMessage());
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
                Dispatcher.Invoke(() => _waitWindowDeleteBank.Close());
            }
        }

        #endregion

        #region public static bool LoadValuesAdded()
        public static bool LoadValuesAdded()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                var dataAdapterValuesAdded = new SqlDataAdapter("SELECT * FROM ValuesAdded " +
                                                                "WHERE Id <> 0 " +
                                                                "ORDER BY Year ASC",
                    MainWindow.PublicConnection);
                CalculateBaseHelper.DataTableValuesAdded = new DataTable();
                dataAdapterValuesAdded.Fill(CalculateBaseHelper.DataTableValuesAdded);
                return true;
            }
            catch (Exception exception)
            {
                MainWindow.ErrorMessage = exception.Message;
                return false;
            }
        }

        #endregion

        #region private void DeleteValueAdded()

        private void DeleteValueAdded()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                if (MainWindow.PublicConnection.DeleteSqlData("ValuesAdded", "Id = " + SelectedValueAddedId))
                {
                    if(LoadValuesAdded()) Dispatcher.Invoke(() => "ارزش افزوده‌ی".DeletedMessage().ShowMessage());
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
                Dispatcher.Invoke(() => _waitWindowDeleteValueAdded.Close());
            }
        }

        #endregion

        #region private void DeleteVersion()

        private void DeleteVersion()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                var versionFeatureIdList = new List<int>();
                var commandGetVersionFeatureIdList = new SqlCommand("SELECT Id " +
                                                                    "FROM VersionsFeatures " +
                                                                    "WHERE VersionId = @VersionId",
                    MainWindow.PublicConnection);
                commandGetVersionFeatureIdList.Parameters.AddWithValue("@VersionId", SelectedVersionId);
                var dataReaderVersionFeatureId = commandGetVersionFeatureIdList.ExecuteReader();
                while (dataReaderVersionFeatureId.Read())
                    versionFeatureIdList.Add(Convert.ToInt32(dataReaderVersionFeatureId["Id"]));
                dataReaderVersionFeatureId.Close();

                foreach (var item in versionFeatureIdList)
                    MainWindow.PublicConnection.DeleteSqlData("VersionsFeatures", "Id = " + item);

                MainWindow.PublicConnection.DeleteSqlData("Versions", "Id = " + SelectedVersionId);
                // Load data again
                var dataAdapterVersions = new SqlDataAdapter("SELECT * FROM Versions " +
                                                             "WHERE Id <> 0 " +
                                                             "ORDER BY Id ASC",
                    MainWindow.PublicConnection);
                DataTableVersions = new DataTable();
                dataAdapterVersions.Fill(DataTableVersions);
                Dispatcher.Invoke(() => "نسخه‌ی".DeletedMessage().ShowMessage());
            }
            catch (Exception exception)
            {
                MainWindow.ErrorMessage = exception.Message;
                Dispatcher.Invoke(() => WpfHelper.GetErrorMessage().ShowMessage());
            }
            finally
            {
                Dispatcher.Invoke(() => _waitWindowDeleteVersion.Close());
            }
        }

        #endregion

        #region private void DeleteVersionFeature()

        private void DeleteVersionFeature()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                if (MainWindow.PublicConnection.DeleteSqlData("VersionsFeatures", "Id = " + SelectedVersionFeatureId))
                {
                    var dataAdapterVersionFeatures = new SqlDataAdapter("SELECT * FROM VersionsFeatures " +
                                                                        "WHERE VersionId = @VersionId",
                        MainWindow.PublicConnection);
                    dataAdapterVersionFeatures.SelectCommand.Parameters.AddWithValue("@VersionId", SelectedVersionId);
                    DataTableVersionFeatures = new DataTable();
                    dataAdapterVersionFeatures.Fill(DataTableVersionFeatures);
                    Dispatcher.Invoke(() => "ویژگی نسخه‌ی".DeletedMessage().ShowMessage());
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
                Dispatcher.Invoke(() => _waitWindowDeleteVersionFeature.Close());
            }
        }

        #endregion

        #region public static string GetBankTitle(object id)
        public static string GetBankTitle(object id)
        {
            var result = "";
            foreach (DataRow bank in DataTableBanks.Rows)
            {
                if (bank["Id"].ToString() == id.ToString())
                {
                    result = bank["Title"].ToString();
                    break;
                }
            }
            return result;
        }

        #endregion

        #region private void ShowBanks(DataTable dataTable, string searchTerm, DataGrid dataGrid)
        private void ShowBanks(DataTable dataTable, string searchTerm, DataGrid dataGrid)
        {
            _dataTableBanks.Clear();
            foreach (DataRow data in dataTable.Rows)
                if (data["Title"].ToString().ToCorrectKeYe().ToLower().Contains(searchTerm))
                    _dataTableBanks.Rows.Add(data["Id"],
                        data["Title"].ToString().ToFarsiFormat(),
                        data["CreatedAt"].ToFarsiFormatDateTimeFromSql(),
                        data["ModifiedAt"].ToFarsiFormatDateTimeFromSql());
            dataGrid.ItemsSource = _dataTableBanks.Copy().DefaultView;
        }
        #endregion

        #region private void ShowValuesAdded(DataTable dataTable, string searchTerm, DataGrid dataGrid)
        private void ShowValuesAdded(DataTable dataTable, string searchTerm, DataGrid dataGrid)
        {
            _dataTableValuesAdded.Clear();
            foreach (DataRow data in dataTable.Rows)
                if (data["Year"].ToString().ToLower().Contains(searchTerm))
                    _dataTableValuesAdded.Rows.Add(data["Id"],
                        data["Year"].ToString().ToFarsiFormat(),
                        data["ValueAdded"].ToString().ToFarsiFormat(),
                        data["CreatedAt"].ToFarsiFormatDateTimeFromSql(),
                        data["ModifiedAt"].ToFarsiFormatDateTimeFromSql());
            dataGrid.ItemsSource = _dataTableValuesAdded.Copy().DefaultView;
        }
        #endregion

        #region private void ShowVersions(DataTable dataTable, string searchTerm, DataGrid dataGrid)
        private void ShowVersions(DataTable dataTable, string searchTerm, DataGrid dataGrid)
        {
            _dataTableVersions.Clear();
            foreach (DataRow data in dataTable.Rows)
                if (data["Version"].ToString().ToLower().Contains(searchTerm))
                    _dataTableVersions.Rows.Add(data["Id"],
                        data["Version"].ToString().ToFarsiFormat(),
                        data["CreatedAt"].ToFarsiFormatDateTimeFromSql(),
                        data["ModifiedAt"].ToFarsiFormatDateTimeFromSql());
            dataGrid.ItemsSource = _dataTableVersions.Copy().DefaultView;
        }
        #endregion

        #region private void ShowVersionFeatures(DataTable dataTable, string searchTerm, DataGrid dataGrid)
        private void ShowVersionFeatures(DataTable dataTable, string searchTerm, DataGrid dataGrid)
        {
            _dataTableVersionFeatures.Clear();
            foreach (DataRow data in dataTable.Rows)
                if (data["Feature"].ToString().ToCorrectKeYe().ToLower().Contains(searchTerm))
                    _dataTableVersionFeatures.Rows.Add(data["Id"],
                        data["Feature"].ToString().ToFarsiFormat(),
                        data["CreatedAt"].ToFarsiFormatDateTimeFromSql(),
                        data["ModifiedAt"].ToFarsiFormatDateTimeFromSql());
            dataGrid.ItemsSource = _dataTableVersionFeatures.Copy().DefaultView;
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

        #region DataGridBanks_SelectionChanged

        private void DataGridBanks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridBanks.SelectedItem == null || DataGridBanks.SelectedItem.ToString().Contains("NewItemPlaceholder")) return;
            var row = (DataRowView)DataGridBanks.SelectedItem;
            SelectedBankId = Convert.ToInt32(row["Id"]);
        }

        #endregion

        #region DataGridValuesAdded_SelectionChanged

        private void DataGridValuesAdded_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridValuesAdded.SelectedItem == null || DataGridValuesAdded.SelectedItem.ToString().Contains("NewItemPlaceholder")) return;
            var row = (DataRowView)DataGridValuesAdded.SelectedItem;
            SelectedValueAddedId = Convert.ToInt32(row["Id"]);
        }

        #endregion

        #region DataGridVersions_SelectionChanged

        private void DataGridVersions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBoxSearchVersionFeature.Text = "";
            DataGridVersionFeatures.ItemsSource = null;
            if (DataGridVersions.SelectedItem == null || DataGridVersions.SelectedItem.ToString().Contains("NewItemPlaceholder")) return;
            var row = (DataRowView)DataGridVersions.SelectedItem;
            SelectedVersionId = Convert.ToInt32(row["Id"]);
            SelectedVersionTitle = row["نسخه"].ToString();

            var threadLoadVersionFeatures = new Thread(LoadVersionFeatures);
            threadLoadVersionFeatures.Start();
            _waitWindowLoadVersionFeatures = new WaitWindow { Owner = this };
            _waitWindowLoadVersionFeatures.ShowDialog();
            ShowVersionFeatures(DataTableVersionFeatures.Copy(), _searchTextVersionFeature, DataGridVersionFeatures);
        }

        #endregion

        #region DataGridVersionFeatures_SelectionChanged

        private void DataGridVersionFeatures_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridVersionFeatures.SelectedItem == null || DataGridVersionFeatures.SelectedItem.ToString().Contains("NewItemPlaceholder")) return;
            var row = (DataRowView)DataGridVersionFeatures.SelectedItem;
            SelectedVersionFeatureId = Convert.ToInt32(row["Id"]);
        }

        #endregion

        #endregion

        #region TextBox_Events

        #region TextBoxSearchBank_TextChanged

        private void TextBoxSearchBank_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchTextBank = TextBoxSearchBank.Text.Trim().ToCorrectKeYe();
            ShowBanks(DataTableBanks.Copy(), _searchTextBank, DataGridBanks);
        }

        #endregion

        #region TextBoxSearchValueAdded_TextChanged

        private void TextBoxSearchValueAdded_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchTextValueAdded = TextBoxSearchValueAdded.Text.Trim();
            ShowValuesAdded(CalculateBaseHelper.DataTableValuesAdded.Copy(), _searchTextValueAdded, DataGridValuesAdded);
        }

        #endregion

        #region TextBoxSearchVersion_TextChanged

        private void TextBoxSearchVersion_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchTextVersion = TextBoxSearchVersion.Text.Trim();
            ShowVersions(DataTableVersions.Copy(), _searchTextVersion, DataGridVersions);
        }

        #endregion

        #region TextBoxSearchVersionFeature_TextChanged

        private void TextBoxSearchVersionFeature_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchTextVersionFeature = TextBoxSearchVersionFeature.Text.Trim().ToCorrectKeYe();
            ShowVersionFeatures(DataTableVersionFeatures.Copy(), _searchTextVersionFeature, DataGridVersionFeatures);
        }

        #endregion

        #endregion

        #region Button_Events

        #region ButtonNewBank_Click

        private void ButtonNewBank_Click(object sender, RoutedEventArgs e)
        {
            ChangeBanksMode = CommonEnum.ChangeDatabaseMode.InsertInToDatabase;
            var addEditBankWindow = new AddEditBankWindow { Owner = this };
            addEditBankWindow.ShowDialog();
            ShowBanks(DataTableBanks.Copy(), _searchTextBank, DataGridBanks);
            DataGridBanks.SelectDataGridRow(SelectedBankId);
        }

        #endregion

        #region ButtonEditBank_Click

        private void ButtonEditBank_Click(object sender, RoutedEventArgs e)
        {
            if (!Convert.ToBoolean(WpfHelper.GetSelectedDataGridRowInfo(DataGridBanks, ref SelectedBankId, "بانکی"))) return;
            ChangeBanksMode = CommonEnum.ChangeDatabaseMode.UpdateDatabase;
            var addEditBankWindow = new AddEditBankWindow { Owner = this };
            addEditBankWindow.ShowDialog();
            ShowBanks(DataTableBanks.Copy(), _searchTextBank, DataGridBanks);
            DataGridBanks.SelectDataGridRow(SelectedBankId);
        }

        #endregion

        #region ButtonDeleteBank_Click

        private void ButtonDeleteBank_Click(object sender, RoutedEventArgs e)
        {
            if (!Convert.ToBoolean(WpfHelper.GetSelectedDataGridRowInfo(DataGridBanks, ref SelectedBankId, "بانکی"))) return;
            var dialogResult = PersianMessageBox_Mhclass.Show("آیا از حذف بانک انتخاب‌شده اطمینان دارید؟",
                "حذف بانک", PersianMessageBox_Mhclass.MsgMhButton.YesNo, PersianMessageBox_Mhclass.MsgMhIcon.Question);
            if (dialogResult == MessageBoxResult.No) return;
            var threadDeleteBank = new Thread(DeleteBank);
            threadDeleteBank.Start();
            _waitWindowDeleteBank = new WaitWindow { Owner = this };
            _waitWindowDeleteBank.ShowDialog();
            ShowBanks(DataTableBanks.Copy(), _searchTextBank, DataGridBanks);
        }

        #endregion

        #region ButtonNewValueAdded_Click

        private void ButtonNewValueAdded_Click(object sender, RoutedEventArgs e)
        {
            ChangeValuesAddedMode = CommonEnum.ChangeDatabaseMode.InsertInToDatabase;
            var addEditValueAddedWindow = new AddEditValueAddedWindow { Owner = this };
            addEditValueAddedWindow.ShowDialog();
            ShowValuesAdded(CalculateBaseHelper.DataTableValuesAdded.Copy(), _searchTextValueAdded, DataGridValuesAdded);
            DataGridValuesAdded.SelectDataGridRow(SelectedValueAddedId);
        }

        #endregion

        #region ButtonEditValueAdded_Click

        private void ButtonEditValueAdded_Click(object sender, RoutedEventArgs e)
        {
            if (!Convert.ToBoolean(WpfHelper.GetSelectedDataGridRowInfo(DataGridValuesAdded, ref SelectedValueAddedId, "ارزش افزوده‌ای"))) return;
            ChangeValuesAddedMode = CommonEnum.ChangeDatabaseMode.UpdateDatabase;
            var addEditValueAddedWindow = new AddEditValueAddedWindow { Owner = this };
            addEditValueAddedWindow.ShowDialog();
            ShowValuesAdded(CalculateBaseHelper.DataTableValuesAdded.Copy(), _searchTextValueAdded, DataGridValuesAdded);
            DataGridValuesAdded.SelectDataGridRow(SelectedValueAddedId);
        }

        #endregion

        #region ButtonDeleteValueAdded_Click

        private void ButtonDeleteValueAdded_Click(object sender, RoutedEventArgs e)
        {
            if (!Convert.ToBoolean(WpfHelper.GetSelectedDataGridRowInfo(DataGridValuesAdded, ref SelectedValueAddedId, "ارزش افزوده‌ای"))) return;
            var dialogResult = PersianMessageBox_Mhclass.Show("آیا از حذف ارزش افزوده‌ی انتخاب‌شده اطمینان دارید؟",
                "حذف ارزش افزوده", PersianMessageBox_Mhclass.MsgMhButton.YesNo, PersianMessageBox_Mhclass.MsgMhIcon.Question);
            if (dialogResult == MessageBoxResult.No) return;
            var threadDeleteValueAdded = new Thread(DeleteValueAdded);
            threadDeleteValueAdded.Start();
            _waitWindowDeleteValueAdded = new WaitWindow { Owner = this };
            _waitWindowDeleteValueAdded.ShowDialog();
            ShowValuesAdded(CalculateBaseHelper.DataTableValuesAdded.Copy(), _searchTextValueAdded, DataGridValuesAdded);
        }

        #endregion

        #region ButtonNewVersion_Click

        private void ButtonNewVersion_Click(object sender, RoutedEventArgs e)
        {
            ChangeVersionsMode = CommonEnum.ChangeDatabaseMode.InsertInToDatabase;
            var addEditVersionWindow = new AddEditVersionWindow { Owner = this };
            addEditVersionWindow.ShowDialog();
            ShowVersions(DataTableVersions.Copy(), _searchTextVersion, DataGridVersions);
            DataGridVersions.SelectDataGridRow(SelectedVersionId);
        }

        #endregion

        #region ButtonEditVersion_Click

        private void ButtonEditVersion_Click(object sender, RoutedEventArgs e)
        {
            if (!Convert.ToBoolean(WpfHelper.GetSelectedDataGridRowInfo(DataGridVersions, ref SelectedVersionId, "نسخه‌ای"))) return;
            ChangeVersionsMode = CommonEnum.ChangeDatabaseMode.UpdateDatabase;
            var aAddEditVersionWindow = new AddEditVersionWindow { Owner = this };
            aAddEditVersionWindow.ShowDialog();
            ShowVersions(DataTableVersions.Copy(), _searchTextVersion, DataGridVersions);
            DataGridVersions.SelectDataGridRow(SelectedVersionId);
        }

        #endregion

        #region ButtonDeleteVersion_Click

        private void ButtonDeleteVersion_Click(object sender, RoutedEventArgs e)
        {
            if (!Convert.ToBoolean(WpfHelper.GetSelectedDataGridRowInfo(DataGridVersions, ref SelectedVersionId, "نسخه‌ای"))) return;

            var dialogResult = PersianMessageBox_Mhclass.Show("آیا از حذف نسخه‌ی انتخاب‌شده اطمینان دارید؟",
                "حذف نسخه", PersianMessageBox_Mhclass.MsgMhButton.YesNo, PersianMessageBox_Mhclass.MsgMhIcon.Question);
            if (dialogResult == MessageBoxResult.No) return;

            dialogResult = PersianMessageBox_Mhclass.Show("با حذف این نسخه تمامی اطلاعات مربوط به ویژگی‌های آن حذف خواهند شد؟\n" +
                                             "آیا آین مورد را تأیید می‌کنید؟",
                "حذف نسخه", PersianMessageBox_Mhclass.MsgMhButton.YesNo, PersianMessageBox_Mhclass.MsgMhIcon.Question);
            if (dialogResult == MessageBoxResult.No) return;
            var threadDeleteVersion = new Thread(DeleteVersion);
            threadDeleteVersion.Start();
            _waitWindowDeleteVersion = new WaitWindow { Owner = this };
            _waitWindowDeleteVersion.ShowDialog();
            ShowVersions(DataTableVersions.Copy(), _searchTextVersion, DataGridVersions);
        }

        #endregion

        #region ButtonNewVersionFeature_Click

        private void ButtonNewVersionFeature_Click(object sender, RoutedEventArgs e)
        {
            if (!Convert.ToBoolean(WpfHelper.GetSelectedDataGridRowInfo(DataGridVersions, ref SelectedVersionId, "نسخه‌ای"))) return;
            ChangeVersionsFeaturesMode = CommonEnum.ChangeDatabaseMode.InsertInToDatabase;
            var addEditVersionFeatureWindow = new AddEditVersionFeatureWindow { Owner = this };
            addEditVersionFeatureWindow.ShowDialog();
            ShowVersionFeatures(DataTableVersionFeatures.Copy(), _searchTextVersionFeature, DataGridVersionFeatures);
            DataGridVersionFeatures.SelectDataGridRow(SelectedVersionFeatureId);
        }

        #endregion

        #region ButtonEditVersionFeature_Click

        private void ButtonEditVersionFeature_Click(object sender, RoutedEventArgs e)
        {
            if (!Convert.ToBoolean(WpfHelper.GetSelectedDataGridRowInfo(DataGridVersionFeatures, ref SelectedVersionFeatureId, "ویژگی نسخه‌ای"))) return;
            ChangeVersionsFeaturesMode = CommonEnum.ChangeDatabaseMode.UpdateDatabase;
            var addEditVersionFeatureWindow = new AddEditVersionFeatureWindow { Owner = this };
            addEditVersionFeatureWindow.ShowDialog();
            ShowVersionFeatures(DataTableVersionFeatures.Copy(), _searchTextVersionFeature, DataGridVersionFeatures);
            DataGridVersionFeatures.SelectDataGridRow(SelectedVersionFeatureId);
        }

        #endregion

        #region ButtonDeleteVersionFeature_Click

        private void ButtonDeleteVersionFeature_Click(object sender, RoutedEventArgs e)
        {
            if (!Convert.ToBoolean(WpfHelper.GetSelectedDataGridRowInfo(DataGridVersionFeatures, ref SelectedVersionFeatureId, "ویژگی نسخه‌ای"))) return;

            var dialogResult = PersianMessageBox_Mhclass.Show("آیا از حذف ویژگی نسخه‌ی انتخاب‌شده اطمینان دارید؟",
                "حذف ویژگی نسخه", PersianMessageBox_Mhclass.MsgMhButton.YesNo, PersianMessageBox_Mhclass.MsgMhIcon.Question);
            if (dialogResult == MessageBoxResult.No) return;

            var threadDeleteVersionFeature = new Thread(DeleteVersionFeature);
            threadDeleteVersionFeature.Start();
            _waitWindowDeleteVersionFeature = new WaitWindow { Owner = this };
            _waitWindowDeleteVersionFeature.ShowDialog();
            ShowVersionFeatures(DataTableVersionFeatures.Copy(), _searchTextVersionFeature, DataGridVersionFeatures);
        }

        #endregion

        #endregion
    }
}
