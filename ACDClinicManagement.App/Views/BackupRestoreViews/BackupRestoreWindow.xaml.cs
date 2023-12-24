using System;
using System.Data.SqlClient;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using ACDClinicManagement.App.Views.CommonViews;
using ACDClinicManagement.AppHelpers.CommonHelpers;
using ACDClinicManagement.Common.Enums;
using ACDClinicManagement.Common.SpecialHelpers;
using ACDClinicManagement.Helpers;
using MhclassLib;
using Microsoft.Win32;

namespace ACDClinicManagement.App.Views.BackupRestoreViews
{
    /// <summary>
    /// Interaction logic for BackupRestoreWindow.xaml
    /// </summary>
    public partial class BackupRestoreWindow
    {
        public BackupRestoreWindow()
        {
            InitializeComponent();
            InitializeObjects();
        }

        // ••••••••••••
        // DEFINATIONS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region Windows

        private WaitWindow _waitWindowBackupDataBase,
            _waitWindowRestoreDataBase;

        #endregion

        #region Classes


        #endregion

        #region Objects

        private object _lockerRestoreDataBase;

        #endregion

        #region Variables

        private string _path;
        private bool _isSuccess;

        #endregion

        // ••••••••••••
        // METHODS     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region private void InitializeObjects()
        private void InitializeObjects()
        {
            _lockerRestoreDataBase = new object();
        }

        #endregion

        #region private void LoadDefaults()
        private void LoadDefaults()
        {
            this.ShowWindow(CommonEnum.WindowStyleMode.MiniTool);
        }

        #endregion

        #region private void BackupDataBase()

        private void BackupDataBase()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                var commandBackup = new SqlCommand { Connection = MainWindow.PublicConnection };
                if (MainWindow.PublicConnectionString.Contains("Attach"))
                    commandBackup.CommandText = "BACKUP DATABASE[" + AppDomain.CurrentDomain.BaseDirectory +
                                                "ACDCM_Db.mdf] TO DISK='" + _path + "' WITH init";
                else
                    commandBackup.CommandText = @"BACKUP DATABASE ACDCM_Db TO DISK='" + _path + "' WITH COMPRESSION";
                commandBackup.CommandTimeout = 0;
                commandBackup.ExecuteNonQuery();
                Dispatcher.Invoke(() => "عملیات پشتیبان‌گیری با موفقیت انجام شد".ShowMessage());
            }
            catch (Exception exception)
            {
                MainWindow.ErrorMessage = exception.Message;
                Dispatcher.Invoke(() => "عملیات پشتیبان‌گیری انجام نپذیرفت".ShowMessage());
            }
            finally
            {
                Dispatcher.Invoke(() => _waitWindowBackupDataBase.Close());
            }
        }

        #endregion

        #region private void RestoreDataBase()
        private void RestoreDataBase()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                var commandRestoreDataBase = new SqlCommand { Connection = MainWindow.PublicConnection };
                if (MainWindow.PublicConnectionString.Contains("Attach"))
                    commandRestoreDataBase.CommandText = @"ALTER DATABASE[" +
                                                         AppDomain.CurrentDomain.BaseDirectory +
                                                         "ACDCM_Db.mdf] SET SINGLE_USER WITH ROLLBACK IMMEDIATE " +
                                                         "USE master RESTORE DATABASE[" +
                                                         AppDomain.CurrentDomain.BaseDirectory +
                                                         "ACDCM_Db.mdf] FROM DISK='" + _path + "' WITH REPLACE";
                else
                    commandRestoreDataBase.CommandText = @"ALTER DATABASE ACDCM_Db " +
                                                         "SET SINGLE_USER WITH ROLLBACK IMMEDIATE " +
                                                         "USE master RESTORE DATABASE ACDCM_Db " +
                                                         "FROM DISK='" + _path + "' WITH REPLACE";
                commandRestoreDataBase.CommandTimeout = 0;
                commandRestoreDataBase.ExecuteNonQuery();
                _isSuccess = true;
                Dispatcher.Invoke(() => "عملیات بازیابی با موفقیت انجام شد".ShowMessage());
            }
            catch (Exception exception)
            {
                _isSuccess = false;
                MainWindow.ErrorMessage = exception.Message;
                Dispatcher.Invoke(() => "عملیات بازیابی انجام نپذیرفت".ShowMessage());
            }
            finally
            {
                Dispatcher.Invoke(() => _waitWindowRestoreDataBase.Close());
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

        #region ButtonSelectBackupPath_Click
        private void ButtonSelectBackupPath_Click(object sender, RoutedEventArgs e)
        {
            TextBlockBackupPath.Text = "";
            _path = "";
            var saveFileDialog = new SaveFileDialog
            {
                DefaultExt = "bak",
                FileName = "ACDCM_Db" + "-" +
                           DateTime.Now.ToPersianDateTime().Year.ToString("D4") +
                           DateTime.Now.ToPersianDateTime().Month.ToString("D2") +
                           DateTime.Now.ToPersianDateTime().Day.ToString("D2") +
                           "-" +
                           DateTime.Now.Hour.ToString("D2") +
                           DateTime.Now.Minute.ToString("D2") +
                           DateTime.Now.Second.ToString("D2"),
                Filter = @"SQL Backup Files (*.bak) |*.bak|All Files (*.*) |*.*",
                FilterIndex = 1,
                OverwritePrompt = true,
                Title = "Backup SQL File"
            };

            var resultSaveFileDialog = saveFileDialog.ShowDialog();
            if (resultSaveFileDialog == true)
                TextBlockBackupPath.Text = saveFileDialog.FileName;
            _path = TextBlockBackupPath.Text;
        }
        #endregion

        #region ButtonBackupDataBase_Click

        private void ButtonBackupDataBase_Click(object sender, RoutedEventArgs e)
        {
            if (!WpfHelper.IsAccessToOperation(CommonEnum.UserClaim.BackupOperation)) return;
            if (string.IsNullOrWhiteSpace(TextBlockBackupPath.Text))
            {
                "لطفا مکان ذخیره‌ی فایل پشتیبان را مشخص کنید".ShowMessage();
                return;
            }
            var threadBackupDataBase = new Thread(BackupDataBase);
            threadBackupDataBase.Start();
            _waitWindowBackupDataBase = new WaitWindow { Owner = this };
            _waitWindowBackupDataBase.ShowDialog();
        }

        #endregion

        #region ButtonSelectRestorePath_Click
        private void ButtonSelectRestorePath_Click(object sender, RoutedEventArgs e)
        {
            TextBlockRestorePath.Text = "";
            _path = "";
            var openFileDialog = new OpenFileDialog
            {
                Filter = @"SQL Backup Files (*.bak) |*.bak|All Files (*.*) |*.*",
                FilterIndex = 1,
                Title = "Restore SQL Files"
            };

            var resultOpenFileDialog = openFileDialog.ShowDialog();
            if (resultOpenFileDialog == true)
                TextBlockRestorePath.Text = openFileDialog.FileName;
            _path = TextBlockRestorePath.Text;
        }

        #endregion

        #region ButtonRestoreDataBase_Click

        private void ButtonRestoreDataBase_Click(object sender, RoutedEventArgs e)
        {
            if (!WpfHelper.IsAccessToOperation(CommonEnum.UserClaim.RestoreOperation)) return;
            if (string.IsNullOrWhiteSpace(TextBlockRestorePath.Text))
            {
                "لطفا مکان بازیابی فایل پشتیبان را مشخص کنید".ShowMessage();
                return;
            }
            var dialgResult = PersianMessageBox_Mhclass.Show("با انجام گرفتن عملیات بازیابی، اطلاعات فعلی پایگاه‌داده با اطلاعات فایل بازیابی تعویض خواهند شد" + "\n" +
                    "آیا این عملیات را تأیید می‌کنید؟", "بازیابی پایگاه‌داده", PersianMessageBox_Mhclass.MsgMhButton.YesNo, PersianMessageBox_Mhclass.MsgMhIcon.Question);
            if (dialgResult == MessageBoxResult.No) return;
            dialgResult = PersianMessageBox_Mhclass.Show("کلیه اطلاعات پایگاه‌داده با فایل بازیابی تعویض خواهند شد" + "\n" +
                    "آیا این عملیات را تأیید می‌کنید؟", "بازیابی پایگاه‌داده", PersianMessageBox_Mhclass.MsgMhButton.YesNo, PersianMessageBox_Mhclass.MsgMhIcon.Question);
            if (dialgResult == MessageBoxResult.No) return;
            var threadRestoreDataBase = new Thread(RestoreDataBase);
            threadRestoreDataBase.Start();
            _waitWindowRestoreDataBase = new WaitWindow { Owner = this };
            _waitWindowRestoreDataBase.ShowDialog();
            if (!_isSuccess) return;
            PersianMessageBox_Mhclass.Show("نرم‌افزار برای کارکرد صحیح با نسخه‌ی پشتیبان جدید باید راه‌اندازی مجدد شود",
                "راه‌اندازی مجدد", PersianMessageBox_Mhclass.MsgMhButton.OK, PersianMessageBox_Mhclass.MsgMhIcon.Information);
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        #endregion

        #endregion
    }
}
