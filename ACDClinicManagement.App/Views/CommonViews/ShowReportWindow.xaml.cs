using ACDClinicManagement.Common.Enums;
using ACDClinicManagement.Common.SpecialHelpers;
using Microsoft.Reporting.WinForms;
using System.Drawing.Printing;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace ACDClinicManagement.App.Views.CommonViews
{
    /// <summary>
    /// Interaction logic for ShowReportWindow.xaml
    /// </summary>
    public partial class ShowReportWindow
    {
        public ShowReportWindow()
        {
            InitializeComponent();
            InitializeObjects();
        }

        #region Windows

        #endregion

        #region Objects

        public static readonly DataSetMain DataSetMain = new DataSetMain();

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
            this.ShowWindow(CommonEnum.WindowStyleMode.Normal);
            var threadLoadData = new Thread(LoadData);
            threadLoadData.Start();
        }

        #endregion

        #region private void LoadData()

        private void LoadData()
        {
            var billSize = new PaperSize();
            switch (SpecialOphthalmologyHelper.ToShowReportMode)
            {
                case CommonEnum.ShowReportMode.Bill:
                    break;
                case CommonEnum.ShowReportMode.Recoupment:
                    break;
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

        #region ReportViewer_Events

        #region ReportViewer_RenderingComplete
        private void ReportViewer_RenderingComplete(object sender, RenderingCompleteEventArgs e)
        {
            ((ReportViewer)sender).PrintDialog();
        }

        #endregion

        #endregion
    }
}
