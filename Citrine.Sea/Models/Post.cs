#pragma warning disable CS8618 // API POCO クラスは除外
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Citrine.Sea
{
    public class Post
	{
		[JsonProperty("application")]
		public Application Application { get; set; }

		[JsonProperty("createdAt")]
		public DateTime CreatedAt { get; set; }

		[JsonProperty("files")]
		public List<File> Files { get; set; }

		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("text")]
		public string Text { get; set; }

		[JsonProperty("updatedAt")]
		public DateTime UpdatedAt { get; set; }

		[JsonProperty("user")]
		public User User { get; set; }
	}
}
