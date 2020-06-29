using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SPHandler;

namespace GUI
{
    /// <summary>
    /// Interaction logic for AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        #region constructors

        public AuthWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region methods
        
        /// <summary>
        /// redirect to mainwin
        /// </summary>
        /// <param name="connectionStatus"></param>
        private void RedirectToMainWindow(MainWindow.ConnectionModus connectionStatus)
        {
            MainWindow settingsWin = new MainWindow(connectionStatus);
            settingsWin.Show();
            this.Close();
        }

        /// <summary>
        /// buttonclick event redirect to mainwin in offline mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OfflineMode_ButtonClick(object sender, RoutedEventArgs e)
        {
            RedirectToMainWindow(MainWindow.ConnectionModus.Offline);
        }

        /// <summary>
        /// buttonclick event try to connect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Connect_ButtonClick(object sender, RoutedEventArgs e)
        {
            SPHandler.Handler.SetPassword(PasswordBox.Password);
            SPHandler.Handler.SetUsername(NameBox.Text);
            bool hasConnection;
            string errorMessage = SPHandler.Handler.TestConnection(out hasConnection);

            if (hasConnection)
            {
                // redirect to mainwindow
                RedirectToMainWindow(MainWindow.ConnectionModus.Online);
                // store and save username
                Properties.Settings1.Default.SpUserName = NameBox.Text;
                Properties.Settings1.Default.Save();
            }
            else
            {
                // display error message
                ErrorMessageContainer.Visibility = Visibility.Visible;
                ErrorMessage.Text = errorMessage;
            }
                
        }

        /// <summary>
        /// close error message button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseErrorMessage_ButtonClick(object sender, RoutedEventArgs e)
        {
            ErrorMessageContainer.Visibility = Visibility.Collapsed;
        }

        #endregion
    }
}
