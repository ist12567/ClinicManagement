using ACDClinicManagement.Common.Enums;
using ACDClinicManagement.Helpers;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ACDClinicManagement.App.Views.CommonViews
{
    /// <summary>
    /// Interaction logic for ShowImageWindow.xaml
    /// </summary>
    public partial class ShowImageWindow
    {
        public ShowImageWindow()
        {
            InitializeComponent();
            InitializeObjects();
        }

        #region Windows

        #endregion

        #region Objects

        public ImageSource ImageSource;
        private TransformGroup _transformGroup;
        private ScaleTransform _scaleTransform;
        private TranslateTransform _translateTransform;

        #endregion

        #region Variables

        public string WindowTitle;
        Point _start;
        Point _origin;

        #endregion

        // ••••••••••••
        // METHODS     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region private void InitializeObjects()
        private void InitializeObjects()
        {
            _transformGroup = new TransformGroup();
            _scaleTransform = new ScaleTransform();
            _translateTransform = new TranslateTransform();
        }

        #endregion

        #region private void LoadDefaults()

        private void LoadDefaults()
        {
            _transformGroup.Children.Add(_scaleTransform);
            _transformGroup.Children.Add(_translateTransform);
            ImageMain.RenderTransform = _transformGroup;

            this.ShowWindow(CommonEnum.WindowStyleMode.Normal);

            Title = WindowTitle;
            ImageMain.Width = ImageSource.Width;
            ImageMain.Height = Height - 50;
            ImageMain.Source = ImageSource;
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

        #region ButtonPrint_Click
        private void ButtonPrint_Click(object sender, RoutedEventArgs e)
        {
            var bitmapImage = ImageMain.Source as BitmapImage;

            var drawingVisual = new DrawingVisual();
            var document = drawingVisual.RenderOpen();
            if (bitmapImage != null)
                document.DrawImage(bitmapImage, new Rect { Width = bitmapImage.Width, Height = bitmapImage.Height });
            document.Close();

            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(drawingVisual, "Waste Image");
            }
        }

        #endregion

        #endregion

        #region Image_Events

        #region Image_MouseWheel
        private void Image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            TransformGroup transformGroup = (TransformGroup)ImageMain.RenderTransform;
            ScaleTransform transform = (ScaleTransform)transformGroup.Children[0];

            double zoom = e.Delta > 0 ? .2 : -.2;
            transform.ScaleX += zoom;
            transform.ScaleY += zoom;
        }

        #endregion

        #region Image_MouseLeftButtonDown
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ImageMain.CaptureMouse();
            var tt = (TranslateTransform)((TransformGroup)ImageMain.RenderTransform).Children.First(tr => tr is TranslateTransform);
            _start = e.GetPosition(BorderMain);
            _origin = new Point(tt.X, tt.Y);
        }

        #endregion

        #region Image_MouseLeftButtonUp
        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ImageMain.ReleaseMouseCapture();
        }

        #endregion

        #region Image_MouseMove
        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (!ImageMain.IsMouseCaptured) return;

            var tt = (TranslateTransform)((TransformGroup)ImageMain.RenderTransform).Children.First(tr => tr is TranslateTransform);
            Vector v = _start - e.GetPosition(BorderMain);
            tt.X = _origin.X - v.X;
            tt.Y = _origin.Y - v.Y;
        }

        #endregion

        #endregion
    }
}
