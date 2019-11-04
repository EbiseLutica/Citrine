#pragma warning disable CS8618 // API POCO クラスは除外
using System;
using Newtonsoft.Json;

namespace Citrine.Sea
{
    public class User
	{
		[JsonProperty("avatarFile")]
		public File? AvatarFile { get; set; }

		[JsonProperty("createdAt")]
		public DateTime CreatedAt { get; set; }

		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("postsCount")]
		public int PostsCount { get; set; }

		[JsonProperty("screenName")]
		public string ScreenName { get; set; }

		[JsonProperty("updatedAt")]
		public DateTime UpdatedAt { get; set; }
	}
}
