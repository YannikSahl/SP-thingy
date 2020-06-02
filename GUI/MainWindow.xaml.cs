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
        private DBHandler.DBHandler m_databaseConnection;

        public MainWindow()
        {
            InitializeComponent();
            SetDatabase();
            SetPPDataTable();
            SetPlAndPhTables();
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
            m_databaseConnection = new DBHandler.DBHandler("..\\..\\..\\..\\DBHandler\\Datenmodell.accdb");
        }

        /// <summary>
        /// Sets PP main table to data view
        /// </summary>
        private void SetPPDataTable()
        {
            DataTable pp = m_databaseConnection.dbData.Tables["PP"];
            PP_TABELLE.DataContext = pp;
        }

        private void SetPlAndPhTables()
        {
            DataTable ph = m_databaseConnection.dbData.Tables["PH"];
            PH_TABELLE.ItemsSource = ph.DefaultView;
            DataTable pl = m_databaseConnection.dbData.Tables["PL"];
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
            m_databaseConnection.updateDatabases();
        }

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

        // https://stackoverflow.com/questions/25229503/findvisualchild-reference-issue?noredirect=1&lq=1
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj)
            where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public static childItem FindVisualChild<childItem>(DependencyObject obj)
            where childItem : DependencyObject
        {
            foreach (childItem child in FindVisualChildren<childItem>(obj))
            {
                return child;
            }

            return null;
        }

        // https://stackoverflow.com/questions/3671003/wpf-how-to-get-a-cell-from-a-datagridrow/21295435
        static DataGridCell GetCell(DataGrid grid, DataGridRow row, int columnIndex = 0)
        {
            if (row == null) return null;

            //var presenter = row.FindVisualChild<DataGridCellsPresenter>();
            //var presenter = FindVisualChildren<DataGridCellsPresenter>(row);
            var presenter = FindVisualChild<DataGridCellsPresenter>(row);

            if (presenter == null) return null;

            var cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
            if (cell != null) return cell;

            // now try to bring into view and retreive the cell
            grid.ScrollIntoView(row, grid.Columns[columnIndex]);
            cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);

            return cell;
        }

        // HIER MUSS DATA BINDING GEMACHT WERDEN! so ist das leider sehr kompliziert
        private void RowSelected(object sender, RoutedEventArgs e)
        {
            //// hotfix vor Sprint Ende
            //// komplizierter weg um die PAD zu bekommen
            //var row = sender as DataGridRow;
            //// String Operationen um die PAD zu bekommen
            //var padRaw = GetCell(PP_TABELLE, row).ToString().Split(" ").ToList();
            //padRaw.RemoveAt(0);
            //var pad = string.Concat(padRaw);
            //SelectedPad.Text = pad; //(DataGrid)sender;
        }

        private void CloseApplication(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
