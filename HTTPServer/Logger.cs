using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        public static void LogException(Exception ex)
        {
            // TODO: Create log file named log.txt to log exception details in it
            // for each exception write its details associated with datetime 
            FileStream fr = new FileStream("log.txt", FileMode.OpenOrCreate,FileAccess.Write);
            StreamWriter sw = new StreamWriter(fr);
            sw.WriteLine("****************************");
            sw.WriteLine(DateTime.Now);
            sw.WriteLine(ex.ToString());
            sw.WriteLine("****************************");
            sw.Close();
        
        }
    }
}
