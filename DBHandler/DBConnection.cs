using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
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

            // Test
            OleDbCommand command = new OleDbCommand("SELECT * FROM PH");
            // string connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=E:\\Benutzer\\Yannik\\NcDrive\\HTW\\Module\\4 (5) Softwareentwicklungsprojekt\\Team 5\\Gegeben\\Datenmodell;";
            string connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\Yannik\\Desktop\\Datenmodell.accdb;";

            using (OleDbConnection connection = new OleDbConnection(connString))
            {

                command.Connection = connection;

                try
                {
                    connection.Open();
                    Console.WriteLine(command.ExecuteNonQuery());
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
