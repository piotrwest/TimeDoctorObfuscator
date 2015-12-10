using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Fiddler;

namespace TimeDoctorObfuscator.Tampering
{
    public class TimeuseDecorator
    {
        private static HashSet<string> processedTimeuseParts = new HashSet<string>
        {
            "url", "window_title", "process_name", "document", "sub_category", "work_mode"
        };
        public void ProcessTimeuse(Session sess)
        {
            var reqBody = sess.GetRequestBodyAsString();

            File.AppendAllText("timeuse.txt", sess.PathAndQuery + "--->" + reqBody + Environment.NewLine);
            if (reqBody.Contains("&"))
            {
                var sb = new StringBuilder();
                var parts = reqBody.Split(new[] { "&" }, StringSplitOptions.None);
                foreach (var p in parts)
                {
                    var ret = ProcessTimeusePart(p);
                    sb.Append(ret);
                    sb.Append("&");
                }

                var result = sb.ToString();
                result = result.TrimEnd('&');
                if (reqBody != result)
                {
                    Console.WriteLine($"{DateTime.Now}: Replaced some timeuse stats :)");
                }
                reqBody = result;
            }
            File.AppendAllText("timeuse.txt", "+++>" + reqBody + Environment.NewLine);

            sess.utilSetRequestBody(reqBody);
        }

        private static string ProcessTimeusePart(string p)
        {
            //p is like start_time[1]=2015-12-09T18%3A50%3A00
            //p can be start_time[1]=
            var result = p;
            //timeusePartType is like start_time
            var timeusePartType = GetEverythingInStringBeforeCharacter(p, '[');
            if (CanProcess(timeusePartType))
            {
                //timeuseValue is like 2015-12-09T18%3A50%3A00
                //timeuseValue can be empty
                var timeuseValue = GetEverythingInStringAfterCharacter(p, '=');
                if (string.IsNullOrWhiteSpace(timeuseValue))
                    return result;

                var censoredTimeuseValue = GetCensoredValue(timeusePartType);
                //result = result.Replace(timeuseValue, censoredTimeuseValue); //cant replace because timeuseValue can be in timeusePartType
                var timeusePartTypeAndOrder = GetEverythingInStringBeforeCharacter(p, '=');
                result = $"{timeusePartTypeAndOrder}={censoredTimeuseValue}";
            }

            return result;
        }

        private static string GetCensoredValue(string timeusePartType)
        {
            switch (timeusePartType)
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
                    return "0"; //don't know what it is exactly but when computer is unlocked it's always 0
                case "work_mode":
                    return "0"; //don't know what it is exactly but when computer is unlocked it's always 0
                default:
                    throw new Exception($"Don't know what to do with TimeusePartType: {timeusePartType}");
            }
        }

        private static bool CanProcess(string timeusePartType)
        {
            return processedTimeuseParts.Contains(timeusePartType);
        }
        
        private static string GetEverythingInStringBeforeCharacter(string sourceString, char c)
        {
            var l = sourceString.IndexOf(c);
            if (l > 0)
            {
                return sourceString.Substring(0, l);
            }
            return "";
        }

        private static string GetEverythingInStringAfterCharacter(string sourceString, char c)
        {
            var index = sourceString.IndexOf(c);
            if (index > 0)
            {
                var x = sourceString.Substring(index + 1);
                return x;
            }
            return "";
        }
    }
}