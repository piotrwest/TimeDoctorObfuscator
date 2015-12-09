using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace TimeDoctorObfuscator
{
    public class UrlCaptureConfiguration
    {
        private const string CaptureconfigFile = "captureConfig.txt";

        [XmlIgnore]
        [Browsable(false)]
        public int ProcessId { get; set; }

        public int ProxyPort { get; set; }
        public bool IgnoreResources { get; set; }
        public string CaptureDomain { get; set; }
        
        public List<string> UrlFilterExclusions { get; set; }
        public List<string> ExtensionFilterExclusions { get; set; }

        [Browsable(false)]
        public string Cert { get; set; }

        [Browsable(false)]
        public string Key { get; set; }


        public UrlCaptureConfiguration()
        {
            IgnoreResources = true;
            ProxyPort = 8888;
            UrlFilterExclusions = new List<string>();
            ExtensionFilterExclusions = new List<string>();
        }

        public void Initialize()
        {
            if (UrlFilterExclusions.Count < 1)
            {
                UrlFilterExclusions = new List<string>()
                {
                    "analytics.com",
                    "google-syndication.com",
                    "google.com",
                    "live.com",
                    "microsoft.com",
                    "/chrome-sync/",
                    "client=chrome-omni",
                    "doubleclick.net",
                    "googleads.com"
                };
            }
            if (ExtensionFilterExclusions.Count < 1)
            {
                ExtensionFilterExclusions =
                    new List<string>(".css|.js|.png|.jpg|.gif|.ico|.svg|.fon".Split('|'));
            }
        }

        public static UrlCaptureConfiguration Read()
        {
            if (!File.Exists(CaptureconfigFile))
            {
                var config = new UrlCaptureConfiguration();
                config.Initialize();
                File.WriteAllText(CaptureconfigFile, JsonConvert.SerializeObject(config, Formatting.Indented), Encoding.UTF8);
            }
            return JsonConvert.DeserializeObject<UrlCaptureConfiguration>(File.ReadAllText(CaptureconfigFile));
        }

        public void Save()
        {
            File.WriteAllText(CaptureconfigFile, JsonConvert.SerializeObject(this, Formatting.Indented), Encoding.UTF8);
        }
    }
}