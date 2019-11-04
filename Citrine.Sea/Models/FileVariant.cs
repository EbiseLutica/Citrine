#pragma warning disable CS8618 // API POCO クラスは除外
using Newtonsoft.Json;

namespace Citrine.Sea
{
    public class FileVariant
	{
		[JsonProperty("extension")]
		public string Extension { get; set; }

		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("mime")]
		public string Mime { get; set; }

		[JsonProperty("score")]
		public int Score { get; set; }

		[JsonProperty("size")]
		public int Size { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }
	}
}
