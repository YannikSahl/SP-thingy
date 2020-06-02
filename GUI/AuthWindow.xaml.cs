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

        private void OfflineModus_ButtonClick(object sender, RoutedEventArgs e)
        {
            MainWindow settingsWin = new MainWindow(MainWindow.ConnectionModus.Offline);
            settingsWin.Show();
            this.Close();
        }
    }
}
