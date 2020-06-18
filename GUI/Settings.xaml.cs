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
        public Settings()
        {
            InitializeComponent();
            DbSave.Text = Path.GetFullPath("..\\..\\..\\..\\DBHandler");
        }

        public void saveAuth()
        {
            SPHandler.Handler.SetUsername(sp_name.Text);
            SPHandler.Handler.SetPassword(sp_pw.Password);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            saveAuth();
            bool hasConnection;
            string errorMessage = SPHandler.Handler.TestConnection(out hasConnection);

            if (hasConnection)
                sp_test.Background = new SolidColorBrush(Color.FromRgb(20, 200, 20));
            else
                sp_test.Background = new SolidColorBrush(Color.FromRgb(200,20,20));
        }

        private void TexBoxLinkEnter(object sender, RoutedEventArgs e)
        {
            var children = ((DockPanel) ((Button) sender).Parent).Children;
            foreach (var child in children)
            {
                var pSender = child as TextBox;
                if (pSender == null)
                    continue;

                var dir = Path.GetFullPath(pSender.Text);
                if (Directory.Exists(dir))
                {
                    pSender.Foreground = new SolidColorBrush(Colors.ForestGreen);
                    return;
                }
                else
                {
                    pSender.Foreground = new SolidColorBrush(Colors.Red);
                    MessageBox.Show("Kein gültiger Pfad", "Bitte gib einen gültigen Pfad ein", MessageBoxButton.OK);
                    return;
                }
            }
        }

        private void SelectDirectory(object sender, RoutedEventArgs e)
        {
            // https://stackoverflow.com/questions/1922204/open-directory-dialog

            //using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            //{
            //    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            //}
        }
    }
}
