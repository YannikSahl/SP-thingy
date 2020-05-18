using System;
using DBHandler;

namespace DBHandlerTest
{
    class Program
    {
        static void Main(string[] args)
        {

            DBConnection dbConn = new DBConnection("", "", "");
            dbConn.openDBFile();
            
        }
    }
}
