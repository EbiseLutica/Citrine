using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Citrine.Core
{
    public class Config
    {
        public static Config Instance { get; }

        static Config()
        {
            if (File.Exists("./config.json"))
            {
                Instance = JsonConvert.DeserializeObject<Config>(File.ReadAllText("./config.json"));
            }
            else
            {
                Instance = new Config();
            }
        }

        [JsonProperty("moderators")]
        public List<string> Moderators { get; set; } = new List<string>();

        [JsonProperty("admin")]
        public string Admin { get; set; }

        [JsonProperty("loggingLevel")]
        public LoggingLevel LoggingLevel { get; set; } = LoggingLevel.Info;

        [JsonProperty("logPath")]
        public string LogPath { get; set; } = "./log";

        [JsonProperty("useFileLogging")]        
        public bool UseFileLogging { get; set; } = true;

        [JsonProperty("useConsoleLogging")]        
        public bool UseConsoleLogging { get; set; } = true;

        public void Save()
        {
            File.WriteAllText("./config.json", JsonConvert.SerializeObject(this));
        }
    }

    public enum LoggingLevel
    {
        Debug = 1,
        Write = 2,
        Info = 3,
        Warn = 4,
        Error = 5,
    }
}