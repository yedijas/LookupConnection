using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;

namespace LookupConnection
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] myURIs = new string[4];
            string availableURL = "";
            StringBuilder result = new StringBuilder();
            myURIs[0] = ConfigurationManager.AppSettings["URI0"];
            myURIs[1] = ConfigurationManager.AppSettings["URI1"];
            myURIs[2] = ConfigurationManager.AppSettings["URI2"];
            myURIs[3] = ConfigurationManager.AppSettings["URI3"];
            availableURL = ConnectedToAnyOf(myURIs);

            if (availableURL != "")
            {
                result.Append("Connected to : ");
                result.Append(availableURL);
            }
            else
            {
                result.Append("Failed to connect!");
            }
            Console.WriteLine(result.ToString());
            Console.Read();
        }

        private static bool testConnection(string theURI, int timeOutMs)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(theURI);
                request.KeepAlive = false;
                request.Timeout = timeOutMs;
                var response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true; // successfully connected
                }
                else
                {
                    return false; // network wise connected but host error
                }
            }
            catch
            {
                return false; // fail with no reason
            }
        }

        public static string ConnectedToAnyOf(string[] url)
        {
            for (var index = 0; index < url.Length; index++)
            {
                try
                {
                    if (testConnection(url[index], 10000)) // test for 10s timeout
                    {
                        return url[index];
                    }
                }
                catch
                {
                    return "";
                }
            }
            return "";
        }

    }
}
