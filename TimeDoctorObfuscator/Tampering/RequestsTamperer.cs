using System.Linq;
using Fiddler;

namespace TimeDoctorObfuscator.Tampering
{
    public class RequestsTamperer
    {
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

            if (session.PathAndQuery.ToLower().Contains("upload_screen"))
            {
                var screenshotDecorator = new ScreenshotDecorator();
                screenshotDecorator.ProcessScreenUpload(session);
            }
            else
            {
                if (session.PathAndQuery.ToLower().Contains("upload_timeuse"))
                {
                    var timeuseDecorator = new TimeuseDecorator();
                    timeuseDecorator.ProcessTimeuse(session);
                }
            }
        }

        private static void LogUrlHit(Session session)
        {
            var logMsg = session.PathAndQuery;
            if (logMsg.StartsWith(@"/v2/api/execute.php?ver=tds-win-2.3.47.11&method="))
            {
                logMsg = logMsg.Replace(@"/v2/api/execute.php?ver=tds-win-2.3.47.11&method=", "Method: ");
            }
            Logger.LogDebug(logMsg);
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
