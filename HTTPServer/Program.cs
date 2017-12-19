using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: Call CreateRedirectionRulesFile() function to create the rules of redirection 
            
            string path = "C:\\inetpub\\RedirectionRules.txt";
            CreateRedirectionRulesFile(path);

            //Start server
            Server ser = new Server(1000, path);
            ser.StartServer();
            // 1) Make server object on port 1000
            // 2) Start Server
        }

        static void CreateRedirectionRulesFile(string path)
        {
            // TODO: Create file named redirectionRules.txt
            // each line in the file specify a redirection rule
            // example: "aboutus.html,aboutus2.html"
            // means that when making request to aboustus.html,, it redirects me to aboutus2
            FileStream fs1 = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter writer = new StreamWriter(fs1);
            writer.Write("aboutus.html,aboutus2.html");
            writer.Close();
        }
         
    }
}
