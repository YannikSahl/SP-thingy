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
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
        }

        public void saveAuth()
        {
            SPHandler.Handler.setUsername(sp_name.Text);
            SPHandler.Handler.setPassword(sp_pw.Password);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            saveAuth();
            if (SPHandler.Handler.testConnection())
                sp_test.Background = new SolidColorBrush(Color.FromRgb(20, 200, 20));
            else
                sp_test.Background = new SolidColorBrush(Color.FromRgb(200,20,20));
        }
    }
}
