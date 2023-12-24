using ACDClinicManagement.App.Views.CommonViews;
using ACDClinicManagement.AppHelpers.CommonHelpers;
using ACDClinicManagement.Common.Enums;
using ACDClinicManagement.Common.SpecialHelpers;
using System;
using System.Data.SqlClient;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace ACDClinicManagement.App.Views.ResetPasswordViews
{
    /// <summary>
    /// Interaction logic for ResetPasswordWindow.xaml
    /// </summary>
    public partial class ResetPasswordWindow
    {
        public ResetPasswordWindow()
        {
            InitializeObjects();
            InitializeComponent();
        }

        // ••••••••••••
        // DEFINATIONS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region Windows

        private WaitWindow _waitWindowResetPassword;

        #endregion

        #region Objects


        #endregion

        #region Variables

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
            TextBoxUserName.Focus();
        }

        #endregion

        #region private void IsValidate()
        private bool IsValidate()
        {
            if (!string.IsNullOrWhiteSpace(TextBoxPassword.Text)) return true;
            "وارد کردن کلمه‌ی عبور جایگزین الزامی است".ShowMessage();
            TextBoxPassword.Focus();
            return false;
        }

        #endregion

        #region private void ResetPassword()

        private void ResetPassword()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                string userName = "", password = "";
                Dispatcher.Invoke(() =>
                {
                    password = TextBoxPassword.Text;
                    userName = TextBoxUserName.Text.Trim();
                });
                var commandUserId = new SqlCommand("SELECT Id " +
                                                   "FROM Users " +
                                                   "WHERE UserName = @UserName",
                    MainWindow.PublicConnection);
                commandUserId.Parameters.AddWithValue("@UserName", userName);
                var userId = commandUserId.ExecuteScalar();
                if (userId == DBNull.Value || userId == null)
                {
                    Dispatcher.Invoke(() => "کاربری با این نام کاربری در سیستم موجود نیست".ShowMessage());
                    return;
                }
                var commandUpdateData = new SqlCommand("UPDATE Users " +
                                                       "SET Password = @Password, " +
                                                       "ModifiedBy = @ModifiedBy, " +
                                                       "ModifiedAt = @ModifiedAt " +
                                                       "WHERE Id = @Id",
                    MainWindow.PublicConnection);
                commandUpdateData.Parameters.AddWithValue("@Password", password.ToHashPassword());
                commandUpdateData.Parameters.AddWithValue("@ModifiedBy", SpecialBaseHelper.UserId);
                commandUpdateData.Parameters.AddWithValue("@ModifiedAt", DateTime.Now);
                commandUpdateData.Parameters.AddWithValue("@Id", userId);
                if (commandUpdateData.ExecuteNonQuery() > 0)
                    Dispatcher.Invoke(() => "کلمه‌ی عبور کاربر مورد نظر با موفقیت ثبت گردید".ShowMessage());
                else
                    Dispatcher.Invoke(() => "ثبت کلمه‌ی عبور انجام نپذیرفت".ShowMessage());
            }
            catch (Exception exception)
            {
                MainWindow.ErrorMessage = exception.Message;
            }
            finally
            {
                Dispatcher.Invoke(() => _waitWindowResetPassword.Close());
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
                    var threadResetPassword = new Thread(ResetPassword);
                    threadResetPassword.Start();
                    _waitWindowResetPassword = new WaitWindow { Owner = this };
                    _waitWindowResetPassword.ShowDialog();
                    TextBoxPassword.Text = "";
                    break;
                case Key.Escape:
                    Close();
                    break;
            }
        }

        #endregion

        #endregion

        #region Button_Events

        #region ButtonGeneratePassword_Click
        private void ButtonGeneratePassword_Click(object sender, RoutedEventArgs e)
        {
            TextBoxPassword.Text = Guid.NewGuid().ToString("d").Substring(1, 7);
        }

        #endregion

        #region ButtonSavePassword_Click

        private void ButtonSavePassword_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValidate()) return;
            var threadResetPassword = new Thread(ResetPassword);
            threadResetPassword.Start();
            _waitWindowResetPassword = new WaitWindow { Owner = this };
            _waitWindowResetPassword.ShowDialog();
            TextBoxPassword.Text = "";
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
