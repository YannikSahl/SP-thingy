using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DBHandler;
using GUI.Properties;

namespace GUI
{
    /// <summary>
    /// Interaction logic for Abfragen.xaml
    /// </summary>
    public partial class Abfragen : Window
    {
        #region statics

        #endregion

        #region members

        private MainWindow _mainWindow;

        #endregion

        #region constructors

        public Abfragen(MainWindow mw)
        {
            InitializeComponent();
            InitSavedQueriesTable();
            _mainWindow = mw;
        }

        #endregion

        #region methods

        /// <summary>
        /// set up columns for saved queries table
        /// </summary>
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
            dt.Columns.Add(new DataColumn("AbfrageTyp", typeof(string)));
            dt.Columns.Add(new DataColumn("erstellt", typeof(DateTime)));
            dt.Columns.Add(new DataColumn("Hash", typeof(string)));

            foreach (SettingsPropertyValue s in SavedQueries.Default.PropertyValues)
            {
                //dt.Rows.Add((DataRow)s.PropertyValue);
                var row = s.PropertyValue as DataRow;
                if (row == null)
                    continue;
                dt.ImportRow(row);
            }

            SavedQueriesGrid.DataContext = dt;
            SavedQueriesGrid.ItemsSource = dt.DefaultView;
        }

        /// <summary>
        /// generates hash from hashing algorithm and message
        /// </summary>
        /// <param name="hashAlgorithm"></param>
        /// <param name="input"></param>
        /// <returns>string</returns>
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

        /// <summary>
        /// generates a hash from a whole dataRow
        /// </summary>
        /// <param name="dr"></param>
        /// <returns>string</returns>
        private string HashFromRow(DataRow dr)
        {
            StringBuilder s = new StringBuilder();
            foreach (var item in dr.ItemArray)
            {
                if (item.Equals(dr["Hash"]) || item.Equals(dr["erstellt"]))
                    continue;
                s.Append(item.ToString());
            }

            var msg = s.ToString();
            var hash = GetHash(MD5.Create(), msg);
            return hash;
        }

        /// <summary>
        /// checks if a saved query exists by comparing hashes
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="equalRowReference"></param>
        /// <returns></returns>
        private DataRow SavedQueryExists(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
                return null;
            DataTable dt = (DataTable)SavedQueriesGrid.DataContext;
            foreach (DataRow row in dt.Rows)
            {
                if (row["Hash"].Equals(hash))
                {
                    return row;
                }
            }

            return null;
        }

        /// <summary>
        /// splits string by separator chars and returns list as hashlist
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private HashSet<string> TextToHashSet(string text)
        {
            var attributeSet = new HashSet<string>();

            if (string.IsNullOrWhiteSpace(text))
                return attributeSet;

            char[] separators = { ',', ';' };
            var arr = text.Split(separators);
            foreach (var att in arr)
            {
                if (!"".Equals(att))
                {
                    attributeSet.Add(att.Trim());
                }
            }

            return attributeSet;
        }

        /// <summary>
        /// creates setting property for a savedquery row
        /// TODO: currently not working. instead of using settings file, normal xml should be used
        /// TODO: also, in the create hash method, a separator like ";;" should be used between cells so that only the hash
        /// TODO: can be stored in the xml settings. When loading it can be decrypted and split into an array by separator to get cell values
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="dr"></param>
        private void CreateSetting(string hash, DataRow dr)
        {
            // https://stackoverflow.com/questions/175726/c-create-new-settings-at-run-time

            //// create new setting from a base setting:
            //var property = new SettingsProperty(hash);
            ////property.Name = hash;
            //property.PropertyType = typeof(DataRow);
            //property.SerializeAs = SettingsSerializeAs.Xml;
            //SavedQueries.Default.Properties.Add(property);
            //SavedQueries.Default.Save();
            //// will have the stored value:
            ////var dynamicSetting = SavedQueries.Default[hash];
            ///
            ApplicationSettingsBase settings = SavedQueries.Default;
            SettingsProvider sp = settings.Providers["LocalFileSettingsProvider"];
            SettingsProperty p = new SettingsProperty(hash);
            DataRow conf = null;
            p.PropertyType = typeof(DataRow);
            p.Attributes.Add(typeof(UserScopedSettingAttribute), new UserScopedSettingAttribute());
            p.Provider = sp;
            p.SerializeAs = SettingsSerializeAs.Xml;
            SettingsPropertyValue v = new SettingsPropertyValue(p);
            settings.Properties.Add(p);

            settings.Reload();
            conf = (DataRow)settings[hash];
            if (conf == null)
            {
                settings[hash] = conf = dr;
                settings.Save();
            }
        }

        #endregion

        #region events

        /// <summary>
        /// adds current query "session" to table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddSavedQueryButton_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = (DataTable) SavedQueriesGrid.DataContext;
            DataRow dr = dt.NewRow();
            // TODO: Das alles muss in methoiden verpackt werden
            dr["Strecke"] = StreckeInput.Text;
            dr["Auftrag"] = AuftragInput.Text;
            dr["Punktart"] = PunktartInput.Text;
            // von km
            dr["von_km"] = VonKm.Text;
            dr["bis_km"] = BisKm.Text;
            // Punkt
            dr["von_Punkt"] = VonPunkt.Text;
            dr["bis_Punkt"] = BisPunkt.Text;
            // Blatt
            dr["von_Blatt"] = VonBlatt.Text;
            dr["bis_Blatt"] = BisBlatt.Text;
            // Operator
            var isAndOperator = AndOp.IsChecked.HasValue ? AndOp.IsChecked.Value : false;
            //dr["Operator"] = isAndOperator ? SqlOperators.And : SqlOperators.Or;
            dr["Operator"] = isAndOperator ? "and" : "or";
            // SQL Text
            dr["SQL"] = RawQuery.Text;
            // Query Typ
            var queryByText = QueryInA.IsChecked.HasValue ? QueryInA.IsChecked.Value : false;
            //dr["AbfrageTyp"] = queryByText ? QueryTypes.Parameter : QueryTypes.Sql;
            dr["AbfrageTyp"] = queryByText ? "param" : "code";
            // Hash
            var hash = HashFromRow(dr);
            dr["Hash"] = hash;
            dr["erstellt"] = DateTime.UtcNow;

            DataRow sameRow = SavedQueryExists(hash);

            // if no same row, meaning that no such row exists
            if (sameRow == null)
            {
                dt.Rows.Add(dr);
                SavedQueriesGrid.DataContext = dt;
                SavedQueriesGrid.ItemsSource = dt.DefaultView;
            }
        }

        /// <summary>
        /// starts query and redirects to mainwindow if query successful
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AbfrageStartButton_Click(object sender, RoutedEventArgs e)
        {
            DbHandler dbh = new DbHandler("..\\..\\..\\..\\DBHandler\\Datenmodell.accdb");
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
            else if (queryExecuted == StatusCode.OleDbNotRegistered)
            {
                MessageBox.Show(
                    $"Access Runtime ist nicht installiert\nBitte hier runterladen: https://www.microsoft.com/en-us/download/confirmation.aspx?id=13255", 
                    "Abfrage Fehlgeschlagen", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error,
                    MessageBoxResult.OK);
            }
            else
            {
                MessageBox.Show(
                    $"Die eingegebene SQL Abfrage konnte nicht ausgeführt werden ({Enum.GetName(typeof(StatusCode), queryExecuted)})",
                    "Abfrage Fehlgeschlagen",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information,
                    MessageBoxResult.OK);
            }
        }

        /// <summary>
        /// loads currently selected row as query from table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadSavedQueryButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = SavedQueriesGrid.SelectedItem as DataRowView;
            if (selected == null)
                return;
            DataRow dr = selected.Row;

            StreckeInput.Text = dr["Strecke"] as string ?? "";
            AuftragInput.Text = dr["Auftrag"] as string ?? "";
            PunktartInput.Text =dr["Punktart"] as string ?? "";
            // von km
            VonKm.Text = dr["von_km"] as string ?? "";
            BisKm.Text = dr["bis_km"] as string ?? "";
            // Punkt
            VonPunkt.Text = dr["von_Punkt"] as string ?? "";
            BisPunkt.Text = dr["bis_Punkt"] as string ?? "";
            // Blatt
            VonBlatt.Text = dr["von_Blatt"] as string ?? "";
            BisBlatt.Text = dr["bis_Blatt"] as string ?? "";
            // Operator
            var op = dr["Operator"] as string ?? "";
            if ("or".Equals(op.ToLower()))
            {
                OrOp.IsChecked = true;
            }
            else
            {
                AndOp.IsChecked = true;
            }
            // SQL Text
            RawQuery.Text = dr["SQL"] as string ?? "";
            // Query Typ
            //if (QueryTypes.Parameter.Equals((QueryTypes)dr["AbfrageTyp"]))
            if ("param".Equals(dr["AbfrageTyp"] as string))
            {
                QueryInA.IsChecked = true;
            }
            else
            {
                QueryInB.IsChecked = true;
            }
        }

        /// <summary>
        /// Lost Cell focus Event (calculates hash again)
        /// has to be lost focus so that changes to cell can be committed before calculating hash again
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCellLostFocus(object sender, RoutedEventArgs e)
        {
            var cell = sender as DataGridCell;
            if (cell == null)
                return;
            if (!cell.IsEditing)
                SavedQueriesGrid.CommitEdit();
            var dr = SavedQueriesGrid.SelectedItem as DataRowView;
            if (dr == null)
                return;
            var hash = HashFromRow(dr.Row);
            dr.Row["Hash"] = hash;
            DataRow sameRowReference = SavedQueryExists(hash);
            // clear duplicates by removing first row with same hash if exists
            if (sameRowReference != null && sameRowReference != dr.Row)
                ((DataTable)SavedQueriesGrid.DataContext).Rows.Remove(sameRowReference);
        }

        /// <summary>
        /// window closing event, saves queries and resets mainwin reference
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var dt = SavedQueriesGrid.DataContext as DataTable;
            if (dt == null)
                return;
            foreach (DataRow row in dt.Rows)
            {
                var hash = (string)row["Hash"];
                try
                {
                    SavedQueries.Default[hash] = row;
                }
                catch (SettingsPropertyNotFoundException E)
                {
                    CreateSetting(hash, row);
                    //SavedQueries.Default.Properties[hash].DefaultValue = row;
                }
            }

            SavedQueries.Default.Save();
            _mainWindow._abfrageWindow = null;
        }

        #endregion
    }
}
