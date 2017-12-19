using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        public HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            // throw new NotImplementedException();

            //TODO: parse the receivedRequest using the \r\n delimeter   
            string[] delimeter = new string[] { "\r\n" };
            requestLines = requestString.Split(delimeter, StringSplitOptions.None);

            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (requestLines.Length < 3)
                return false;
            // Parse Request line
          



            // Load header lines into HeaderLines dictionary

            bool res = (ParseRequestLine() && LoadHeaderLines() && ValidateIsURI(relativeURI) && ValidateBlankLine());
            return res;

            //     return false;
        }

        private bool ParseRequestLine()
        {
            // throw new NotImplementedException();
              string[] requestLineTokens = requestLines[0].Split(' ');
            method = (RequestMethod)Enum.Parse(typeof(RequestMethod), requestLineTokens[0]);
            relativeURI = requestLineTokens[1];
            if (requestLineTokens[2].Equals("HTTP/1.0")) //SlaamComment HTTP 0.9 
                httpVersion = HTTPVersion.HTTP10;
            else if (requestLineTokens[2].Equals("HTTP/1.1"))
                httpVersion = HTTPVersion.HTTP11;
            else if (requestLineTokens[2].Equals("HTTP/0.9") || requestLineTokens.Contains("HTTP"))
                httpVersion = HTTPVersion.HTTP09;

            return (method == RequestMethod.GET);

        }

        private bool ValidateIsURI(string uri)
        {

            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);

        }

        private bool LoadHeaderLines()
        {
            // throw new NotImplementedException();
            int i = 1;
            headerLines = new Dictionary<string, string>();
            while (i < requestLines.Length - 1)
            {
                if (requestLines[i] == "")
                    break;
                string[] headers = requestLines[i].Split(':');

                headerLines.Add(headers[0], headers[1]);
                i++;
            }
            if (httpVersion == HTTPVersion.HTTP11)
                return headerLines.ContainsKey("Host");
            else
                return true;
        }

        private bool ValidateBlankLine()
        {
            //throw new NotImplementedException();
            return (requestLines[requestLines.Length - 2] == "");
        }

    }
}