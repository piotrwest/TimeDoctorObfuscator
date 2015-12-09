using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Fiddler;

namespace TimeDoctorObfuscator
{
    public class RequestsTamperer
    {
        private readonly UrlCaptureConfiguration _config;
        private static readonly Random Rand = new Random();

        public RequestsTamperer(UrlCaptureConfiguration config)
        {
            _config = config;
        }

        private const string placeholder = "E03C3BBD328B4987BFF6C5E83814D311";
        private const string Separator = "------------------------------------------------------------------";

        private static string GetCensoredOutput(string s)
        {
            var wot = sep(s);
            switch (wot)
            {
                case "url":
                    return "";
                //return "censored.com";
                case "window_title":
                    return "Censored%20Window%20Title";
                case "process_name":
                    return "CensoredProcess";
                case "document": //full http
                    return ""; //no www usage
                               //return "http%3A%2F%2FCensored.Website.com";
                case "sub_category":
                    return "0";
                case "work_mode":
                    return "0";
                default:
                    return s;
            }
        }

        private static string sep(string s)
        {
            int l = s.IndexOf("[");
            if (l > 0)
            {
                return s.Substring(0, l);
            }
            return "";

        }

        public void BeforeRequest(Session session)
        {
            // Ignore HTTPS connect requests
            if (session.RequestMethod == "CONNECT")
                return;

            if (_config.ProcessId > 0)
            {
                if (session.LocalProcessID != 0 && session.LocalProcessID != _config.ProcessId)
                    return;
            }

            var filters = _config.UrlFilterExclusions;
            foreach (var urlFilter in filters)
            {
                string url = session.fullUrl.ToLower();
                if (url.Contains(urlFilter))
                    return;
            }

            if (!string.IsNullOrEmpty(_config.CaptureDomain))
            {
                if (!session.hostname.ToLower().Contains(_config.CaptureDomain.Trim().ToLower()))
                    return;
            }

            if (_config.IgnoreResources)
            {
                string url = session.fullUrl.ToLower();

                var extensions = _config.ExtensionFilterExclusions;
                foreach (var ext in extensions)
                {
                    if (url.Contains(ext))
                        return;
                }
            }

            if (session == null || session.oRequest == null || session.oRequest.headers == null)
                return;

            if (session.PathAndQuery.ToLower().Contains("upload_screen"))
            {
                ProcessScreenUpload(session);
            }
            else
            {
                if (session.PathAndQuery.ToLower().Contains("upload_timeuse"))
                {
                    ProcessTimeuse(session);
                }
            }

            // if you wanted to capture the response
            //string respHeaders = session.oResponse.headers.ToString();
            //var respBody = Encoding.UTF8.GetString(session.ResponseBody);

            // replace the HTTP line to inject full URL
            //string firstLine = session.RequestMethod + " " + session.fullUrl + " " + session.oRequest.headers.HTTPVersion;
            //int at = headers.IndexOf("\r\n");
            //if (at < 0)
            //    return;
            //headers = firstLine + "\r\n" + headers.Substring(at + 1);

            //string output = headers + "\r\n" +
            //                (!string.IsNullOrEmpty(reqBody) ? reqBody + "\r\n" : string.Empty) +
            //                Separator + "\r\n\r\n";

            //File.AppendAllText("x.txt", output);
        }

        private static void ProcessTimeuse(Session sess)
        {
            string contentType =
                sess.oRequest.headers.Where(hd => hd.Name.ToLower() == "content-type")
                    .Select(hd => hd.Name)
                    .FirstOrDefault();

            string reqBody = null;
            if (sess.RequestBody.Length > 0)
            {
                if (sess.requestBodyBytes.Contains((byte)0) || contentType.StartsWith("image/"))
                    reqBody = "b64_" + Convert.ToBase64String(sess.requestBodyBytes);
                else
                {
                    //reqBody = Encoding.Default.GetString(session.ResponseBody);
                    reqBody = sess.GetRequestBodyAsString();
                }
            }

            if (reqBody.Contains("&"))
            {
                var sb = new StringBuilder();
                var parts = reqBody.Split(new[] { "&" }, StringSplitOptions.None);
                foreach (var p in parts)
                {
                    //p is like start_time[1]=2015-12-09T18%3A50%3A00&
                    var ret = p;
                    //if(CanProcess(p))
                    if (!p.StartsWith("end_time") && !p.StartsWith("start_time"))
                    {
                        int index = p.IndexOf("=");
                        if (index > 0)
                        {
                            var x = p.Substring(index + 1);
                            if (!string.IsNullOrEmpty(x))
                            {
                                ret = p.Replace(x, GetCensoredOutput(p));
                            }
                        }
                    }
                    sb.Append(ret);
                    sb.Append("&");
                }
                var result = sb.ToString();
                result = result.Trim('&');
                reqBody = result;
            }

            sess.utilSetRequestBody(reqBody);
        }

        private static void ProcessScreenUpload(Session sess)
        {
            string contentType =
                sess.oRequest.headers.Where(hd => hd.Name.ToLower() == "content-type")
                    .Select(hd => hd.Name)
                    .FirstOrDefault();

            string reqBody = null;
            if (sess.RequestBody.Length > 0)
            {
                if (sess.requestBodyBytes.Contains((byte)0) || contentType.StartsWith("image/"))
                    reqBody = "b64_" + Convert.ToBase64String(sess.requestBodyBytes);
                else
                {
                    //reqBody = Encoding.Default.GetString(session.ResponseBody);
                    reqBody = sess.GetRequestBodyAsString();
                }
            }

            for (int i = 0; i < 20; i++)
            {
                if (reqBody.Contains($"file[{i}]="))
                {
                    var regex = @"(file\[" + i + @"]=)([A-Za-z0-9%]+)";
                    var replaced = Regex.Replace(reqBody, regex, "$1" + placeholder);
                    var kitten = File.ReadAllBytes($@"CompressedCats\cat-wallpaper-{Rand.Next(1, 51)}.jpg");
                    var kittenString = Convert.ToBase64String(kitten, Base64FormattingOptions.None);
                    var kittenEncoded = kittenString.Replace("+", "%2B").Replace("/", "%2F").Replace(" ", "%20");
                    reqBody = replaced.Replace(placeholder, kittenEncoded);
                    Console.WriteLine($"Replaced {i} image:)");
                }
            }

            sess.utilSetRequestBody(reqBody);
        }
    }
}
