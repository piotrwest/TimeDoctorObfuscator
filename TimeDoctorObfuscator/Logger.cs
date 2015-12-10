using System;

namespace TimeDoctorObfuscator
{
    public static class Logger
    {
        public static int DestinationWidth = 120;
        public static int DestinationHeight = 50;

        private static int _lastTop;

        private static readonly object LockObj = new object();

        public static void Log(string message)
        {
            lock (LockObj)
            {
                Console.WriteLine($"{DateTime.Now.ToString("t")}: {message}");
            }
        }

        public static void LogDebug(string message)
        {
            lock (LockObj)
            {
                var origTop = Console.CursorTop;
                var origLeft = Console.CursorLeft;
                var origColor = Console.ForegroundColor;

                Console.ForegroundColor = ConsoleColor.Gray;

                _lastTop++;
                if (_lastTop > DestinationWidth - 1)
                {
                    _lastTop = 0;
                }
                Console.SetCursorPosition(60, _lastTop);
                var log = $"|{DateTime.Now.ToString("t")}: {message}";
                log = log.Substring(0, Math.Min(log.Length, 60));
                Console.Write(log);

                Console.SetCursorPosition(origLeft, origTop);
                Console.ForegroundColor = origColor;
            }
        }
    }
}