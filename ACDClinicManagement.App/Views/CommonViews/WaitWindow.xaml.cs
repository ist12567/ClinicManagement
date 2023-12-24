using System.Timers;
using System.Windows;

namespace ACDClinicManagement.App.Views.CommonViews
{
    /// <summary>
    /// Interaction logic for WaitWindow.xaml
    /// </summary>
    public partial class WaitWindow
    {
        public WaitWindow(string message = "در حال پردازش ...")
        {
            _message = message;
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

        private Timer _timerConnectionSpeedTest;

        #endregion

        #region Variables

        string _message;
        int _processedTime;

        #endregion

        // ••••••••••••
        // METHODS     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region private void InitializeEvents()

        private void InitializeEvents()
        {
            _timerConnectionSpeedTest.Elapsed += _timerConnectionSpeedTest_Elapsed;
        }

        #endregion

        #region private void InitializeObjects()

        private void InitializeObjects()
        {
            _timerConnectionSpeedTest = new Timer();
        }

        #endregion

        #region private void LoadDefaults()
        private void LoadDefaults()
        {
            TextBlockMessage.Text = _message;
            _processedTime = 0;
            _timerConnectionSpeedTest.Interval = 1000;
            _timerConnectionSpeedTest.Start();
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

        #region Timer_Events

        #region _timerDateTime_Elapsed

        private void _timerConnectionSpeedTest_Elapsed(object sender, ElapsedEventArgs e)
        {
            _timerConnectionSpeedTest.Stop();
            if (++_processedTime > 5)
                Dispatcher.Invoke(() => TextBlockPoorConnection.Visibility = Visibility.Visible);
            _timerConnectionSpeedTest.Start();
        }

        #endregion

        #endregion
    }
}
