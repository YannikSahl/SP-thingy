using System;
using System.Security;
using System.Threading.Tasks;

namespace SPHandlerTest
{
    /// <summary>
    /// The SPHandlerTest is used for testing with SharePointOnline.CSOM, starting from version 16.1.
    /// Functions are currently not in use.
    /// </summary>
    internal class Program
    {
        private const string RootSite = "https://htwberlinde.sharepoint.com";
        private const string SourceSite = "/sites/SWE";
        private const string FolderSite = "/Freigegebene%20Dokumente";
        private static string username = "";
        private static string password = "";

        
        
        public static async Task Main(string[] args)
        {
            Uri site = new Uri(RootSite + SourceSite + FolderSite); 
            SecureString secpassword = ToSecureString(password);

            // Note: The PnP Sites Core AuthenticationManager class also supports this
            using (var authenticationManager = new SPHandlerTest.AuthentificationManagerCSOM())
            using (var context = authenticationManager.GetContext(site, username, secpassword))
            {
                context.Load(context.Web, p => p.Title);
                await context.ExecuteQueryAsync();
                Console.WriteLine($"Title: {context.Web.Title}");
            }
        }
        
        /// <summary>
        /// returns SecureString 
        /// <param name="_self">string</param>
        /// </summary>
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

       
       
    }
}