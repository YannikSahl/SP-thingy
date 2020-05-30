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
            string fileLocation = "E:\\Benutzer\\Yannik\\NcDrive\\Ablage\\Datenmodell.accdb";

            // Create object
            DBConnection dbConn = new DBConnection(fileLocation);

            // Extract data table for pp table
            System.Data.DataTable dt = dbConn.dbData.Tables["PP"];

            // Count rows
            Console.WriteLine(dt.Rows.Count);

            // Write new row
            Console.WriteLine("Adding row...");
            DataRow newRow = dt.NewRow();
            newRow["PAD"] = "1337 Leet";
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
            dbConn.updateDatabases();

        }
    }
}
