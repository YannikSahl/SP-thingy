using System;
using System.IO;
using Microsoft.SharePoint.Client;
using System.Security;
using File = Microsoft.SharePoint.Client.File;

namespace SharePointTryOut
{
    internal class Program
    {
        private const string rootSite = "https://htwberlinde.sharepoint.com/";
        private const string sourceSite = "https://htwberlinde.sharepoint.com/sites/SWE";
        private static string sourceLibrary = "Dokumente";
        private static string destinationPath = "C:\\downloads";
        private static string username;
        private static string password;


        private static void Main(string[] args)
        {
            username = "s0568476@htw-berlin.de";
            password = "G1ftnude/";

            var Credentials = new SharePointOnlineCredentials(username, password);
            var context = new ClientContext(sourceSite); //create context
            context.Credentials = Credentials;
            Web site = context.Web;

            //var list = context.Web.Lists.GetByTitle(sourceLibrary); //retrieve list
            //context.Load(list);
            
            //context.ExecuteQueryAsync().Wait();

            //FolderCollection collFolder = site.Folders;
            //context.Load(collFolder);
            //context.ExecuteQueryAsync().Wait();

            GetFolders(site);

            //Get all Folders and Subfolders
            FolderCollection GetFolders(Web folderUrl)
            {
                FolderCollection folderColl = folderUrl.Folders;
                context.Load(folderColl);
                context.ExecuteQueryAsync().Wait();

                foreach (Folder folder in folderColl)
                {
                    Console.WriteLine(folder.Name);
                    //Web newFolderURL = new Web(new ObjectPath(folderUrl + "/" + folder.Name), );
                    //GetFolders(newFolderURL);

                }
                return folderColl;
            }


        //FileCollection files = list.RootFolder.Folders.GetByUrl(sourceSite + sourceLibrary + "04321_DB_Festp/03_Skizzen/JPG/6000/6400/6441").Files;
        //context.Load(files);
        //context.ExecuteQueryAsync().Wait();



        Console.ReadLine();

            //var query = new CamlQuery(); //retrieve all items
            //var ListItems = list.GetItems(query);
            //context.Load(ListItems);
            //context.ExecuteQueryAsync().Wait();


            //foreach (var item in ListItems)
            //{
            //    //File Variables
            //    var fileName = item["FileLeafRef"].ToString();
            //    var fileUrl = item["FileRef"].ToString();
            //    var modified = item["Modified"].ToString();
            //    var created = item["Created"].ToString();
            //    var sourceItemPath = rootSite + fileUrl;
            //    var destinationFolderPath = "C:\\Users\\multi\\Documents\\SWE";
            //    var destinationItemPath = destinationFolderPath + fileName;


            //    var client = new System.Net.WebClient();
            //    client.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
            //    client.DownloadFile(sourceItemPath, destinationItemPath);
            //    client.Dispose();
            //    //File file = Directory.GetFiles(destinationItemPath);
            //    //file.LastWriteTime = $modified;
            //    //file.CreationTime = $created

            //    Console.WriteLine("New document: " + destinationItemPath);






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
            //}
        }
    }
}