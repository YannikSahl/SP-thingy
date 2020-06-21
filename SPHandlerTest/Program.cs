using System;
using System.Net;
using Microsoft.SharePoint.Client;

namespace SharePointTryOut
{
    internal class Program
    {
        private const string rootSite = "https://htwberlinde.sharepoint.com";
        private const string sourceSite = "/sites/SWE";
        private const string folderSite = "/Freigegebene%20Dokumente";
        private const string fileSite = "/04321_DB_Festp/03_Skizzen/PDF/6000/6400/6441/Arbeitspaket4.pdf";
        private static string sourceLibrary = "Dokumente";
        private static string destinationPath = "C:\\temp\\test.pdf";
        private static string username;
        private static string password;


        private static void Main(string[] args)
        {
            username = "";
            password = "";

            using (var ctx = new ClientContext(rootSite))
            {
                ctx.Credentials = new SharePointOnlineCredentials(username, password);
                var web = ctx.Web;
                ctx.Load(web);
                ctx.ExecuteQueryAsync().Wait();


                DownloadFile(sourceSite + folderSite + fileSite,ctx.Credentials, destinationPath);
            }

            Console.ReadLine();
        }

        private static void DownloadFile(string webUrl, ICredentials credentials, string destinationPath)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
                client.Headers.Add("User-Agent: Other");
                client.Credentials = credentials;
                client.DownloadFile(webUrl, destinationPath);
            }
        }

        public static string GetFileUrlFromDB(int entry, bool type)
        {
            string fileRelativUrl;

            var thousands = entry / 1000 % 10 * 1000;
            var hundreds  = thousands + entry / 100 % 10 * 100;

            if (type)
                fileRelativUrl = "/04321_DB_Festp/03_Skizzen/PDF/" + thousands + "/" + hundreds + "/" + entry;
            else
                fileRelativUrl = "/04321_DB_Festp/03_Skizzen/JPG/" + thousands + "/" + hundreds + "/" + entry;
            
            return fileRelativUrl;
        }
    }
}