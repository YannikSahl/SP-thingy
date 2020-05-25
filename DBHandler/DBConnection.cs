using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.ComponentModel;

public enum StatusCode
{
    CommandOK = 0,
    CommandFailed = 1,
    ConnectionFailed = 2
}

namespace DBHandler
{
    public class DBConnection
    {

        // Member variables
        OleDbConnection dbConn;
        String downloadLocation;
        //SPH.SharePointHandler SPHandler;

        // Constructor
        public DBConnection(String SharePointPath, String apikey, String downloadLocation)
        {

            // Save download location for further use
            this.downloadLocation = downloadLocation;

            // Create SPHandler to connect to sharepoint
            //this.SPHandler = new SPH.SharePointHandler(SharePointPath, apikey, downloadLocation);

        }


        // Open .accdb file
        public StatusCode openDBFile()
        {

            // string connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=E:\\Benutzer\\Yannik\\NcDrive\\HTW\\Module\\4 (5) Softwareentwicklungsprojekt\\Team 5\\Gegeben\\Datenmodell;";
            string connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\Yannik\\Desktop\\Datenmodell.accdb;";

            using (OleDbConnection connection = new OleDbConnection(connString))
            {
                

                // Select
                OleDbCommand commandSelect = new OleDbCommand("SELECT PAD, HStat FROM PH WHERE HStat = '1';", connection);
                commandSelect = new OleDbCommand("SELECT PAD, HStat FROM PH;", connection);
                commandSelect.Parameters.Add("HSys", OleDbType.VarChar, 15);

                // Insert
                OleDbCommand commandInsert = new OleDbCommand("SELECT PAD, HStat FROM PH WHERE HStat = '1';", connection);

                // Delete
                OleDbCommand commandDelete = new OleDbCommand("SELECT PAD, HStat FROM PH WHERE HStat = '1';", connection);

                try
                {

                    connection.Open();

                    // Print schema info
                    DataTable table = connection.GetSchema();

                    // Execute commands
                    OleDbDataReader reader = commandSelect.ExecuteReader(); //Console.WriteLine(commandSelect.ExecuteNonQuery());
                    Console.WriteLine("Field count: " + reader.FieldCount);

                    // Read output
                    while (reader.Read())
                    {
                        Console.WriteLine(reader[0].ToString());
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
                
            OleDbConnection myConn = new OleDbConnection(connString);

            return StatusCode.CommandOK;

        }


    }

}
