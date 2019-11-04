#pragma warning disable CS8618 // API POCO クラスは除外
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Citrine.Sea
{
    public class File
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("variants")]
        public List<FileVariant> Variants { get; set; }
    }
}
