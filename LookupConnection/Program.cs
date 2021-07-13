using System;
using System.Configuration;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

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

        /// <summary>
        /// Check any of 4 URIs to connect and return the first successful one.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ConnectedToAnyOf(string[] url)
        {
            int method = Int32.Parse(ConfigurationManager.AppSettings["Method"]);
            for (var index = 0; index < url.Length; index++)
            {
                #region test each listed URL
                try
                {
                    bool testResult = false;
                    Uri myUri = new Uri(url[index]);
                    string ip = Dns.GetHostAddresses(myUri.Host)[0].ToString();

                    if (method == 0)
                    {
                        // webrequest
                        testResult = testConnection(url[index], 10000);
                    }
                    else if (method == 1)
                    {
                        // webclient
                        testResult = testConnection(url[index]);
                    }
                    else if (method == 2)
                    {
                        // telnet
                        testResult = testConnection(ip, myUri.Port.ToString());
                    }else if (method == 3)
                    {
                        // ping
                        testResult = testConnection(ip, 200, 128);
                    }
                    else
                    {
                        // invalid method
                        return "";
                    }

                    if (testResult)
                    {
                        return url[index];
                    }
                }
                catch
                {
                    return "";
                }
                #endregion
            }
            return "";
        }

        #region testing methods
        /// <summary>
        /// Test using telnet/TCP connection.
        /// </summary>
        /// <param name="IPAddress"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        private static bool testConnection(string IPAddress, string port)
        {
            TcpClient myTcpClient = new TcpClient();
            myTcpClient.Connect(IPAddress, Int32.Parse(port));
            if (myTcpClient.Connected)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Test using WebClient
        /// </summary>
        /// <param name="theURI"></param>
        /// <returns></returns>
        private static bool testConnection(string theURI)
        {
            try
            {
                WebClient client = new WebClient();
                client.DownloadData(theURI);
                return true;
            }
            catch
            {
                // cannot connect or error
                return false;
            }
        }

        /// <summary>
        /// Test using Ping
        /// </summary>
        /// <param name="theIPAddress"></param>
        /// <param name="timeOut"></param>
        /// <param name="tTl"></param>
        /// <param name="fragmentation"></param>
        /// <returns></returns>
        private static bool testConnection(string theIPAddress, int timeOut = 200, int tTl = 128, bool fragmentation = true)
        {
            Ping myPing = new Ping();

            PingOptions myPingOpts = new PingOptions();
            myPingOpts.Ttl = tTl;
            myPingOpts.DontFragment = fragmentation;

            // 32 bit buffer
            byte[] buffer = Encoding.ASCII.GetBytes("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
            PingReply myReply = myPing.Send(theIPAddress, timeOut, buffer, myPingOpts);
            if (myReply.Status == IPStatus.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Test using WebRequest
        /// </summary>
        /// <param name="theURI"></param>
        /// <param name="timeOutMs"></param>
        /// <returns></returns>
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
                    // successfully connected
                    return true;
                }
                else
                {
                    // network wise connected but host error
                    return false;
                }
            }
            catch
            {
                // fail with no reason
                return false;
            }
        }
        #endregion
    }
}
