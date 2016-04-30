using System;

namespace CSP.Core.Util
{
    public static class Logger
    {
        public static void Log(String text)
        {
            Console.WriteLine("[%s] %s", DateTime.Now, text);
        }
    }
}
