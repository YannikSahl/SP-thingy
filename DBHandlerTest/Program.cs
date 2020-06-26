using System;
using System.Collections.Generic;
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
            //FilterTable(dbHandler);

            // Fill in data
            FillInData(dbHandler);

        }

        public static void FillInData(DbHandler dbHandler)
        {
            HashSet<string> padFilter = new HashSet<string>() {};
            HashSet<string> pstreckeFilter = new HashSet<string>() {};
            HashSet<string> pArtFilter = new HashSet<string>() {};
            HashSet<string> pAuftrFilter = new HashSet<string>() {};

            dbHandler.FillInData(padFilter, pstreckeFilter, pArtFilter, pAuftrFilter, false);
        }

        public static void FilterTable(DbHandler dbHandler)
        {

            // Filter by primary key (PAD + LSys / HSys)
            DataRow dataRowPh = dbHandler.RetrieveRowByPrimaryKey("PH", "1112DB   40", "O00");
            DataRow dataRowPl = dbHandler.RetrieveRowByPrimaryKey("PL", "1122BP  301", "ER0");

            // Filter by PAD
            DataRow[] dataRowsPh = dbHandler.RetrieveRowByPad("PH", "1112DB   40");
            DataRow[] dataRowsPl = dbHandler.RetrieveRowByPad("PL", "1122BP  301");

            // Debug
            Console.WriteLine($"Filter by PAD: Found {dataRowsPh.Length} matches in PH table and {dataRowsPl.Length} matches in PL table.");

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
