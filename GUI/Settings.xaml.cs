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
        private SolidColorBrush _settingWrongColor = new SolidColorBrush(Colors.Red);
        private SolidColorBrush _settingRightColor = new SolidColorBrush(Colors.ForestGreen);
        public Settings()
        {
            InitializeComponent();
            DbLocalSave.Text = Path.GetFullPath("..\\..\\..\\..\\DBHandler");
            SkizzeLocalSave.Text = Path.GetFullPath("..\\..\\..\\..\\DBHandler");
        }

        public void saveAuth()
        {
            SPHandler.Handler.SetUsername(sp_name.Text);
            SPHandler.Handler.SetPassword(sp_pw.Password);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button mSender = (Button) sender;
            saveAuth();
            bool hasConnection;
            string errorMessage = SPHandler.Handler.TestConnection(out hasConnection);

            if (hasConnection)
                mSender.Background = _settingRightColor;
            else
                mSender.Background = _settingWrongColor;
        }

        //private void TexBoxLinkEnter(object sender, RoutedEventArgs e)
        //{
        //    var children = ((DockPanel) ((Button) sender).Parent).Children;
        //    foreach (var child in children)
        //    {
        //        var pSender = child as TextBox;
        //        if (pSender == null)
        //            continue;

        //        var dir = Path.GetFullPath(pSender.Text);
        //        if (Directory.Exists(dir))
        //        {
        //            pSender.Foreground = new SolidColorBrush(Colors.ForestGreen);
        //            return;
        //        }
        //        else
        //        {
        //            pSender.Foreground = new SolidColorBrush(Colors.Red);
        //            MessageBox.Show("Kein gültiger Pfad", "Bitte gib einen gültigen Pfad ein", MessageBoxButton.OK);
        //            return;
        //        }
        //    }
        //}

        private void SelectDirectory(object sender, RoutedEventArgs e)
        {
            // https://stackoverflow.com/questions/1922204/open-directory-dialog

            //using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            //{
            //    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            //}
        }

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

        private void TextBoxTextChangedLinkPath(object sender, TextChangedEventArgs e)
        {
            var pSender = sender as TextBox;
            if (pSender == null)
                return;

            TextBoxPathIsValid(pSender);
        }

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
    }
}
