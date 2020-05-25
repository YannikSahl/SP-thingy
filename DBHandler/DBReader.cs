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
        public void readSchema()
        {

        }


        public void readMeta()
        {
            /*
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
            */
            

        }


        public void readTable(String tableName)
        {

            try
            {

                // Open file
                dbConn.Open();

                //dbCommand.CommandText


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //return StatusCode.ConnectionFailed;
            }

        }

        public void readRowFromTable(String tableName, String[] conditions)
        {



        }


    }
}
