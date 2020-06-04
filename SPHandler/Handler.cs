﻿using System;
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
        static string sourceLibrary = "Documents";
        static string destinationPath = "Pfad aus Settings";
        private static string username;
        private static string password;

        public enum FehlerCodes
        {
            WrongCredidentials,
            SharePointPathNonExistent
        }

        public static void setUsername(string name)
        {
            username = name;
        }

        public static void setPassword(string pw)
        {
            password = Authentification.StringToSecureString(pw).ToString(); //SecureString war in der eigentlich CSOM vorgegeben, in der Neuen aber nicht, wird noch geändert
        }

        public static bool testConnection()
        {
            try
            {
                SharePointOnlineCredentials Credentials = new SharePointOnlineCredentials(username, password);

                ClientContext context = new ClientContext(sourceSite); //create context
                context.Credentials = Credentials;

                List list = context.Web.Lists.GetByTitle(sourceLibrary); //retrieve list
                context.Load(list);
                context.ExecuteQueryAsync().RunSynchronously();

                CamlQuery query = new CamlQuery(); //retrieve all items
                ListItemCollection ListItems = list.GetItems(query);
                context.Load(ListItems);
                context.ExecuteQueryAsync().RunSynchronously();
            }catch(Exception e)
            {
                return false;
            }
            return true;
        }


        static void SharePointTryOut()
        {
            username = Authentification.GetUserName();
            password = Authentification.GetPassword().ToString();
            SharePointOnlineCredentials Credentials = new SharePointOnlineCredentials(username, password);

            ClientContext context = new ClientContext(sourceSite); //create context
            context.Credentials = Credentials;

            List list = context.Web.Lists.GetByTitle(sourceLibrary); //retrieve list
            context.Load(list);
            context.ExecuteQueryAsync();

            CamlQuery query = new CamlQuery(); //retrieve all items
            ListItemCollection ListItems = list.GetItems(query);
            context.Load(ListItems);
            context.ExecuteQueryAsync();


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
                   throw new Exception("something went wrong");
                }


                //OVERWRITE ITEMS CHECK
                //if (item.LastWriteTime.ToString("d-M-yyyy hh:mm:ss") - ne modified.addhours(1).ToString("d-M-yyyy hh:mm:ss"))
                //{
                //    System.Net.WebClient client = new System.Net.WebClient;
                //    client.Credentials = Credentials;
                //    client.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
                //    client.DownloadFile(sourceItemPath, destinationItemPath);
                //    client.Dispose();
                //    file = Get - Item destinationItemPath;
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
