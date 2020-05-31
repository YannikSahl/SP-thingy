using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DBHandler;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DBConnection m_databaseConnection;

        public MainWindow()
        {
            InitializeComponent();
            SetDatabase();
            SetPPDataTable();
        }

        /// <summary>
        /// Opens Settings Window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openSettingsWindow(object sender, RoutedEventArgs e)
        {
            Settings settingsWin = new Settings();
            settingsWin.Show();
        }

        /// <summary>
        /// Opens Query Window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openAbfrageWindow(object sender, RoutedEventArgs e)
        {
            Abfragen abfrageWin = new Abfragen();
            abfrageWin.Show();
        }

        /// <summary>
        /// Sets member database variable
        /// </summary>
        private void SetDatabase()
        {
            // establish connection
            m_databaseConnection = new DBConnection("..\\..\\..\\..\\DBHandler\\Datenmodell.accdb");
        }

        /// <summary>
        /// Sets PP main table to data view
        /// </summary>
        private void SetPPDataTable()
        {
            DataTable pp = m_databaseConnection.dbData.Tables["PP"];
            PP_TABELLE.ItemsSource = pp.DefaultView;
        }

        /// <summary>
        /// Do something after row editing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PP_TABELLE_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            // update after row was edited
            m_databaseConnection.updateDatabases();
        }

        private void PP_TABELLE_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        // HIER MUSS DATA BINDING GEMACHT WERDEN! so geht das leider nicht :(
        private void RowSelected(object sender, RoutedEventArgs e)
        {
            //var row = sender as DataGridRow;
            //var pad = row.Item;
            //SelectedPad.Text = pad.ToString(); //(DataGrid)sender;
        }
    }
}
