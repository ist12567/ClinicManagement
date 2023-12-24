using ACDClinicManagement.Helpers;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Timers;
using System.Windows.Media;
using POS_PC_v3;
using System.Collections.Generic;
using ACDClinicManagement.Common.Enums;
using ACDClinicManagement.Common.Helpers;
using ACDClinicManagement.Common.SpecialHelpers;

namespace ACDClinicManagement.App.Views.CommonViews
{
    /// <summary>
    /// Interaction logic for PayByPosWindow.xaml
    /// </summary>
    public partial class PayByPosWindow : Window
    {
        public PayByPosWindow()
        {
            InitializeComponent();
            InitializeObjects();
            InitializeEvents();
        }

        // ••••••••••••
        // DEFINATIONS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region Windows

        #endregion

        #region Classes


        #endregion

        #region Objects

        private System.Timers.Timer _timerTimeout;
        private Thread _threadDoTransactionByBehPardakhtPos;

        #endregion

        #region Variables

        private int _time;
        public static CommonEnum.GeneralMode PosTransactionMode;
        public static decimal TotalAmount = 0;
        public static string PosPaymentInfo = "", TrackinCode = "";

        #endregion

        // ••••••••••••
        // METHODS     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region private void InitializeObjects()
        private void InitializeObjects()
        {
            _timerTimeout = new System.Timers.Timer();
        }

        #endregion

        #region private void InitializeEvents()
        private void InitializeEvents()
        {
            _timerTimeout.Elapsed += _timerTimeout_Elapsed;
        }

        #endregion

        #region private void LoadDefaults()

        private void LoadDefaults()
        {
            this.ShowWindow(CommonEnum.WindowStyleMode.ExtraMiniTool);
            PosTransactionMode = CommonEnum.GeneralMode.None;
            PosPaymentInfo = "";
            TrackinCode = "";

            TextBlockTotalAmount.Text = TotalAmount.ToThousandsPlaceFarsiFormat() + " ریال";
        }

        #endregion

        #region private void IsValidate()
        private bool IsValidate()
        {
            var status = true;
            return status;
        }

        #endregion

        #region private void PayByPos()

        private void PayByPos()
        {
            var connect = new Transaction.Connection
            {
                CommunicationType = "tcp/ip",
                POSPC_TCPCOMMU_SocketRecTimeout = 60000,
                POS_PORTtcp = SpecialOphthalmologyHelper.OrganizationPosPort,
                POS_IP = SpecialOphthalmologyHelper.OrganizationPosIp
            };
            Transaction transaction = new Transaction(connect);

            var requestList = new Transaction.MultiPaymentReqDataSet[SpecialBaseHelper.CurrentUserType != CommonEnum.UserType.SubsetUser ? 2 : 3];
            requestList[0].AccountID = SpecialOphthalmologyHelper.OrganizationAccountId;
            requestList[0].Amount = TotalAmount.ToString();
            requestList[0].PayerID = GeneratePayerId(SpecialOphthalmologyHelper.OrganizationAccountNumber);

            var returnCode = transaction.MultiPayment("", TotalAmount.ToString(), requestList, 1, "");
            if (returnCode.ReturnCode != 100)
            {
                Dispatcher.Invoke(() =>
                {
                    TextBlockResult.Foreground = Brushes.OrangeRed;
                    TextBlockResult.Text = $"عملیات پرداخت با کد  {returnCode.ReturnCode} متوقف شد".ToFarsiFormat();
                });
                return;
            }

            PosTransactionMode = CommonEnum.GeneralMode.Succeed;
            PosPaymentInfo = "شماره کارت بانکی: " + returnCode.PAN.ToFarsiFormat() + "\n" +
                             "شماره‌ی مرجع: " + returnCode.TraceNumber.ToFarsiFormat() + "\n" +
                             "شماره‌ی پایانه: " + returnCode.TerminalNo.ToFarsiFormat() + "\n" +
                             "مبلغ: " + TotalAmount.ToThousandsPlaceFarsiFormat() + " ریال" + "\n" +
                             new PersianDateTime(DateTime.Now).ToFarsiFormatDateTime();
            TrackinCode = returnCode.TerminalNo + " " + returnCode.TraceNumber;
            Dispatcher.Invoke(() =>
            {
                TextBlockResult.Foreground = Brushes.MediumSeaGreen;
                TextBlockResult.Text = "عملیات انجام تراکنش موفقیت آمیز بود";
                Close();
            });
        }

        #endregion

        #region public string GeneratePayerId(string accountNumber)
        public string GeneratePayerId(string accountNumber)
        {

            var constantList = new List<int> { 15, 14, 13, 12, 11, 10, 9, 1, 2, 3, 4, 5, 6, 7, 8 };
            var accountIdList = new List<int>();

            foreach (char item in Convert.ToInt64(accountNumber).ToString("D15"))
                accountIdList.Add(Convert.ToInt32(item.ToString()));

            var sum = 0;
            for (var i = 0; i < 15; i++)
                sum += accountIdList[i] * constantList[i];

            return accountNumber + (sum % 99).ToString("D2");
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

        #region Window_Closing
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _timerTimeout.Stop();
            _timerTimeout.Dispose();
        }

        #endregion

        #endregion

        #region Timer_Events

        #region _timerTimeout_Elapsed

        private void _timerTimeout_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (_time == 0)
                {
                    _timerTimeout.Stop();
                    if (_threadDoTransactionByBehPardakhtPos != null) _threadDoTransactionByBehPardakhtPos.Abort();
                    PosTransactionMode = CommonEnum.GeneralMode.Failed;
                    PosPaymentInfo = "";
                    TrackinCode = "";
                    Close();
                    return;
                }
                _timerTimeout.Stop();
                TextBlockTimeout.Text = $"مقدار زمان باقیمانده جهت پاسخ‌دهی پایانه: {(--_time).ToString().ToFarsiFormat()} ثانیه";
                TextBlockTimeout.Foreground = _time > 60000 / 2000
                    ? Brushes.MediumSeaGreen
                    : Brushes.OrangeRed;
                _timerTimeout.Start();
            });
        }

        #endregion

        #endregion

        #region Button_Events

        #region ButtonPayByPos_Click
        private void ButtonPayByPos_Click(object sender, RoutedEventArgs e)
        {
            _timerTimeout.Interval = 1000;
            _time = 60;
            _timerTimeout.Start();

            PosTransactionMode = CommonEnum.GeneralMode.None;
            PosPaymentInfo = "";
            TrackinCode = "";
            TextBlockResult.Foreground = Brushes.Black;
            TextBlockResult.Text = "در حال برقراری ارتباط با پایانه ...";

            try
            {
                _threadDoTransactionByBehPardakhtPos = new Thread(PayByPos);
                _threadDoTransactionByBehPardakhtPos.Start();
            }
            catch (Exception exception)
            {
                ButtonPayByPos.IsEnabled = true;
                ButtonCancel.IsEnabled = false;
                TextBlockResult.Foreground = Brushes.OrangeRed;
                TextBlockResult.Text = "ارتباط با پایانه‌ی فروش برقرار نشد";
                MainWindow.ErrorMessage = exception.Message;
            }
        }

        #endregion

        #region ButtonCancel_Click
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            if (_threadDoTransactionByBehPardakhtPos != null) _threadDoTransactionByBehPardakhtPos.Abort();
            PosTransactionMode = CommonEnum.GeneralMode.Failed;
            PosPaymentInfo = "";
            TrackinCode = "";
            Close();
        }

        #endregion

        #endregion
    }
}
