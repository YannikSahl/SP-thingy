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
        public AuthWindow()
        {
            InitializeComponent();
        }

        private void RedirectToMainWindow(MainWindow.ConnectionModus connectionStatus)
        {
            MainWindow settingsWin = new MainWindow(connectionStatus);
            settingsWin.Show();
            this.Close();
        }

        private void OfflineModus_ButtonClick(object sender, RoutedEventArgs e)
        {
            RedirectToMainWindow(MainWindow.ConnectionModus.Offline);
        }

        private void Connect_ButtonClick(object sender, RoutedEventArgs e)
        {
            SPHandler.Handler.SetPassword(PasswordBox.Password);
            SPHandler.Handler.SetUsername(NameBox.Text);
            bool hasConnection;
            string errorMessage = SPHandler.Handler.TestConnection(out hasConnection);

            if (hasConnection)
            {
                RedirectToMainWindow(MainWindow.ConnectionModus.Online);
                Properties.Settings1.Default.Save();
            }
            else
            {
                ErrorMessageContainer.Visibility = Visibility.Visible;
                ErrorMessage.Text = errorMessage;
            }
                
        }

        private void CloseErrorMessage_ButtonClick(object sender, RoutedEventArgs e)
        {
            ErrorMessageContainer.Visibility = Visibility.Collapsed;
        }

        private void ShowPw_ButtonClick(object sender, RoutedEventArgs e)
        {
            // TODO: show pw
        }
    }
}
