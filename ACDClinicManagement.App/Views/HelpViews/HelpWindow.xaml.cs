using ACDClinicManagement.App.Views.CommonViews;
using ACDClinicManagement.Common.Enums;
using ACDClinicManagement.Helpers;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace ACDClinicManagement.App.Views.HelpViews
{
    /// <summary>
    /// Interaction logic for HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow
    {
        public HelpWindow()
        {
            InitializeComponent();
        }

        // ••••••••••••
        // DEFINATIONS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region Windows

        private WaitWindow _waitWindowDoSomething;

        #endregion

        #region Classes


        #endregion

        #region Objects

        #endregion

        #region Variables

        #endregion

        // ••••••••••••
        // METHODS     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // ••••••••••••

        #region private void LoadDefaults()
        private void LoadDefaults()
        {
            this.ShowWindow(CommonEnum.WindowStyleMode.Normal);
        }

        #endregion

        #region private void DoSomething()
        private void DoSomething()
        {
            int currentidv = 0;
            try
            {
                var dataAdapterSource = new SqlDataAdapter("SELECT * FROM OphthalmologyRecords", MainWindow.PublicConnection);
                var dataTableSource = new DataTable();
                dataAdapterSource.Fill(dataTableSource);

                foreach (DataRow record in dataTableSource.Rows)
                {
                    var dateCommand = new SqlCommand("SELECT CreatedAt FROM DailyReferences WHERE Id = @Id", MainWindow.PublicConnection);
                    dateCommand.Parameters.AddWithValue("@Id", record["ReferenceId"]);
                    var date = Convert.ToDateTime(dateCommand.ExecuteScalar());
                    var commandUpdateRecord = new SqlCommand("UPDATE OphthalmologyRecords SET CreatedAt = @CreatedAt, ModifiedAt = @ModifiedAt WHERE Id = @Id", MainWindow.PublicConnection);
                    commandUpdateRecord.Parameters.AddWithValue("@CreatedAt", date);
                    commandUpdateRecord.Parameters.AddWithValue("@ModifiedAt", date);
                    commandUpdateRecord.Parameters.AddWithValue("@Id", record["Id"]);
                    commandUpdateRecord.ExecuteNonQuery();
                }
            }
            catch (Exception exception)
            {
                MainWindow.ErrorMessage = exception.Message;
                MessageBox.Show(MainWindow.ErrorMessage + "\n" + "Currentidr is " + currentidv, "Error");
            }
            finally
            {
                Dispatcher.Invoke(() => _waitWindowDoSomething.Close());
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

        #region ButtonDoSomething_Click
        private void ButtonDoSomething_Click(object sender, RoutedEventArgs e)
        {
            var dialogResult = MessageBox.Show("Do you absolutely want do this?",
                "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (dialogResult != MessageBoxResult.Yes) return;

            var threadDoSomething = new Thread(DoSomething);
            threadDoSomething.Start();
            _waitWindowDoSomething = new WaitWindow { Owner = this };
            _waitWindowDoSomething.ShowDialog();
        }

        #endregion

        #endregion
    }
}
