using System;
using System.ComponentModel;
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
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region constructors

        public MainWindow(ConnectionModus conMod = ConnectionModus.Online)
        {
            InitializeComponent();

            SetConnectionStatus(conMod);
            CollapseExpander();
            SetStatusBarLastSaved();
        }

        #endregion

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

        private const string PpTableName = "PP";
        private const string PhTableName = "PH";
        private const string PlTableName = "PL";

        #endregion

        #region members

        private ConnectionModus _mConnectionMode;

        //ConnectionStatus
        private DbHandler _mDatabaseConnection;
        private bool _mIsEditable;
        private bool _mViewModeOnly;

        public Settings SettingsWindow;
        public Abfragen AbfrageWindow;
        public Export ExportWindow;

        private readonly BitmapImage _unlockedIcon =
            new BitmapImage(new Uri("..\\..\\..\\gui_resources\\entsperren.png", UriKind.Relative));

        private readonly BitmapImage _lockedIcon =
            new BitmapImage(new Uri("..\\..\\..\\gui_resources\\sperren.png", UriKind.Relative));

        #endregion

        #region Custom Methods

        private bool UpdateDatabases()
        {
            if (_mDatabaseConnection == null)
                return true;
            var result = _mDatabaseConnection.UpdateDatabases();
            if (result == StatusCode.CommandOk || result == StatusCode.NoDatabaseChanges)
            {
                SetStatusBarLastSaved();
                return true;
            }

            MessageBox.Show($"Status Code: {Enum.GetName(result.GetType(), result)}", "Speichern Fehlgeschlagen",
                MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            return false;
        }

        //private DateTime _lastSaved;
        private void SetStatusBarLastSaved()
        {
            SavedStatus.Text = DateTime.Now.ToLongTimeString();
        }

        /// <summary>
        ///     sets the filter by string for column PAD in the PP table
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
            var padColumnName = "PAD"; //table.Columns[0].ColumnName;
            // hier könnte man like verbinden mit "AND" expression um mehrere Zeilen zu suchen
            try
            {
                table.DefaultView.RowFilter = $"{padColumnName} LIKE '%{filter}%'";
            }
            catch (EvaluateException e)
            {
                MessageBox.Show(e.ToString(), "Interner Fehler", MessageBoxButton.OK, MessageBoxImage.Error,
                    MessageBoxResult.OK);
            }

            PpCount.Text = table.DefaultView.Count.ToString();
            SetPlOrPhTableByPad(PlTableName, null);
            SetPlOrPhTableByPad(PhTableName, null);
        }

        /// <summary>
        ///     Sets member database variable
        /// </summary>
        public void SetDatabase(DbHandler dbh)
        {
            SetPlOrPhTableByPad(PlTableName, null);
            SetPlOrPhTableByPad(PhTableName, null);
            // establish connection
            _mDatabaseConnection = dbh;
        }

        /// <summary>
        ///     Sets PP main table to data view
        /// </summary>
        public void LoadTables()
        {
            //SetDatabase(new DBHandler.DbHandler("..\\..\\..\\..\\DBHandler\\Datenmodell.accdb"));

            var pp = _mDatabaseConnection.DbData.Tables[PpTableName];
            PpTable.DataContext = pp;
            if (pp.Rows.Count == 0)
                PpTableEmptyMessage.Visibility = Visibility.Visible;
            else
                PpTableEmptyMessage.Visibility = Visibility.Hidden;
            PpCount.Text = pp.Rows.Count.ToString();
            var ph = _mDatabaseConnection.DbData.Tables[PhTableName];
            var pl = _mDatabaseConnection.DbData.Tables[PlTableName];

            // set columns for ph and pl
            var phColumnsOnly = new DataTable(PhTableName);
            foreach (DataColumn col in ph.Columns) phColumnsOnly.Columns.Add(new DataColumn(col.ColumnName));
            var plColumnsOnly = new DataTable(PlTableName);
            foreach (DataColumn col in pl.Columns) plColumnsOnly.Columns.Add(new DataColumn(col.ColumnName));

            PlTable.DataContext = plColumnsOnly;
            PhTable.DataContext = phColumnsOnly;
        }

        /// <summary>
        ///     displays rows that have the input pad
        ///     sets either PL or PH table
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pad"></param>
        private void SetPlOrPhTableByPad(string tableName, string pad)
        {
            DataGrid dg;
            TextBlock emptyMessageBlock;
            TextBlock counter;
            if (tableName == PlTableName)
            {
                emptyMessageBlock = PlTableEmptyMessage;
                dg = PlTable;
                counter = PlCount;
            }
            else if (tableName == PhTableName)
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

            var rows = _mDatabaseConnection.RetrieveRowByPad(tableName, pad);
            if (rows.Length == 0)
            {
                emptyMessageBlock.Visibility = Visibility.Visible;
                counter.Text = "0";
                return;
            }

            emptyMessageBlock.Visibility = Visibility.Hidden;

            var dt = (DataTable) dg.DataContext;
            dt.Rows.Clear();

            foreach (var row in rows) dt.ImportRow(row);
            dg.DataContext = dt;
            dg.ItemsSource = dt.DefaultView;
            counter.Text = rows.Length.ToString();
        }

        /// <summary>
        ///     adds FileView class elements as child of file preview frame
        /// </summary>
        /// <param name="path"></param>
        private void AddFilePreview(string path)
        {
            DocumentView.Children.Add(new FileView(path));
        }

        /// <summary>
        ///     sets style parameters for connection status bar popout
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
        ///     hides connection status bar popout
        /// </summary>
        private void HideConnectionStatusBar()
        {
            ConnectionStatusBar.Visibility = Visibility.Collapsed;
            m_connectionStatusBarActive = false;
        }

        // TODO: use this to determine whether timer should be reset when popout during popout
        private bool m_connectionStatusBarActive;

        /// <summary>
        ///     starts time for popout visibility
        /// </summary>
        /// <param name="seconds"></param>
        private void StartTimer(double seconds)
        {
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(seconds)
            };
            timer.Tick += TimerTick;
            timer.Start();
        }

        /// <summary>
        ///     pops out connection status bar according to connection mode
        /// </summary>
        private void PopoutConnectionStatusBar()
        {
            switch (_mConnectionMode)
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
            }
        }

        /// <summary>
        ///     Close Application
        /// </summary>
        private void CloseApplication()
        {
            System.Windows.Application.Current.Shutdown();
        }

        /// <summary>
        ///     wird aufgerufen wenn Expander kollabiert
        ///     Anpassung anderer Elemente auf kollabierten Expander
        /// </summary>
        private void CollapseExpander()
        {
            DocumentViewContainer.Width = new GridLength(DocumentViewContainer.MinWidth);
            ViewSplitter.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        ///     wird aufgerufen wenn Expander expandiert
        ///     Anpassung anderer Elemente auf expandierten Expander
        /// </summary>
        private void ExpandExpander()
        {
            DocumentViewContainer.Width = new GridLength(200);
            ViewSplitter.Visibility = Visibility.Visible;
        }

        /// <summary>
        ///     set whether tables should be editable or not
        /// </summary>
        /// <param name="editable"></param>
        private void SetEditable(bool editable)
        {
            _mIsEditable = editable;
            PhTable.IsReadOnly = !editable;
            PpTable.IsReadOnly = !editable;
            PlTable.IsReadOnly = !editable;
            BitmapImage icon;
            if (editable)
                icon = _unlockedIcon;
            else
                icon = _lockedIcon;
            var ib = new ImageBrush(icon);
            ib.Stretch = Stretch.Uniform;
            EditModeIcon.Background = ib;

            // Statusanzeige (unten)
            EditStatus.Text = editable ? "AN" : "AUS";
        }

        /// <summary>
        ///     sets status bar connection mode
        /// </summary>
        private void SetConnectionStatusTextInStatusBar()
        {
            ConnectionStatus.Text = $"{Enum.GetName(typeof(ConnectionModus), _mConnectionMode)} (Bearbeitung {(_mViewModeOnly ? "nicht" : "")} möglich)";
        }

        /// <summary>
        ///     modify basic parameters based on connection mode
        ///     accessible from outside so connection status can be modified
        /// </summary>
        /// <param name="mode"></param>
        public void SetConnectionStatus(ConnectionModus mode)
        {
            _mConnectionMode = mode;
            switch (mode)
            {
                case ConnectionModus.LostConnection:
                    //_mViewModeOnly = true;
                    //SetEditable(false);
                    _mViewModeOnly = false;
                    SetEditable(true);
                    ConnectionStatus.Background = _lostConnectionColor;
                    break;
                case ConnectionModus.Offline:
                    //_mViewModeOnly = true;
                    //SetEditable(false);
                    _mViewModeOnly = false;
                    SetEditable(true);
                    // TODO: da SP nicht ganz funktioniert, ist die Bearbeitung im Offline Modus auch aktiv
                    ConnectionStatus.Background = _offlineColor;
                    break;
                case ConnectionModus.Online:
                    _mViewModeOnly = false;
                    SetEditable(true);
                    ConnectionStatus.Background = _onlineColor;
                    break;
            }
            // Statusanzeige (unten)
            SetConnectionStatusTextInStatusBar();
            PopoutConnectionStatusBar();
        }

        #endregion

        #region Events

        /// <summary>
        ///     stops the currently running timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerTick(object sender, EventArgs e)
        {
            var timer = (DispatcherTimer) sender;
            timer.Stop();
            timer.Tick -= TimerTick;
            HideConnectionStatusBar();
        }

        /// <summary>
        ///     Opens Settings Window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenSettingsWindow(object sender, RoutedEventArgs e)
        {
            // bedeutet Fenster existiert schon
            if (SettingsWindow != null)
            {
                //SettingsWindow.Topmost = true;
                SettingsWindow.Activate();
                return;
            }

            SettingsWindow = new Settings(this);
            SettingsWindow.Show();
        }

        /// <summary>
        ///     Opens Query Window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenAbfrageWindow(object sender, RoutedEventArgs e)
        {
            // bedeutet Fenster existiert schon
            if (AbfrageWindow != null)
            {
                //SettingsWindow.Topmost = true;
                AbfrageWindow.Activate();
                return;
            }

            AbfrageWindow = new Abfragen(this);
            AbfrageWindow.Show();
        }

        /// <summary>
        ///     Row selection changed
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
            SetPlOrPhTableByPad(PhTableName, pad);
            SetPlOrPhTableByPad(PlTableName, pad);

            // Dokumente anzeigen
            // Nur testdokumente soweit
            DocumentView.Children.Clear();
            AddFilePreview("..\\..\\..\\..\\README.md");
            AddFilePreview("..\\..\\..\\..\\DBHandler\\Datenmodell.accdb");
            AddFilePreview("..\\..\\..\\test_images\\png_test.png");
            AddFilePreview("..\\..\\..\\test_images\\jpeg_test.jpeg");
            AddFilePreview("..\\..\\..\\test_images\\pdf_test.pdf");
            AddFilePreview("..\\..\\..\\test_images\\txt_test.txt");
            AddFilePreview("..\\..\\..\\test_images\\pptx_test.pptx");
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
            var ex = (Expander) sender;
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
        ///     editmode button clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditModeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_mViewModeOnly)
            {
                PopoutConnectionStatusBar();
                return;
            }

            SetEditable(!_mIsEditable);
        }

        /// <summary>
        ///     window closing event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var updated = UpdateDatabases();
            if (!updated)
            {
                var result = MessageBox.Show(
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
        ///     menu exit app click event closes the entire app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuCloseApp_Click(object sender, RoutedEventArgs e)
        {
            var updated = UpdateDatabases();
            if (!updated)
            {
                var result = MessageBox.Show(
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

        /// <summary>
        ///     button click event, saves database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            PpTable.CommitEdit();
            UpdateDatabases();
        }

        /// <summary>
        ///     window focus lost event, saves database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_OnLostFocus(object sender, RoutedEventArgs e)
        {
            //UpdateDatabases();
        }

        /// <summary>
        ///     menu item click event, opens export window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenExportWindow(object sender, RoutedEventArgs e)
        {
            // bedeutet Fenster existiert schon
            if (ExportWindow != null)
            {
                //SettingsWindow.Topmost = true;
                ExportWindow.Activate();
                return;
            }

            ExportWindow = new Export(this);
            ExportWindow.Show();
        }

        #endregion
    }
}