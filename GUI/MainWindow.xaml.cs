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
using System.Windows.Threading;
using DBHandler;

namespace GUI
{
    /*
    Author: Oliver Tworkowski
    Credit for license-free Icons:
    - sperren.png - https://www.flaticon.com/de/kostenloses-icon/sperren_483408?term=lock&page=1&position=7
    - entsperren.png - https://www.flaticon.com/de/kostenloses-icon/vorhangeschloss_126479
    - auge.png - https://www.flaticon.com/de/kostenloses-icon/auge_609494?term=view&page=1&position=67

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

        private static string m_phTableName = "PH";
        private static string m_plTableName = "PL";

        private ConnectionModus m_connectionModus;
        //ConnectionStatus
        private DBHandler.DbHandler m_databaseConnection;
        private bool m_isEditable = false;
        public bool isEditable
        {
            get { return m_isEditable; }
            set
            {
                SetEditable(value);
            }
        }

        public MainWindow(ConnectionModus conMod = ConnectionModus.Online)
        {
            InitializeComponent();
            m_connectionModus = conMod;
            PopoutConnectionStatusBar();

            if (m_connectionModus == ConnectionModus.Online)
            {
                SetEditable(true);
            }
            SetDatabase();
            SetPPDataTable();

            CollapseExpander();
            AddFilePreview("..\\..\\..\\..\\README.md");
            //AddFilePreview("..\\..\\..\\..\\DBHandler\\Datenmodell.accdb");
            string pt = "\\..jpeg_test.jpeg";
            AddFilePreview("..\\..\\..\\test_images\\png_test.png");
            AddFilePreview("..\\..\\..\\test_images\\jpeg_test.jpeg");
            AddFilePreview("..\\..\\..\\test_images\\pdf_test.pdf");
            AddFilePreview("..\\..\\..\\test_images\\txt_test.txt");
            AddFilePreview("..\\..\\..\\test_images\\pptx_test.pptx");
            //AddFilePreview("D:/GoldSquare_N.jpg");
            //AddFilePreview("D:/img1.png");
        }

        private void AddFilePreview(string path)
        {
            DocumentView.Children.Add(new FileView(path));
        }

        private void SetConnectionStatusBarStyle(string text, Color color)
        {
            ConnectionStatusBar.Text = text;
            var brush = new SolidColorBrush(color);
            ConnectionStatusBar.Background = brush;
            ConnectionStatusBar.Visibility = Visibility.Visible;
            StartTimer(5d);
            m_connectionStatusBarActive = true;
        }

        private void HideConnectionStatusBar()
        {
            ConnectionStatusBar.Visibility = Visibility.Hidden;
            m_connectionStatusBarActive = false;
        }

        // TODO: use this to determine whether timer should be reset when popout during popout
        private bool m_connectionStatusBarActive;
        private void StartTimer(double seconds)
        {
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(seconds)
            };
            timer.Tick += TimerTick;
            timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.Stop();
            timer.Tick -= TimerTick;
            HideConnectionStatusBar();
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
            PpTable.DataContext = pp;
            DataTable ph = m_databaseConnection.DbData.Tables[m_phTableName];
            DataTable pl = m_databaseConnection.DbData.Tables[m_plTableName];
            var phColumnsOnly = new DataTable(m_phTableName);
            foreach (DataColumn col in ph.Columns)
            {
                phColumnsOnly.Columns.Add(new DataColumn(col.ColumnName));
            }
            var plColumnsOnly = new DataTable(m_plTableName);
            foreach (DataColumn col in pl.Columns)
            {
                plColumnsOnly.Columns.Add(new DataColumn(col.ColumnName));
            }

            PlTable.DataContext = plColumnsOnly;
            PhTable.DataContext = phColumnsOnly;
        }

        private void SetPlOrPhTableByPad(string tableName, string pad)
        {
            DataGrid dg;
            TextBlock emptyMessageBlock;
            if (tableName == m_plTableName)
            {
                emptyMessageBlock = PlTableEmptyMessage;
                dg = PlTable;
            }else if (tableName == m_phTableName)
            {
                emptyMessageBlock = PhTableEmptyMessage;
                dg = PhTable;
            }
            else
            {
                throw new Exception("Weder PH noch PL wurde als Argument übergeben");
            }

            dg.ItemsSource = null;

            if (pad == null)
            {
                emptyMessageBlock.Visibility = Visibility.Visible;
                return;
            }
            var rows = m_databaseConnection.RetrieveRowByPad(tableName, pad);
            if (rows.Length == 0)
            {
                emptyMessageBlock.Visibility = Visibility.Visible;
                return;
            }
            emptyMessageBlock.Visibility = Visibility.Hidden;

            DataTable dt = (DataTable)dg.DataContext;
            dt.Rows.Clear();

            foreach (var row in rows)
            {
                dt.ImportRow(row);
            }
            dg.DataContext = dt;
            dg.ItemsSource = dt.DefaultView;
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
            SetPlOrPhTableByPad(m_phTableName, pad);
            SetPlOrPhTableByPad(m_plTableName, pad);
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

        /// <summary>
        /// wird aufgerufen wenn Expander kollabiert
        /// Anpassung anderer Elemente auf kollabierten Expander
        /// </summary>
        private void CollapseExpander()
        {
            DocumentViewContainer.Width = new GridLength(DocumentViewContainer.MinWidth);
            ViewSplitter.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// wird aufgerufen wenn Expander expandiert
        /// Anpassung anderer Elemente auf expandierten Expander
        /// </summary>
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

        private void Expander_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Expander ex = (Expander) sender;
            // Automatisches Collapsen vom Expander auslösen
            if (ex.ActualWidth < 35)
            {
                // Touch für ViewSplitter wegnehmen
                ViewSplitter.ReleaseAllTouchCaptures();
                ViewSplitter.IsHitTestVisible = false;

                ex.IsExpanded = false;
                // Expander_Collapsed Event wurde ausgelöst

                ViewSplitter.IsHitTestVisible = true;
            }
        }

        private void SetEditable(bool editable)
        {
            m_isEditable = editable;
            PhTable.IsReadOnly = !editable;
            PpTable.IsReadOnly = !editable;
            PlTable.IsReadOnly = !editable;
            BitmapImage icon;
            if (editable)
                icon = new BitmapImage(new Uri("..\\..\\..\\gui_resources\\entsperren.png", UriKind.Relative));
            else
                icon = new BitmapImage(new Uri("..\\..\\..\\gui_resources\\sperren.png", UriKind.Relative));
            ImageBrush ib = new ImageBrush(icon);
            ib.Stretch = Stretch.Uniform;
            EditModeIcon.Background = ib;
        }

        private void Bearbeiten_ButtonClick(object sender, RoutedEventArgs e)
        {
            SetEditable(!m_isEditable);
        }
    }
}
