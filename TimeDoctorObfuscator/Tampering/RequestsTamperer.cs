using System;
using System.IO;
using System.Text.RegularExpressions;
using Fiddler;
using NLog;
using Logger = NLog.Logger;

namespace TimeDoctorObfuscator.Tampering
{
    public class RequestsTamperer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly UrlCaptureConfiguration _config;

        public RequestsTamperer(UrlCaptureConfiguration config)
        {
            _config = config;
        }

        public void BeforeRequest(Session session)
        {
            // Ignore HTTPS connect requests
            if (session.RequestMethod == "CONNECT")
                return;

            var fullUrl = session.fullUrl.ToLower();
            //if (StaticConfig.UrlFilterExclusions.Contains(fullUrl)) //not needed - we are looking only for CaptureDomain
            //    return;

            if (!session.hostname.ToLower().Contains(StaticConfig.CaptureDomain))
                return;

            if (RequestIsForStaticFile(fullUrl))
                return;

            LogUrlHit(session);
#if DEBUG
            LogMethodHitWithBody(session);
#endif

            if (session.PathAndQuery.ToLower().Contains("upload_screen"))
            {
                var screenshotDecorator = new ScreenshotDecorator();
                screenshotDecorator.ProcessScreenUpload(session);
            }
            else
            {
                if (session.PathAndQuery.ToLower().Contains("upload_time")) //matches methods: upload_timeuse, upload_time
                {
                    var timeuseDecorator = new TimeuseDecorator();
                    timeuseDecorator.ProcessTimeuse(session);
                }
            }
        }

        private static void LogMethodHitWithBody(Session session)
        {
            var logMsg = session.PathAndQuery;
            var regex = new Regex(@"(method=)(?<method>.*)(&)");
            var match = regex.Match(logMsg);
            if (match.Success)
            {
                if (logMsg.StartsWith(@"/v2/api/execute.php?ver=tds-win-2.3.47.11&method="))
                {
                    logMsg = logMsg.Replace(@"/v2/api/execute.php?ver=tds-win-2.3.47.11&method=", "");
                }
                var content = session.GetRequestBodyAsString();
                if (content.Contains("&file["))
                {
                    for (int i = 0; i < 20; i++)
                    {
                        var regex1 = @"(file\[" + i + @"]=)([A-Za-z0-9%]+)";
                        content = Regex.Replace(content, regex1, "$1" + "omnomnom");
                    }
                }
                File.AppendAllText("log-" + match.Groups["method"].Value + ".txt",
                    DateTime.Now.ToLongTimeString() + Environment.NewLine + logMsg + Environment.NewLine + content +
                    Environment.NewLine);
            }
        }

        private static void LogUrlHit(Session session)
        {
            var logMsg = session.PathAndQuery;
            if (logMsg.StartsWith(@"/v2/api/execute.php?ver=tds-win-2.3.47.11&method="))
            {
                logMsg = logMsg.Replace(@"/v2/api/execute.php?ver=tds-win-2.3.47.11&method=", "Method: ");
            }
            Logger.Debug(logMsg);
        }

        private static bool RequestIsForStaticFile(string fullUrl)
        {
            foreach (var ext in StaticConfig.ExtensionFilterExclusions)
            {
                if (fullUrl.Contains(ext))
                    return true;
            }
            return false;
        }
    }
}
