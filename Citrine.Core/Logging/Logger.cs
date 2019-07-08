using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Citrine.Core
{
    public class Logger
    {
        public string Name { get; }


        public void Debug(object data) 
        {
            if (Config.Instance.LoggingLevel <= LoggingLevel.Debug)
                Output(data, "[DEBUG]");
        }

        public Logger(string name = null)
        {
            Name = name;
        }

        public void Write(object data) 
        {
            if (Config.Instance.LoggingLevel <= LoggingLevel.Write)
                Output(data);
        }

        public void Info(object data) 
        {
            if (Config.Instance.LoggingLevel <= LoggingLevel.Info)
                Output(data, "[INFO]");
        }

        public void Warn(object data) 
        {
            if (Config.Instance.LoggingLevel <= LoggingLevel.Warn)
                Output(data, "[WARN]");
        }

        public void Error(object data) 
        {
            if (Config.Instance.LoggingLevel <= LoggingLevel.Error)
                Output(data, "[ERROR]");
        }

        protected void Output(object data, string prefix = "")
        {
            LoggerServer.Instance.Output(data, $"{NamePrefix}{prefix ?? ""}");
        }

        protected string NamePrefix => string.IsNullOrEmpty(Name) ? "" : $"[{Name}]";

        protected class LoggerServer : IDisposable
        {
            protected LoggerServer()
            {
                var streams = new List<Stream>();

                if (Config.Instance.UseFileLogging)
                {
                    // ファイルロギング
                    if (!Directory.Exists(Config.Instance.LogPath))
                        Directory.CreateDirectory(Config.Instance.LogPath);
                    streams.Add(File.OpenWrite(Path.Combine(Config.Instance.LogPath, DateTime.Now.ToString(@"yyMMdd-HHmmss-fff.lo\g"))));    
                }

                if (Config.Instance.UseConsoleLogging)
                {
                    streams.Add(Console.OpenStandardOutput());
                }
                
                loggingStreams = streams.Select(s => new StreamWriter(s)).ToArray();
            }

            public void Dispose()
            {
                loggingStreams?.ForEach(l => l.Close());
            }

            public void Output(object obj, string prefix = "")
            {
                loggingStreams?.ForEach(l => 
                {
                    l.WriteLine($"{DateTime.Now.ToString("[HH:mm:ss]")}{prefix}: {obj ?? "null"}");
                    l.Flush();
                });
            }

            public async Task OutputAsync(object obj, string prefix = "")
            {
                await loggingStreams?.ForEach(l => l.WriteLineAsync($"{DateTime.Now.ToString("[HH:mm:ss]")}{prefix}: {obj ?? "null"}"));
            }

            internal static LoggerServer Instance { get; } = new LoggerServer();

            private StreamWriter[] loggingStreams;
        }
    }
}