using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Data.Common;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using Exceptions;

public enum StatusCode
{
    CommandOk = 0,
    CommandFailed = 1,
    OleDbNotRegistered = 2,
    NoDatabaseChanges = 3,
    SqlError = 4
}

namespace DBHandler
{
    public class DbHandler : IDisposable
    {

        // Member variables
        public OleDbConnection DbConn { get; }
        public OleDbDataAdapter DbAdapterPh { get; set; }
        public OleDbDataAdapter DbAdapterPl { get; set; }
        public OleDbDataAdapter DbAdapterPp { get; set; }
        public DataSet DbData { get; set; }

        // Constructor
        public DbHandler(string fileLocation)
        {

            // Initialize DataSet
            DbData = new DataSet();

            // Open file
            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation;
            DbConn = new OleDbConnection(connectionString);

        }

        // Destructor
        ~DbHandler()
        {
            Dispose(false);
        }

        /// <summary>
        /// Fills in data from .accdb file by building query from passed parameter values.
        /// </summary>
        /// <param name="Pad">Values for 'PAD' to be filtered by.</param>
        /// <param name="PStrecke">Values for 'PStrecke' to be filtered by.</param>
        /// <param name="PArt">Values for 'PArt' to be filtered by.</param>
        /// <param name="PAuftr">Values for 'PAuftr' to be filtered by.</param>
        /// <param name="isAndConnector">Whether queries should be AND or OR connected.</param>
        /// <returns></returns>
        public StatusCode FillInData(HashSet<string> Pad, HashSet<string> PStrecke, HashSet<string> PArt, HashSet<string> PAuftr, bool isAndConnector)
        {

            // Build query string
            string queryStringFilterPP = BuildQueryString(Pad, PStrecke, PArt, PAuftr, isAndConnector);

            // Try to access data
            try
            {
                // Create OleDbAdapters
                DbAdapterPh = CreateDataAdapter("PH", DbConn, "SELECT * FROM PH");
                DbAdapterPl = CreateDataAdapter("PL", DbConn, "SELECT * FROM PL");
                DbAdapterPp = CreateDataAdapter("PP", DbConn, queryStringFilterPP);

            }
            // Catch exception on runtime missing error, return appropriate error code
            catch (InvalidOperationException ioe)
            {
                return StatusCode.OleDbNotRegistered;
            }

            return StatusCode.CommandOk;

        }

        /// <summary>
        /// Fills in data from .accdb file with custom query.
        /// </summary>
        /// <param name="queryString">Custom query string that user may input in GUI settings.</param>
        /// <returns>StatusCode: 'OleDbNotRegistered' when OleDbProvider was not found on system, 'SqlError' when SQL errors were found in custom query, else 'CommandOk'.</returns>
        public StatusCode FillInData(string queryString)
        {

            // Try to access data
            try
            {
                // Create OleDbAdapters
                DbAdapterPh = CreateDataAdapter("PH", DbConn, "SELECT * FROM PH");
                DbAdapterPl = CreateDataAdapter("PL", DbConn, "SELECT * FROM PL");
                DbAdapterPp = CreateDataAdapter("PP", DbConn, queryString);

            }
            // Catch exception on runtime missing error, return appropriate error code
            catch (InvalidOperationException ioe)
            {
                return StatusCode.OleDbNotRegistered;
            }
            catch (Exception ex)
            {
                return StatusCode.SqlError;
            }

            return StatusCode.CommandOk;

        }

        /// <summary>
        /// Builds query string for PP table based on GUI input.
        /// </summary>
        /// <param name="PAD">Values for 'PAD' to be filtered by.</param>
        /// <param name="PStrecke">Values for 'PStrecke' to be filtered by.</param>
        /// <param name="PArt">Values for 'PArt' to be filtered by.</param>
        /// <param name="PAuftr">Values for 'PAuftr' to be filtered by.</param>
        /// <param name="isAndConnector">Whether queries should be AND or OR connected.</param>
        /// <returns>Built query string.</returns>
        public string BuildQueryString(HashSet<string> PAD, HashSet<string> PStrecke, HashSet<string> PArt, HashSet<string> PAuftr, bool isAndConnector)
        {

            var queryString = "SELECT * FROM PP";
            var queryConnector = isAndConnector ? "AND" : "OR";

            // Append where
            if (PAD.Count != 0 || PStrecke.Count != 0 || PArt.Count != 0 || PAuftr.Count != 0)
            {
                queryString += " WHERE";
            }

            // Build filter
            BuildFilter(PAD, "PAD", ref queryString, queryConnector);
            BuildFilter(PStrecke, "PStrecke",  ref queryString, queryConnector);
            BuildFilter(PArt, "PArt", ref queryString, queryConnector);
            BuildFilter(PAuftr, "PAuftr", ref queryString, queryConnector);

            // Return built string
            Console.WriteLine($"Query string: {queryString}");
            return queryString;
        }

        /// <summary>
        /// Builds filter queries used for building the dataset according to GUI settings.
        /// </summary>
        /// <param name="filterSet">Multiple values for a variable.</param>
        /// <param name="filterVariable">Value to be filtered by.</param>
        /// <param name="queryString">Query string already built.</param>
        /// <param name="queryConnector">AND or OR connector.</param>
        public void BuildFilter(HashSet<string> filterSet, string filterVariable, ref string queryString, string queryConnector)
        {

            // Check if there were filters built before this one
            if (filterSet.Count != 0)
            {
                var querySubString = queryString.Substring(queryString.Length - 2);
                if (querySubString == ") ")
                {
                    queryString += queryConnector;
                }
                queryString += " (";
            }

            // Prepare connector
            var innerQueryConnector = "";

            // Build filter
            foreach (string filter in filterSet)
            {
                queryString += $"{innerQueryConnector} {filterVariable} = '{filter}' ";
                innerQueryConnector = "OR";
            }

            if (filterSet.Count != 0) queryString += ") ";
        }

        /// <summary>
        /// Updates database file according to DataSet changes.
        /// </summary>
        /// <returns>StatusCode: 'CommandOk' when rows were changed, 'NoDatabaseChanges' when no rows were affected.</returns>
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
            return (rowsChangedPh > 0 || rowsChangedPl > 0 || rowsChangedPp > 0) ? StatusCode.CommandOk : StatusCode.NoDatabaseChanges;

        }

        // Source: https://docs.microsoft.com/en-us/dotnet/api/system.data.oledb.oledbdataadapter?view=dotnet-plat-ext-3.1
        /// <summary>
        /// Initializes DataAdapter used to generate SQL queries.
        /// </summary>
        /// <param name="tableName">Name of corresponding table [PH, PL or PP].</param>
        /// <param name="connection">Connection to be used.</param>
        /// <param name="selectCommand">Command to be used for Select to initially query data.</param>
        /// <returns>OleDbAdapter</returns>
        public OleDbDataAdapter CreateDataAdapter(string tableName, OleDbConnection connection, string selectCommand)
        {

            // Create adapter
            OleDbDataAdapter adapter = new OleDbDataAdapter(selectCommand, connection);

            // Create command builder (automatically generates single-table sql commands)
            OleDbCommandBuilder commandBuilder = new OleDbCommandBuilder(adapter)
            {
                QuotePrefix = "[",
                QuoteSuffix = "]"
            };

            // Open connection
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            // Acquire built commands
            adapter.UpdateCommand = commandBuilder.GetUpdateCommand();
            adapter.DeleteCommand = commandBuilder.GetDeleteCommand();
            adapter.InsertCommand = commandBuilder.GetInsertCommand();

            // Fill DataSet
            adapter.Fill(DbData, tableName);

            // Retrieve DataTable
            DataTable dataTable = DbData.Tables[tableName];
            int dataTableRows = dataTable.Rows.Count;
            Console.WriteLine($"{tableName} Table row count: {dataTableRows}");

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
            object[] keys = new object[2] { pad, hSysOrLSys };

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
