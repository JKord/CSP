#region namespace
using System;
using System.IO;
using System.Collections.Generic;
using Jint;
using Jint.Native;
#endregion

namespace CSP.Core.Util
{
    public class JSRun
    {
        public static JsValue Execute(string pathFile)
        {
            return Execute(pathFile, new Dictionary<string, object>());
        }

        public static JsValue Execute(string pathFile, Dictionary<string, object> inputData)
        {
            string code = File.ReadAllText(pathFile);
            var engine = new Engine()
                .SetValue("log", new Action<string>(Logger.Log));

            foreach (var pair in inputData)
                engine.SetValue(pair.Key, pair.Value);
            engine.Execute(code);

            return engine.GetCompletionValue();
        }
    }
}
