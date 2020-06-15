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
    }
}
