using System;
using System.IO;
using System.Threading.Tasks;

namespace Citrine.Core
{
    public class Logger
    {
        public string Name { get; }
        public Logger(string name)
        {
            Name = name;
        }

        public void Info(object data) => Output(data, "[INFO]");
        public void Warn(object data) => Output(data, "[WARN]");
        public void Error(object data) => Output(data, "[ERROR]");
        public void Write(object data) => Output(data);

        protected void Output(object data, string prefix = "")
        {
            LoggerServer.Instance.Output(data, $"[{Name}]{prefix ?? ""}");
        }



        protected class LoggerServer : IDisposable
        {
            protected LoggerServer()
            {
                // ファイルロギング
                if (!Directory.Exists("log"))
                    Directory.CreateDirectory("log");
                loggingStreams = new []
                {
                    new StreamWriter(File.OpenWrite($"log/{DateTime.Now.ToString(@"yyMMdd-HHmmss-fff.lo\g")}")),
                    new StreamWriter(Console.OpenStandardOutput())
                };
            }

            public void Dispose()
            {
                loggingStreams?.ForEach(l => l.Close());
            }

            public void Output(object obj, string prefix = "")
            {
                loggingStreams?.ForEach(l => l.WriteLine($"{DateTime.Now.ToString("[HH:mm:ss]")}{prefix}: {obj ?? "null"}"));
            }

            public async Task OutputAsync(object obj, string prefix = "")
            {
                await loggingStreams?.ForEach(l => l.WriteLineAsync($"{DateTime.Now.ToString("[HH:mm:ss]")}{prefix}: {obj ?? "null"}"));
            }

            internal static LoggerServer Instance { get; }

            private StreamWriter[] loggingStreams;
        }
    }
}