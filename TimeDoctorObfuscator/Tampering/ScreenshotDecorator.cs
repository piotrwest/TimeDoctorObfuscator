using System;
using System.IO;
using System.Text.RegularExpressions;
using Fiddler;
using NLog;
using Logger = NLog.Logger;

namespace TimeDoctorObfuscator.Tampering
{
    public class ScreenshotDecorator
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly Random Rand = new Random();
        private const string Placeholder = "E03C3BBD328B4987BFF6C5E83814D311";

        public void ProcessScreenUpload(Session sess)
        {
            //string contentType =
            //    sess.oRequest.headers.Where(hd => hd.Name.ToLower() == "content-type")
            //        .Select(hd => hd.Name)
            //        .FirstOrDefault();

            //string reqBody = null;
            //if (sess.RequestBody.Length > 0)
            //{
            //    if (sess.requestBodyBytes.Contains((byte)0) || contentType.StartsWith("image/"))
            //        reqBody = "b64_" + Convert.ToBase64String(sess.requestBodyBytes);
            //    else
            //    {
            //reqBody = Encoding.Default.GetString(session.ResponseBody);
            var reqBody = sess.GetRequestBodyAsString();
            //    }
            //}

            for (int i = 0; i < 20; i++)
            {
                if (reqBody.Contains($"file[{i}]="))
                {
                    var regex = @"(file\[" + i + @"]=)([A-Za-z0-9%]+)";
                    var replaced = Regex.Replace(reqBody, regex, "$1" + Placeholder);
                    var kitten = File.ReadAllBytes($@"CompressedCats\cat-wallpaper-{Rand.Next(StaticConfig.FirstPictureNumber, StaticConfig.LastPictureNumber+1)}.jpg");
                    var kittenString = Convert.ToBase64String(kitten, Base64FormattingOptions.None);
                    var kittenEncoded = kittenString.Replace("+", "%2B").Replace("/", "%2F").Replace(" ", "%20");
                    reqBody = replaced.Replace(Placeholder, kittenEncoded);
                    Logger.Info($"Replaced {i} image :)");
                }
            }

            reqBody = Regex.Replace(reqBody, @"(keystrokes\[\d\]=)([0-9]+)", "${1}" + Rand.Next(100, 400));
            reqBody = Regex.Replace(reqBody, @"(mousemovements\[\d\]=)([0-9]+)", "${1}" + Rand.Next(100, 400));
            sess.utilSetRequestBody(reqBody);
        }
    }
}