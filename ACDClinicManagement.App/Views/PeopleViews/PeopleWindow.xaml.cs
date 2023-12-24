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

namespace ACDClinicManagement.App.Views.PeopleViews
{
    /// <summary>
    /// Interaction logic for PeopleWindow.xaml
    /// </summary>
    public partial class PeopleWindow : Window
    {
        public PeopleWindow()
        {
            InitializeComponent();
            InitializeObjects();
        }

        // ••••••••••••
        // DEFINATIONS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region Windows

        private WaitWindow _waitWindowLoadData, _waitWindowDeletePerson;

        #endregion

        #region Classes


        #endregion

        #region Objects

        public static CommonEnum.ChangeDatabaseMode ChangePeopleMode { get; set; }

        public static DataTable DataTablePeople;

        private DataTable _dataTablePeople;

        #endregion

        #region Variables

        public static int SelectedPersonId;

        public static string SearchTextPerson;


        #endregion

        // ••••••••••••
        // METHODS     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region private void InitializeObjects()

        private void InitializeObjects()
        {
            _dataTablePeople = new DataTable();
            _dataTablePeople.Columns.Add("Id", typeof(int));
            _dataTablePeople.Columns.Add("کد", typeof(string));
            _dataTablePeople.Columns.Add("جنسیت", typeof(string));
            _dataTablePeople.Columns.Add("نام", typeof(string));
            _dataTablePeople.Columns.Add("نام خانوادگی", typeof(string));
            _dataTablePeople.Columns.Add("سال تولد", typeof(string));
            _dataTablePeople.Columns.Add("کد ملی", typeof(string));
            _dataTablePeople.Columns.Add("آدرس", typeof(string));
            _dataTablePeople.Columns.Add("توضیحات", typeof(string));
            _dataTablePeople.Columns.Add("تاریخ و زمان ایجاد", typeof(string));
            _dataTablePeople.Columns.Add("تاریخ و زمان ویرایش", typeof(string));
        }

        #endregion

        #region private void LoadDefaults()

        private void LoadDefaults()
        {
            InputLanguageHelper.LoadPersianKeyboardLayout();
            if (Owner is MainWindow)
            {
                this.ShowWindow(CommonEnum.WindowStyleMode.Normal);
                ButtonSelectPerson.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.ShowWindow(CommonEnum.WindowStyleMode.Owned);
                ButtonSelectPerson.Visibility = Visibility.Visible;
                TextBoxSearchPerson.Focus();
            }
            SearchTextPerson = "";
            var threadLoadData = new Thread(LoadData);
            threadLoadData.Start();
            _waitWindowLoadData = new WaitWindow { Owner = this };
            _waitWindowLoadData.ShowDialog();
            ShowPeople(DataTablePeople, SearchTextPerson, DataGridPeople);
        }

        #endregion

        #region private void LoadData()

        private void LoadData()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                var dataAdapterPeople = new SqlDataAdapter("SELECT TOP(500) * FROM People " +
                                                           "WHERE Id <> 0 AND " +
                                                           "(CAST(Code AS NVARCHAR) = @Code OR " +
                                                           "CAST(OldCode AS NVARCHAR) = @OldCode OR " +
                                                           "FirstName LIKE Concat('%', @FirstName, '%') OR " +
                                                           "LastName LIKE Concat('%', @LastName, '%') OR " +
                                                           "(FirstName + ' ' + LastName) LIKE Concat('%', @FirstNameLastName, '%') OR " +
                                                           "NationalCode = @NationalCode) " +
                                                           "ORDER BY ModifiedAt DESC",
                    MainWindow.PublicConnection);
                dataAdapterPeople.SelectCommand.Parameters.AddWithValue("@Code", SearchTextPerson);
                dataAdapterPeople.SelectCommand.Parameters.AddWithValue("@OldCode", SearchTextPerson);
                dataAdapterPeople.SelectCommand.Parameters.AddWithValue("@FirstName", SearchTextPerson);
                dataAdapterPeople.SelectCommand.Parameters.AddWithValue("@LastName", SearchTextPerson);
                dataAdapterPeople.SelectCommand.Parameters.AddWithValue("@FirstNameLastName", SearchTextPerson);
                dataAdapterPeople.SelectCommand.Parameters.AddWithValue("@NationalCode", SearchTextPerson);
                DataTablePeople = new DataTable();
                dataAdapterPeople.Fill(DataTablePeople);
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

        #region private void DeletePerson()

        private void DeletePerson()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                if (MainWindow.PublicConnection.DeleteSqlData("People", "Id = " + SelectedPersonId))
                {
                    var dataAdapterPeople = new SqlDataAdapter("SELECT TOP(500) * FROM People " +
                                                               "WHERE Id <> 0 AND " +
                                                               "(CAST(Code AS NVARCHAR) = @Code OR " +
                                                               "CAST(OldCode AS NVARCHAR) = @OldCode OR " +
                                                               "FirstName LIKE Concat('%', @FirstName, '%') OR " +
                                                               "LastName LIKE Concat('%', @LastName, '%') OR " +
                                                               "(FirstName + ' ' + LastName) LIKE Concat('%', @FirstNameLastName, '%') OR " +
                                                               "NationalCode = @NationalCode) " +
                                                               "ORDER BY ModifiedAt DESC",
                    MainWindow.PublicConnection);
                    dataAdapterPeople.SelectCommand.Parameters.AddWithValue("@Code", SearchTextPerson);
                    dataAdapterPeople.SelectCommand.Parameters.AddWithValue("@OldCode", SearchTextPerson);
                    dataAdapterPeople.SelectCommand.Parameters.AddWithValue("@FirstName", SearchTextPerson);
                    dataAdapterPeople.SelectCommand.Parameters.AddWithValue("@LastName", SearchTextPerson);
                    dataAdapterPeople.SelectCommand.Parameters.AddWithValue("@FirstNameLastName", SearchTextPerson);
                    dataAdapterPeople.SelectCommand.Parameters.AddWithValue("@NationalCode", SearchTextPerson);
                    DataTablePeople = new DataTable();
                    dataAdapterPeople.Fill(DataTablePeople);
                    Dispatcher.Invoke(() => "شخص".DeletedMessage().ShowMessage());
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
                Dispatcher.Invoke(() => _waitWindowDeletePerson.Close());
            }
        }

        #endregion

        #region private void ShowPeople(DataTable dataTable, string searchTerm, DataGrid dataGrid)
        private void ShowPeople(DataTable dataTable, string searchTerm, DataGrid dataGrid)
        {
            _dataTablePeople.Clear();
            foreach (DataRow data in dataTable.Rows)
                _dataTablePeople.Rows.Add(data["Id"],
                    data["Code"].ToString().ToFarsiFormat(),
                    ((CommonEnum.GenderMode)Convert.ToInt16(data["GenderMode"])).GetEnumDescription(),
                    data["FirstName"].ToString().ToFarsiFormat(),
                    data["LastName"].ToString().ToFarsiFormat(),
                    data["BirthDate"].ToString().Length == 10 ? data["BirthDate"].ToString().ToPersianDateTime().Year.ToString().ToFarsiFormat() : "",
                    data["NationalCode"].ToString().ToFarsiFormat(),
                    data["Address"].ToString().ToFarsiFormat(),
                    data["Comments"].ToString().ToFarsiFormat(),
                    data["CreatedAt"].ToFarsiFormatDateTimeFromSql(),
                    data["ModifiedAt"].ToFarsiFormatDateTimeFromSql());
            dataGrid.ItemsSource = _dataTablePeople.Copy().DefaultView;
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

        #region Window_Closed
        private void Window_Closed(object sender, EventArgs e)
        {
            InputLanguageHelper.LoadEnglishKeyboardLayout();
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

        #region DataGridPeople_MouseDoubleClick
        private void DataGridPeople_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Owner is MainWindow) return;
            if (DataGridPeople.SelectedItem == null || DataGridPeople.SelectedItem.ToString().Contains("NewItemPlaceholder"))
            {
                "شخصی".NotSelectedMessage().ShowMessage();
                return;
            }
            var row = (DataRowView)DataGridPeople.SelectedItem;
            DailyReferencesWindow.SelectedPersonId = Convert.ToInt32(row["Id"]);
            Close();
        }

        #endregion

        #endregion

        #region TextBox_Events

        #region TextBoxSearchPerson_TextChanged

        private void TextBoxSearchPerson_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchTextPerson = TextBoxSearchPerson.Text.Trim().ToCorrectKeYe();
        }

        #endregion

        #region TextBoxSearchPerson_KeyDown
        private void TextBoxSearchPerson_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            var threadLoadData = new Thread(LoadData);
            threadLoadData.Start();
            _waitWindowLoadData = new WaitWindow { Owner = this };
            _waitWindowLoadData.ShowDialog();
            ShowPeople(DataTablePeople, SearchTextPerson, DataGridPeople);
            if (DataGridPeople.Items.Count == 1)
            {
                var row = (DataRowView)DataGridPeople.Items[0];
                SelectedPersonId = Convert.ToInt32(row["Id"]);
                WpfHelper.SelectDataGridRow(DataGridPeople, SelectedPersonId);
            }
        }

        #endregion

        #endregion

        #region Button_Events

        #region ButtonNewPerson_Click

        private void ButtonNewPerson_Click(object sender, RoutedEventArgs e)
        {
            ChangePeopleMode = CommonEnum.ChangeDatabaseMode.InsertInToDatabase;
            var addEditPersonWindow = new AddEditPersonWindow { Owner = this };
            addEditPersonWindow.ShowDialog();
            ShowPeople(DataTablePeople, SearchTextPerson, DataGridPeople);
            DataGridPeople.SelectDataGridRow(SelectedPersonId);
        }

        #endregion

        #region ButtonEditPerson_Click

        private void ButtonEditPerson_Click(object sender, RoutedEventArgs e)
        {
            if (!Convert.ToBoolean(WpfHelper.GetSelectedDataGridRowInfo(DataGridPeople, ref SelectedPersonId, "شخصی"))) return;
            ChangePeopleMode = CommonEnum.ChangeDatabaseMode.UpdateDatabase;
            var addEditPersonWindow = new AddEditPersonWindow { Owner = this };
            addEditPersonWindow.ShowDialog();
            ShowPeople(DataTablePeople, SearchTextPerson, DataGridPeople);
            DataGridPeople.SelectDataGridRow(SelectedPersonId);
        }

        #endregion

        #region ButtonDeletePerson_Click

        private void ButtonDeletePerson_Click(object sender, RoutedEventArgs e)
        {
            if (!Convert.ToBoolean(WpfHelper.GetSelectedDataGridRowInfo(DataGridPeople, ref SelectedPersonId, "شخصی"))) return;
            var dialogResult = PersianMessageBox_Mhclass.Show("آیا از حذف شخص انتخاب‌شده اطمینان دارید؟",
                "حذف شخص", PersianMessageBox_Mhclass.MsgMhButton.YesNo, PersianMessageBox_Mhclass.MsgMhIcon.Question);
            if (dialogResult == MessageBoxResult.No) return;
            var threadDeletePerson = new Thread(DeletePerson);
            threadDeletePerson.Start();
            _waitWindowDeletePerson = new WaitWindow { Owner = this };
            _waitWindowDeletePerson.ShowDialog();
            ShowPeople(DataTablePeople, SearchTextPerson, DataGridPeople);
        }

        #endregion

        #region ButtonSelectPerson_Click

        private void ButtonSelectPerson_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridPeople.SelectedItem == null || DataGridPeople.SelectedItem.ToString().Contains("NewItemPlaceholder"))
            {
                "شخصی".NotSelectedMessage().ShowMessage();
                return;
            }
            var row = (DataRowView)DataGridPeople.SelectedItem;
            DailyReferencesWindow.SelectedPersonId = Convert.ToInt32(row["Id"]);
            Close();
        }

        #endregion

        #endregion
    }
}
