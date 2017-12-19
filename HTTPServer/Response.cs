using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath,string Version)
        {
            //throw new NotImplementedException();
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            this.code = code;
            string StatusLine = GetStatusLine(code);
            responseString = "";
            responseString += Version + " " + StatusLine+ "\n";
            responseString += "Content-Type:" + "text/html" + "\n";
            responseString += "Date:" + DateTime.Now + "\n";
            responseString+="Content-Length:"+content.Length+"\n";
            if (redirectoinPath != null)
                responseString+="Location:" + redirectoinPath+"\n";
            responseString += "\n";
            responseString+=content;


          

        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = Enum.GetName(typeof(StatusCode), code);

            return statusLine;
        }
    }
}
