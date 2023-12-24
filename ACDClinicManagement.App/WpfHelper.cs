using ACDClinicManagement.Common.Enums;
using ACDClinicManagement.Common.SpecialHelpers;
using ACDClinicManagement.Helpers;
using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static ACDClinicManagement.Common.Enums.CommonEnum;

namespace ACDClinicManagement.App
{
    public static class WpfHelper
    {
        // ••••••••••••
        // Messages     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region public static void ShowMessage(this string value)

        public static void ShowMessage(this string value)
        {
            var mainWindow = Application.Current.Windows.Cast<Window>().FirstOrDefault(window => window is MainWindow) as MainWindow;
            if (mainWindow != null)
                mainWindow.TextBoxMessage.Text = value;
        }

        #endregion

        #region public static MainWindow GetMainWindow()
        public static MainWindow GetMainWindow()
        {
            var mainWindow = Application.Current.Windows.Cast<Window>()
                .FirstOrDefault(window => window is MainWindow) as MainWindow;
            return mainWindow;
        }

        #endregion

        #region Validation Messages

        #region public static string InputValidationMessage(this string item)
        public static string InputValidationMessage(this string item)
        {
            return ($"وارد کردن {item} الزامی است");
        }
        #endregion

        #region public static string SelectValidationMessage(this string item)
        public static string SelectValidationMessage(this string item)
        {
            return ($"انتخاب {item} الزامی است");
        }
        #endregion

        #region public static string NotValidMessage(this string item)
        public static string NotValidMessage(this string item)
        {
            return ($"{item} واردشده معتبر نیست");
        }
        #endregion

        #region public static string NotSelectedMessage(this string item)
        public static string NotSelectedMessage(this string item)
        {
            return ($"{item} جهت انجام عملیات موردنظر انتخاب نشده است");
        }
        #endregion

        #region public static string ValidationPasswordMessage(this string item)
        public static string ValidationPasswordMessage(this string item)
        {
            return ($"{item} واردشده با تکرار آن مطابقت ندارد");
        }
        #endregion

        #endregion

        #region Operation Messages

        #region public static string AddedMessage(this string item)
        public static string AddedMessage(this string item)
        {
            return ($"{item} جدید با موفقیت به سیستم افزوده گردید");
        }
        #endregion

        #region public static string UpdatedMessage(this string item)
        public static string UpdatedMessage(this string item)
        {
            return ($"{item} موردنظر با موفقیت در سیستم به‌روزرسانی گردید");
        }
        #endregion

        #region public static string DeletedMessage(this string item)
        public static string DeletedMessage(this string item)
        {
            return ($"{item} موردنظر با موفقیت از سیستم حذف گردید");
        }
        #endregion

        #region public static string DuplicateMessage(this string item)
        public static string DuplicateMessage(this string item)
        {
            return ($"{item} موردنظر در این سطح تکراریست");
        }
        #endregion

        #endregion

        #region public static string DoEditMessage()
        public static string DoEditMessage()
        {
            return ($"لطفا مشخصات پرونده را ویرایش نمایید");
        }
        #endregion

        #region public static string GetErrorMessage()
        public static string GetErrorMessage()
        {
            return ("تغییرات موردنظر در سیستم انجام نپذیرفت، لطفا مجددا تلاش کنید");
        }
        #endregion

        // ••••••••••••
        // Windows     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region public static void ShowWindow(this Window window, SpecialHelper.WindowStyleMode windowStyleMode)
        public static void ShowWindow(this Window window, CommonEnum.WindowStyleMode windowStyleMode)
        {
            switch (windowStyleMode)
            {
                case CommonEnum.WindowStyleMode.Normal:
                    window.MinWidth = SystemParameters.WorkArea.Width - 60;
                    window.MaxWidth = SystemParameters.WorkArea.Width - 60;
                    window.MinHeight = SystemParameters.WorkArea.Height - 140;
                    window.MaxHeight = SystemParameters.WorkArea.Height - 140;
                    window.Left = 30;
                    window.Top = 45;
                    break;
                case CommonEnum.WindowStyleMode.Owned:
                    window.MinWidth = SystemParameters.WorkArea.Width - SystemParameters.WorkArea.Width * 10 / 100;
                    window.MaxWidth = SystemParameters.WorkArea.Width - SystemParameters.WorkArea.Width * 10 / 100;
                    window.MinHeight = SystemParameters.WorkArea.Height - SystemParameters.WorkArea.Height * 20 / 100;
                    window.MaxHeight = SystemParameters.WorkArea.Height - SystemParameters.WorkArea.Height * 20 / 100;
                    window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    break;
                case CommonEnum.WindowStyleMode.Tool:
                    window.MinWidth = SystemParameters.WorkArea.Width - SystemParameters.WorkArea.Width * 20 / 100;
                    window.MaxWidth = SystemParameters.WorkArea.Width - SystemParameters.WorkArea.Width * 20 / 100;
                    window.MaxHeight = SystemParameters.WorkArea.Height - SystemParameters.WorkArea.Height * 30 / 100;
                    window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    break;
                case CommonEnum.WindowStyleMode.MiniTool:
                    window.MinWidth = SystemParameters.WorkArea.Width - SystemParameters.WorkArea.Width * 40 / 100;
                    window.MaxWidth = SystemParameters.WorkArea.Width - SystemParameters.WorkArea.Width * 40 / 100;
                    window.MaxHeight = SystemParameters.WorkArea.Height - SystemParameters.WorkArea.Height * 30 / 100;
                    window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    break;
                case CommonEnum.WindowStyleMode.ExtraMiniTool:
                    window.MinWidth = SystemParameters.WorkArea.Width - SystemParameters.WorkArea.Width * 70 / 100;
                    window.MaxWidth = SystemParameters.WorkArea.Width - SystemParameters.WorkArea.Width * 70 / 100;
                    window.MaxHeight = SystemParameters.WorkArea.Height - SystemParameters.WorkArea.Height * 40 / 100;
                    window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    break;
            }
        }

        #endregion

        // ••••••••••••
        // DataGrid     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region public static int GetSelectedDataGridRowInfo(this DataGrid dataGrid, ref int rowId, string item)
        public static int GetSelectedDataGridRowInfo(this DataGrid dataGrid, ref int rowId, string item)
        {
            if (dataGrid.SelectedItem == null || dataGrid.SelectedItem.ToString().Contains("NewItemPlaceholder"))
            {
                item.NotSelectedMessage().ShowMessage();
                return 0;
            }
            rowId = Convert.ToInt32(((DataRowView)dataGrid.SelectedItem)["Id"]);
            return rowId;
        }

        #endregion

        #region public static int GetSelectedDataGridRowInfo(this DataGrid dataGrid, ref int rowId, ref string rowTitle, string item)
        public static int GetSelectedDataGridRowInfo(this DataGrid dataGrid, ref int rowId, ref string rowTitle, string item)
        {
            if (dataGrid.SelectedItem == null || dataGrid.SelectedItem.ToString().Contains("NewItemPlaceholder"))
            {
                item.NotSelectedMessage().ShowMessage();
                return 0;
            }
            rowId = Convert.ToInt32(((DataRowView)dataGrid.SelectedItem)["Id"]);
            rowTitle = ((DataRowView)dataGrid.SelectedItem)["عنوان"].ToString();
            return rowId;
        }

        #endregion

        #region public static void SelectDataGridRow(this DataGrid dataGrid, string id)
        public static void SelectDataGridRow(this DataGrid dataGrid, int id)
        {
            foreach (var item in dataGrid.Items)
                if (((DataRowView)item).Row["Id"].ToString() == id.ToString()) dataGrid.SelectedItem = (DataRowView)item;
        }

        #endregion

        #region public static void SetDataGridSelectedValue(DataGrid dataGrid, int value)

        public static void SetDataGridSelectedValue(DataGrid dataGrid, int value)
        {
            if (value == 0) return;
            var index = 0;
            foreach (DataRowView dataRowView in dataGrid.Items)
            {
                if (dataRowView["Id"].ToString() == value.ToString())
                    break;
                index++;
            }
            dataGrid.SelectedIndex = index;
        }

        #endregion

        // ••••••••••••
        // ComboBox     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region public static void FillComboBoxWithIdAndTitle(DataTable dataTableSource, string dataRowCriteria, SelectionChangedEventArgs matchCriteria, ComboBox comboBox)
        public static void FillComboBoxWithIdAndTitle(DataTable dataTableSource, string dataRowCriteria, SelectionChangedEventArgs matchCriteria, ComboBox comboBox)
        {
            if (string.IsNullOrWhiteSpace(dataRowCriteria) && matchCriteria is null)
            {
                var dataRow = dataTableSource.NewRow();
                dataRow["Id"] = 0;
                dataRow["Title"] = "انتخاب کنید ...";
                dataTableSource.Rows.Add(dataRow);
                var dataView = dataTableSource.DefaultView;
                dataView.Sort = "Id ASC";
                var sortedDataTable = dataView.ToTable();
                comboBox.ItemsSource = sortedDataTable.Copy().DefaultView;
                comboBox.DisplayMemberPath = sortedDataTable.Columns["Title"].ToString();
                comboBox.SelectedValuePath = sortedDataTable.Columns["Id"].ToString();
                comboBox.SelectedIndex = 0;
            }
            else
            {
                var dataTable = new DataTable();
                dataTable.Columns.Add("Id", typeof(int));
                dataTable.Columns.Add("Title", typeof(string));
                dataTable.Rows.Add(0, "انتخاب کنید ...");
                foreach (var item in dataTableSource.Rows.Cast<DataRow>().Where(item => matchCriteria.AddedItems.Count != 0 && (item[dataRowCriteria].ToString() == ((DataRowView)matchCriteria.AddedItems[0]).Row.ItemArray[0].ToString() || item[dataRowCriteria].ToString() == "0")))
                    dataTable.Rows.Add(item["Id"], item["Title"]);
                comboBox.ItemsSource = dataTable.Copy().DefaultView;
                comboBox.DisplayMemberPath = dataTable.Columns["Title"].ToString();
                comboBox.SelectedValuePath = dataTable.Columns["Id"].ToString();
                comboBox.SelectedIndex = 0;
            }
        }

        #endregion

        // ••••••••••••
        // Access     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region public static bool IsAccessToOperation(UserClaim userClaim)
        public static bool IsAccessToOperation(UserClaim userClaim)
        {
            if (SpecialBaseHelper.UserId == 1 || SpecialBaseHelper.IsAccessTo((UserClaim)Enum.Parse(typeof(UserClaim), userClaim.ToString()))) return true;
            "نبود سطح دسترسی در کاربری شما".ShowMessage();
            return false;
        }

        #endregion
    }
}
