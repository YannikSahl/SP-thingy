using System;
using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client.WorkflowServices;
using System.Globalization;

namespace SPHandler
{
    public class Handler
    {
        const string rootSite = "https://htwberlinde.sharepoint.com";
        const string sourceSite = "https://htwberlinde.sharepoint.com/sites/SWE";
        static string sourceLibrary = "Dokumente";
        static string destinationPath = "Pfad aus Settings";
        private static string username;
        private static string password;

        public static void SetUsername(string name)
        {
            username = name;
        }

        public static void SetPassword(string pw)
        {
            password = pw;
        }

        public static string TestConnection(out bool success)
        {
            try
            {
                SharePointOnlineCredentials Credentials = new SharePointOnlineCredentials(username, password);

                ClientContext context = new ClientContext(sourceSite); //create context
                context.Credentials = Credentials;

                List list = context.Web.Lists.GetByTitle(sourceLibrary); //retrieve list
                context.Load(list);
                context.ExecuteQueryAsync().Wait();

                CamlQuery query = new CamlQuery(); //retrieve all items
                ListItemCollection ListItems = list.GetItems(query);
                context.Load(ListItems);
                context.ExecuteQueryAsync().Wait();
            }catch(Exception e)
            {
                success = false;
                return e.Message;
            }
            success = true;
            return null;
        }
        public static string GetFileUrlFromDb(int entry, bool type)
        {
            string fileRelativUrl;

            var thousands = entry / 1000 % 10 * 1000;
            var hundreds = thousands + entry / 100 % 10 * 100;

            if (type)
                fileRelativUrl = "/04321_DB_Festp/03_Skizzen/PDF/" + thousands + "/" + hundreds + "/" + entry;
            else
                fileRelativUrl = "/04321_DB_Festp/03_Skizzen/JPG/" + thousands + "/" + hundreds + "/" + entry;

            return fileRelativUrl;
        }

        public static int GetIntfromPAD(string PAD)
        {
            double forPath = 0;

            char[] ar = PAD.ToCharArray();

            forPath = char.GetNumericValue(ar[0]) * 1000 + char.GetNumericValue(ar[1]) * 100 + char.GetNumericValue(ar[2]) * 10 + char.GetNumericValue(ar[3]);

            Console.WriteLine(forPath);

            return (int)forPath;
        }











    }
}
