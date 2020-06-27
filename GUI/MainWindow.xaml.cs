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
using System.Xml.Schema;
using DBHandler;

/*
    Author: Oliver Tworkowski, s0568202

    Credit for license-free Icons:
    - sperren.png - https://www.flaticon.com/de/kostenloses-icon/sperren_483408?term=lock&page=1&position=7
    - entsperren.png - https://www.flaticon.com/de/kostenloses-icon/vorhangeschloss_126479
    - auge.png - https://www.flaticon.com/de/kostenloses-icon/auge_609494?term=view&page=1&position=67

*/

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Statics

        public enum ConnectionModus
        {
            Offline,
            Online,
            LostConnection
        }

        private static readonly string m_ppTableName = "PP";
        private static readonly string m_phTableName = "PH";
        private static readonly string m_plTableName = "PL";

        #endregion

        #region Members

        private ConnectionModus m_connectionModus;
        
        //ConnectionStatus
        private DbHandler m_databaseConnection;
        private bool m_isEditable = false;

        private Settings _settingsWindow;
        public Abfragen _abfrageWindow;

        #endregion

        #region Constructors

        public MainWindow(ConnectionModus conMod = ConnectionModus.Online)
        {
            InitializeComponent();
            SetConnectionStatus(conMod);
            PopoutConnectionStatusBar();

            if (m_connectionModus == ConnectionModus.Online)
            {
                SetEditable(true);
            }
            else
            {
                SetEditable(false);
            }
            //LoadTables();
            //_abfrageWindow = new Abfragen(this);
            //_abfrageWindow.Show();
            //_abfrageWindow.Activate();
            //_abfrageWindow.Focus();

            CollapseExpander();
            AddFilePreview("..\\..\\..\\..\\README.md");
            //AddFilePreview("..\\..\\..\\..\\DBHandler\\Datenmodell.accdb");
            //AddFilePreview("..\\..\\..\\test_images\\png_test.png");
            //AddFilePreview("..\\..\\..\\test_images\\jpeg_test.jpeg");
            //AddFilePreview("..\\..\\..\\test_images\\pdf_test.pdf");
            //AddFilePreview("..\\..\\..\\test_images\\txt_test.txt");
            //AddFilePreview("..\\..\\..\\test_images\\pptx_test.pptx");
        }

        #endregion

        #region Custom Methods

        private void SetPpSearchFilter(string filter)
        {
            if (filter == null)
                return;
            var table = PpTable.DataContext as DataTable;
            if (table == null)
                return;
            // index 0 sollte PAD sein
            string padColumnName = "PAD"; //table.Columns[0].ColumnName;
            // hier könnte man like verbinden mit "AND" expression um mehrere Zeilen zu suchen
            try
            {
                table.DefaultView.RowFilter = $"{padColumnName} LIKE '%{filter}%'";
            }
            catch (EvaluateException E)
            {
                MessageBox.Show(E.ToString(), $"Interner Fehler", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }

            PpCount.Text = table.DefaultView.Count.ToString();
            SetPlOrPhTableByPad(m_plTableName, null);
            SetPlOrPhTableByPad(m_phTableName, null);
        }

        /// <summary>
        /// Sets member database variable
        /// </summary>
        public void SetDatabase(DBHandler.DbHandler dbh)
        {
            SetPlOrPhTableByPad(m_plTableName, null);
            SetPlOrPhTableByPad(m_phTableName, null);
            // establish connection
            m_databaseConnection = dbh;
        }

        /// <summary>
        /// Sets PP main table to data view
        /// </summary>
        public void LoadTables()
        {
            //SetDatabase(new DBHandler.DbHandler("..\\..\\..\\..\\DBHandler\\Datenmodell.accdb"));

            DataTable pp = m_databaseConnection.DbData.Tables[m_ppTableName];
            PpTable.DataContext = pp;
            if (pp.Rows.Count == 0)
                PpTableEmptyMessage.Visibility = Visibility.Visible;
            else
                PpTableEmptyMessage.Visibility = Visibility.Hidden;
            PpCount.Text = pp.Rows.Count.ToString();
            DataTable ph = m_databaseConnection.DbData.Tables[m_phTableName];
            DataTable pl = m_databaseConnection.DbData.Tables[m_plTableName];

            // set columns for ph and pl
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
            TextBlock counter;
            if (tableName == m_plTableName)
            {
                emptyMessageBlock = PlTableEmptyMessage;
                dg = PlTable;
                counter = PlCount;
            }
            else if (tableName == m_phTableName)
            {
                emptyMessageBlock = PhTableEmptyMessage;
                dg = PhTable;
                counter = PhCount;
            }
            else
            {
                throw new Exception("Weder PH noch PL wurde als Argument übergeben");
            }

            dg.ItemsSource = null;

            if (pad == null)
            {
                emptyMessageBlock.Visibility = Visibility.Visible;
                counter.Text = "0";
                return;
            }
            var rows = m_databaseConnection.RetrieveRowByPad(tableName, pad);
            if (rows.Length == 0)
            {
                emptyMessageBlock.Visibility = Visibility.Visible;
                counter.Text = "0";
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
            counter.Text = rows.Length.ToString();
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
            ConnectionStatusBar.Visibility = Visibility.Collapsed;
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
        /// Close Application
        /// </summary>
        private void CloseApplication()
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

        private void SetEditable(bool editable)
        {
            m_isEditable = editable;
            PhTable.IsReadOnly = !editable;
            PpTable.IsReadOnly = !editable;
            PlTable.IsReadOnly = !editable;
            BitmapImage icon;
            if (editable)
            {
                icon = new BitmapImage(new Uri("..\\..\\..\\gui_resources\\entsperren.png", UriKind.Relative));
                EditModeButton.Foreground = new SolidColorBrush(Colors.DarkGreen);
            }
            else
            {
                icon = new BitmapImage(new Uri("..\\..\\..\\gui_resources\\sperren.png", UriKind.Relative));
                EditModeButton.Foreground = new SolidColorBrush(Colors.Black);
            }
            ImageBrush ib = new ImageBrush(icon);
            ib.Stretch = Stretch.Uniform;
            EditModeIcon.Background = ib;

            // Statusanzeige (unten)
            EditStatus.Text = editable ? "AN" : "AUS";
        }

        private void SetConnectionStatus(ConnectionModus mode)
        {
            this.m_connectionModus = mode;
            // Statusanzeige (unten)
            ConnectionStatus.Text = Enum.GetName(typeof(ConnectionModus), mode);
        }

        #endregion

        #region Events

        private void TimerTick(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.Stop();
            timer.Tick -= TimerTick;
            HideConnectionStatusBar();
        }

        /// <summary>
        /// Opens Settings Window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenSettingsWindow(object sender, RoutedEventArgs e)
        {
            // bedeutet Fenster existiert schon
            if (_settingsWindow != null)
            {
                //_settingsWindow.Topmost = true;
                _settingsWindow.Activate();
                return;
            }

            _settingsWindow = new Settings();
            _settingsWindow.Show();
        }

        /// <summary>
        /// Opens Query Window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenAbfrageWindow(object sender, RoutedEventArgs e)
        {
            // bedeutet Fenster existiert schon
            if (_abfrageWindow != null)
            {
                //_settingsWindow.Topmost = true;
                _abfrageWindow.Activate();
                return;
            }

            _abfrageWindow = new Abfragen(this);
            _abfrageWindow.Show();
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
            DataRowView dataRowView = null;
            try
            {
                dataRowView = e.AddedItems[0] as DataRowView;
            }
            catch (IndexOutOfRangeException E)
            {
                // bedeutet dass zeile gelöscht ist
                return;
            }

            if (dataRowView == null)
                return;
            // get PAD from DataRow
            var pad = dataRowView.Row["PAD"] as string;
            // pad == null falls PAD nicht string ist (z.B. wenn leere Zeile ausgewählt)
            if (pad == null)
                return;
            SelectedPad.Text = pad; //(DataGrid)sender;
            SetPlOrPhTableByPad(m_phTableName, pad);
            SetPlOrPhTableByPad(m_plTableName, pad);
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

        private void EditModeButton_Click(object sender, RoutedEventArgs e)
        {
            SetEditable(!m_isEditable);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CloseApplication();
        }

        private void MenuCloseApp_Click(object sender, RoutedEventArgs e)
        {
            CloseApplication();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchPpTextfield.Text == "")
                return;
            SetPpSearchFilter(SearchPpTextfield.Text);
            //DataTable pp = _ppOriginal;
            //DataTable replace = new DataTable("PpSearched");
            //// Refractoring!
            //foreach (DataColumn column in pp.Columns)
            //{
            //    replace.Columns.Add(new DataColumn(column.ColumnName));
            //}
            //_ppOriginal = pp;
            //foreach (DataRow row in pp.Rows)
            //{
            //    var pad = row.ItemArray[0] as string;
            //    if (pad == null)
            //        continue;

            //    if (pad.Trim().Contains(SearchPpTextfield.Text.Trim()))
            //    {
            //        replace.ImportRow(row);
            //        PpTable.DataContext = replace;
            //    }
            //}
        }

        private void ClearPpSearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchPpTextfield.Text == "")
                return;
            SetPpSearchFilter("");
            SearchPpTextfield.Text = "";
        }

        #endregion
    }
}
