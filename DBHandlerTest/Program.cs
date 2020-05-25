using System;
using System.Data.Common;
using DBHandler;

namespace DBHandlerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            String fileLocation = "C:\\Users\\Yannik\\Desktop\\Datenmodell.accdb";

            // Create objects
            DBReader dbRead = new DBReader(fileLocation);
            DBWriter dbWrite = new DBWriter(fileLocation);

            // Read info on tables

            // Read all rows

            // Write new row

            // Read all rows


            // Test
            

        }
    }
}
