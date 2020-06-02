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
    CommandOk = 0,
    CommandFailed = 1,
    ConnectionFailed = 2
}

namespace DBHandler
{
    public class DbHandler : IDisposable
    {

        // Member variables
        public OleDbConnection DbConn { get; }
        public OleDbDataAdapter DbAdapterPh { get; }
        public OleDbDataAdapter DbAdapterPl { get; }
        public OleDbDataAdapter DbAdapterPp { get; }
        public DataSet DbData { get; set; }

        // Constructor
        public DbHandler(string fileLocation)
        {

            // Initialize DataSet
            DbData = new DataSet();

            // Open file
            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation;
            DbConn = new OleDbConnection(connectionString);

            // Create OleDbAdapters
            DbAdapterPh = CreateDataAdapter("PH", DbConn);
            DbAdapterPl = CreateDataAdapter("PL", DbConn);
            DbAdapterPp = CreateDataAdapter("PP", DbConn);
 
        }

        // Destructor
        ~DbHandler()
        {
            Dispose(false);
        }

        // Update databases (e.g. to be used on "save changed" click)
        public StatusCode UpdateDatabases()
        {

            // Update databases
            int rowsChangedPh = DbAdapterPh.Update(DbData.Tables["PH"]);
            int rowsChangedPl = DbAdapterPl.Update(DbData.Tables["PL"]);
            int rowsChangedPp = DbAdapterPp.Update(DbData.Tables["PP"]);

            // Log info
            Console.WriteLine("Database Update: " + rowsChangedPh + " rows affected in Table PH");
            Console.WriteLine("Database Update: " + rowsChangedPl + " rows affected in Table PL");
            Console.WriteLine("Database Update: " + rowsChangedPp + " rows affected in Table PP");

            // Return status code
            return (rowsChangedPh > 0 || rowsChangedPl > 0 || rowsChangedPp > 0) ? StatusCode.CommandOk : StatusCode.CommandFailed;

        }

        // Source: https://docs.microsoft.com/en-us/dotnet/api/system.data.oledb.oledbdataadapter?view=dotnet-plat-ext-3.1
        // Initializes DataAdapter
        public OleDbDataAdapter CreateDataAdapter(string tableName, OleDbConnection connection)
        {

            // Create OleDbAdapter
            string selectCommand = "SELECT * FROM " + tableName;
            OleDbDataAdapter adapter = new OleDbDataAdapter(selectCommand, connection);

            // Create command builder (automatically generates single-table sql commands)
            OleDbCommandBuilder commandBuilder = new OleDbCommandBuilder(adapter)
            {
                QuotePrefix = "[", QuoteSuffix = "]"
            };

            // Acquire built commands
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            adapter.UpdateCommand = commandBuilder.GetUpdateCommand();
            adapter.DeleteCommand = commandBuilder.GetDeleteCommand();
            adapter.InsertCommand = commandBuilder.GetInsertCommand();

            // Include primary key information
            adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

            // Fill DataSet
            adapter.Fill(DbData, tableName);

            // Close connection
            connection.Close();

            // Return adapter
            return adapter;

        }

        // ************ DISPOSE

        // To detect redundant calls
        private bool _disposed = false;

        // Free any unmanaged resources
        private void ReleaseUnmanagedResources()
        {
            // None yet
        }

        // Private dispose function that runs checks based on bool
        private void Dispose(bool disposing)
        {

            // Check if already disposed
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                // Dispose managed resources
                DbConn?.Dispose();
                DbAdapterPh?.Dispose();
                DbAdapterPl?.Dispose();
                DbAdapterPp?.Dispose();
                DbData?.Dispose();
            }

            // Dispose unmanaged resources
            ReleaseUnmanagedResources();

            // Set to disposed
            _disposed = true;
        }

        // Public dispose function
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

}
