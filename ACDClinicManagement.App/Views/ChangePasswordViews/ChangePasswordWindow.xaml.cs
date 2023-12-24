using ACDClinicManagement.App.Views.CommonViews;
using ACDClinicManagement.AppHelpers.CommonHelpers;
using ACDClinicManagement.Common.Enums;
using ACDClinicManagement.Common.SpecialHelpers;
using System;
using System.Data.SqlClient;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace ACDClinicManagement.App.Views.ChangePasswordViews
{
    /// <summary>
    /// Interaction logic for ChangePassword.xaml
    /// </summary>
    public partial class ChangePasswordWindow
    {
        public ChangePasswordWindow()
        {
            InitializeObjects();
            InitializeComponent();
        }

        // ••••••••••••
        // DEFINATIONS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region Windows

        private WaitWindow _waitWindowLoadData, _waitWindowChangePassword;

        #endregion

        #region Objects


        #endregion

        #region Variables

        private byte[] _currentPassword;
        private bool _isSuccess;

        #endregion

        // ••••••••••••
        // METHODS     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region private void InitializeObjects()
        private void InitializeObjects()
        {

        }

        #endregion

        #region private void LoadDefaults()
        private void LoadDefaults()
        {
            this.ShowWindow(CommonEnum.WindowStyleMode.ExtraMiniTool);
            PasswordBoxCurrentPassword.Focus();
            var threadLoadData = new Thread(LoadData);
            threadLoadData.Start();
            _waitWindowLoadData = new WaitWindow { Owner = this };
            _waitWindowLoadData.ShowDialog();
        }

        #endregion

        #region private void LoadData()
        private void LoadData()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                var commandLoadData = new SqlCommand("SELECT UserName, Password " +
                                                     "FROM Users " +
                                                     "WHERE Id = @Id",
                    MainWindow.PublicConnection);
                commandLoadData.Parameters.AddWithValue("@Id", SpecialBaseHelper.UserId);
                var dataReaderLoadData = commandLoadData.ExecuteReader();
                while (dataReaderLoadData.Read())
                {
                    Dispatcher.Invoke(() => { TextBlockUserName.Text = dataReaderLoadData["UserName"].ToString(); });
                    _currentPassword = (byte[])dataReaderLoadData["Password"];
                }
                dataReaderLoadData.Close();
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

        #region private void IsValidate()
        private bool IsValidate()
        {
            var status = true;
            if (string.IsNullOrWhiteSpace(PasswordBoxCurrentPassword.Password))
            {
                status = false;
                "وارد کردن کلمه‌ی عبور فعلی الزامی است".ShowMessage();
                PasswordBoxCurrentPassword.Focus();
            }
            else if (string.IsNullOrWhiteSpace(PasswordBoxNewPassword.Password))
            {
                status = false;
                "وارد کردن کلمه‌ی عبور جدید الزامی است".ShowMessage();
                PasswordBoxNewPassword.Focus();
            }
            else if (string.IsNullOrWhiteSpace(PasswordBoxReNewPassword.Password))
            {
                status = false;
                "وارد کردن تکرار کلمه‌ی عبور جدید الزامی است".ShowMessage();
                PasswordBoxReNewPassword.Focus();
            }
            else if (PasswordBoxNewPassword.Password != PasswordBoxReNewPassword.Password)
            {
                status = false;
                "کلمه‌ی عبور جدید و تکرار آن باید با همدیگر مطابقت داشته باشند".ShowMessage();
                PasswordBoxNewPassword.Password = "";
                PasswordBoxReNewPassword.Password = "";
                PasswordBoxNewPassword.Focus();
            }
            else if (!SecurityHelper.VerifyPassword(PasswordBoxCurrentPassword.Password, _currentPassword))
            {
                status = false;
                "کلمه‌ی عبور فعلی نادرست است".ShowMessage();
                PasswordBoxCurrentPassword.Password = "";
                PasswordBoxCurrentPassword.Focus();
            }
            return status;
        }

        #endregion

        #region private void ChangePassword()

        private void ChangePassword()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                var password = "";
                Dispatcher.Invoke(() =>
                {
                    password = PasswordBoxNewPassword.Password;
                });
                var commandUpdateData = new SqlCommand("UPDATE Users " +
                                                       "SET Password = @Password, " +
                                                       "ModifiedBy = @ModifiedBy, " +
                                                       "ModifiedAt = @ModifiedAt " +
                                                       "WHERE Id = @Id",
                    MainWindow.PublicConnection);
                commandUpdateData.Parameters.AddWithValue("@Password", password.ToHashPassword());
                commandUpdateData.Parameters.AddWithValue("@ModifiedBy", SpecialBaseHelper.UserId);
                commandUpdateData.Parameters.AddWithValue("@ModifiedAt", DateTime.Now);
                commandUpdateData.Parameters.AddWithValue("@Id", SpecialBaseHelper.UserId);
                if (commandUpdateData.ExecuteNonQuery() > 0)
                {
                    Dispatcher.Invoke(() => "کلمه‌ی عبور شما با موفقیت تغییر یافت".ShowMessage());
                    _isSuccess = true;
                }
                else
                    Dispatcher.Invoke(() => "تغییر کلمه‌ی عبور انجام نپذیرفت".ShowMessage());
            }
            catch (Exception exception)
            {
                MainWindow.ErrorMessage = exception.Message;
            }
            finally
            {
                Dispatcher.Invoke(() => _waitWindowChangePassword.Close());
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
            switch (e.Key)
            {
                case Key.Enter:
                    if (!IsValidate()) return;
                    var threadChangePassword = new Thread(ChangePassword);
                    threadChangePassword.Start();
                    _waitWindowChangePassword = new WaitWindow { Owner = this };
                    _waitWindowChangePassword.ShowDialog();
                    if (_isSuccess)
                        Close();
                    break;
                case Key.Escape:
                    Close();
                    break;
            }
        }

        #endregion

        #endregion

        #region Button_Events

        #region ButtonChangePassword_Click

        private void ButtonChangePassword_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValidate()) return;
            var threadChangePassword = new Thread(ChangePassword);
            threadChangePassword.Start();
            _waitWindowChangePassword = new WaitWindow { Owner = this };
            _waitWindowChangePassword.ShowDialog();
            if (_isSuccess)
                Close();
        }

        #endregion

        #region ButtonCancel_Click

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion

        #endregion
    }
}
