using System;
using Microsoft.SharePoint.Client;
using System.Security;

namespace SharePointTryOut
{
    class Program
    {
        const string rootSite = "https://htwberlinde.sharepoint.com/";
        const string sourceSite = "https://htwberlinde.sharepoint.com/sites/SWE";
        static string sourceLibrary = "Documents";
        static string destinationPath = "C:\\downloads";
        static string username;
        static string password;
        

        static void Main(string[] args)
        {
            username = Authentification.GetUserName();
            password = Authentification.GetPassword().ToString(); //Authentification.StringToSecureString("");

            //using (var cntxt = new ClientContext(sourceSite))
            //{
            //    cntxt.Credentials = new SharePointOnlineCredentials(username, password);
            //    Web web = cntxt.Web;
            //    cntxt.Load(web.Lists,
            //        lists => lists.Include(list => list.Title,
            //            list => list.Id));
            //    cntxt.ExecuteQuery();
            //    foreach (List ls in web.Lists)
            //    {
            //        Console.WriteLine("List title is: " + ls.Title);
            //    }
            //}

            
            SharePointOnlineCredentials Credentials = new SharePointOnlineCredentials(username, password);
            ClientContext context = new ClientContext(sourceSite); //create context
            context.Credentials = Credentials;

            List list = context.Web.Lists.GetByTitle(sourceLibrary); //retrieve list
            context.Load(list);
            try
            {
                context.ExecuteQueryAsync().Wait();

            }
            catch 
            {
                throw new Exception("Anmeldedaten falsch");
            }

            CamlQuery query = new CamlQuery(); //retrieve all items
            ListItemCollection ListItems = list.GetItems(query);
            context.Load(ListItems);
            context.ExecuteQueryAsync().Wait();
             


            foreach (ListItem item in ListItems)
            {             
            //File Variables
            string fileName = item["FileLeafRef"].ToString();
            string fileUrl = item["FileRef"].ToString();
            string fileMeta = item["Meta"].ToString();
            string modified = item["Modified"].ToString();
            string created = item["Created"].ToString();
            string sourceItemPath = rootSite + fileUrl;
            string destinationFolderPath = destinationPath + fileMeta;
            string destinationItemPath = destinationFolderPath + fileName;

                try 
                {
                    System.Net.WebClient client = new System.Net.WebClient();
                    client.Credentials = new SharePointOnlineCredentials(username, password);
                    client.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
                    client.DownloadFile(sourceItemPath, destinationItemPath);
                    client.Dispose();
                    //File file = Directory.GetFiles(destinationItemPath);
                    //file.LastWriteTime = $modified;
                    //file.CreationTime = $created

                    Console.WriteLine("New document: " + destinationItemPath);
                    
                }

            
                catch
                {
                    Console.WriteLine("Error occurred: " + destinationItemPath);
                }


                //OVERWRITE ITEMS CHECK
                //if (item.LastWriteTime.ToString("d-M-yyyy hh:mm:ss") - ne $modified.addhours(1).ToString("d-M-yyyy hh:mm:ss"))
                //{
                //    System.Net.WebClient client = new System.Net.WebClient;
                //    client.Credentials = Credentials;
                //    client.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
                //    client.DownloadFile(sourceItemPath, destinationItemPath);
                //    client.Dispose();
                //    file = Get - Item $destinationItemPath;
                //    file.LastWriteTime = $modified;

                //    Console.WriteLine("Overwritten document" + destinationItemPath);                
                //}
                //else
                //{
                //    Console.WriteLine("Skipped document" + destinationItemPath);
                //}
            }







        }
    }
        

       
      


       

        
    
}
