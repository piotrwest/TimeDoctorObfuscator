using System.ComponentModel;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace TimeDoctorObfuscator
{
    public class UrlCaptureConfiguration
    {
        private const string CaptureconfigFile = "captureConfig.txt";

        public int ProxyPort { get; set; }

        [Browsable(false)]
        public string Cert { get; set; }

        [Browsable(false)]
        public string Key { get; set; }


        public UrlCaptureConfiguration()
        {
            ProxyPort = 7878;
        }

        public static UrlCaptureConfiguration Read()
        {
            if (!File.Exists(CaptureconfigFile))
            {
                var config = new UrlCaptureConfiguration();
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