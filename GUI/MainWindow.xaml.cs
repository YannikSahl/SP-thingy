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
        public MainWindow()
        {
            InitializeComponent();
            //fillPP();
            //initPP();
            //addPPRow(new PPRow{ pad = "Test1", pid = "Test2" });
            //PProw = new Dictionary<string, object>();
            //PProw.Add("PAD", "hgafg1f"); PProw.Add("PID", 7482);
            //columnLength = PProw.Count;
            //initPP();
            //FillPP(ref PP_TABELLE);
            SetDataTable();
            FillPP(ref PL_TABELLE);
            FillPP(ref PH_TABELLE);
        }
        #region testing
        int columnLength = 0;

        public void FillPP(ref DataGrid tabelle)
        {
            DataTable dt = new DataTable();

            List<DataColumn> cols = new List<DataColumn>();

            cols.Add(new DataColumn("PAD", typeof(string)));
            cols.Add(new DataColumn("PAuftr", typeof(string)));
            cols.Add(new DataColumn("PDatum", typeof(DateTime)));
            cols.Add(new DataColumn("loeschDatum", typeof(DateTime)));
            cols.Add(new DataColumn("PArt", typeof(string)));

            //DataColumn datum = new DataColumn("PDatum", typeof(DateTime));
            //DataColumn loeschDatum = new DataColumn("loeschDatum", typeof(DateTime));
            //DataColumn art = new DataColumn("PArt", typeof(string));

            foreach(var dc in cols)
            {
                dt.Columns.Add(dc);
            }

            DataRow rowTest = dt.NewRow();
            rowTest[0] = "1z0825z080";
            rowTest[1] = "Auftrag";
            rowTest[2] = new DateTime(2020, 12, 24);
            rowTest[3] = new DateTime(2020, 3, 12);
            rowTest[4] = "Eine Art";

            dt.Rows.Add(rowTest);
            tabelle.ItemsSource = dt.DefaultView;
        }

        public enum tables
        {
            PP_TABELLE,
            PH_TABELLE,
            PL_TABELLE
        }

        // fuktioniert nicht :(
        /*public void addPPRow(object)
        {
            DataTable dt = new DataTable();
            dt = ((DataView)PP_TABELLE.ItemsSource).ToTable();
            DataRow dr = dt.NewRow();

            foreach (var column in PProw)
            {
                pp.Columns.Add(new DataColumn(column.Key, column.Value.GetType()));
            }

            dt.Rows.Add(dr);

            PP_TABELLE.ItemsSource = dt.DefaultView;
        }*/

        Dictionary<string, object> PProw;

        private void initPP()
        {
            DataTable pp = new DataTable();
            //var members = typeof(PPRow).GetMembers();
            //foreach(var member in members)
            //{
            //    pp.Columns.Add(new DataColumn(member.Name, member.GetType()));
            //}
            foreach (var column in PProw)
            {
                pp.Columns.Add(new DataColumn(column.Key, column.Value.GetType()));
            }
            PP_TABELLE.ItemsSource = pp.DefaultView;
        }

        #endregion

        private void openSettingsWindow(object sender, RoutedEventArgs e)
        {
            Settings settingsWin = new Settings();
            settingsWin.Show();
        }

        private void openAbfrageWindow(object sender, RoutedEventArgs e)
        {
            Abfragen abfrageWin = new Abfragen();
            abfrageWin.Show();
        }

        private void SetDataTable()
        {
            DBConnection dbc = new DBConnection("..\\..\\..\\..\\DBHandler\\Datenmodell.accdb");
            DataTable pp = dbc.dbData.Tables["PP"];
            //PP_TABELLE.DataContext = pp;
            PP_TABELLE.ItemsSource = pp.DefaultView;
        }

        /// <summary>
        /// das anzuzeigende Thumbnail im Dataviewer
        /// </summary>
        public class displayFile
        {
            public displayFile() { }
            public displayFile(string path) {
                load(path);
            }

            public string extension;
            public string name;
            public string fullpath;
            public string customColorGradient;

            /// <summary>
            /// lädt sich selber aus Dateipfad
            /// </summary>
            /// <returns>true if correctly loaded</returns>
            public bool load(string path)
            {
                var file = new FileInfo(path);
                // do some exception handling here!
                // return false;
                this.extension = file.Extension;
                this.name = file.Name;
                this.fullpath = file.FullName;
                // custom color gradients hier
                if (file.Extension == "jpg" || file.Extension == "jpeg")
                    this.customColorGradient = "";
                return true;
            }

            /// <summary>
            /// zeigt Element als Thumbnail im Documentviewer an
            /// </summary>
            public void display()
            {
                // add file symbol with gradient + filename + path to the Dataview
            }
        }

        //public displayFile loadFile(string path) {
        //    var file = new FileInfo(path);
        //    // do some exception handling here!
        //    // return null;
        //    var displFile = new displayFile { extension = file.Extension, name = file.Name, fullpath = file.FullName};
        //    // custom color gradients hier
        //    if (file.Extension == "jpg" || file.Extension == "jpeg")
        //        displFile.customColorGradient = "";
        //    return displFile;
        //}
    }
}
