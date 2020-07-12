using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GUI.Properties;
using Microsoft.Win32;
using SPHandler;

namespace GUI
{
    /// <summary>
    /// Interaction logic for AuthWindow.xaml
    /// </summary>
    public partial class FilePathWindow : Window
    {
        #region constructors

        public FilePathWindow()
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
        /// redirect to mainwin
        /// </summary>
        /// <param name="connectionStatus"></param>
        private void RedirectToMainWindow(MainWindow.ConnectionModus connectionStatus)
        {

            // Show main window
            MainWindow settingsWin = new MainWindow(connectionStatus);
            settingsWin.Show();

            // Pass filepath


            // Close this window
            this.Close();
        }



        #endregion

        #region events

        private void Confirm_ButtonClick(object sender, RoutedEventArgs e)
        {

            // Read textbot
            var filePath = FilePathBox.Text;

            // Get the file attributes for file or directory
            try
            {

                // Inspect file
                FileAttributes attr = File.GetAttributes(filePath);

                // Detect whether its a directory or file
                if (attr.HasFlag(FileAttributes.Directory) || !filePath.EndsWith(".accdb"))
                    MessageBox.Show("Bitte wählen Sie eine Microsoft Acess Datei (Dateiendung .accdb) aus!");
                else
                {

                    // Show main window
                    RedirectToMainWindow(MainWindow.ConnectionModus.Offline);
                    Settings1.Default.PathDbLocal = Path.GetDirectoryName(filePath);

                }
            }
            catch (System.IO.FileNotFoundException)
            {

                MessageBox.Show("Datei konnte nicht gefunden werden. Bitte überprüfen Sie den Pfad!");
            }
            catch (System.ArgumentException)
            {
                MessageBox.Show("Dateipfad darf nicht leer sein!");
            }

        }


        #endregion

        private void OpenDataButton_Click(object sender, RoutedEventArgs e)
        {
            // Datei öffnen Dialog
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Access DB Dateien (*.accdb)|*.accdb"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                var path = openFileDialog.FileName;
                FilePathBox.Text = path;
                if (File.Exists(path))
                {
                    Settings1.Default.PathDbLocalFile = path;
                }
            }
        }
    }
}
