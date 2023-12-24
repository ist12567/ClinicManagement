using ACDClinicManagement.App.Views.CommonViews;
using ACDClinicManagement.AppHelpers.CommonHelpers;
using ACDClinicManagement.Common.Enums;
using ACDClinicManagement.Common.Helpers;
using ACDClinicManagement.Common.SpecialHelpers;
using Microsoft.Win32;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ACDClinicManagement.App.Views.GeneralSettingsViews
{
    /// <summary>
    /// Interaction logic for AddEditBankWindow.xaml
    /// </summary>
    public partial class AddEditBankWindow : Window
    {
        public AddEditBankWindow()
        {
            InitializeComponent();
            InitializeObjects();
        }

        // ••••••••••••
        // DEFINATIONS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region Windows

        private WaitWindow _waitWindowLoadData, _waitWindowSave;

        #endregion

        #region Classes


        #endregion

        #region Objects


        #endregion

        #region Variables

        private bool _isClose;

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
            this.ShowWindow(CommonEnum.WindowStyleMode.MiniTool);
            TextBoxBankTitle.Focus();
            switch (GeneralSettingsWindow.ChangeBanksMode)
            {
                case CommonEnum.ChangeDatabaseMode.InsertInToDatabase:
                    Title = "افزودن بانک جدید";
                    ImageLogo.Source = new BitmapImage(new Uri("/Contents/Images/organization-xxl.png", UriKind.Relative));
                    break;
                case CommonEnum.ChangeDatabaseMode.UpdateDatabase:
                    Title = "ویرایش مشخصات بانک";
                    var threadLoadData = new Thread(LoadData);
                    threadLoadData.Start();
                    _waitWindowLoadData = new WaitWindow { Owner = this };
                    _waitWindowLoadData.ShowDialog();
                    break;
            }
        }

        #endregion

        #region private void IsValidate()
        private bool IsValidate()
        {
            var status = true;
            if (string.IsNullOrWhiteSpace(TextBoxBankTitle.Text))
            {
                "عنوان".InputValidationMessage().ShowMessage();
                TextBoxBankTitle.Focus();
                status = false;
            }
            else if (ImageLogo.Source == null)
            {
                status = false;
                "لوگو".SelectValidationMessage().ShowMessage();
                ButtonSelectLogo.Focus();
            }
            return status;
        }

        #endregion

        #region private void LoadData()

        private void LoadData()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                var commandData = new SqlCommand("SELECT * FROM Banks " +
                                                 "WHERE Id = @Id",
                    MainWindow.PublicConnection);
                commandData.Parameters.AddWithValue("@Id", GeneralSettingsWindow.SelectedBankId);
                var dataReaderData = commandData.ExecuteReader();
                while (dataReaderData.Read())
                {
                    Dispatcher.Invoke(() =>
                    {
                        TextBoxBankTitle.Text = dataReaderData["Title"].ToString();
                        if (dataReaderData["Logo"] == DBNull.Value) return;
                        var memoryStream = new MemoryStream();
                        var data = (byte[])dataReaderData["Logo"];
                        memoryStream.Write(data, 0, data.Length);
                        memoryStream.Position = 0;
                        var img = Image.FromStream(memoryStream);
                        var bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        var ms = new MemoryStream();
                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                        ms.Seek(0, SeekOrigin.Begin);
                        bitmapImage.StreamSource = ms;
                        bitmapImage.EndInit();
                        ImageLogo.Source = bitmapImage;
                    });
                }
                dataReaderData.Close();
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

        #region private void Save()
        private void Save()
        {
            try
            {
                MainWindow.PublicConnection.LoadConnection();
                var bitmapImage = new BitmapImage();
                Dispatcher.Invoke(() =>
                {
                    bitmapImage = ImageLogo.Source as BitmapImage;
                });

                var memStream = new MemoryStream();
                var encoder = new JpegBitmapEncoder();
                if (bitmapImage != null) encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(memStream);
                switch (GeneralSettingsWindow.ChangeBanksMode)
                {
                    case CommonEnum.ChangeDatabaseMode.InsertInToDatabase:
                        var commandCheckInfo = new SqlCommand("SELECT COUNT(*) " +
                                                              "FROM Banks " +
                                                              "WHERE Title = @Title",
                            MainWindow.PublicConnection);
                        Dispatcher.Invoke(() => commandCheckInfo.Parameters.AddWithValue("@Title", TextBoxBankTitle.Text.Trim().ToCorrectKeYe()));
                        if (Convert.ToInt16(commandCheckInfo.ExecuteScalar()) > 0)
                            Dispatcher.Invoke(() => "بانک".DuplicateMessage().ShowMessage());
                        else
                        {
                            var maxRecord = Convert.ToInt32(SqlHelper.MaxSqlRecord(MainWindow.PublicConnection, "Banks")) + 1;
                            var data = new object[0];
                            Dispatcher.Invoke(() =>
                            {
                                data = new object[]
                                {
                                    maxRecord,
                                    TextBoxBankTitle.Text.Trim().ToCorrectKeYe(),
                                    memStream.GetBuffer(),
                                    SpecialBaseHelper.UserId,
                                    DateTime.Now,
                                    SpecialBaseHelper.UserId,
                                    DateTime.Now
                                };
                            });
                            if (MainWindow.PublicConnection.InsertSqlData("Banks", data))
                            {
                                GeneralSettingsWindow.SelectedBankId = maxRecord;
                                _isClose = true;
                                Dispatcher.Invoke(() => "بانک".AddedMessage().ShowMessage());
                            }
                            else
                                Dispatcher.Invoke(() => WpfHelper.GetErrorMessage().ShowMessage());
                        }
                        break;
                    case CommonEnum.ChangeDatabaseMode.UpdateDatabase:
                        commandCheckInfo = new SqlCommand("SELECT COUNT(*) " +
                                                          "FROM Banks " +
                                                          "WHERE Title = @Title AND " +
                                                          "Id <> @Id",
                            MainWindow.PublicConnection);
                        Dispatcher.Invoke(() => commandCheckInfo.Parameters.AddWithValue("@Title", TextBoxBankTitle.Text.Trim().ToCorrectKeYe()));
                        commandCheckInfo.Parameters.AddWithValue("@Id", GeneralSettingsWindow.SelectedBankId);
                        if (Convert.ToInt16(commandCheckInfo.ExecuteScalar()) > 0)
                            Dispatcher.Invoke(() => "بانک".DuplicateMessage().ShowMessage());
                        else
                        {
                            var commandUpdateData = new SqlCommand
                            {
                                Connection = MainWindow.PublicConnection,
                                CommandText = "UPDATE Banks " +
                                              "SET Title = @Title, " +
                                              "Logo = @Logo, " +
                                              "ModifiedAt = @ModifiedAt, " +
                                              "ModifiedBy = @ModifiedBy " +
                                              "WHERE Id = @Id"
                            };
                            Dispatcher.Invoke(() =>
                            {
                                commandUpdateData.Parameters.AddWithValue("@Title", TextBoxBankTitle.Text.Trim().ToCorrectKeYe());
                                commandUpdateData.Parameters.AddWithValue("@Logo", memStream.GetBuffer());
                                commandUpdateData.Parameters.AddWithValue("@ModifiedBy", SpecialBaseHelper.UserId);
                                commandUpdateData.Parameters.AddWithValue("@ModifiedAt", DateTime.Now);
                                commandUpdateData.Parameters.AddWithValue("@Id", GeneralSettingsWindow.SelectedBankId);
                            });
                            if (commandUpdateData.ExecuteNonQuery() == 1)
                            {
                                _isClose = true;
                                Dispatcher.Invoke(() => "بانک".UpdatedMessage().ShowMessage());
                            }
                            else
                                Dispatcher.Invoke(() => WpfHelper.GetErrorMessage().ShowMessage());
                        }
                        break;
                }
                if (_isClose)
                {
                    var dataAdapterData = new SqlDataAdapter("SELECT * FROM Banks " +
                                                             "WHERE Id <> 0 " +
                                                             "ORDER BY Title ASC",
                        MainWindow.PublicConnection);
                    GeneralSettingsWindow.DataTableBanks = new DataTable();
                    dataAdapterData.Fill(GeneralSettingsWindow.DataTableBanks);
                }
            }
            catch (Exception exception)
            {
                Dispatcher.Invoke(() => WpfHelper.GetErrorMessage().ShowMessage());
                MainWindow.ErrorMessage = exception.Message;
                _isClose = false;
            }
            finally
            {
                Dispatcher.Invoke(() => _waitWindowSave.Close());
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

        #region ButtonSelectLogo_Click

        private void ButtonSelectLogo_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = @"JPEG File (*.jpg) |*.jpg|All Files (*.*) |*.*",
                FilterIndex = 1,
                Title = "Image Files"
            };
            var resultOpenFileDialog = openFileDialog.ShowDialog();
            if (resultOpenFileDialog != true) return;
            ImageLogo.Source = new BitmapImage(new Uri(openFileDialog.FileName));
        }

        #endregion

        #region ButtonSave_Click

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValidate()) return;
            var threadSave = new Thread(Save);
            threadSave.Start();
            _waitWindowSave = new WaitWindow { Owner = this };
            _waitWindowSave.ShowDialog();
            if (_isClose) Close();
        }

        #endregion

        #endregion
    }
}
