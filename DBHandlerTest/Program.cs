using System;
using System.Data;
using System.Data.Common;
using DBHandler;

namespace DBHandlerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileLocation = "Datenmodell.accdb";

            // Create object
            DBHandler.DbHandler dbHandler = new DBHandler.DbHandler(fileLocation);

            // Add row
            //AddRow(dbHandler);

            // Filter by id
            FilterbyId(dbHandler);

        }

        public static void FilterbyId(DbHandler dbHandler)
        {
            DataRow dataRowPh = dbHandler.RetrieveRowByPrimaryKey("PH", "1112DB   40", "O00");
            DataRow dataRowPl = dbHandler.RetrieveRowByPrimaryKey("PL", "1122BP  301", "ER0");
        }


        public static void AddRow(DbHandler dbConn)
        {
            // Extract data table for pp table
            DataTable dt = dbConn.DbData.Tables["PP"];

            // Count rows
            Console.WriteLine(dt.Rows.Count);

            // Write new row
            Console.WriteLine("Adding row...");
            DataRow newRow = dt.NewRow();
            newRow["PAD"] = "1350";
            newRow["PArt"] = "PS4";
            newRow["VermArt"] = 1;
            newRow["Stabil"] = 1;
            newRow["Pdatum"] = "";
            newRow["PBearb"] = "";
            newRow["PAuftr"] = "";
            newRow["PProg"] = "";
            newRow["PText"] = "";
            newRow["PStrecke"] = "0644";
            newRow["PSTRRiKz"] = 1;
            newRow["Station"] = 2;
            newRow["Import"] = new DateTime();
            newRow["loeschDatum"] = new DateTime();
            dt.Rows.Add(newRow);

            // Read all rows
            Console.WriteLine(dt.Rows.Count);

            // Update database
            dbConn.UpdateDatabases();
        }
    }
}
