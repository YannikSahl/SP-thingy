using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Controls;

namespace GUI
{
    class GUITest
    {
        
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


        //private void initPP()
        //{
        //    DataTable pp = new DataTable();
        //    //var members = typeof(PPRow).GetMembers();
        //    //foreach(var member in members)
        //    //{
        //    //    pp.Columns.Add(new DataColumn(member.Name, member.GetType()));
        //    //}
        //    foreach (var column in PProw)
        //    {
        //        pp.Columns.Add(new DataColumn(column.Key, column.Value.GetType()));
        //    }
        //    PP_TABELLE.ItemsSource = pp.DefaultView;
        //}

        #endregion
        

        /// <summary>
        /// das anzuzeigende Thumbnail im Dataviewer
        /// </summary>
        public class displayFile
        {
            public displayFile() { }
            public displayFile(string path)
            {
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
