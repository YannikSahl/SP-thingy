using System;
using System.Collections.Generic;
using System.Text;

namespace DBHandler
{
    public class DBReader : DBConnection
    {


        // Constructor
        public DBReader(String fileLocation) : base(fileLocation)
        {


        }


        // Methods
        public StatusCode readSchema()
        {

        }


        public StatusCode readMeta()
        {
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }


        public StatusCode readTable(String tableName)
        {

            try
            {

                // Open file
                dbConn.Open();

                dbCommand.CommandText


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode.ConnectionFailed;
            }

        }

        public StatusCode readRowFromTable(String tableName, String[] conditions)
        {



        }


    }
}
