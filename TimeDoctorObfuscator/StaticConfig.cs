using System.Collections.Generic;

namespace TimeDoctorObfuscator
{
    public static class StaticConfig
    {
        public const int FirstPictureNumber = 1;
        public const int LastPictureNumber = 50;
        public static string CaptureDomain = "timedoctor.com";

        //public static HashSet<string> UrlFilterExclusions { get; set; }
        public static List<string> ExtensionFilterExclusions { get; set; }

        static StaticConfig()
        {
            //UrlFilterExclusions = new HashSet<string>
            //    {
            //        "analytics.com",
            //        "google-syndication.com",
            //        "google.com",
            //        "live.com",
            //        "microsoft.com",
            //        "/chrome-sync/",
            //        "client=chrome-omni",
            //        "doubleclick.net",
            //        "googleads.com"
            //    };
            ExtensionFilterExclusions = new List<string>(".css|.js|.png|.jpg|.gif|.ico|.svg|.fon".Split('|'));
        }
    }
}