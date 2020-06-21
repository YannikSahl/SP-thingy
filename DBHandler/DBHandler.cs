using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Data.Common;
using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;
using Exceptions;

public enum StatusCode
{
    CommandOk = 0,
    CommandFailed = 1,
    OleDbNotRegistered = 2
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
                try
                {
                    connection.Open();
                }
                catch (InvalidOperationException ioe)
                {
                    throw new OleDbProviderMissingException("Access Runtime is missing from your computer. Please download it from here: https://www.microsoft.com/en-us/download/confirmation.aspx?id=13255");
                }
                
            }

            adapter.UpdateCommand = commandBuilder.GetUpdateCommand();
            adapter.DeleteCommand = commandBuilder.GetDeleteCommand();
            adapter.InsertCommand = commandBuilder.GetInsertCommand();

            // Fill DataSet
            adapter.Fill(DbData, tableName);

            // Retrieve DataTable
            DataTable dataTable = DbData.Tables[tableName];

            // Add PrimaryKey information
            DataColumn[] keyColumns = new DataColumn[3];
            keyColumns[0] = dataTable.Columns["PAD"];

            switch (tableName)
            {
                case "PH":
                    keyColumns[1] = dataTable.Columns["HSys"];
                    break;

                case "PL":
                    keyColumns[1] = dataTable.Columns["LSys"];
                    break;

                case "PP":
                    keyColumns[1] = dataTable.Columns["PStrecke"];
                    keyColumns[2] = dataTable.Columns["PSTRRiKz"];
                    break;
            }
            dataTable.PrimaryKey = keyColumns;

            // Close connection
            connection.Close();

            // Return adapter
            return adapter;

        }

        // 
        /// <summary>
        /// Retrieves a DataRow from PH or PL Table by Primary Key values
        /// </summary>
        /// <param name="tableName">Table that is to be queried; must be PL or PH</param>
        /// <param name="pad">Corresponding PAD value</param>
        /// <param name="hSysOrLSys">Corresponding HSys (PH) or LSys (PH) value</param>
        /// <returns>DataRow that matches parameter values</returns>
        public DataRow RetrieveRowByPrimaryKey(string tableName, string pad, string hSysOrLSys)
        {

            // Get table
            DataTable dataTable = DbData.Tables[tableName];
            DataRowCollection dataRows = dataTable.Rows;

            // Construct key
            object[] keys = new object[2]{ pad, hSysOrLSys };

            // Find DataRow by pad
            DataRow dataRow = dataRows.Find(keys);
            return dataRow;

        }

        /// <summary>
        /// Retrieves DataRows from PH or PL Table that match PAD value.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="pad">Corresponding PAD value</param>
        /// <returns>DataRow that contain PAD value</returns>
        public DataRow[] RetrieveRowByPad(string tableName, string pad)
        {

            // Get table
            DataTable dataTable = DbData.Tables[tableName];

            // Select by pad
            string filterExpression = $"PAD = '{pad}'";
            DataRow[] selectedRows = dataTable.Select(filterExpression);
            return selectedRows;

        }



        // ************ DISPOSE

        // To detect redundant calls
        private bool _disposed = false;

        // Free any unmanaged resources (not managed by GC)
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
                // Dispose managed resources (managed by GC)
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
