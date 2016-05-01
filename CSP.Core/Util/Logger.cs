using System;

namespace CSP.Core.Util
{
    public class Logger
    {
        public delegate void ShowLog(string s);

        #region Fields

        public static ShowLog Show;
        private static readonly object padlock = new object();
        private static Logger instance;

        #endregion

        #region Properties

        public static Logger Instance
        {
            get {
                if (instance == null)
                    lock (padlock) {
                        if (instance == null) {
                            instance = new Logger();
                            Show = s => Console.WriteLine(s);
                        }
                    }

                return instance;
            }
        }

        #endregion

        #region Methods

        public static void Log(string text) => Show(string.Format("[{0}] {1}", DateTime.Now, text));
        public static void Log(string text, params object[] arg) => Log(string.Format(text, arg));

        #endregion
    }
}
