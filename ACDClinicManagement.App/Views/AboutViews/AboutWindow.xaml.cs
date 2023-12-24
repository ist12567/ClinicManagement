using ACDClinicManagement.App.Views.CommonViews;
using MhclassLib;
using ACDClinicManagement.Helpers;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Deployment.Application;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using ACDClinicManagement.Common.Helpers;
using ACDClinicManagement.Common.SpecialHelpers;
using ACDClinicManagement.Common.Enums;

namespace ACDClinicManagement.App.Views.AboutViews
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow
    {
        public AboutWindow()
        {
            InitializeComponent();
            InitializeObjects();
        }

        // ••••••••••••
        // DEFINATIONS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region Windows

        private WaitWindow _waitWindowLoadData, _waitWindowUpdateSoftware;

        #endregion

        #region Classes


        #endregion

        #region Objects

        private DataTable dataTableVersionFeatures, _dataTableVersionFeatures;

        #endregion

        #region Variables

        bool _updateCompleted;

        #endregion

        // ••••••••••••
        // METHODS     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region private void InitializeObjects()
        private void InitializeObjects()
        {
            _dataTableVersionFeatures = new DataTable();
            _dataTableVersionFeatures.Columns.Add("ویژگی", typeof(string));
            _dataTableVersionFeatures.Columns.Add("تاریخ و زمان ایجاد", typeof(string));
            _dataTableVersionFeatures.Columns.Add("تاریخ و زمان ویرایش", typeof(string));
        }

        #endregion

        #region private void LoadDefaults()
        private void LoadDefaults()
        {
            this.ShowWindow(CommonEnum.WindowStyleMode.Normal);

            TextBlockVersion.Text = ApplicationDeployment.IsNetworkDeployed
                ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString().ToFarsiFormat()
                : "1.0.0.0".ToFarsiFormat();

            TextBlockSoftwareAbout.Text = ("نرم‌افزار مدیریت مطب چشم‌پزشکی، به سفارش کلینیک چشم‌پزشکی نور، دارای قابلیت‌ها " +
                                           "اختصاصی برای مدیریت مطب چشم‌پزشکی بوده که با امکاناتی نظیر مراجعات روزانه، " +
                                           "سوابق و گزارشات مبادرت به انجام این کار می‌کند.").ToFarsiFormat();
            TextBlockCompanyAbout.Text = ("مجتمع داده‌آوران آسیاکندو در سال 1390 تحت شماره 13198 به ثبت رسیده و " +
                                           "هدف اصلی خود را روی گسترش نرم‌افزارهای تحت ویندوز و وب با الویت کاربرد " +
                                           "تکنولوژی‌های به‌روز قرار داده است. این شرکت آمادگی خود را برای انجام " +
                                           "پروژه‌های نرم‌افزاری در مقیاس بزرگ و کوچک اعلام می‌دارد.").ToFarsiFormat();

            var threadLoadData = new Thread(LoadData);
            threadLoadData.Start();
            _waitWindowLoadData = new WaitWindow { Owner = this };
            _waitWindowLoadData.ShowDialog();
            ShowVersionFeatures(dataTableVersionFeatures, DataGridVersionFeatures);
        }

        #endregion

        #region private void LoadData()

        private void LoadData()
        {
            try
            {
                var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MainConnectionString"].ConnectionString);
                connection.Open();
                var dataAdapterFeatures = new SqlDataAdapter("SELECT VersionsFeatures.Feature, VersionsFeatures.CreatedAt, VersionsFeatures.ModifiedAt " +
                                                             "FROM Versions, VersionsFeatures " +
                                                             "WHERE Versions.Id = VersionsFeatures.VersionId AND " +
                                                             "Versions.Version = @Version " +
                                                             "ORDER BY Feature ASC",
                    connection);
                dataAdapterFeatures.SelectCommand.Parameters.AddWithValue("Version", ApplicationDeployment.IsNetworkDeployed ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString() : "1.0.0.0");
                dataTableVersionFeatures = new DataTable();
                dataAdapterFeatures.Fill(dataTableVersionFeatures);
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

        #region private void ShowVersionFeatures(DataTable dataTable, DataGrid dataGrid)
        private void ShowVersionFeatures(DataTable dataTable, DataGrid dataGrid)
        {
            _dataTableVersionFeatures.Clear();
            foreach (DataRow data in dataTable.Rows)
                _dataTableVersionFeatures.Rows.Add(data["Feature"].ToString().ToFarsiFormat(),
                    data["CreatedAt"].ToFarsiFormatDateTimeFromSql(),
                    data["ModifiedAt"].ToFarsiFormatDateTimeFromSql());
            dataGrid.ItemsSource = _dataTableVersionFeatures.Copy().DefaultView;
        }
        #endregion

        #region private void UpdateSoftware()

        private void UpdateSoftware()
        {
            try
            {
                var applicationDeployment = ApplicationDeployment.CurrentDeployment;
                UpdateCheckInfo updateCheckInfo = applicationDeployment.CheckForDetailedUpdate();
                if (updateCheckInfo.UpdateAvailable)
                {
                    applicationDeployment.Update();
                    _updateCompleted = true;
                }
                else
                    Dispatcher.Invoke(() => "به‌روزرسانی جدیدی برای این نرم‌افزار وجود ندارد".ShowMessage());
            }
            catch (Exception exception)
            {
                MainWindow.ErrorMessage = exception.Message;
            }
            finally
            {
                Dispatcher.Invoke(() => _waitWindowUpdateSoftware.Close());
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

        #region ButtonUpdateSoftware_Click
        private void ButtonUpdateSoftware_Click(object sender, RoutedEventArgs e)
        {
            if (!ApplicationDeployment.IsNetworkDeployed) return;
            _updateCompleted = false;
            var dialogResult = PersianMessageBox_Mhclass.Show("آیا مایل به بررسی به‌روزرسانی نرم‌افزار هستید",
                "به‌روزرسانی نرم‌افزار", PersianMessageBox_Mhclass.MsgMhButton.YesNo, PersianMessageBox_Mhclass.MsgMhIcon.Question,
                PersianMessageBox_Mhclass.DefaultSelectedButton.No);
            if (dialogResult == MessageBoxResult.No) return;
            dialogResult = PersianMessageBox_Mhclass.Show("در صورت وجود، به‌روزرسانی جدید دانلود و نصب خواهد شد، آیا تأیید می‌کنید؟",
                "به‌روزرسانی نرم‌افزار", PersianMessageBox_Mhclass.MsgMhButton.YesNo, PersianMessageBox_Mhclass.MsgMhIcon.Question,
                PersianMessageBox_Mhclass.DefaultSelectedButton.No);
            if (dialogResult == MessageBoxResult.No) return;
            var threadUpdateSoftware = new Thread(UpdateSoftware);
            threadUpdateSoftware.Start();
            _waitWindowUpdateSoftware = new WaitWindow { Owner = this };
            _waitWindowUpdateSoftware.ShowDialog();
            if (_updateCompleted)
            {
                PersianMessageBox_Mhclass.Show("برای کارکرد بهتر و همچنین استفاده از ویژگی‌های جدید، نرم‌افزار خود را دوباره راه‌اندازی نمایید",
                        "به‌روزرسانی نرم‌افزار", PersianMessageBox_Mhclass.MsgMhButton.YesNo, PersianMessageBox_Mhclass.MsgMhIcon.Question);
                Application.Current.Shutdown();
            }
        }

        #endregion

        #endregion

        #region Hyperlink_Events

        #region Hyperlink_RequestNavigate
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        #endregion

        #endregion
    }
}
