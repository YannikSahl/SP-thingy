using System;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using SPHandlerTest;

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
        private static string password = "G1ftnude/";

        public static async Task Main(string[] args)
        {
            Uri site = new Uri(rootSite + sourceSite + folderSite); //"https://contoso.sharepoint.com/sites/siteA
            string user = "s0568476@htw-berlin.de";
            SecureString password = ToSecureString(Program.password);

            // Note: The PnP Sites Core AuthenticationManager class also supports this
            using (var authenticationManager = new SPHandlerTest.AuthentificationManagerCSOM())
            using (var context = authenticationManager.GetContext(site, user, password))
            {
                context.Load(context.Web, p => p.Title);
                await context.ExecuteQueryAsync();
                Console.WriteLine($"Title: {context.Web.Title}");
            }
        }
        //private static void Main(string[] args)
        //{
        //    username = "s0568476@htw-berlin.de";
        //    password = "G1ftnude/";
        //    SecureString knox = ToSecureString(password);
        //    Uri site = new Uri(rootSite+sourceSite);


        //    Console.WriteLine(rootSite + sourceSite + folderSite + fileSite);

        //    _client.Sites[siteId].Drives[docLibId].Items[driveItemId].Content.Request().GetAsync()‘
        //    client.DownloadFile(rootSite + sourceSite + folderSite + fileSite, destinationPath);


        //    Console.ReadLine();
        //    GetIntfromPAD("6020CZ 2303");
        //}



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
        public static SecureString ToSecureString(string _self)
        {
            SecureString knox = new SecureString();
            char[] chars = _self.ToCharArray();
            foreach (char c in chars)
            {
                knox.AppendChar(c);
            }
            return knox;
        }

        public static int GetIntfromPAD(string PAD)
        {
            double forPath = 0;

            char[] ar = PAD.ToCharArray();

            forPath = char.GetNumericValue(ar[0])*1000 + char.GetNumericValue(ar[1]) * 100 + char.GetNumericValue(ar[2]) * 10 + char.GetNumericValue(ar[3]);

            Console.WriteLine(forPath);

            return (int)forPath;
        }
       
    }
}