using System;
using Citrine.Core.Api;
using Disboard.Misskey.Models;

namespace Citrine.Misskey
{
	public class MiAttachment : IAttachment
	{
		public string Id { get; private set; }

		public string Name { get; private set; }

		public string Url { get; private set; }

		public string PreviewUrl { get; private set; }

		public DateTime CreatedAt { get; private set; }

		public string Comment { get; private set; }

		public File Native { get; private set; }

		public MiAttachment(File file)
		{
			Native = file;
			Id = file.Id;
			Name = file.Name;
			Url = file.Url;
			PreviewUrl = file.ThumbnailUrl;
			CreatedAt = file.CreatedAt;
			Comment = file.Comment;
		}
	}


}
