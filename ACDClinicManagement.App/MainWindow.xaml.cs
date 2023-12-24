using ACDClinicManagement.App.Views.AboutViews;
using ACDClinicManagement.App.Views.BackupRestoreViews;
using ACDClinicManagement.App.Views.ChangePasswordViews;
using ACDClinicManagement.App.Views.CommonViews;
using ACDClinicManagement.App.Views.DailyReferencesViews;
using ACDClinicManagement.App.Views.GeneralSettingsViews;
using ACDClinicManagement.App.Views.HelpViews;
using ACDClinicManagement.App.Views.LocationsViews;
using ACDClinicManagement.App.Views.LoginViews;
using ACDClinicManagement.App.Views.OrganizationsSubsetsViews;
using ACDClinicManagement.App.Views.PeopleViews;
using ACDClinicManagement.App.Views.ResetPasswordViews;
using ACDClinicManagement.App.Views.UsersViews;
using ACDClinicManagement.AppHelpers;
using ACDClinicManagement.AppHelpers.CommonHelpers;
using ACDClinicManagement.Common.Enums;
using ACDClinicManagement.Common.Helpers;
using ACDClinicManagement.Common.SpecialHelpers;
using ACDClinicManagement.Helpers;
using MhclassLib;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Deployment.Application;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ACDClinicManagement.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeObjects();
            InitializeEvents();
        }

        // ••••••••••••
        // DEFINATIONS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region Windows

        private WaitWindow _waitWindowCheckDataBaseConnection;

        #endregion

        #region Classes


        #endregion

        #region Objects

        private System.Timers.Timer _timerDateTime, _timerMessage;
        internal static SqlConnection PublicConnection;
        public static Image ShowIcon, DeleteIcon, RotateRightIcon, RotateLeftIcon;

        #endregion

        #region Variables

        public static string ErrorMessage = "", PublicConnectionString = "";
        private int _messageAlarmCounter;
        public static PersianDateTime CurrenDateTime;

        #endregion

        // ••••••••••••
        // METHODS     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region private void InitializeEvents()

        private void InitializeEvents()
        {
            _timerDateTime.Elapsed += _timerDateTime_Elapsed;
            _timerMessage.Elapsed += _timerMessage_Elapsed;
        }

        #endregion

        #region private void InitializeObjects()

        private void InitializeObjects()
        {
            _timerDateTime = new System.Timers.Timer();
            _timerMessage = new System.Timers.Timer();

            ShowIcon = new Image { Width = 12, Height = 12 };
            var showLogo = new BitmapImage();
            showLogo.BeginInit();
            showLogo.UriSource = new Uri("pack://application:,,,/Contents/Images/visible-xxl.png");
            showLogo.EndInit();
            ShowIcon.Source = showLogo;

            DeleteIcon = new Image { Width = 12, Height = 12 };
            var deleteLogo = new BitmapImage();
            deleteLogo.BeginInit();
            deleteLogo.UriSource = new Uri("pack://application:,,,/Contents/Images/delete-xxl.png");
            deleteLogo.EndInit();
            DeleteIcon.Source = deleteLogo;

            RotateRightIcon = new Image { Width = 12, Height = 12 };
            var rotateRightLogo = new BitmapImage();
            rotateRightLogo.BeginInit();
            rotateRightLogo.UriSource = new Uri("pack://application:,,,/Contents/Images/rotate-clockwise-xxl.png");
            rotateRightLogo.EndInit();
            RotateRightIcon.Source = rotateRightLogo;

            RotateLeftIcon = new Image { Width = 12, Height = 12 };
            var rotateLeftLogo = new BitmapImage();
            rotateLeftLogo.BeginInit();
            rotateLeftLogo.UriSource = new Uri("pack://application:,,,/Contents/Images/rotate-counter-clockwise-xxl.png");
            rotateLeftLogo.EndInit();
            RotateLeftIcon.Source = rotateLeftLogo;
        }

        #endregion

        #region private void LoadDefaults()

        private void LoadDefaults()
        {
            PublicConnectionString = ConfigurationManager.ConnectionStrings["MainConnectionString"].ConnectionString;
            PublicConnection = new SqlConnection(PublicConnectionString);
            var threadCheckDateBaseConnection = new Thread(CheckDataBaseConnection);
            threadCheckDateBaseConnection.Start();
            _waitWindowCheckDataBaseConnection = new WaitWindow("در حال اتصال به پایگاه‌داده") { Owner = this };
            _waitWindowCheckDataBaseConnection.ShowDialog();

            if (!SpecialBaseHelper.ConnectionState)
            {
                var dialogResult = PersianMessageBox_Mhclass.Show("در بارگذاری اولیه اطلاعات مشکلی پیش آمد" + "\n" +
                                                    "آیا مایل به تلاش مجدد هستید؟", "عدم بارگذاری اطلاعات",
                    PersianMessageBox_Mhclass.MsgMhButton.YesNo, PersianMessageBox_Mhclass.MsgMhIcon.Question, PersianMessageBox_Mhclass.DefaultSelectedButton.Yes);
                if (dialogResult == MessageBoxResult.Yes)
                {
                    if (Application.ResourceAssembly.Location != null)
                        System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                    Application.Current.Shutdown();
                }
            }
            if (!SpecialBaseHelper.ConnectionState) return;
            var serverDateTime = Convert.ToDateTime(SpecialBaseHelper.ServerDate).ToPersianDateTime();
            TextBlockServerDateTime.Text = serverDateTime.DayName + " " +
                                           serverDateTime.Day.ToString().ToFarsiFormat() + " " +
                                           serverDateTime.MonthName + " سال " +
                                           serverDateTime.Year.ToString().ToFarsiFormat();
            if (Convert.ToDateTime(SpecialBaseHelper.ServerDate).ToPersianDateTime().Date != DateTime.Now.ToPersianDateTime().Date)
            {
                PersianMessageBox_Mhclass.Show("کاربر گرامی" + "\n" +
                                 "لطفا قبل از ادامه تاریخ سیستم خود را به‌روز نمایید و مجدداً وارد سیستم شوید",
                    "مغایرت تاریخ سیستم با تاریخ سرور",
                    PersianMessageBox_Mhclass.MsgMhButton.OK, PersianMessageBox_Mhclass.MsgMhIcon.Error);
                Application.Current.Shutdown();
            }
            SpecialAppHelper.CurrentDateTime = DateTime.Now.ToPersianDateTime();
            var loginWindow = new LoginWindow { Owner = this };
            loginWindow.ShowDialog();
        }

        #endregion

        #region private void GetSetDateTime()

        private void GetSetDateTime()
        {
            Dispatcher.InvokeAsync(() =>
            {
                TextBlockTime.Text = "ساعت " + DateTime.Now.Hour.ToString("D2").ToFarsiFormat() + ":" +
                                     DateTime.Now.Minute.ToString("D2").ToFarsiFormat() + ":" +
                                     DateTime.Now.Second.ToString("D2").ToFarsiFormat();
                TextBlockDate.Text = DateTime.Now.ToPersianDateTime().DayName + " " +
                                     DateTime.Now.ToPersianDateTime().Day.ToString().ToFarsiFormat() + " " +
                                     DateTime.Now.ToPersianDateTime().MonthName + " سال " +
                                     DateTime.Now.ToPersianDateTime().Year.ToString().ToFarsiFormat();
            });

        }

        #endregion

        #region private void CheckDataBaseConnection()

        private void CheckDataBaseConnection()
        {
            try
            {
                var tryCount = 5;
                while (tryCount > 0 && PublicConnection.State != ConnectionState.Open)
                {
                    try
                    {
                        PublicConnection.LoadConnection();
                        var commandDateTime = new SqlCommand("SELECT GETDATE()", PublicConnection);
                        SpecialBaseHelper.ServerDate = commandDateTime.ExecuteScalar().ToString();
                        var commandCheckAdminUser = new SqlCommand("SELECT COUNT (*) FROM Users " +
                                                                           "WHERE Id = 1 AND " +
                                                                           "UserName = 'sa'",
                            PublicConnection);
                        if (Convert.ToInt16(commandCheckAdminUser.ExecuteScalar()) == 0)
                        {
                            var data = new object[]
                                {
                                1,
                                Convert.ToInt16(CommonEnum.UserType.Administrator),
                                0,
                                0,
                                0,
                                0,
                                "Asia",
                                "CanDo",
                                "sa",
                                "sa123sa".ToHashPassword(),
                                "",
                                "09141409050".ToEncryptData(),
                                "".ToEncryptData(),
                                Convert.ToInt16(CommonEnum.ActiveType.Active),
                                0,
                                DBNull.Value,
                                "".ToEncryptData(),
                                1,
                                DateTime.Now,
                                1,
                                DateTime.Now
                                };
                            PublicConnection.InsertSqlData("Users", data);
                        }
                        SpecialBaseHelper.ConnectionState = true;
                        tryCount--;
                        Dispatcher.Invoke(() => TextBlockServerState.Text = "اتصال موفق با سرور");
                    }
                    catch (Exception exception)
                    {
                        tryCount -= 1;
                        ErrorMessage = exception.Message;
                        Dispatcher.Invoke(() => TextBlockServerState.Text = "اتصال ناموفق با سرور");
                    }
                }
                if (PublicConnection.State != ConnectionState.Open)
                    Dispatcher.Invoke(() => TextBoxMessage.Text = "اتصال به بانک اطلاعاتی انجام نپذیرفت");
            }
            catch (Exception exception)
            {
                ErrorMessage = ErrorMessage + exception.Message;
                SpecialBaseHelper.ConnectionState = false;
                MessageBox.Show(ErrorMessage);
            }
            finally
            {
                Dispatcher.InvokeAsync(_waitWindowCheckDataBaseConnection.Close);
            }
        }

        #endregion

        #region private void SaveToDoList()

        private void SaveToDoList()
        {
            using (var connectionSaveToDoList = new SqlConnection(PublicConnectionString))
            {
                try
                {
                    connectionSaveToDoList.Open();
                    var toDoList = "";
                    Dispatcher.Invoke(() => toDoList = TextBoxToDoList.Text.TrimEnd().ToCorrectKeYe());
                    var commandUpdateUser = new SqlCommand("UPDATE Users " +
                                                           "SET ToDoList = @ToDoList, " +
                                                           "ModifiedBy = @ModifiedBy, " +
                                                           "ModifiedAt = @ModifiedAt " +
                                                           "WHERE Id = @Id",
                        connectionSaveToDoList);
                    commandUpdateUser.Parameters.AddWithValue("@ToDoList", toDoList.ToEncryptData());
                    commandUpdateUser.Parameters.AddWithValue("@ModifiedBy", SpecialBaseHelper.UserId);
                    commandUpdateUser.Parameters.AddWithValue("@ModifiedAt", DateTime.Now);
                    commandUpdateUser.Parameters.AddWithValue("@Id", SpecialBaseHelper.UserId);
                    if (commandUpdateUser.ExecuteNonQuery() == 1)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            "بخش یادداشت‌های من با موفقیت ویرایش شد".ShowMessage();
                            TextBoxToDoList.IsEnabled = false;
                            TextBoxToDoList.Background = (Brush)new BrushConverter().ConvertFrom("#3474ec");
                            TextBoxToDoList.Foreground = Brushes.Black;
                            ImageSaveToDoList.Visibility = Visibility.Collapsed;
                        });
                    }
                }
                catch (Exception exception)
                {
                    ErrorMessage = exception.Message;
                }
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
            _timerDateTime.Interval = 500;
            _timerDateTime.Enabled = true;
            _timerDateTime.Start();

            _timerMessage.Interval = 700;

            if (ApplicationDeployment.IsNetworkDeployed && ApplicationDeployment.CurrentDeployment.IsFirstRun)
            {
                var aboutWindow = new AboutWindow { Owner = this };
                aboutWindow.ShowDialog();
            }
            LoadDefaults();
        }

        #endregion

        #region Window_Closed

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
            //Application.Current.Shutdown();
        }

        #endregion

        #endregion

        #region Timer_Events

        #region _timerDateTime_Elapsed

        private void _timerDateTime_Elapsed(object sender, ElapsedEventArgs e)
        {
            GetSetDateTime();
        }

        #endregion

        #region _timerMessage_Elapsed

        private void _timerMessage_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.InvokeAsync(() =>
            {
                if (_messageAlarmCounter < 7)
                {
                    StatusBarMessage.Background = Equals(StatusBarMessage.Background, Brushes.LightCoral)
                        ? Brushes.LightBlue
                        : Brushes.LightCoral;
                    TextBoxMessage.Background = Equals(TextBoxMessage.Background, Brushes.LightCoral)
                        ? Brushes.LightBlue
                        : Brushes.LightCoral;
                    _messageAlarmCounter++;
                }
                else
                {
                    TextBoxMessage.Text = "";
                    _timerMessage.Stop();
                    StatusBarMessage.Background = Brushes.AliceBlue;
                    TextBoxMessage.Background = Brushes.AliceBlue;
                    _messageAlarmCounter = 0;
                }
            });
        }

        #endregion

        #endregion

        #region TextBox_Events

        #region TextBoxMessage_TextChanged

        private void TextBoxMessage_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBoxMessage.Text)) return;
            StatusBarMessage.Background = Brushes.LightCoral;
            TextBoxMessage.Background = Brushes.LightCoral;
            _timerMessage.Start();
        }

        #endregion

        #endregion

        #region Image_Events

        #region ImageEditToDoList_MouseLeftButtonDown

        private void ImageEditToDoList_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (SpecialBaseHelper.CurrentUserType == 0)
            {
                TextBoxMessage.Text = "نبود اتصال به بانک اطلاعاتی";
                return;
            }
            TextBoxToDoList.IsEnabled = true;
            TextBoxToDoList.Background = Brushes.White;
            TextBoxToDoList.Foreground = Brushes.Black;
            ImageSaveToDoList.Visibility = Visibility.Visible;
        }

        #endregion

        #region ImageSaveToDoList_MouseLeftButtonDown

        private void ImageSaveToDoList_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var threadSaveToDoList = new Thread(SaveToDoList);
            threadSaveToDoList.Start();
        }

        #endregion

        #endregion

        #region Button_Events

        #region Button_MouseStuff

        #region ButtonToDoList_MouseEnter

        private void ButtonToDoList_MouseEnter(object sender, MouseEventArgs e)
        {
            if (ImageSaveToDoList.Visibility != Visibility.Visible)
                ImageEditToDoList.Visibility = Visibility.Visible;
        }

        #endregion

        #region ButtonToDoList_MouseLeave

        private void ButtonToDoList_MouseLeave(object sender, MouseEventArgs e)
        {
            ImageEditToDoList.Visibility = Visibility.Collapsed;

        }

        #endregion

        #endregion

        #region Button_Click

        #region ButtonChangePassword_Click

        private void ButtonChangePassword_Click(object sender, RoutedEventArgs e)
        {
            var changePasswordWindow = new ChangePasswordWindow { Owner = this };
            changePasswordWindow.ShowDialog();
        }

        #endregion

        #region ButtonResetPassword_Click

        private void ButtonResetPassword_Click(object sender, RoutedEventArgs e)
        {
            if (!WpfHelper.IsAccessToOperation(CommonEnum.UserClaim.ResetPassword)) return;
            var resetPasswordWindow = new ResetPasswordWindow { Owner = this };
            resetPasswordWindow.ShowDialog();
        }

        #endregion

        #region ButtonGeneralSettings_Click

        private void ButtonGeneralSettings_Click(object sender, RoutedEventArgs e)
        {
            if (!WpfHelper.IsAccessToOperation(CommonEnum.UserClaim.GeneralSettings)) return;
            var generalSettingsWindow = new GeneralSettingsWindow { Owner = this };
            generalSettingsWindow.ShowDialog();
        }

        #endregion

        #region ButtonLocations_Click

        private void ButtonLocations_Click(object sender, RoutedEventArgs e)
        {
            if (!WpfHelper.IsAccessToOperation(CommonEnum.UserClaim.Locations)) return;
            var locationsWindow = new LocationsWindow { Owner = this };
            locationsWindow.ShowDialog();
        }

        #endregion

        #region ButtonOrganizationsSubsets_Click

        private void ButtonOrganizationsSubsets_Click(object sender, RoutedEventArgs e)
        {
            if (!WpfHelper.IsAccessToOperation(CommonEnum.UserClaim.OrganizationsSubsets)) return;
            var organizationsSubsetsWindow = new OrganizationsSubsetsWindow { Owner = this };
            organizationsSubsetsWindow.ShowDialog();
        }

        #endregion

        #region ButtonBackupRestore_Click

        private void ButtonBackupRestore_Click(object sender, RoutedEventArgs e)
        {
            if (!WpfHelper.IsAccessToOperation(CommonEnum.UserClaim.BackupOperation) && !WpfHelper.IsAccessToOperation(CommonEnum.UserClaim.RestoreOperation)) return;
            var backupRestoreWindow = new BackupRestoreWindow { Owner = this };
            backupRestoreWindow.ShowDialog();
        }

        #endregion

        #region ButtonUsers_Click

        private void ButtonUsers_Click(object sender, RoutedEventArgs e)
        {
            if (!WpfHelper.IsAccessToOperation(CommonEnum.UserClaim.Users)) return;
            var usersWindow = new UsersWindow { Owner = this };
            usersWindow.ShowDialog();
        }

        #endregion

        #region ButtonPeople_Click
        private void ButtonPeople_Click(object sender, RoutedEventArgs e)
        {
            if (!WpfHelper.IsAccessToOperation(CommonEnum.UserClaim.People)) return;
            var peopleWindow = new PeopleWindow { Owner = this };
            peopleWindow.ShowDialog();
        }

        #endregion

        #region ButtonReferences_Click
        private void ButtonReferences_Click(object sender, RoutedEventArgs e)
        {
            if (!WpfHelper.IsAccessToOperation(CommonEnum.UserClaim.DailyReferences)) return;
            var dailyReferencesWindow = new DailyReferencesWindow { Owner = this };
            dailyReferencesWindow.ShowDialog();
        }

        #endregion

        #region ButtonPaymentsReport_Click
        private void ButtonPaymentsReport_Click(object sender, RoutedEventArgs e)
        {
            if (!WpfHelper.IsAccessToOperation(CommonEnum.UserClaim.ReferencesReport)) return;
            //var paymentsReportWindow = new PaymentsReportWindow { Owner = this };
            //paymentsReportWindow.ShowDialog();
        }

        #endregion

        #region ButtonAbout_Click

        private void ButtonAbout_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow { Owner = this };
            aboutWindow.ShowDialog();
        }

        #endregion

        #region ButtonHelp_Click

        private void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            var helpWindow = new HelpWindow { Owner = this };
            helpWindow.ShowDialog();
        }

        #endregion

        #endregion

        #endregion
    }
}
