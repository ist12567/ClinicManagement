using ACDClinicManagement.App.Views.CommonViews;
using ACDClinicManagement.App.Views.GeneralSettingsViews;
using ACDClinicManagement.App.Views.LocationsViews;
using ACDClinicManagement.App.Views.OrganizationsSubsetsViews;
using ACDClinicManagement.AppHelpers.AppServices.NewsAnnouncement;
using ACDClinicManagement.AppHelpers.AppServices.References;
using ACDClinicManagement.AppHelpers.CommonHelpers;
using ACDClinicManagement.Common.Enums;
using ACDClinicManagement.Common.Helpers;
using ACDClinicManagement.Common.SpecialHelpers;
using ACDClinicManagement.Helpers;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ACDClinicManagement.App.Views.LoginViews
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow
    {
        public LoginWindow()
        {
            InitializeComponent();
            InitializeObjects();
        }

        // ••••••••••••
        // DEFINATIONS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region Windows

        private WaitWindow _waitWindowLogin;

        #endregion

        #region Classes

        #endregion

        #region Objects

        private object _lockerLogin;

        #endregion

        #region Variables

        #endregion

        // ••••••••••••
        // METHODS     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region private void InitializeObjects()
        private void InitializeObjects()
        {
            _lockerLogin = new object();
        }

        #endregion

        #region private void LoadDefaults()
        private void LoadDefaults()
        {
            this.ShowWindow(CommonEnum.WindowStyleMode.MiniTool);
            TextBoxUserName.Focus();
            SpecialBaseHelper.Cliams = 0;
        }

        #endregion

        #region  private bool IsValidateLogin()
        private bool IsValidateLogin()
        {
            var status = true;
            if (string.IsNullOrWhiteSpace(TextBoxUserName.Text))
            {
                status = false;
                "وارد کردن نام کابری الزامی است".ShowMessage();
                TextBoxUserName.Focus();
            }
            else if (string.IsNullOrWhiteSpace(PasswordBoxPassword.Password))
            {
                status = false;
                "وارد کردن کلمه‌ی عبور الزامی است".ShowMessage();
                PasswordBoxPassword.Focus();
            }
            return status;
        }

        #endregion

        #region private void GetNewsAnnouncements()
        private void GetNewsAnnouncements()
        {
            while (true)
            {
                try
                {
                    var newsAnnouncementStatus = new NewsAnnouncementStatus
                    {
                        ConnectionStringService = ConfigurationManager.ConnectionStrings["MainConnectionString"].ConnectionString
                    };
                    newsAnnouncementStatus.GetNewsAnnouncements();
                    var serverDateTime = Convert.ToDateTime(SpecialBaseHelper.ServerDate).ToPersianDateTime();
                    Dispatcher.Invoke(() =>
                    {
                        WpfHelper.GetMainWindow().TextBlockAnnouncementTitle.Text = SpecialBaseHelper.AnnouncementTitle?.ToFarsiFormat();
                        WpfHelper.GetMainWindow().TextBlockAnnouncementFullContent.Text = SpecialBaseHelper.AnnouncementFullContent?.ToFarsiFormat();
                        WpfHelper.GetMainWindow().TextBlockServerDateTime.Text = serverDateTime.DayName + " " +
                                                       serverDateTime.Day.ToString().ToFarsiFormat() + " " +
                                                       serverDateTime.MonthName + " سال " +
                                                       serverDateTime.Year.ToString().ToFarsiFormat();
                    });
                }
                catch (Exception exception)
                {
                    MainWindow.ErrorMessage = exception.Message;
                }
                finally
                {
                    Thread.Sleep(100000);
                }
            }
        }

        #endregion

        #region private void GetAllReferencesStatus()
        private void GetAllReferencesStatus()
        {
            while (true)
            {
                try
                {
                    var referencesStatus = new ReferencesStatus
                    {
                        ConnectionStringService = ConfigurationManager.ConnectionStrings["MainConnectionString"].ConnectionString
                    };
                    (int referencesCount, int visitedReferencesCount) = referencesStatus.GetReferencesStatus();
                    var statusInfo = "تعداد کل مراجعات" + "\n" +
                        referencesCount.ToThousandsPlaceFarsiFormat() + " مراجعه" + "\n" +
                        "تعداد مراجعات ویزیت‌شده" + "\n" +
                        visitedReferencesCount.ToThousandsPlaceFarsiFormat() + " مراجعه" + "\n" +
                        "آخرین به‌روزرسانی: " + "\n" +
                        new PersianDateTime(DateTime.Now).ToFarsiFormatDateTime();
                    Dispatcher.Invoke(() =>
                    {
                        WpfHelper.GetMainWindow().TextBlockTransactionsState.Text = statusInfo;
                    });
                    Thread.Sleep(150000);
                }
                catch (Exception exception)
                {
                    MainWindow.ErrorMessage = exception.Message;
                }
            }
        }

        #endregion

        #region private SpecialHelper.GeneralMode LoadData()
        private CommonEnum.GeneralMode LoadData()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();

                if (!LocationsWindow.LoadProvinces()) throw new Exception();
                if (!LocationsWindow.LoadCities()) throw new Exception();
                if (!LocationsWindow.LoadOrganizations()) throw new Exception();
                if (!OrganizationsSubsetsWindow.LoadOrganizationsInformation()) throw new Exception();
                if (!OrganizationsSubsetsWindow.LoadSubsets()) throw new Exception();
                if (!GeneralSettingsWindow.LoadBanks()) throw new Exception();
                if (!GeneralSettingsWindow.LoadValuesAdded()) throw new Exception();

                var commandOrganizationInfo = new SqlCommand("SELECT * FROM Organizations, OrganizationsInformation " +
                                                             "WHERE Organizations.Id = OrganizationsInformation.OrganizationId AND " +
                                                             "OrganizationId = @OrganizationId",
                        MainWindow.PublicConnection);
                commandOrganizationInfo.Parameters.AddWithValue("@OrganizationId", SpecialBaseHelper.OrganizationId);
                var dataReaderOrganization = commandOrganizationInfo.ExecuteReader();
                while (dataReaderOrganization.Read())
                {
                    SpecialOphthalmologyHelper.OrganizationTitle = dataReaderOrganization["Title"].ToString();
                    SpecialOphthalmologyHelper.OrganizationAccountNumber = dataReaderOrganization["AccountNumber"].ToString();
                    SpecialOphthalmologyHelper.OrganizationAccountId = dataReaderOrganization["AccountId"].ToString();
                    SpecialOphthalmologyHelper.OrganizationBankId = dataReaderOrganization["BankId"].ToString();
                    SpecialOphthalmologyHelper.OrganizationPaymentMethodMode = (CommonEnum.PaymentMethodMode)Convert.ToInt16(dataReaderOrganization["PaymentMethodMode"]);
                    SpecialOphthalmologyHelper.OrganizationPosIp = dataReaderOrganization["PosIp"].ToString();
                    SpecialOphthalmologyHelper.OrganizationPosPort = Convert.ToInt32(dataReaderOrganization["PosPort"]);
                    SpecialOphthalmologyHelper.OrganizationMessage = dataReaderOrganization["Message"].ToString();
                    SpecialOphthalmologyHelper.OrganizationByteImage = dataReaderOrganization["Logo"] == DBNull.Value ? new byte[0] : (byte[])dataReaderOrganization["Logo"];
                }
                dataReaderOrganization.Close();

                var commandSubset = new SqlCommand("SELECT * FROM Subsets " +
                                                   "WHERE Id <> 0 AND " +
                                                   "Id = @Id",
                        MainWindow.PublicConnection);
                commandSubset.Parameters.AddWithValue("@Id", SpecialBaseHelper.SubsetId);
                var dataReaderSubset = commandSubset.ExecuteReader();
                while (dataReaderSubset.Read())
                {
                    SpecialOphthalmologyHelper.SubsetTitle = dataReaderSubset["Title"].ToString();
                    SpecialOphthalmologyHelper.SubsetAccountNumber = dataReaderSubset["AccountNumber"].ToString();
                    SpecialOphthalmologyHelper.SubsetAccountId = dataReaderSubset["AccountId"].ToString();
                    SpecialOphthalmologyHelper.SubsetPaymentMethodMode = (CommonEnum.PaymentMethodMode)Convert.ToInt16(dataReaderSubset["PaymentMethodMode"]);
                    SpecialOphthalmologyHelper.SubsetPosIp = dataReaderSubset["PosIp"].ToString();
                    SpecialOphthalmologyHelper.SubsetPosPort = Convert.ToInt32(dataReaderSubset["PosPort"]);
                }
                dataReaderSubset.Close();
                return CommonEnum.GeneralMode.Succeed;
            }
            catch (Exception exception)
            {
                MainWindow.ErrorMessage = exception.Message;
                return CommonEnum.GeneralMode.Failed;
            }
        }

        #endregion

        #region private void Login()
        private void Login()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                var dataAdapterLogin = new SqlDataAdapter("SELECT Id, ProvinceId, CityId, " +
                                                          "OrganizationId, SubsetId, " +
                                                          "TypeMode, FirstName, LastName, " +
                                                          "TellNumber, UserName, Password, " +
                                                          "ActiveMode, Cliams, Image, ToDoList " +
                                                          "FROM Users " +
                                                          "WHERE UserName = @UserName",
                    MainWindow.PublicConnection);
                Dispatcher.Invoke(() =>
                {
                    dataAdapterLogin.SelectCommand.Parameters.AddWithValue("@UserName", TextBoxUserName.Text);
                });
                var dataTableLogin = new DataTable();
                dataAdapterLogin.Fill(dataTableLogin);
                if (dataTableLogin.Rows.Count == 0)
                    Dispatcher.Invoke(() => "نام کاربری در پایگاه‌داده موجود نیست".ShowMessage());

                else
                {
                    foreach (DataRow user in dataTableLogin.Rows)
                    {
                        if (SecurityHelper.VerifyPassword(PasswordBoxPassword.Password,
                            (byte[])user["Password"]))
                        {
                            SpecialBaseHelper.UserId = Convert.ToInt32(user["Id"]);
                            SpecialBaseHelper.UserName = user["UserName"].ToString();
                            SpecialBaseHelper.ProvinceId = Convert.ToInt32(user["ProvinceId"]);
                            SpecialBaseHelper.CityId = Convert.ToInt32(user["CityId"]);
                            SpecialBaseHelper.OrganizationId = Convert.ToInt32(user["OrganizationId"]);
                            SpecialBaseHelper.SubsetId = Convert.ToInt32(user["SubsetId"]);
                            SpecialBaseHelper.CurrentUserType = (CommonEnum.UserType)Convert.ToInt16(user["TypeMode"]);
                            SpecialBaseHelper.ActiveMode = (CommonEnum.ActiveType)Convert.ToInt16(user["ActiveMode"]);
                            SpecialBaseHelper.FullName = $"{user["FirstName"]} {user["LastName"]}";
                            SpecialBaseHelper.ToDoList = user["ToDoList"].ToString().ToDecryptData();
                            SpecialBaseHelper.Cliams = (CommonEnum.UserClaim)Convert.ToInt64(user["Cliams"]);
                            if (user["Image"] != DBNull.Value)
                            {
                                var data = (byte[])user["Image"];
                                var memoryStream = new MemoryStream();
                                memoryStream.Write(data, 0, data.Length);
                                memoryStream.Position = 0;
                                var img = Image.FromStream(memoryStream);
                                Dispatcher.Invoke(() =>
                                {
                                    var bitmapImage = new BitmapImage();
                                    bitmapImage.BeginInit();
                                    var ms = new MemoryStream();
                                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                                    ms.Seek(0, SeekOrigin.Begin);
                                    bitmapImage.StreamSource = ms;
                                    bitmapImage.EndInit();
                                    WpfHelper.GetMainWindow().ImageUser.Source = bitmapImage;
                                });
                            }
                            if (SpecialBaseHelper.ActiveMode == CommonEnum.ActiveType.DeActive)
                            {
                                Dispatcher.Invoke(() => "حساب کابری شما غیرفعال است".ShowMessage());
                                return;
                            }
                            else
                            {
                                if (LoadData() != CommonEnum.GeneralMode.Succeed)
                                {
                                    Dispatcher.Invoke(() => "بارگزاری اطلاعات اولیه موفقیت‌آمیز نبود".ShowMessage());
                                    return;
                                }
                                var threadGetNewsAnnouncements = new Thread(GetNewsAnnouncements) { IsBackground = true };
                                threadGetNewsAnnouncements.Start();
                                var threadGetAllTransactionsStatus = new Thread(GetAllReferencesStatus) { IsBackground = true };
                                threadGetAllTransactionsStatus.Start();
                                Dispatcher.Invoke(() =>
                                {
                                    WpfHelper.GetMainWindow().TextBoxToDoList.Text = SpecialBaseHelper.ToDoList.ToFarsiFormat();
                                    WpfHelper.GetMainWindow().TextBlockAnnouncementTitle.Text = SpecialBaseHelper.AnnouncementTitle?.ToFarsiFormat();
                                    WpfHelper.GetMainWindow().TextBlockAnnouncementFullContent.Text = SpecialBaseHelper.AnnouncementFullContent?.ToFarsiFormat();
                                    WpfHelper.GetMainWindow().TextBlockUser.Text = SpecialBaseHelper.FullName;
                                    WpfHelper.GetMainWindow().TextBlockUserType.Text = SpecialBaseHelper.CurrentUserType.GetEnumDescription();
                                    WpfHelper.GetMainWindow().ImageUser.Visibility = Visibility.Visible;
                                    Close();
                                });
                            }
                        }
                        else
                            Dispatcher.Invoke(() => "کلمه‌ی عبور وارد شده صحیح نیست".ShowMessage());
                    }
                }
            }
            catch (Exception exception)
            {
                MainWindow.ErrorMessage = exception.Message;
            }
            finally
            {
                Dispatcher.Invoke(_waitWindowLogin.Close);
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

        #endregion

        #region Button_Events

        #region ButtonLogin_Click

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            WpfHelper.GetMainWindow().ImageUser.Source = null;
            if (!IsValidateLogin()) return;
            var threadLogin = new Thread(Login);
            threadLogin.Start();
            _waitWindowLogin = new WaitWindow("در حال بارگزاری اطلاعات اولیه") { Owner = this };
            _waitWindowLogin.ShowDialog();
        }

        #endregion

        #region ButtonExit_Click

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion

        #endregion
    }
}
