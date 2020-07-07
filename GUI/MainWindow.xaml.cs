using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
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
        #region statics

        public enum ConnectionModus
        {
            Offline,
            Online,
            LostConnection
        }

        private readonly SolidColorBrush _lostConnectionColor = new SolidColorBrush(Colors.Red);
        private readonly SolidColorBrush _onlineColor = new SolidColorBrush(Colors.ForestGreen);
        private readonly SolidColorBrush _offlineColor = new SolidColorBrush(Colors.DarkOrange);

        private static readonly string m_ppTableName = "PP";
        private static readonly string m_phTableName = "PH";
        private static readonly string m_plTableName = "PL";

        #endregion

        #region members

        private ConnectionModus m_connectionModus;
        
        //ConnectionStatus
        private DbHandler m_databaseConnection;
        private bool m_isEditable;
        private bool m_viewModeOnly;

        public Settings _settingsWindow;
        public Abfragen _abfrageWindow;

        #endregion

        #region constructors

        public MainWindow(ConnectionModus conMod = ConnectionModus.Online)
        {
            InitializeComponent();
            SetConnectionStatus(conMod);

            //LoadTables();
            //_abfrageWindow = new Abfragen(this);
            //_abfrageWindow.Show();
            //_abfrageWindow.Activate();
            //_abfrageWindow.Focus();

            CollapseExpander();
            AddFilePreview("..\\..\\..\\..\\README.md");
            AddFilePreview("..\\..\\..\\..\\DBHandler\\Datenmodell.accdb");
            AddFilePreview("..\\..\\..\\test_images\\png_test.png");
            AddFilePreview("..\\..\\..\\test_images\\jpeg_test.jpeg");
            AddFilePreview("..\\..\\..\\test_images\\pdf_test.pdf");
            AddFilePreview("..\\..\\..\\test_images\\txt_test.txt");
            AddFilePreview("..\\..\\..\\test_images\\pptx_test.pptx");
        }

        #endregion

        #region Custom Methods

        private bool UpdateDatabases()
        {
            if (m_databaseConnection == null)
                return true;
            var result = m_databaseConnection.UpdateDatabases();
            if (result != StatusCode.CommandOk && result != StatusCode.NoDatabaseChanges)
            {
                MessageBox.Show($"Status Code: {Enum.GetName(result.GetType(), result)}", "Speichern Fehlgeschlagen", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return false;
            }

            SetStatusBarLastSaved();
            return true;
        }

        //private DateTime _lastSaved;
        private void SetStatusBarLastSaved()
        {
            SavedStatus.Text = DateTime.Now.ToLongTimeString();
        }

        /// <summary>
        /// sets the filter by string for column PAD in the PP table
        /// </summary>
        /// <param name="filter"></param>
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
            catch (EvaluateException e)
            {
                MessageBox.Show(e.ToString(), $"Interner Fehler", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
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

        /// <summary>
        /// displays rows that have the input pad
        /// sets either PL or PH table
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pad"></param>
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

        /// <summary>
        /// adds FileView class elements as child of file preview frame
        /// </summary>
        /// <param name="path"></param>
        private void AddFilePreview(string path)
        {
            DocumentView.Children.Add(new FileView(path));
        }

        /// <summary>
        /// sets style parameters for connection status bar popout
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        private void SetConnectionStatusBarStyle(string text, SolidColorBrush color)
        {
            ConnectionStatusBar.Text = text;
            ConnectionStatusBar.Background = color;
            ConnectionStatusBar.Visibility = Visibility.Visible;
            StartTimer(5d);
            m_connectionStatusBarActive = true;
        }

        /// <summary>
        /// hides connection status bar popout
        /// </summary>
        private void HideConnectionStatusBar()
        {
            ConnectionStatusBar.Visibility = Visibility.Collapsed;
            m_connectionStatusBarActive = false;
        }

        // TODO: use this to determine whether timer should be reset when popout during popout
        private bool m_connectionStatusBarActive;
        /// <summary>
        /// starts time for popout visibility
        /// </summary>
        /// <param name="seconds"></param>
        private void StartTimer(double seconds)
        {
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(seconds)
            };
            timer.Tick += TimerTick;
            timer.Start();
        }

        /// <summary>
        /// pops out connection status bar according to connection mode
        /// </summary>
        private void PopoutConnectionStatusBar()
        {
            switch (m_connectionModus)
            {
                case ConnectionModus.Online:
                {
                    SetConnectionStatusBarStyle("Verbunden!", _onlineColor);
                    break;
                }
                case ConnectionModus.Offline:
                {
                    SetConnectionStatusBarStyle("Offline Modus", _offlineColor);
                    break;
                }
                case ConnectionModus.LostConnection:
                {
                    SetConnectionStatusBarStyle("Verbindung Fehlgeschlagen!", _lostConnectionColor);
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

        /// <summary>
        /// set whether tables should be editable or not
        /// </summary>
        /// <param name="editable"></param>
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

        /// <summary>
        /// sets status bar connection mode
        /// </summary>
        private void SetConnectionStatusTextInStatusBar()
        {
            ConnectionStatus.Text = Enum.GetName(typeof(ConnectionModus), m_connectionModus);
        }

        /// <summary>
        /// modify basic parameters based on connection mode
        /// accessible from outside so connection status can be modified
        /// </summary>
        /// <param name="mode"></param>
        public void SetConnectionStatus(ConnectionModus mode)
        {
            this.m_connectionModus = mode;
            // Statusanzeige (unten)
            SetConnectionStatusTextInStatusBar();
            PopoutConnectionStatusBar();
            switch (mode)
            {
                case ConnectionModus.LostConnection:
                    m_viewModeOnly = true;
                    SetEditable(false);
                    ConnectionStatus.Background = _lostConnectionColor;
                    break;
                case ConnectionModus.Offline:
                    m_viewModeOnly = true;
                    SetEditable(false);
                    ConnectionStatus.Background = _offlineColor;
                    break;
                case ConnectionModus.Online:
                    m_viewModeOnly = false;
                    SetEditable(true);
                    ConnectionStatus.Background = _onlineColor;
                    break;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// stops the currently running timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            _settingsWindow = new Settings(this);
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
        /// Row selection changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PpTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        #region expander events

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

        #endregion

        /// <summary>
        /// editmode button clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditModeButton_Click(object sender, RoutedEventArgs e)
        {
            if (m_viewModeOnly)
            {
                PopoutConnectionStatusBar();
                return;
            }
            SetEditable(!m_isEditable);
        }

        /// <summary>
        /// window closing event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var updated = UpdateDatabases();
            if (!updated)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Es sind noch ungespeicherte Änderungen vorhanden\nMöchten Sie die App trotzdem schließen?",
                    "Schließen wird verhindert",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Exclamation,
                    MessageBoxResult.Cancel
                    );
                if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }
            CloseApplication();
        }

        /// <summary>
        /// menu exit app click event closes the entire app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuCloseApp_Click(object sender, RoutedEventArgs e)
        {
            var updated = UpdateDatabases();
            if (!updated)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Es sind noch ungespeicherte Änderungen vorhanden\nMöchten Sie die App trotzdem schließen?",
                    "Schließen wird verhindert",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Exclamation,
                    MessageBoxResult.Cancel
                );
                if (result == MessageBoxResult.Cancel)
                    return;
            }
            CloseApplication();
        }

        #region search table events

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchPpTextfield.Text == "")
                return;
            SetPpSearchFilter(SearchPpTextfield.Text);
        }

        private void ClearPpSearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchPpTextfield.Text == "")
                return;
            SetPpSearchFilter("");
            SearchPpTextfield.Text = "";
        }

        #endregion

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            PpTable.CommitEdit();
            UpdateDatabases();
        }

        private void MainWindow_OnLostFocus(object sender, RoutedEventArgs e)
        {
            //UpdateDatabases();
        }

        #endregion
    }
}
