using ACDClinicManagement.App.Views.CommonViews;
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

namespace ACDClinicManagement.App.Views.UsersViews
{
    /// <summary>
    /// Interaction logic for UsersWindow.xaml
    /// </summary>
    public partial class UsersWindow
    {
        public UsersWindow()
        {
            InitializeComponent();
            InitializeObjects();
        }

        // ••••••••••••
        // DEFINATIONS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region Windows

        private WaitWindow _waitWindowLoadData, _waitWindowDeleteUser;

        #endregion

        #region Classes


        #endregion

        #region Objects

        public static DataTable DataTableUsers;
        private DataTable _dataTableUsers;
        public static CommonEnum.ChangeDatabaseMode ChangeUsersMode { get; private set; }

        #endregion

        #region Variables

        public static int SelectedUserId;
        private string _searchText;

        #endregion

        // ••••••••••••
        // METHODS     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region private void InitializeObjects()

        private void InitializeObjects()
        {
            _dataTableUsers = new DataTable();
            _dataTableUsers.Columns.Add("Id", typeof(int));
            _dataTableUsers.Columns.Add("نوع کاربری", typeof(string));
            _dataTableUsers.Columns.Add("نام", typeof(string));
            _dataTableUsers.Columns.Add("نام خانوادگی", typeof(string));
            _dataTableUsers.Columns.Add("نام کاربری", typeof(string));
            _dataTableUsers.Columns.Add("استان", typeof(string));
            _dataTableUsers.Columns.Add("شهر", typeof(string));
            _dataTableUsers.Columns.Add("سازمان", typeof(string));
            _dataTableUsers.Columns.Add("زیرمجموعه", typeof(string));
            _dataTableUsers.Columns.Add("وضعیت", typeof(string));
            _dataTableUsers.Columns.Add("تاریخ و زمان ایجاد", typeof(string));
            _dataTableUsers.Columns.Add("تاریخ و زمان ویرایش", typeof(string));
        }

        #endregion

        #region private void LoadDefaults()

        private void LoadDefaults()
        {
            this.ShowWindow(CommonEnum.WindowStyleMode.Normal);
            _searchText = "";
            var threadLoadData = new Thread(LoadData);
            threadLoadData.Start();
            _waitWindowLoadData = new WaitWindow { Owner = this };
            _waitWindowLoadData.ShowDialog();
            ShowUsers(DataTableUsers, _searchText, DataGridUsers);
        }

        #endregion

        #region private void LoadData()

        private void LoadData()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                var dataAdapterUsers = new SqlDataAdapter("SELECT Users.Id, TypeMode, " +
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
                dataAdapterUsers.SelectCommand.Parameters.AddWithValue("@ProvinceId", SpecialBaseHelper.ProvinceId);
                dataAdapterUsers.SelectCommand.Parameters.AddWithValue("@CityId", SpecialBaseHelper.CityId);
                dataAdapterUsers.SelectCommand.Parameters.AddWithValue("@OrganizationId", SpecialBaseHelper.OrganizationId);
                dataAdapterUsers.SelectCommand.Parameters.AddWithValue("@SubsetId", SpecialBaseHelper.SubsetId);
                dataAdapterUsers.SelectCommand.Parameters.AddWithValue("@Id", SpecialBaseHelper.UserId);
                dataAdapterUsers.SelectCommand.Parameters.AddWithValue("@TypeMode", Convert.ToInt16(SpecialBaseHelper.CurrentUserType));
                DataTableUsers = new DataTable();
                dataAdapterUsers.Fill(DataTableUsers);

            }
            catch (Exception exception)
            {
                MainWindow.ErrorMessage = exception.Message;
            }
            finally
            {
                Dispatcher.Invoke(_waitWindowLoadData.Close);
            }
        }

        #endregion

        #region private void DeleteUser()

        private void DeleteUser()
        {
            try
            {
                if (MainWindow.PublicConnection.AnyActionInDatabaseByUser(SelectedUserId))
                {
                    Dispatcher.Invoke(() => "حداقل یک عملیات به وسیله این کاربر در پایگاه‌داده ثبت شده است، لذا قابل حذف نیست".ShowMessage());
                    return;
                }
                if (MainWindow.PublicConnection.DeleteSqlData("Users", "Id = " + SelectedUserId))
                {
                    var dataAdapterUsers = new SqlDataAdapter("SELECT Users.Id, TypeMode, " +
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
                    dataAdapterUsers.SelectCommand.Parameters.AddWithValue("@ProvinceId", SpecialBaseHelper.ProvinceId);
                    dataAdapterUsers.SelectCommand.Parameters.AddWithValue("@CityId", SpecialBaseHelper.CityId);
                    dataAdapterUsers.SelectCommand.Parameters.AddWithValue("@OrganizationId", SpecialBaseHelper.OrganizationId);
                    dataAdapterUsers.SelectCommand.Parameters.AddWithValue("@SubsetId", SpecialBaseHelper.SubsetId);
                    dataAdapterUsers.SelectCommand.Parameters.AddWithValue("@Id", SpecialBaseHelper.UserId);
                    dataAdapterUsers.SelectCommand.Parameters.AddWithValue("@TypeMode", Convert.ToInt16(SpecialBaseHelper.CurrentUserType));
                    DataTableUsers = new DataTable();
                    dataAdapterUsers.Fill(DataTableUsers);
                    Dispatcher.Invoke(() => "حساب کاربری".DeletedMessage().ShowMessage());
                }
                else
                    Dispatcher.Invoke(() => WpfHelper.GetErrorMessage().ShowMessage());
            }
            catch (Exception exception)
            {
                MainWindow.ErrorMessage = exception.Message;
                Dispatcher.Invoke(() => WpfHelper.GetErrorMessage().ShowMessage());
            }
            finally
            {
                Dispatcher.Invoke(() => _waitWindowDeleteUser.Close());
            }
        }

        #endregion

        #region private void ShowUsers(DataTable dataTable, string searchTerm, DataGrid dataGrid)
        private void ShowUsers(DataTable dataTable, string searchTerm, DataGrid dataGrid)
        {
            _dataTableUsers.Clear();
            foreach (DataRow data in dataTable.Rows)
                if (data["FirstName"].ToString().ToCorrectKeYe().ToLower().Contains(searchTerm.ToLower()) || data["LastName"].ToString().ToCorrectKeYe().ToLower().Contains(searchTerm.ToLower()))
                    _dataTableUsers.Rows.Add(data["Id"],
                        ((CommonEnum.UserType)Convert.ToInt16(data["TypeMode"])).GetEnumDescription(),
                        data["FirstName"].ToString().ToFarsiFormat(),
                        data["LastName"].ToString().ToFarsiFormat(),
                        data["UserName"].ToString(),
                        data["Province"].ToString() == "... انتخاب کنید ..." ? "" : data["Province"].ToString().ToFarsiFormat(),
                        data["City"].ToString() == "... انتخاب کنید ..." ? "" : data["City"].ToString().ToFarsiFormat(),
                        data["Organization"].ToString() == "... انتخاب کنید ..." ? "" : data["Organization"].ToString().ToFarsiFormat(),
                        data["Subset"].ToString() == "... انتخاب کنید ..." ? "" : data["Subset"].ToString().ToFarsiFormat(),
                        ((CommonEnum.ActiveType)Convert.ToInt16(data["ActiveMode"])).GetEnumDescription(),
                        data["CreatedAt"].ToFarsiFormatDateTimeFromSql(),
                        data["ModifiedAt"].ToFarsiFormatDateTimeFromSql());
            dataGrid.ItemsSource = _dataTableUsers.Copy().DefaultView;
        }

        #endregion

        // ••••••••••••
        // EVENTS       ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region Window_Events

        #region Windows_Loaded

        private void Windows_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDefaults();
        }

        #endregion

        #region Windows_KeyDown

        private void Windows_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        #endregion

        #endregion

        #region TextBox_Events

        #region TextBoxSearch_TextChanged

        private void TextBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchText = TextBoxSearch.Text.Trim().ToCorrectKeYe();
            ShowUsers(DataTableUsers.Copy(), _searchText, DataGridUsers);
        }

        #endregion

        #endregion

        #region DataGrid_Events

        #region DataGridUsers_AutoGeneratingColumn

        private void DataGridUsers_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString() == "Id")
                e.Column.Visibility = Visibility.Hidden;
        }

        #endregion

        #endregion

        #region Button_Events

        #region ButtonNewUser_Click

        private void ButtonNewUser_Click(object sender, RoutedEventArgs e)
        {
            ChangeUsersMode = CommonEnum.ChangeDatabaseMode.InsertInToDatabase;
            var addEditUserWindow = new AddEditUserWindow { Owner = this };
            addEditUserWindow.ShowDialog();
            ShowUsers(DataTableUsers.Copy(), _searchText, DataGridUsers);
            DataGridUsers.SelectDataGridRow(SelectedUserId);
        }

        #endregion

        #region ButtonEditUser_Click

        private void ButtonEditUser_Click(object sender, RoutedEventArgs e)
        {
            if (!Convert.ToBoolean(WpfHelper.GetSelectedDataGridRowInfo(DataGridUsers, ref SelectedUserId, "حساب کاربری‌ای"))) return;
            ChangeUsersMode = CommonEnum.ChangeDatabaseMode.UpdateDatabase;
            var addEditUserWindow = new AddEditUserWindow { Owner = this };
            addEditUserWindow.ShowDialog();
            ShowUsers(DataTableUsers.Copy(), _searchText, DataGridUsers);
            DataGridUsers.SelectDataGridRow(SelectedUserId);
        }

        #endregion

        #region ButtonDeleteUser_Click

        private void ButtonDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (!Convert.ToBoolean(WpfHelper.GetSelectedDataGridRowInfo(DataGridUsers, ref SelectedUserId, "حساب کاربری‌ای"))) return;
            var dialogResult = PersianMessageBox_Mhclass.Show("آیا از حذف کاربر انتخاب‌شده اطمینان دارید؟",
                "حذف کاربر", PersianMessageBox_Mhclass.MsgMhButton.YesNo, PersianMessageBox_Mhclass.MsgMhIcon.Question);
            if (dialogResult == MessageBoxResult.No) return;
            var threadDeleteUser = new Thread(DeleteUser);
            threadDeleteUser.Start();
            _waitWindowDeleteUser = new WaitWindow { Owner = this };
            _waitWindowDeleteUser.ShowDialog();
            ShowUsers(DataTableUsers.Copy(), _searchText, DataGridUsers);
        }

        #endregion

        #endregion
    }
}
