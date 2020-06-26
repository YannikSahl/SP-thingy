using System;
using System.Collections;
using System.Collections.Generic;
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
            var hashCol = new DataColumn("Hash", typeof(string));
            //hashCol.ColumnMapping = MappingType.Hidden;
            dt.Columns.Add(hashCol);
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

        private void AddSavedQueryButton_Click(object sender, RoutedEventArgs e)
        {
            string hashMessage = $"{StreckeInput.Text}{AuftragInput.Text}{PunktartInput.Text}";
            if ("".Equals(hashMessage))
                return;
            string hash = GetHash(MD5.Create(), hashMessage);
            DataTable dt = (DataTable) SavedQueriesGrid.DataContext;
            DataRow dr = dt.NewRow();
            dr[0] = StreckeInput.Text;
            dr[1] = AuftragInput.Text;
            dr[2] = PunktartInput.Text;
            dr[3] = hash;
            var exists = false;
            // check if row exists
            foreach (DataRow row in dt.Rows)
            {
                if (row.ItemArray[3].Equals(hash))
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
                //queryExecuted = dbh.FillInData(RawQuery.Text);
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
    }
}
