using System;
using Citrine.Core.Api;
using Disboard.Mastodon.Models;

namespace Citrine.Mastodon
{
	public class DonAttachment : IAttachment
	{
		public DonAttachment(Attachment native, string id, string name, string url, string previewUrl, DateTime createdAt, string comment)
		{
			this.Native = native;
			this.Id = id;
			this.Name = name;
			this.Url = url;
			this.PreviewUrl = previewUrl;
			this.CreatedAt = createdAt;
			this.Comment = comment;

		}
		public Attachment Native { get; private set; }

		public string Id { get; private set; }

		public string Name { get; private set; }

		public string Url { get; private set; }

		public string PreviewUrl { get; private set; }

		public DateTime CreatedAt { get; private set; }

		public string Comment { get; private set; }

		public DonAttachment(Attachment m)
		{
			Native = m;
			Id = m.Id.ToString();
			Comment = m.Description;
			Name = "";
			Url = m.Url;
			PreviewUrl = m.PreviewUrl;
		}
	}
}
