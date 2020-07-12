using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GUI.Properties;
using SPHandler;

namespace GUI
{
    /// <summary>
    ///     Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        #region constructors

        public Settings(MainWindow mainWin)
        {
            InitializeComponent();
            //DbLocalSave.Text = Path.GetFullPath("..\\..\\..\\..\\DBHandler");
            //SkizzeLocalSave.Text = Path.GetFullPath("..\\..\\..\\..\\DBHandler");
            InitLicenseText();
            _mainWindow = mainWin;
            SetSelectedThemeItem();
        }

        #endregion

        #region members

        private readonly MainWindow _mainWindow;
        private readonly SolidColorBrush _settingWrongColor = new SolidColorBrush(Colors.Red);
        private readonly SolidColorBrush _settingRightColor = new SolidColorBrush(Colors.ForestGreen);

        #endregion

        #region methods

        /// <summary>
        ///     Set Combobox selection by current skin
        ///     should only be used initially
        /// </summary>
        private void SetSelectedThemeItem()
        {
            foreach (var item in ThemeSelector.Items)
                if (((ComboBoxItem) item).Tag.ToString().ToLower() == Enum.GetName(typeof(Application.Skins),
                    ((Application) System.Windows.Application.Current).Skin).ToLower())
                {
                    ThemeSelector.SelectedItem = item;
                    return;
                }
        }

        /// <summary>
        ///     Checks a website exists or not
        ///     useful for checking if sp exists
        ///     source: https://stackoverflow.com/questions/924679/c-sharp-how-can-i-check-if-a-url-exists-is-valid
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private bool RemoteFileExists(string url)
        {
            try
            {
                //Creating the HttpWebRequest
                var request = WebRequest.Create(url) as HttpWebRequest;
                if (request == null)
                    return false;
                //Setting the Request method HEAD, you can also use GET too.
                request.Method = "HEAD";
                //Getting the Web Response.
                var response = request.GetResponse() as HttpWebResponse;
                //Returns TRUE if the Status code == 200
                response.Close();
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch
            {
                //Any exception will returns false.
                return false;
            }
        }

        /// <summary>
        ///     License text setter
        /// </summary>
        private void InitLicenseText()
        {
            var text = "MIT License\n" +
                       "Copyright (c) 2020 Softwareentwicklungsprojekt / SoSe2020\n\n" +
                       "Permission is hereby granted, free of charge, to any person obtaining a copy" +
                       "of this software and associated documentation files (the \"Software\"), to deal" +
                       "in the Software without restriction, including without limitation the rights" +
                       "to use, copy, modify, merge, publish, distribute, sublicense, and/or sell" +
                       "copies of the Software, and to permit persons to whom the Software is" +
                       "furnished to do so, subject to the following conditions: " +
                       "The above copyright notice and this permission notice shall be included in all " +
                       "copies or substantial portions of the Software.\n" +
                       "THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR " +
                       "IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, " +
                       "FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE " +
                       "AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER " +
                       "LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, " +
                       "OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.";
            LizenzTextBox.Text = text;
        }

        /// <summary>
        ///     set SP pw and username
        /// </summary>
        public void SetCredentials()
        {
            Handler.SetUsername(SpUserName.Text);
            Handler.SetPassword(SpUserPw.Password);
        }

        /// <summary>
        ///     check if TextBox.Text is a valid path and color box accordingly
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

            tb.Foreground = _settingWrongColor;
            //MessageBox.Show("Kein gültiger Pfad", "Bitte gib einen gültigen Pfad ein", MessageBoxButton.OK);
            return false;
        }

        #endregion

        #region events

        private bool _userLoginInProgress;

        /// <summary>
        ///     test credentials and connection to SP
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_userLoginInProgress)
                return;
            _userLoginInProgress = true;
            var mSender = (Button) sender;
            SetCredentials();
            var errorMessage = await Task.Run(() => { return Handler.TryUserLogin(); });

            var hasConnection = errorMessage == null;

            if (hasConnection)
            {
                mSender.Background = _settingRightColor;
                Settings1.Default.SpUserName = SpUserName.Text;
                _mainWindow.SetConnectionStatus(MainWindow.ConnectionModus.Online);
            }
            else
            {
                mSender.Background = _settingWrongColor;
            }

            _userLoginInProgress = false;
        }

        /// <summary>
        ///     select directory through selector window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectDirectory(object sender, RoutedEventArgs e)
        {
            // https://stackoverflow.com/questions/1922204/open-directory-dialog
        }

        /// <summary>
        ///     on every text change, check if path is valid
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
        ///     on window closing event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var checkElementsList = new List<TextBox> {DbLocalSave, SkizzeLocalSave};
            foreach (var textBox in checkElementsList)
                if (!TextBoxPathIsValid(textBox))
                {
                    MessageBox.Show("Bitte geben Sie überall gültige Pfade ein um fortzufahren",
                        "Kein gültiger Pfad",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    e.Cancel = true;
                    return;
                }

            Settings1.Default.PathSp = SpSave.Text;
            Settings1.Default.PathDbInSp = DbSpSave.Text;
            Settings1.Default.PathDbLocal = DbLocalSave.Text;
            Settings1.Default.PathSkizzenInSp = SkizzeSpSave.Text;
            Settings1.Default.PathSkizzenLocal = SkizzeLocalSave.Text;
            Settings1.Default.Save();
            _mainWindow.SettingsWindow = null;
        }

        private bool skipInitialSelection = true;

        /// <summary>
        ///     Skin/Theme Combobox selection changed event, sets selected skin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThemeSelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (skipInitialSelection)
            {
                skipInitialSelection = false;
                return;
            }

            var selected = ThemeSelector.SelectedItem as ComboBoxItem;
            if (selected == null)
                return;
            Enum.TryParse(typeof(Application.Skins), (string) selected.Tag, out var index);
            ((Application) System.Windows.Application.Current).ChangeSkin((Application.Skins) index);
        }

        #endregion
    }
}