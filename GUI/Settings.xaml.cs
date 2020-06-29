using System;
using System.Collections.Generic;
using System.IO;
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
using Path = System.IO.Path;

namespace GUI
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        #region members

        private readonly SolidColorBrush _settingWrongColor = new SolidColorBrush(Colors.Red);
        private readonly SolidColorBrush _settingRightColor = new SolidColorBrush(Colors.ForestGreen);

        #endregion

        #region constructors

        public Settings()
        {
            InitializeComponent();
            DbLocalSave.Text = Path.GetFullPath("..\\..\\..\\..\\DBHandler");
            SkizzeLocalSave.Text = Path.GetFullPath("..\\..\\..\\..\\DBHandler");
        }

        #endregion

        #region methods

        /// <summary>
        /// set SP pw and username
        /// </summary>
        public void SaveAuth()
        {
            SPHandler.Handler.SetUsername(SpUserName.Text);
            SPHandler.Handler.SetPassword(SpUserPw.Password);
        }

        // TODO: falls im offline modus kann man hier seine verbindung auf online stellen müssen
        /// <summary>
        /// test credidentials and connection to SP
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button mSender = (Button) sender;
            SaveAuth();
            bool hasConnection;
            string errorMessage = SPHandler.Handler.TestConnection(out hasConnection);

            if (hasConnection)
            {
                mSender.Background = _settingRightColor;
                Properties.Settings1.Default.SpUserName = SpUserName.Text;
            }
            else
                mSender.Background = _settingWrongColor;
        }

        /// <summary>
        /// select directory through selector window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectDirectory(object sender, RoutedEventArgs e)
        {
            // https://stackoverflow.com/questions/1922204/open-directory-dialog
            return;
        }

        /// <summary>
        /// check if TextBox.Text is a valid path and color box accordingly
        /// </summary>
        /// <param name="tb"></param>
        /// <returns></returns>
        private bool TextBoxPathIsValid(TextBox tb)
        {
            string dir;
            try
            {
                dir = Path.GetFullPath(tb.Text);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e);
                return false;
            }

            if (Directory.Exists(dir))
            {
                tb.Foreground = _settingRightColor;
                return true;
            }
            else
            {
                tb.Foreground = _settingWrongColor;
                //MessageBox.Show("Kein gültiger Pfad", "Bitte gib einen gültigen Pfad ein", MessageBoxButton.OK);
                return false;
            }
        }

        /// <summary>
        /// on every text change, check if path is valid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxTextChangedLinkPath(object sender, TextChangedEventArgs e)
        {
            var pSender = sender as TextBox;
            if (pSender == null)
                return;

            TextBoxPathIsValid(pSender);
        }

        /// <summary>
        /// on window closing event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var checkElementsList = new List<TextBox>{SpSave, SkizzeSpSave, DbLocalSave, DbSpSave, SkizzeLocalSave};
            foreach (var textBox in checkElementsList)
            {
                if (!TextBoxPathIsValid(textBox))
                {
                    MessageBox.Show("Bitte geben Sie überall gültige Pfade ein um fortzufahren", "Kein gültiger Pfad", MessageBoxButton.OK, MessageBoxImage.Error);
                    e.Cancel = true;
                    return;
                }
            }
            Properties.Settings1.Default.Save();
        }

        #endregion
    }
}
