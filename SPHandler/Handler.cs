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
    /// <summary>
    /// The SPHandler namespace contains functions for the current GUI version.
    /// Only working with TTCUE.NetCore.SharePointCSOM
    /// </summary>
    public class Handler
    {
        private const string rootSite = "https://htwberlinde.sharepoint.com";
        private const string sourceSite = "https://htwberlinde.sharepoint.com/sites/SWE";
        private static readonly string sourceLibrary = "Dokumente";
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

        public static string TryUserLogin()
        {
            try
            {
                var Credentials = new SharePointOnlineCredentials(username, password);

                var context = new ClientContext(sourceSite); //create context
                context.Credentials = Credentials;

                var list = context.Web.Lists.GetByTitle(sourceLibrary); //retrieve list
                context.Load(list);
                context.ExecuteQueryAsync().Wait();

                var query = new CamlQuery(); //retrieve all items
                var ListItems = list.GetItems(query);
                context.Load(ListItems);
                context.ExecuteQueryAsync().Wait();
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return null;
        }

        /// <summary>
        /// testing connection to the SharePoint 
        /// </summary>
        /// <param name="success">used for displaying the result</param>
        public static Task<string> TestConnection() //async 
        {
            try
            {
                var Credentials = new SharePointOnlineCredentials(username, password);

                var context = new ClientContext(sourceSite); //create context
                context.Credentials = Credentials;

                var list = context.Web.Lists.GetByTitle(sourceLibrary); //retrieve list
                context.Load(list);
                context.ExecuteQueryAsync().Wait();

                var query = new CamlQuery(); //retrieve all items
                var ListItems = list.GetItems(query);
                context.Load(ListItems);
                context.ExecuteQueryAsync().Wait();
            }
            catch (Exception e)
            {
                return Task.FromResult(e.Message);
            }
            return Task.FromResult("");
        }

        /// <summary>
        /// Async Method to test the connection, while the programm is already displaying the database
        /// </summary>
        public async Task<String> TestConnectionAsync()
        {
            string ret = await TestConnection();

            return ret;
        }

        /// <summary>Returns FileRelativUrl for Files on SharePoint
        /// <param name="pad">PAD</param>
        /// <param name="type">true=PDF, false=JPG</param>
        /// </summary>
        public static string GetFileUrlFromPad(string pad, bool type)
        {
            var entry = GetIntfromPad(pad);
            string fileRelativUrl;

            var thousands = entry / 1000 % 10 * 1000;
            var hundreds = thousands + entry / 100 % 10 * 100;

            if (type)
                fileRelativUrl = "/04321_DB_Festp/03_Skizzen/PDF/" + thousands + "/" + hundreds + "/" + entry + "/" + pad + ".pdf";
            else
                fileRelativUrl = "/04321_DB_Festp/03_Skizzen/JPG/" + thousands + "/" + hundreds + "/" + entry + "/" + pad + ".jpg";

            return fileRelativUrl;
        }

        /// <summary>Returns Int to handle PAD
        /// <param name="pad">PAD</param>
        /// </summary>
        public static int GetIntfromPad(string pad)
        {
            double forPath = 0;

            var ar = pad.ToCharArray();

            forPath = char.GetNumericValue(ar[0]) * 1000 + char.GetNumericValue(ar[1]) * 100 +
                      char.GetNumericValue(ar[2]) * 10 + char.GetNumericValue(ar[3]);

            return (int) forPath;
        }
    }
}