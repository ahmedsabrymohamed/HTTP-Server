using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            IPEndPoint IPEpoint = new IPEndPoint(IPAddress.Any,portNumber);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(IPEpoint);

        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            serverSocket.Listen(1000);

            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
             //TODO: accept connections and start thread for each accepted connection.
            while (true)
            {
                Socket client=serverSocket.Accept();
                Console.WriteLine("new Clinet Accepted");
                ThreadPool.QueueUserWorkItem(HandleConnection, client);
            }


            
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            Socket Client = (Socket)obj;
            Client.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    // TODO: Receive request
                 //   while(SocketConnected(Client));
                    byte[] requestData = new byte[1024];
                    
                    int len = Client.Receive(requestData);

                    // TODO: break the while loop if receivedLen==0
                    if (len == 0)
                        break;
                    // TODO: Create a Request object using received request string
                    string message = Encoding.ASCII.GetString(requestData);
                    Request request=new Request(message);
                    // TODO: Call HandleRequest Method that returns the response
                   Response Response=HandleRequest(request);
                    // TODO: Send Response back to client
                   int response_len = Encoding.ASCII.GetByteCount(Response.ResponseString);
                   byte[] response_Data = new byte[response_len+5];
                   response_Data = Encoding.ASCII.GetBytes(Response.ResponseString);
                   Client.Send(response_Data, 0, response_len, SocketFlags.None);
              //     throw new NotImplementedException();
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
            Client.Close();
        }

        Response HandleRequest(Request request)
        {
            
            string content;
           
            try
            {
                //TODO: check for bad request 
                //throw new NotImplementedException();
                if (request.ParseRequest() == false)
                {

                    Console.Write("BAD REQUEST 400 \n");
       
                    return new Response(StatusCode.BadRequest , "text/html",
                        readcontent(Path.Combine(Configuration.RootPath, Configuration.BadRequestDefaultPageName)), null, "HTTP/1.0"); 
                }
                //TODO: map the relativeURI in request to get the physical path of the resource.
                string physical_path=Configuration.RootPath + request.relativeURI;

                //TODO: check for redirect
                string path=GetRedirectionPagePathIFExist(request.relativeURI);
                if (!path.Equals(""))
                {
                    Console.WriteLine("REDIRECTION ERROR  301");
                    return new Response(StatusCode.Redirect, "text/html", readcontent(Path.Combine(Configuration.RootPath, Configuration.RedirectionDefaultPageName)), path, "HTTP/1.0"); 
                }
                else
                {
                    string defultPageName = getPageName(request.relativeURI);
                    string filePath=LoadDefaultPage(defultPageName);
                    if (filePath.Equals(""))
                    {
                        Console.WriteLine("PAGE NOT EXIST 404");
                        return new Response(StatusCode.NotFound, "text/html",
                        readcontent(Path.Combine(Configuration.RootPath, Configuration.NotFoundDefaultPageName)), null, "HTTP/1.0");
                    }

                    else
                    {
                        Console.WriteLine("PAGE OK");
                        return new Response(StatusCode.OK, "text/html",
                        readcontent(Path.Combine(Configuration.RootPath, defultPageName)), null, "HTTP/1.0");
                    }
                }
                //TODO: check file exists
               


                //TODO: read the physical file

                // Create OK response
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error. 
                Console.WriteLine(" Internal Server Error ");
                return new Response(StatusCode.InternalServerError, "text/html",
                        readcontent(Path.Combine(Configuration.RootPath, Configuration.InternalErrorDefaultPageName)), null, "HTTP/1.0");
            }
       
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            string pagename = getPageName(relativePath);
            string pagePath;
            if (Configuration.RedirectionRules.ContainsKey(pagename))
            {
                Configuration.RedirectionRules.TryGetValue(pagename, out pagePath);
                return Path.Combine(Configuration.RootPath, pagePath);
            }
            return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            if (File.Exists(filePath))
            {
                return readfile(filePath).ToString(); 
            }

            // TODO: check if filepath not exist log exception using Logger class and return empty string
            
            // else read file and return its content
            return string.Empty;
        }

        private void LoadRedirectionRules(String filePath)
        {
            
           
            try
            {

                // TODO: using the filepath paramter read the redirection rules from file 
                Configuration.RedirectionRules = new Dictionary<string, string>();
                List<String>fileLines=readfile(filePath);
                foreach(String line in fileLines)
                {
                    String[] rule = line.Split(',');
                    Configuration.RedirectionRules.Add(rule[0], rule[1]);
                } 
                 
                // then fill Configuration.RedirectionRules dictionary 

            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
        private List<String> readfile(String path)
        {
            String line; 
            FileStream fs2 = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read);
            StreamReader reader = new StreamReader(fs2);
            List<String> lines = new List<String>();
            while ((line = reader.ReadLine()) != null)
            {
                lines.Add(line);
                
            }
            return lines;
        }
        private string readcontent(string path)
        {
            StreamReader sr = new StreamReader(path);
            return sr.ReadToEnd();
            
        }
        bool SocketConnected(Socket s)
        {
            bool part1 = s.Poll(1000, SelectMode.SelectRead);
            bool part2 = (s.Available == 0);
            if (part1 && part2)
            {
                Console.WriteLine("socket is not connected");
                return true;
            }
            else
            {
               // Console.WriteLine("socket is connected");
                return false;
            }
        }
        string getPageName(string relativePath)
        {
            string pagename = "";
            foreach (char x in relativePath)
            {
                if (x == '/')
                    pagename = "";
                else
                    pagename += x;
            }
            return pagename;
        }
    
    }
}
