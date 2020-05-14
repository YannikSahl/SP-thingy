using System;
using System.Collections.Generic;
using System.Data;
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
            fillPP();
        }

        public void fillPP()
        {
            DataTable pp = new DataTable();

            List<DataColumn> cols = new List<DataColumn>();

            cols.Add(new DataColumn("PAD", typeof(String)));
            cols.Add(new DataColumn("PAuftr", typeof(String)));
            cols.Add(new DataColumn("PDatum", typeof(DateTime)));
            cols.Add(new DataColumn("loeschDatum", typeof(DateTime)));
            cols.Add(new DataColumn("PArt", typeof(String)));

            //DataColumn datum = new DataColumn("PDatum", typeof(DateTime));
            //DataColumn loeschDatum = new DataColumn("loeschDatum", typeof(DateTime));
            //DataColumn art = new DataColumn("PArt", typeof(String));

            foreach(var dc in cols)
            {
                pp.Columns.Add(dc);
            }

            DataRow rowTest = pp.NewRow();
            rowTest[0] = "1z0825z080";
            rowTest[1] = "Auftrag";
            rowTest[2] = new DateTime(2020, 12, 24);
            rowTest[3] = new DateTime(2020, 3, 12);
            rowTest[4] = "Eine Art";

            pp.Rows.Add(rowTest);
            PP_TABELLE.ItemsSource = pp.DefaultView;
        }
    }
}
