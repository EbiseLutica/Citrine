using C = Citrine.Core.Api;
using D = Discord;

namespace Citrine.Discord
{
	using System;

	public class DCAttachment : C.IAttachment
	{
		public string Id { get; private set; }

		public string Name { get; private set; }

		public string Url { get; private set; }

		public string PreviewUrl { get; private set; }

		public DateTime CreatedAt { get; private set; }

		public string Comment { get; private set; }

		public D.IAttachment Native { get; private set; }

		public DCAttachment(D.IAttachment a)
		{
			Native = a;
			Id = a.Id.ToString();
			Name = a.Filename;
			Url = a.Url;
			PreviewUrl = a.Url;
			CreatedAt = default;
			Comment = "";
		}
	}
}
