using ACDClinicManagement.Common.Enums;
using ACDClinicManagement.Common.Helpers;
using ACDClinicManagement.Helpers;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ACDClinicManagement.App.Views.CommonViews
{
    /// <summary>
    /// Interaction logic for DateTimePickerWindow.xaml
    /// </summary>
    public partial class DateTimePickerWindow
    {
        public DateTimePickerWindow(object persianDateTime, bool showTrackingInfo = false, string message = "تاریخ و زمان مدنظر خود را انتخاب نمایید")
        {
            _showTrackingInfo = showTrackingInfo;
            _inputedDateTime = persianDateTime is null ? new PersianDateTime(DateTime.Now) : persianDateTime.ToString().ToPersianDateTime();
            _message = message;
            InitializeComponent();
        }

        // ••••••••••••
        // DEFINATIONS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region Windows

        #endregion

        #region Classes


        #endregion

        #region Objects

        #endregion

        #region Variables

        public static bool Result;
        public static string TrackingCode;
        public static PersianDateTime PickedDateTime;
        private PersianDateTime _inputedDateTime;
        private bool _showTrackingInfo;
        private string _message;

        #endregion

        // ••••••••••••
        // METHODS     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region private void LoadDefaults()
        private void LoadDefaults()
        {
            this.ShowWindow(CommonEnum.WindowStyleMode.ExtraMiniTool);
            TrackingCode = "";
            // Fill ComboBoxDay
            for (var day = 1; day <= 30; day++)
                ComboBoxDay.Items.Add(day);
            // Fill ComboBoxMonth by ShamsiMonth enum
            var shamsiMonths = Enum.GetValues(typeof(CommonEnum.ShamsiMonth));
            foreach (var shamsiMonth in shamsiMonths)
            {
                var comboBoxItem = new ComboBoxItem
                {
                    Content = ((CommonEnum.ShamsiMonth)shamsiMonth).GetEnumDescription(),
                    Tag = (CommonEnum.ShamsiMonth)shamsiMonth
                };
                ComboBoxMonth.Items.Add(comboBoxItem);
            }
            // Fill ComboBoxYear
            for (var year = 1300; year <= DateTime.Now.ToPersianDateTime().Year; year++)
                ComboBoxYear.Items.Add(year.ToString());

            Result = false;
            GridTrackingInfo.Visibility = _showTrackingInfo ? Visibility.Visible : Visibility.Collapsed;
            TextBlockMessage.Text = _message;
            ComboBoxYear.Text = _inputedDateTime is null ? new PersianDateTime(DateTime.Now).Year.ToString() : _inputedDateTime.Year.ToString();
            ComboBoxMonth.SelectedIndex = _inputedDateTime is null ? new PersianDateTime(DateTime.Now).Month - 1 : _inputedDateTime.Month - 1;
            ComboBoxDay.Text = _inputedDateTime is null ? new PersianDateTime(DateTime.Now).Day.ToString() : _inputedDateTime.Day.ToString();
            ShortUpDownHour.Value = _inputedDateTime is null ? (short?)new PersianDateTime(DateTime.Now).Hour : (short?)_inputedDateTime.Hour;
            ShortUpDownMinute.Value = _inputedDateTime is null ? (short?)new PersianDateTime(DateTime.Now).Minute : (short?)_inputedDateTime.Minute;
        }

        #endregion

        #region private void IsValidate()
        private bool IsValidate()
        {
            var status = true;
            if (ComboBoxYear.SelectedItem == null || ComboBoxMonth.SelectedItem == null || ComboBoxDay.SelectedItem == null)
            {
                status = false;
                "تاریخ، به درستی انتخاب نشده است".ShowMessage();
                ComboBoxDay.Focus();
            }
            else if (GridTrackingInfo.Visibility == Visibility.Visible && string.IsNullOrWhiteSpace(TextBoxTrackingCode.Text))
            {
                status = false;
                "شماره‌ی سریال".NotValidMessage().ShowMessage();
                TextBoxTrackingCode.Focus();
            }
            return status;
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

        #region ComboBox_Events

        #region ComboBoxMonth_SelectionChanged

        private void ComboBoxMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxMonth.SelectedIndex < 6)
            {
                switch (ComboBoxDay.Items.Count)
                {
                    case 29:
                        ComboBoxDay.Items.Add(30);
                        ComboBoxDay.Items.Add(31);
                        break;
                    case 30:
                        ComboBoxDay.Items.Add(31);
                        break;
                }
            }
            else if (ComboBoxMonth.SelectedIndex > 5 && ComboBoxMonth.SelectedIndex < 11)
                switch (ComboBoxDay.Items.Count)
                {
                    case 29:
                        ComboBoxDay.Items.Add(30);
                        break;
                    case 31:
                        ComboBoxDay.Items.Remove(31);
                        break;
                }
            else if (ComboBoxMonth.SelectedIndex == 11)
            {
                if (Convert.ToInt32(ComboBoxYear.SelectedItem).IsShamsiYearLeap())
                    switch (ComboBoxDay.Items.Count)
                    {
                        case 29:
                            ComboBoxDay.Items.Add(30);
                            break;
                        case 31:
                            ComboBoxDay.Items.Remove(31);
                            break;
                    }
                else
                {
                    switch (ComboBoxDay.Items.Count)
                    {
                        case 30:
                            ComboBoxDay.Items.Remove(30);
                            break;
                        case 31:
                            ComboBoxDay.Items.Remove(30);
                            ComboBoxDay.Items.Remove(31);
                            break;
                    }
                }
            }
        }

        #endregion

        #region ComboBoxYear_SelectionChanged

        private void ComboBoxYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxMonth.SelectedIndex != 11) return;
            if (Convert.ToInt32(ComboBoxYear.SelectedItem).IsShamsiYearLeap())
            {
                switch (ComboBoxDay.Items.Count)
                {
                    case 29:
                        ComboBoxDay.Items.Add(30);
                        break;
                    case 31:
                        ComboBoxDay.Items.Remove(31);
                        break;
                }
            }
            else
            {
                switch (ComboBoxDay.Items.Count)
                {
                    case 30:
                        ComboBoxDay.Items.Remove(30);
                        break;
                    case 31:
                        ComboBoxDay.Items.Remove(30);
                        ComboBoxDay.Items.Remove(31);
                        break;
                }
            }
        }

        #endregion

        #endregion

        #region ButtonConfirm_Click
        private void ButtonConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValidate()) return;
            Result = true;
            PickedDateTime = new PersianDateTime(Convert.ToInt32(ComboBoxYear.Text), Convert.ToInt16((CommonEnum.ShamsiMonth)((ComboBoxItem)ComboBoxMonth.SelectedItem).Tag),
                Convert.ToInt32(ComboBoxDay.Text), Convert.ToInt32(ShortUpDownHour.Value), Convert.ToInt32(ShortUpDownMinute.Value), 0);
            TrackingCode = TextBoxTrackingCode.Text;
            Close();
        }

        #endregion

        #region ButtonCancel_Click
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            PickedDateTime = new PersianDateTime(DateTime.Now);
            TrackingCode = "";
            Close();
        }

        #endregion
    }
}
