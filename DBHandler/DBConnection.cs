using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Data.Common;
using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;

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
        public OleDbConnection dbConn;
        public OleDbDataAdapter dbAdapterPH;
        public OleDbDataAdapter dbAdapterPL;
        public OleDbDataAdapter dbAdapterPP;
        public DataSet dbData;

        // Constructor
        public DBConnection(String fileLocation)
        {

            // Initialize DataSet
            dbData = new DataSet();

            // Create SPHandler to connect to sharepoint
            //this.SPHandler = new SPH.SharePointHandler(SharePointPath, apikey, downloadLocation);

            // Open file
            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation;
            dbConn = new OleDbConnection(connectionString);

            // Create OleDbAdapters
            dbAdapterPH = createDataAdapter("PH", dbConn);
            dbAdapterPL = createDataAdapter("PL", dbConn);
            dbAdapterPP = createDataAdapter("PP", dbConn);
 
        }

        // Destructor
        ~DBConnection()
        {
            // Close connection
            if (dbConn.State != ConnectionState.Open) dbConn.Close();

            // Dipose of 
            dbConn.Dispose();
        }

        // Update databases (e.g. to be used on "save changed" click)
        public StatusCode updateDatabases()
        {

            // Update databases
            int rowsChanged_PH = dbAdapterPH.Update(dbData.Tables["PH"]);
            int rowsChanged_PL = dbAdapterPL.Update(dbData.Tables["PL"]);
            int rowsChanged_PP = dbAdapterPP.Update(dbData.Tables["PP"]);

            // Log info
            Console.WriteLine("Database Update: " + rowsChanged_PH + " rows affected in Table PH");
            Console.WriteLine("Database Update: " + rowsChanged_PL + " rows affected in Table PL");
            Console.WriteLine("Database Update: " + rowsChanged_PP + " rows affected in Table PP");

            // Return status code
            return (rowsChanged_PH > 0 || rowsChanged_PL > 0 || rowsChanged_PP > 0) ? StatusCode.CommandOK : StatusCode.CommandFailed;

        }

        // Source: https://docs.microsoft.com/en-us/dotnet/api/system.data.oledb.oledbdataadapter?view=dotnet-plat-ext-3.1
        // Initializes DataAdapter
        public OleDbDataAdapter createDataAdapter(string tableName, OleDbConnection connection)
        {

            // Create OleDbAdapter
            string selectCommand = "SELECT * FROM " + tableName;
            OleDbDataAdapter adapter = new OleDbDataAdapter(selectCommand, connection);

            // Create command builder (automatically generates single-table sql commands)
            OleDbCommandBuilder commandBuilder = new OleDbCommandBuilder(adapter);
            commandBuilder.QuotePrefix = "[";
            commandBuilder.QuoteSuffix = "]";

            // Acquire built commands
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();

                adapter.UpdateCommand = commandBuilder.GetUpdateCommand();
                adapter.DeleteCommand = commandBuilder.GetDeleteCommand();
                adapter.InsertCommand = commandBuilder.GetInsertCommand();

                // Include primary key information
                adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

                // Fill DataSet
                adapter.Fill(dbData, tableName);

                connection.Close();
            }

            // Return adapter
            return adapter;

        }

    }

}
