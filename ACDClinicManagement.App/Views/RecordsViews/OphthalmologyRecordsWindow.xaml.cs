using ACDClinicManagement.App.Views.CommonViews;
using ACDClinicManagement.App.Views.DailyReferencesViews;
using ACDClinicManagement.AppHelpers.CommonHelpers;
using ACDClinicManagement.Common.Enums;
using ACDClinicManagement.Common.Helpers;
using ACDClinicManagement.Helpers;
using MhclassLib;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ACDClinicManagement.App.Views.RecordsViews
{
    /// <summary>
    /// Interaction logic for OphthalmologyRecordsWindow.xaml
    /// </summary>
    public partial class OphthalmologyRecordsWindow : Window
    {
        public OphthalmologyRecordsWindow()
        {
            InitializeComponent();
            InitializeObjects();
        }

        // ••••••••••••
        // DEFINATIONS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region Windows

        private WaitWindow _waitWindowLoadData, _waitWindowDeleteRecord;

        #endregion

        #region Classes


        #endregion

        #region Objects

        public static CommonEnum.ChangeDatabaseMode ChangeRecordMode { get; set; }

        public static DataTable DataTableRecords;

        private DataTable _dataTableRecords;

        #endregion

        #region Variables

        public static int SelectedPersonId, SelectedRecordId;

        public static string SelectedPersonTitle, SelectedPersonAge, SearchTextRecord;


        #endregion

        // ••••••••••••
        // METHODS     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region private void InitializeObjects()

        private void InitializeObjects()
        {
            _dataTableRecords = new DataTable();
            _dataTableRecords.Columns.Add("Id", typeof(int));
            _dataTableRecords.Columns.Add("Re-OD", typeof(string));
            _dataTableRecords.Columns.Add("Re-OS", typeof(string));
            _dataTableRecords.Columns.Add("To-OD", typeof(string));
            _dataTableRecords.Columns.Add("To-OS", typeof(string));
            _dataTableRecords.Columns.Add("Cl-OD", typeof(string));
            _dataTableRecords.Columns.Add("Cl-OS", typeof(string));
            _dataTableRecords.Columns.Add("Operation", typeof(string));
            _dataTableRecords.Columns.Add("Note", typeof(string));
            _dataTableRecords.Columns.Add("تاریخ و زمان ایجاد", typeof(string));
            _dataTableRecords.Columns.Add("تاریخ و زمان ویرایش", typeof(string));
        }

        #endregion

        #region private void LoadDefaults()

        private void LoadDefaults()
        {
            InputLanguageHelper.LoadEnglishKeyboardLayout();
            this.ShowWindow(CommonEnum.WindowStyleMode.Owned);
            if (Owner is DailyReferencesWindow)
                TextBlockPersonTitle.Text = $"{DailyReferencesWindow.SelectedPersonTitle}, {DailyReferencesWindow.SelectedPersonAge.ToFarsiFormat()}";
            SearchTextRecord = "";
            var threadLoadData = new Thread(LoadData);
            threadLoadData.Start();
            _waitWindowLoadData = new WaitWindow { Owner = this };
            _waitWindowLoadData.ShowDialog();
            ShowRecords(DataTableRecords, SearchTextRecord, DataGridRecords);
        }

        #endregion

        #region private void LoadData()

        private void LoadData()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                var dataAdapterRecords = new SqlDataAdapter("SELECT OphthalmologyRecords.Id, OphthalmologyRecords.REOD, OphthalmologyRecords.REOS, " +
                                                            "OphthalmologyRecords.TOOD, OphthalmologyRecords.TOOS, " +
                                                            "OphthalmologyRecords.CLOD, OphthalmologyRecords.CLOS, " +
                                                            "OphthalmologyRecords.OPERATION, OphthalmologyRecords.NOTE, " +
                                                            "OphthalmologyRecords.CreatedAt, OphthalmologyRecords.ModifiedAt " +
                                                            "FROM DailyReferences, OphthalmologyRecords " +
                                                            "WHERE DailyReferences.Id = OphthalmologyRecords.ReferenceId AND " +
                                                            "DailyReferences.PersonId = @PersonId " +
                                                            "ORDER BY DailyReferences.CreatedAt ASC",
                    MainWindow.PublicConnection);
                dataAdapterRecords.SelectCommand.Parameters.AddWithValue("@PersonId", SelectedPersonId);
                DataTableRecords = new DataTable();
                dataAdapterRecords.Fill(DataTableRecords);
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

        #region private void DeleteRecord()

        private void DeleteRecord()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                if (MainWindow.PublicConnection.DeleteSqlData("OphthalmologyRecords", "Id = " + SelectedRecordId))
                {
                    var dataAdapterRecords = new SqlDataAdapter("SELECT OphthalmologyRecords.Id, OphthalmologyRecords.REOD, OphthalmologyRecords.REOS, " +
                                                                "OphthalmologyRecords.TOOD, OphthalmologyRecords.TOOS, " +
                                                                "OphthalmologyRecords.CLOD, OphthalmologyRecords.CLOS, " +
                                                                "OphthalmologyRecords.OPERATION, OphthalmologyRecords.NOTE, " +
                                                                "OphthalmologyRecords.CreatedAt, OphthalmologyRecords.ModifiedAt " +
                                                                "FROM DailyReferences, OphthalmologyRecords " +
                                                                "WHERE DailyReferences.Id = OphthalmologyRecords.ReferenceId AND " +
                                                                "DailyReferences.PersonId = @PersonId " +
                                                                "ORDER BY DailyReferences.CreatedAt ASC",
                    MainWindow.PublicConnection);
                    dataAdapterRecords.SelectCommand.Parameters.AddWithValue("@PersonId", SelectedPersonId);
                    DataTableRecords = new DataTable();
                    dataAdapterRecords.Fill(DataTableRecords);
                    Dispatcher.Invoke(() => "سابقه‌ی مراجعه‌ی".DeletedMessage().ShowMessage());
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
                Dispatcher.Invoke(() => _waitWindowDeleteRecord.Close());
            }
        }

        #endregion

        #region private void ShowPeople(DataTable dataTable, string searchTerm, DataGrid dataGrid)
        private void ShowRecords(DataTable dataTable, string searchTerm, DataGrid dataGrid)
        {
            _dataTableRecords.Clear();
            foreach (DataRow data in dataTable.Rows)
                _dataTableRecords.Rows.Add(data["Id"],
                    data["REOD"].ToString(),
                    data["REOS"].ToString(),
                    data["TOOD"].ToString(),
                    data["TOOS"].ToString(),
                    data["CLOD"].ToString(),
                    data["CLOS"].ToString(),
                    data["OPERATION"].ToString(),
                    data["NOTE"].ToString(),
                    data["CreatedAt"].ToFarsiFormatDateTimeFromSql(),
                    data["ModifiedAt"].ToFarsiFormatDateTimeFromSql());
            dataGrid.ItemsSource = _dataTableRecords.Copy().DefaultView;
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

        #region Window_Closed
        private void Window_Closed(object sender, EventArgs e)
        {
            InputLanguageHelper.LoadPersianKeyboardLayout();
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

        #region DataGridRecords_SelectionChanged

        private void DataGridRecords_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridRecords.SelectedItem == null || DataGridRecords.SelectedItem.ToString().Contains("NewItemPlaceholder")) return;
            var row = (DataRowView)DataGridRecords.SelectedItem;
            SelectedRecordId = Convert.ToInt32(row["Id"]);
        }

        #endregion

        #region DataGridRecords_MouseDoubleClick
        private void DataGridRecords_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!Convert.ToBoolean(WpfHelper.GetSelectedDataGridRowInfo(DataGridRecords, ref SelectedRecordId, "سابقه‌ی مراجعه‌ای"))) return;
            ChangeRecordMode = CommonEnum.ChangeDatabaseMode.UpdateDatabase;
            var addEditOphthalmologyRecordWindow = new AddEditOphthalmologyRecordWindow { Owner = this };
            addEditOphthalmologyRecordWindow.ShowDialog();
            ShowRecords(DataTableRecords, SearchTextRecord, DataGridRecords);
            DataGridRecords.SelectDataGridRow(SelectedRecordId);
        }

        #endregion

        #endregion

        #region TextBox_Events

        #region TextBoxSearchRecord_TextChanged

        private void TextBoxSearchRecord_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchTextRecord = TextBoxSearchRecord.Text.Trim().ToCorrectKeYe();
        }

        #endregion

        #endregion

        #region Button_Events

        #region ButtonNewRecord_Click

        private void ButtonNewRecord_Click(object sender, RoutedEventArgs e)
        {
            ChangeRecordMode = CommonEnum.ChangeDatabaseMode.InsertInToDatabase;
            var addEditOphthalmologyRecordWindow = new AddEditOphthalmologyRecordWindow { Owner = this };
            addEditOphthalmologyRecordWindow.ShowDialog();
            ShowRecords(DataTableRecords, SearchTextRecord, DataGridRecords);
            DataGridRecords.SelectDataGridRow(SelectedRecordId);
        }

        #endregion

        #region ButtonEditRecord_Click

        private void ButtonEditRecord_Click(object sender, RoutedEventArgs e)
        {
            if (!Convert.ToBoolean(WpfHelper.GetSelectedDataGridRowInfo(DataGridRecords, ref SelectedRecordId, "سابقه‌ی مراجعه‌ای"))) return;
            ChangeRecordMode = CommonEnum.ChangeDatabaseMode.UpdateDatabase;
            var addEditOphthalmologyRecordWindow = new AddEditOphthalmologyRecordWindow { Owner = this };
            addEditOphthalmologyRecordWindow.ShowDialog();
            ShowRecords(DataTableRecords, SearchTextRecord, DataGridRecords);
            DataGridRecords.SelectDataGridRow(SelectedRecordId);
        }

        #endregion

        #region ButtonDeleteRecord_Click

        private void ButtonDeleteRecord_Click(object sender, RoutedEventArgs e)
        {
            if (!Convert.ToBoolean(WpfHelper.GetSelectedDataGridRowInfo(DataGridRecords, ref SelectedRecordId, "سابقه‌ی مراجعه‌ای"))) return;
            var dialogResult = PersianMessageBox_Mhclass.Show("آیا از حذف سابقه‌ی مراجعه‌ی انتخاب‌شده اطمینان دارید؟",
                "حذف سابقه‌ی مراجعه", PersianMessageBox_Mhclass.MsgMhButton.YesNo, PersianMessageBox_Mhclass.MsgMhIcon.Question);
            if (dialogResult == MessageBoxResult.No) return;
            var threadDeleteRecord = new Thread(DeleteRecord);
            threadDeleteRecord.Start();
            _waitWindowDeleteRecord = new WaitWindow { Owner = this };
            _waitWindowDeleteRecord.ShowDialog();
            ShowRecords(DataTableRecords, SearchTextRecord, DataGridRecords);
        }

        #endregion

        #endregion
    }
}
