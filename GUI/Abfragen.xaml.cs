using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DBHandler;

namespace GUI
{
    /// <summary>
    /// Interaction logic for Abfragen.xaml
    /// </summary>
    public partial class Abfragen : Window
    {
        private enum abfrageTypen
        {
            [Description("SQL")]
            Sql = 1,
            [Description("Text Parameter")]
            Parameter = 2
        }

        private MainWindow _mainWindow;

        public Abfragen(MainWindow mw)
        {
            InitializeComponent();
            InitSavedQueriesTable();
            _mainWindow = mw;
        }

        private void InitSavedQueriesTable()
        {
            DataTable dt = new DataTable("SavedQueries");
            dt.Columns.Add(new DataColumn("Strecke", typeof(string)));
            dt.Columns.Add(new DataColumn("Auftrag", typeof(string)));
            dt.Columns.Add(new DataColumn("Punktart", typeof(string)));
            dt.Columns.Add(new DataColumn("von_km", typeof(string)));
            dt.Columns.Add(new DataColumn("bis_km", typeof(string)));
            dt.Columns.Add(new DataColumn("von_Punkt", typeof(string)));
            dt.Columns.Add(new DataColumn("bis_Punkt", typeof(string)));
            dt.Columns.Add(new DataColumn("von_Blatt", typeof(string)));
            dt.Columns.Add(new DataColumn("bis_Blatt", typeof(string)));
            dt.Columns.Add(new DataColumn("Operator", typeof(string)));
            dt.Columns.Add(new DataColumn("SQL", typeof(string)));
            dt.Columns.Add(new DataColumn("AbfrageTyp", typeof(abfrageTypen)));
            dt.Columns.Add(new DataColumn("Hash", typeof(string)));
            SavedQueriesGrid.DataContext = dt;
        }

        // https://docs.microsoft.com/de-de/dotnet/api/system.security.cryptography.hashalgorithm.computehash?view=netcore-3.1
        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        private string HashFromRow(DataRow dr)
        {
            StringBuilder s = new StringBuilder();
            foreach (var item in dr.ItemArray)
            {
                s.Append(item.ToString());
            }

            var msg = s.ToString();
            var hash = GetHash(MD5.Create(), msg);
            return hash;
        }

        private void AddSavedQueryButton_Click(object sender, RoutedEventArgs e)
        {
            //string hashMessage = $"{StreckeInput.Text}{AuftragInput.Text}{PunktartInput.Text}{}";
            //if ("".Equals(hashMessage))
            //    return;
            //string hash = GetHash(MD5.Create(), hashMessage);
            DataTable dt = (DataTable) SavedQueriesGrid.DataContext;
            DataRow dr = dt.NewRow();
            // TODO: Das alles muss in methoiden verpackt werden
            dr[0] = StreckeInput.Text;
            dr[1] = AuftragInput.Text;
            dr[2] = PunktartInput.Text;
            // von km
            dr[3] = VonKm.Text;
            dr[4] = BisKm.Text;
            // Punkt
            dr[5] = VonPunkt.Text;
            dr[6] = BisPunkt.Text;
            // Blatt
            dr[7] = VonBlatt.Text;
            dr[8] = BisBlatt.Text;
            // Operator
            var isAndOperator = AndOp.IsChecked.HasValue ? AndOp.IsChecked.Value : false;
            dr[9] = isAndOperator ? "AND" : "OR";
            // SQL Text
            dr[10] = RawQuery.Text;
            // Query Typ
            var queryByText = QueryInA.IsChecked.HasValue ? QueryInA.IsChecked.Value : false;
            dr.ItemArray[11] = queryByText ? abfrageTypen.Parameter : abfrageTypen.Sql;
            // Hash
            string hash = HashFromRow(dr);
            dr[12] = hash;

            var exists = false;
            // check if row exists
            foreach (DataRow row in dt.Rows)
            {
                if (row.ItemArray[12].Equals(hash))
                {
                    exists = true;
                    break;
                }
            }

            if (!exists)
            {
                dt.Rows.Add(dr);
                SavedQueriesGrid.DataContext = dt;
                SavedQueriesGrid.ItemsSource = dt.DefaultView;
            }
        }

        private HashSet<string> TextToHashSet(string text)
        {
            char[] separators = {',', ';'};
            var arr = text.Split(separators);

            var attributeSet = new HashSet<string>();
            foreach (var att in arr)
            {
                if (!"".Equals(att))
                {
                    attributeSet.Add(att.Trim());
                }
            }

            return attributeSet;
        }

        private void AbfrageStartButton_Click(object sender, RoutedEventArgs e)
        {
            DBHandler.DbHandler dbh = new DBHandler.DbHandler("..\\..\\..\\..\\DBHandler\\Datenmodell.accdb");
            StatusCode queryExecuted = StatusCode.CommandFailed;
            bool queryByText = QueryInA.IsChecked.HasValue ? QueryInA.IsChecked.Value : false;
            if (queryByText)
            {
                // Query by Parameters from textboxes
                bool isAndOperator = AndOp.IsChecked.HasValue ? AndOp.IsChecked.Value : false;
                queryExecuted = dbh.FillInData(
                    new HashSet<string>(),
                    TextToHashSet(StreckeInput.Text),
                    TextToHashSet(PunktartInput.Text),
                    TextToHashSet(AuftragInput.Text),
                    isAndOperator
                );
            }else
            {
                // Query by raw SQL
                queryExecuted = dbh.FillInData(RawQuery.Text);
            }

            if (queryExecuted == StatusCode.CommandOk)
            {
                _mainWindow.SetDatabase(dbh);
                _mainWindow.LoadTables();
                _mainWindow.Activate();
                _mainWindow._abfrageWindow = null;
                this.Close();
            }
            else
            {
                // TODO: Oleddbexcpetion im DBHandler wenn falsche Query!
                MessageBox.Show(Enum.GetName(typeof(StatusCode), queryExecuted), "Query Fehlgeschlagen");
            }
        }

        private void DeleteSavedQueryButton_Click(object sender, RoutedEventArgs e)
        {
            /// geht alles niucht :(
            //var selected = SavedQueriesGrid.SelectedItems;
            //foreach (DataRowView row in selected)
            //{
            //    return;
            //}
        }

        private void LoadSavedQueryButton_Click(object sender, RoutedEventArgs e)
        {
            //DataTable dt = (DataTable)SavedQueriesGrid.DataContext;
            var selected = SavedQueriesGrid.SelectedItem as DataRowView;
            if (selected == null)
                return;
            DataRow dr = selected.Row;
            // TODO: string conversion error wenn cell = null
            // TODO: nach jedem Cell edit muss der hash neu berechnet werden
            StreckeInput.Text = (string)dr[0];
            AuftragInput.Text = (string)dr[1];
            PunktartInput.Text = (string)dr[2];
            // von km
            VonKm.Text = (string)dr[3];
            BisKm.Text = (string)dr[4];
            // Punkt
            VonPunkt.Text = (string)dr[5];
            BisPunkt.Text = (string)dr[6];
            // Blatt
            VonBlatt.Text = (string)dr[7];
            BisBlatt.Text = (string)dr[8];
            // Operator
            var isAndOperator = AndOp.IsChecked.HasValue ? AndOp.IsChecked.Value : false;
            if ("or".Equals(dr.ItemArray[9].ToString().ToLower()))
            {
                OrOp.IsChecked = true;
            }
            else
            {
                AndOp.IsChecked = true;
            }
            // SQL Text
            RawQuery.Text = (string)dr[10];
            // Query Typ
            if (abfrageTypen.Parameter.Equals((abfrageTypen)dr[11]))
            {
                QueryInA.IsChecked = true;
            }
            else
            {
                QueryInB.IsChecked = true;
            }
        }

        private void SavedQueriesGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var pSender = sender as DataGrid;
            if (pSender == null)
                return;
            var dr = pSender.SelectedItem as DataRowView;
            if (dr == null)
                return;
            dr.Row[12] = HashFromRow(dr.Row);
        }
    }
}
