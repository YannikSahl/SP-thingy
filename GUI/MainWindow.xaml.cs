using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /*
    Author: Oliver Tworkowski
    Credit for license-free Icons:
    - sperren.png - https://www.flaticon.com/de/kostenloses-icon/sperren_483408?term=lock&page=1&position=7

    TODO 1: DataGrid Struktur mit Liste binden, um auf Elemente einzeln zugreifen zu können und einfacher operationen durchführen zu können
    */

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public enum ConnectionModus
        {
            Offline,
            Online,
            LostConnection
        }

        private ConnectionModus m_connectionModus;
        private DBHandler.DbHandler m_databaseConnection;

        public MainWindow(ConnectionModus conMod = ConnectionModus.Online)
        {
            InitializeComponent();
            //SetDatabase();
            //SetPPDataTable();
            //SetPlAndPhTables();
            CollapseExpander();
            m_connectionModus = conMod;
            PopoutConnectionStatusBar();
        }

        private void SetConnectionStatusBarStyle(string text, Color color)
        {
            ConnectionStatusBar.Text = text;
            var brush = new SolidColorBrush(color);
            ConnectionStatusBar.Background = brush;
            ConnectionStatusBar.Visibility = Visibility.Visible;
        }

        private void PopoutConnectionStatusBar()
        {
            switch (m_connectionModus)
            {
                case ConnectionModus.Online:
                {
                    SetConnectionStatusBarStyle("Verbunden!", Colors.ForestGreen);
                    break;
                }
                case ConnectionModus.Offline:
                {
                    SetConnectionStatusBarStyle("Offline Modus", Colors.DarkOrange);
                    break;
                }
                case ConnectionModus.LostConnection:
                {
                    SetConnectionStatusBarStyle("Verbindung Fehlgeschlagen!", Colors.Red);
                    break;
                }
                default: break;
            }
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
            m_databaseConnection = new DBHandler.DbHandler("..\\..\\..\\..\\DBHandler\\Datenmodell.accdb");
        }

        /// <summary>
        /// Sets PP main table to data view
        /// </summary>
        private void SetPPDataTable()
        {
            DataTable pp = m_databaseConnection.DbData.Tables["PP"];
            PP_TABELLE.DataContext = pp;
        }

        private void SetPlAndPhTables()
        {
            DataTable ph = m_databaseConnection.DbData.Tables["PH"];
            PH_TABELLE.ItemsSource = ph.DefaultView;
            DataTable pl = m_databaseConnection.DbData.Tables["PL"];
            PL_TABELLE.ItemsSource = pl.DefaultView;
        }

        /// <summary>
        /// Do something after row editing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PP_TABELLE_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            // update after row was edited
            m_databaseConnection.UpdateDatabases();
        }

        /// <summary>
        /// Row selection changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PP_TABELLE_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // get RowView
            var dataRowView = e.AddedItems[0] as DataRowView;
            if (dataRowView == null)
                return;
            // get PAD from DataRow
            var pad = (string)dataRowView.Row["PAD"];
            SelectedPad.Text = pad; //(DataGrid)sender;
        }

        /// <summary>
        /// Close Application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseApplication(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void CollapseExpander()
        {
            DocumentViewContainer.Width = new GridLength(DocumentViewContainer.MinWidth);
            ViewSplitter.Visibility = Visibility.Collapsed;
        }

        private void ExpandExpander()
        {
            DocumentViewContainer.Width = new GridLength(200);
            ViewSplitter.Visibility = Visibility.Visible;
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            CollapseExpander();
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            ExpandExpander();
        }
    }
}
