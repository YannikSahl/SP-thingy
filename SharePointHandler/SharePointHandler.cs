using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace SharePointHandler
{
    public class SharePointHandler
    {    

        static string siteUrl = "https://htwberlinde.sharepoint.com/sites/SWE";
        static string username = Authentification.GetUserName();
        static SecureString password = Authentification.GetPassword();

        private static ClientContext CreateContext()
        {
            ClientContext ctx = new ClientContext(siteUrl);
            ctx.Credentials = new SharePointOnlineCredentials(username, password);
            ctx.ExecuteQuery();           

            return ctx;
        }

    }
}
