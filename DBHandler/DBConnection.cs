using System;
using SPH = SharePointHandler;
using System.Data.OleDb;


enum Statuscode
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
        SPH.SharePointHandler SPHandler;

        // Constructor
        public DBConnection(String SharePointPath, String apikey, String downloadLocation)
        {

            // Save download location for further use
            this.downloadLocation = downloadLocation;

            // Create SPHandler to connect to sharepoint
            this.SPHandler = new SPH.SharePointHandler(SharePointPath, apikey, downloadLocation);

        }


        // Verbindung zu SharePoint
        string connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\source\\SiteCore65\\Individual-Data.xls;Extended Properties=Excel 8.0;";


        // Öffnen .accdb file


    }
}
