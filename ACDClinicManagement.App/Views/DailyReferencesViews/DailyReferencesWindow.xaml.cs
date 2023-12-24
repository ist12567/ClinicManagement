using ACDClinicManagement.App.Views.CommonViews;
using ACDClinicManagement.App.Views.PeopleViews;
using ACDClinicManagement.App.Views.RecordsViews;
using ACDClinicManagement.AppHelpers;
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
using System.Windows.Media;

namespace ACDClinicManagement.App.Views.DailyReferencesViews
{
    /// <summary>
    /// Interaction logic for ReferencesWindow.xaml
    /// </summary>
    public partial class DailyReferencesWindow : Window
    {
        public DailyReferencesWindow()
        {
            InitializeComponent();
            InitializeObjects();
        }

        // ••••••••••••
        // DEFINATIONS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region Windows

        private WaitWindow _waitWindowLoadData, _waitWindowDeleteDailyReference;

        #endregion

        #region Classes


        #endregion

        #region Objects

        public static CommonEnum.ChangeDatabaseMode ChangeReferencesMode { get; set; }

        public static DataTable DataTableDailyReferences;
        public DataTable _dataTableDailyReferences;

        #endregion

        #region Variables

        public static CommonEnum.ServiceMode SelectedServiceMode;
        public static int SelectedPersonId, SelectedDailyReferenceId;
        public static string SelectedPersonTitle, SelectedPersonAge;
        private string _searchTextDailyReference;
        public static PersianDateTime SelectedDateTime;


        #endregion

        // ••••••••••••
        // METHODS     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region private void InitializeObjects()

        private void InitializeObjects()
        {
            DataTableDailyReferences = new DataTable();

            _dataTableDailyReferences = new DataTable();
            _dataTableDailyReferences.Columns.Add("Id", typeof(int));
            _dataTableDailyReferences.Columns.Add("PersonId", typeof(int));
            _dataTableDailyReferences.Columns.Add("کد", typeof(string));
            _dataTableDailyReferences.Columns.Add("نام", typeof(string));
            _dataTableDailyReferences.Columns.Add("نام خانوادگی", typeof(string));
            _dataTableDailyReferences.Columns.Add("سال تولد", typeof(string));
            _dataTableDailyReferences.Columns.Add("Age", typeof(string));
            _dataTableDailyReferences.Columns.Add("وضعیت ویزیت", typeof(string));
            _dataTableDailyReferences.Columns.Add("VisitStatusMode", typeof(int));
            _dataTableDailyReferences.Columns.Add("تعداد مراجعات قبلی", typeof(string));
            _dataTableDailyReferences.Columns.Add("تاریخ و زمان ایجاد", typeof(string));
            _dataTableDailyReferences.Columns.Add("تاریخ و زمان ویرایش", typeof(string));
        }

        #endregion

        #region private void LoadDefaults()

        private void LoadDefaults()
        {
            this.ShowWindow(CommonEnum.WindowStyleMode.Normal);
            SelectedPersonId = 0;
            _searchTextDailyReference = "";
            TextBlockCurrentDate.Text = SpecialAppHelper.CurrentDateTime.ToFarsiFormatDate();
            TextBlockCurrentDate.Tag = SpecialAppHelper.CurrentDateTime.ToOnlyDateFormat();
            SelectedDateTime = SpecialAppHelper.CurrentDateTime;
            var threadLoadData = new Thread(() => LoadData(0));
            threadLoadData.Start();
            _waitWindowLoadData = new WaitWindow { Owner = this };
            _waitWindowLoadData.ShowDialog();
            ShowReferences(DataTableDailyReferences.Copy(), _searchTextDailyReference, DataGridDailyReferences);
        }

        #endregion

        #region private void LoadData(int personId)

        private void LoadData(int personId)
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                // Insert to DailyReferences
                if (personId != 0)
                {
                    var commandCheckInfo = new SqlCommand("SELECT Id FROM DailyReferences " +
                                                          "WHERE PersonId = @PersonId AND " +
                                                          "Date = @Date",
                            MainWindow.PublicConnection);
                    commandCheckInfo.Parameters.AddWithValue("@PersonId", personId);
                    commandCheckInfo.Parameters.AddWithValue("@Date", SelectedDateTime.ToOnlyDateFormat());
                    var checkResult = commandCheckInfo.ExecuteScalar();
                    if (checkResult != null && checkResult != DBNull.Value)
                    {
                        SelectedDailyReferenceId = Convert.ToInt32(checkResult);
                        Dispatcher.Invoke(() => "شخص موردنظر قبلا به لیست ویزیت اضافه شده است".ShowMessage());
                    }
                    else
                    {
                        var maxRecord = Convert.ToInt32(SqlHelper.MaxSqlRecord(MainWindow.PublicConnection, "DailyReferences")) + 1;
                        var data = new object[]
                            {
                            maxRecord,
                            SpecialBaseHelper.ProvinceId,
                            SpecialBaseHelper.CityId,
                            SpecialBaseHelper.OrganizationId,
                            SpecialBaseHelper.SubsetId,
                            personId,
                            SelectedDateTime.ToOnlyDateFormat(),
                            Convert.ToInt16(CommonEnum.VisitStatusMode.WaitingForVisit),
                            SpecialBaseHelper.UserId,
                            DateTime.Now,
                            SpecialBaseHelper.UserId,
                            DateTime.Now
                            };
                        if (MainWindow.PublicConnection.InsertSqlData("DailyReferences", data))
                            SelectedDailyReferenceId = maxRecord;
                    }
                }
                // Get References
                var dataAdapterReferences = new SqlDataAdapter("SELECT DailyReferences.Id, People.Id AS PersonId, " +
                                                               "People.Code, People.FirstName, People.LastName, " +
                                                               "People.BirthDate, DailyReferences.VisitStatusMode, " +
                                                               "(SELECT COUNT(*) FROM DailyReferences WHERE PersonId = People.Id) AS ReferencesCount, " +
                                                               "DailyReferences.CreatedAt, DailyReferences.ModifiedAt " +
                                                               "FROM People, DailyReferences " +
                                                               "WHERE People.Id = DailyReferences.PersonId AND " +
                                                               "DailyReferences.Date = @Date " +
                                                               "ORDER BY DailyReferences.VisitStatusMode ASC, DailyReferences.ModifiedAt ASC",
                    MainWindow.PublicConnection);
                dataAdapterReferences.SelectCommand.Parameters.AddWithValue("@Date", SelectedDateTime.ToOnlyDateFormat());
                DataTableDailyReferences = new DataTable();
                dataAdapterReferences.Fill(DataTableDailyReferences);
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

        #region private void DeleteDailyReference()

        private void DeleteDailyReference()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                var commandRemoveData = new SqlCommand("DELETE FROM DailyReferences " +
                                                       "WHERE Id = @Id",
                    MainWindow.PublicConnection);
                commandRemoveData.Parameters.AddWithValue("@Id", SelectedDailyReferenceId);
                if (commandRemoveData.ExecuteNonQuery() > 0)
                {
                    var dataAdapterReferences = new SqlDataAdapter("SELECT DailyReferences.Id, People.Id AS PersonId, " +
                                                                   "People.Code, People.FirstName, People.LastName, " +
                                                                   "People.BirthDate, DailyReferences.VisitStatusMode, " +
                                                                   "(SELECT COUNT(*) FROM DailyReferences WHERE PersonId = People.Id) AS ReferencesCount, " +
                                                                   "DailyReferences.CreatedAt, DailyReferences.ModifiedAt " +
                                                                   "FROM People, DailyReferences " +
                                                                   "WHERE People.Id = DailyReferences.PersonId AND " +
                                                                   "DailyReferences.Date = @Date " +
                                                                   "ORDER BY DailyReferences.VisitStatusMode ASC, DailyReferences.ModifiedAt ASC",
                    MainWindow.PublicConnection);
                    dataAdapterReferences.SelectCommand.Parameters.AddWithValue("@Date", SelectedDateTime.ToOnlyDateFormat());
                    DataTableDailyReferences = new DataTable();
                    dataAdapterReferences.Fill(DataTableDailyReferences);
                    Dispatcher.Invoke(() => "مراجعه‌ی".DeletedMessage().ShowMessage());
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
                Dispatcher.Invoke(() => _waitWindowDeleteDailyReference.Close());
            }
        }

        #endregion

        #region private void ShowReferences(DataTable dataTable, string searchTerm, DataGrid dataGrid)
        private void ShowReferences(DataTable dataTable, string searchTerm, DataGrid dataGrid)
        {
            _dataTableDailyReferences.Clear();
            foreach (DataRow data in dataTable.Rows)
                _dataTableDailyReferences.Rows.Add(data["Id"],
                    data["PersonId"],
                    data["Code"].ToString().ToFarsiFormat(),
                    data["FirstName"].ToString(),
                    data["LastName"].ToString(),
                    data["BirthDate"].ToString().Length == 10 ? data["BirthDate"].ToString().ToPersianDateTime().Year.ToString().ToFarsiFormat() : "",
                    data["BirthDate"].ToString().Length == 10 ? data["BirthDate"].ToString().ToPersianDateTime().Year.ToString() : "0",
                    ((CommonEnum.VisitStatusMode)Convert.ToInt16(data["VisitStatusMode"])).GetEnumDescription(),
                    data["VisitStatusMode"],
                    data["ReferencesCount"].ToString().ToFarsiFormat(),
                    data["CreatedAt"].ToFarsiFormatDateTimeFromSql(),
                    data["ModifiedAt"].ToFarsiFormatDateTimeFromSql());
            dataGrid.ItemsSource = _dataTableDailyReferences.Copy().DefaultView;
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
            if (e.Column.Header.ToString() == "Id" || e.Column.Header.ToString() == "PersonId" ||
                e.Column.Header.ToString() == "VisitStatusMode" || e.Column.Header.ToString() == "Age")
                e.Column.Visibility = Visibility.Hidden;
        }

        #endregion

        #region DataGridDailyReferences_LoadingRow
        private void DataGridDailyReferences_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            var rowView = e.Row.Item as DataRowView;
            switch ((CommonEnum.VisitStatusMode)Convert.ToInt16(rowView["VisitStatusMode"]))
            {
                case CommonEnum.VisitStatusMode.WaitingForVisit:
                    e.Row.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#f5a6a6");
                    break;
                case CommonEnum.VisitStatusMode.Visited:
                    e.Row.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#d0fac8");
                    break;
            }
        }

        #endregion

        #region DataGridDailyReference_SelectionChanged

        private void DataGridDailyReference_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridDailyReferences.SelectedItem == null || DataGridDailyReferences.SelectedItem.ToString().Contains("NewItemPlaceholder")) return;
            var row = (DataRowView)DataGridDailyReferences.SelectedItem;
            SelectedDailyReferenceId = Convert.ToInt32(row["Id"]);
            SelectedPersonId = Convert.ToInt32(row["PersonId"]);
            SelectedPersonTitle = $"{row["نام"]} {row["نام خانوادگی"]}";
            SelectedPersonAge = $"{SpecialAppHelper.CurrentDateTime.Year - Convert.ToInt32(row["Age"])} ساله";
            SelectedServiceMode = CommonEnum.ServiceMode.Ophthalmology;
        }

        #endregion

        #region DataGridDailyReferences_MouseDoubleClick
        private void DataGridDailyReferences_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!Convert.ToBoolean(WpfHelper.GetSelectedDataGridRowInfo(DataGridDailyReferences, ref SelectedDailyReferenceId, "مراجعه‌ای"))) return;

            OphthalmologyRecordsWindow.SelectedPersonId = SelectedPersonId;
            OphthalmologyRecordsWindow.SelectedPersonTitle = SelectedPersonTitle;
            OphthalmologyRecordsWindow.SelectedPersonAge = SelectedPersonAge;
            var ophthalmologyRecordsWindow = new OphthalmologyRecordsWindow { Owner = this };
            ophthalmologyRecordsWindow.ShowDialog();
        }

        #endregion

        #endregion

        #region TextBlock_Events

        #region TextBlockCurrentDate_MouseLeftButtonDown
        private void TextBlockCurrentDate_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dateTimePickerWindow = new DateTimePickerWindow(TextBlockCurrentDate.Tag) { Owner = this };
            dateTimePickerWindow.ShowDialog();
            if (DateTimePickerWindow.Result)
            {
                TextBlockCurrentDate.Text = DateTimePickerWindow.PickedDateTime.ToFarsiFormatDate();
                TextBlockCurrentDate.Tag = DateTimePickerWindow.PickedDateTime.ToOnlyDateFormat();
                SelectedDateTime = TextBlockCurrentDate.Tag.ToString().ToPersianDateTime();
                var threadLoadData = new Thread(() => LoadData(0));
                threadLoadData.Start();
                _waitWindowLoadData = new WaitWindow { Owner = this };
                _waitWindowLoadData.ShowDialog();
                ShowReferences(DataTableDailyReferences.Copy(), _searchTextDailyReference, DataGridDailyReferences);
            }
        }

        #endregion

        #endregion

        #region TextBox_Events

        #region TextBoxSearchDailyReference_TextChanged

        private void TextBoxSearchDailyReference_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchTextDailyReference = TextBoxSearchDailyReference.Text.Trim().ToCorrectKeYe();
            ShowReferences(DataTableDailyReferences.Copy(), _searchTextDailyReference, DataGridDailyReferences);
        }

        #endregion

        #endregion

        #region Button_Events

        #region ButtonNewDailyReference_Click

        private void ButtonNewDailyReference_Click(object sender, RoutedEventArgs e)
        {
            SelectedDailyReferenceId = 0;
            SelectedPersonId = 0;
            ChangeReferencesMode = CommonEnum.ChangeDatabaseMode.InsertInToDatabase;
            var peopleWindow = new PeopleWindow { Owner = this };
            peopleWindow.ShowDialog();
            if (SelectedPersonId == 0) return;

            var threadLoadData = new Thread(() => LoadData(SelectedPersonId));
            threadLoadData.Start();
            _waitWindowLoadData = new WaitWindow { Owner = this };
            _waitWindowLoadData.ShowDialog();
            ShowReferences(DataTableDailyReferences.Copy(), _searchTextDailyReference, DataGridDailyReferences);
            DataGridDailyReferences.SelectDataGridRow(SelectedDailyReferenceId);
        }

        #endregion

        #region ButtonDeleteDailyReference_Click

        private void ButtonDeleteDailyReference_Click(object sender, RoutedEventArgs e)
        {
            if (!Convert.ToBoolean(WpfHelper.GetSelectedDataGridRowInfo(DataGridDailyReferences, ref SelectedDailyReferenceId, "مراجعه‌ای"))) return;
            var row = (DataRowView)DataGridDailyReferences.SelectedItem;
            if ((CommonEnum.VisitStatusMode)Convert.ToInt16(row["VisitStatusMode"]) == CommonEnum.VisitStatusMode.Visited)
            {
                $"مراجعه‌های با وضعیت  {CommonEnum.VisitStatusMode.Visited.GetEnumDescription()} قابل حذف نیستند".ShowMessage();
                return;
            }
            var dialogResult = PersianMessageBox_Mhclass.Show("آیا از حذف پرونده‌ی مراجعه‌ی انتخاب‌شده اطمینان دارید؟",
                "حذف مراجعه", PersianMessageBox_Mhclass.MsgMhButton.YesNo, PersianMessageBox_Mhclass.MsgMhIcon.Question);
            if (dialogResult == MessageBoxResult.No) return;
            var threadDeleteDailyReference = new Thread(DeleteDailyReference);
            threadDeleteDailyReference.Start();
            _waitWindowDeleteDailyReference = new WaitWindow { Owner = this };
            _waitWindowDeleteDailyReference.ShowDialog();
            ShowReferences(DataTableDailyReferences.Copy(), _searchTextDailyReference, DataGridDailyReferences);
        }

        #endregion

        #region ButtonShowReferenceRecords_Click
        private void ButtonShowReferenceRecords_Click(object sender, RoutedEventArgs e)
        {
            if (!Convert.ToBoolean(WpfHelper.GetSelectedDataGridRowInfo(DataGridDailyReferences, ref SelectedDailyReferenceId, "مراجعه‌ای"))) return;

            OphthalmologyRecordsWindow.SelectedPersonId = SelectedPersonId;
            OphthalmologyRecordsWindow.SelectedPersonTitle = SelectedPersonTitle;
            OphthalmologyRecordsWindow.SelectedPersonAge = SelectedPersonAge;
            var ophthalmologyRecordsWindow = new OphthalmologyRecordsWindow { Owner = this };
            ophthalmologyRecordsWindow.ShowDialog();
        }

        #endregion

        #region ButtonVisit_Click
        private void ButtonVisit_Click(object sender, RoutedEventArgs e)
        {
            if (!Convert.ToBoolean(WpfHelper.GetSelectedDataGridRowInfo(DataGridDailyReferences, ref SelectedDailyReferenceId, "مراجعه‌ای"))) return;
            var row = (DataRowView)DataGridDailyReferences.SelectedItem;
            if ((CommonEnum.VisitStatusMode)Convert.ToInt16(row["VisitStatusMode"]) != CommonEnum.VisitStatusMode.WaitingForVisit)
            {
                $"فقط مراجعه‌های با وضعیت  {CommonEnum.VisitStatusMode.WaitingForVisit.GetEnumDescription()} قابل ویزیت هستند".ShowMessage();
                return;
            }
            OphthalmologyRecordsWindow.ChangeRecordMode = CommonEnum.ChangeDatabaseMode.InsertInToDatabase;
            var addEditOphthalmologyRecordWindow = new AddEditOphthalmologyRecordWindow { Owner = this };
            addEditOphthalmologyRecordWindow.ShowDialog();
            ShowReferences(DataTableDailyReferences.Copy(), _searchTextDailyReference, DataGridDailyReferences);
            DataGridDailyReferences.SelectDataGridRow(SelectedDailyReferenceId);
        }

        #endregion

        #region ButtonRefresh_Click
        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            var threadLoadData = new Thread(() => LoadData(0));
            threadLoadData.Start();
            _waitWindowLoadData = new WaitWindow { Owner = this };
            _waitWindowLoadData.ShowDialog();
            ShowReferences(DataTableDailyReferences.Copy(), _searchTextDailyReference, DataGridDailyReferences);
        }

        #endregion

        #endregion
    }
}
