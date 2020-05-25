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
        public OleDbDataAdapter dbAdapter;
        public DataSet dbDataPH;
        public DataSet dbDataPL;
        public DataSet dbDataPP;

        // Constructor
        public DBConnection(String fileLocation)
        {

            // Create SPHandler to connect to sharepoint
            //this.SPHandler = new SPH.SharePointHandler(SharePointPath, apikey, downloadLocation);

            // Open file
            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation;
            dbConn = new OleDbConnection(connectionString);

            // Adapter
            dbAdapter = new OleDbDataAdapter();
            dbAdapter.SelectCommand = new OleDbCommand("", dbConn);

            // Create OleDbCommandBuilder object, which generates SQL statements for single-table updates
            OleDbCommandBuilder dbCommandBuilder = new OleDbCommandBuilder(dbAdapter);

        }

        // Destructor
        ~DBConnection()
        {
            dbConn.Close();
            dbConn.Dispose();
        }

        // Update databases (e.g. to be used on "save changed" click)
        public StatusCode updateDatabases()
        {

            // Update databases
            int rowsChanged_PH = dbAdapter.Update(dbDataPH, "PH");
            int rowsChanged_PL = dbAdapter.Update(dbDataPL, "PL");
            int rowsChanged_PP = dbAdapter.Update(dbDataPP, "PP");

            // Log info
            Console.WriteLine("Database Update: " + rowsChanged_PH + " rows affected in Table PH");
            Console.WriteLine("Database Update: " + rowsChanged_PL + " rows affected in Table PL");
            Console.WriteLine("Database Update: " + rowsChanged_PP + " rows affected in Table PP");

            // Return status code
            return (rowsChanged_PH > 0 || rowsChanged_PL > 0 || rowsChanged_PP > 0) ? StatusCode.CommandOK : StatusCode.CommandFailed;

        }

        // Source: https://docs.microsoft.com/en-us/dotnet/api/system.data.oledb.oledbdataadapter?view=dotnet-plat-ext-3.1
        public OleDbDataAdapter CreateDataAdapter(string tableName, OleDbConnection connection)
        {

            // Create OleDbAdapter
            string selectCommand = "SELECT * FROM " + tableName;
            OleDbDataAdapter adapter = new OleDbDataAdapter(selectCommand, connection);

            // Include primary key information
            adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

            // Create Insert commands
            adapter.InsertCommand = new OleDbCommand("INSERT INTO Customers (CustomerID, CompanyName) " + "VALUES (?, ?)");

            // Create Update commands
            adapter.UpdateCommand = new OleDbCommand("UPDATE Customers SET CustomerID = ?, CompanyName = ? " + "WHERE CustomerID = ?");

            // Create Delete commands
            adapter.DeleteCommand = new OleDbCommand("DELETE FROM Customers WHERE CustomerID = ?");


            // Create Insert parameter
            adapter.InsertCommand.Parameters.Add("@CustomerID", OleDbType.Char, 5, "CustomerID");
            adapter.InsertCommand.Parameters.Add("@CompanyName", OleDbType.VarChar, 40, "CompanyName");

            // Create Update parameter
            adapter.UpdateCommand.Parameters.Add("@CustomerID", OleDbType.Char, 5, "CustomerID");
            adapter.UpdateCommand.Parameters.Add("@CompanyName", OleDbType.VarChar, 40, "CompanyName");
            adapter.UpdateCommand.Parameters.Add("@oldCustomerID", OleDbType.Char, 5, "CustomerID").SourceVersion = DataRowVersion.Original;

            // Create Delete Parameter
            adapter.DeleteCommand.Parameters.Add("@CustomerID", OleDbType.Char, 5, "CustomerID").SourceVersion = DataRowVersion.Original;

            // Return adapter
            return adapter;

        }

    }

}
