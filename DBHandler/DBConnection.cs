using System;
using System.Collections.Generic;
using System.Text;


enum StatusCode
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
        StatusCode openDBFile()
        {

            string connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=E:\\Benutzer\\Yannik\\NcDrive\\HTW\\Module\\4 (5) Softwareentwicklungsprojekt\\Team 5\\Gegeben\\Datenmodell;";


            return StatusCode.CommandOK;

        }


    }

}
