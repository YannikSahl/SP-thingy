using System.Threading.Tasks;
using System.Windows;
using GUI.Properties;
using SPHandler;

namespace GUI
{
    /// <summary>
    ///     Interaction logic for AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        #region constructors

        public AuthWindow()
        {
            InitializeComponent();
            // TODO: falls noch kein SP pfad übergeben: settings öffnen
            //if (string.IsNullOrEmpty(Properties.Settings1.Default.PathSp))
            //{
            //    Settings s = new Settings(null);
            //    s.Show();
            //    s.Activate();
            //    s.Topmost = true;
            //    this.Hide();
            //}
        }

        #endregion

        #region methods

        /// <summary>
        ///     redirect to mainwin
        /// </summary>
        /// <param name="connectionStatus"></param>
        private void RedirectToMainWindow(MainWindow.ConnectionModus connectionStatus)
        {
            var settingsWin = new MainWindow(connectionStatus);
            settingsWin.Show();
            Close();
        }

        /// <summary>
        ///     set SP Credidentials
        /// </summary>
        private void SetCredidentials()
        {
            Handler.SetPassword(PasswordBox.Password);
            Handler.SetUsername(NameBox.Text);
        }

        #endregion

        #region events

        /// <summary>
        ///     buttonclick event redirect to mainwin in offline mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OfflineMode_ButtonClick(object sender, RoutedEventArgs e)
        {
            RedirectToMainWindow(MainWindow.ConnectionModus.Offline);
        }

        private bool _userLoginInProgress;

        /// <summary>
        ///     buttonclick event try to connect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Connect_ButtonClick(object sender, RoutedEventArgs e)
        {
            if (_userLoginInProgress)
                return;
            _userLoginInProgress = true;
            SetCredidentials();
            var errorMessage = await Task.Run(() => { return Handler.TryUserLogin(); });

            var hasConnection = errorMessage == null;
            if (hasConnection)
            {
                // redirect to mainwindow
                RedirectToMainWindow(MainWindow.ConnectionModus.Online);
                // store and save username
                Settings1.Default.SpUserName = NameBox.Text;
                Settings1.Default.Save();
            }
            else
            {
                // display error message

                MessageBox.Show(errorMessage, "Fehler bei der Anmeldung", MessageBoxButton.OK,
                    MessageBoxImage.Information, MessageBoxResult.OK);

                ////ErrorMessageContainer.Visibility = Visibility.Visible;
                ////ErrorMessage.Text = errorMessage;
            }

            _userLoginInProgress = true;
        }

        /// <summary>
        ///     close error message button click event
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