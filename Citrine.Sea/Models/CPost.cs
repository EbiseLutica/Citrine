#pragma warning disable CS8618 // API POCO クラスは除外
using System.Collections.Generic;
using System.Linq;
using Citrine.Core.Api;
using Newtonsoft.Json;

namespace Citrine.Sea
{
	public class CPost : IPost
	{
		public string Id { get; set; }

		public IUser User { get; set; }

		public string Text { get; set; }

		public string Via { get; set; }

		public List<IAttachment> Attachments { get; set; }

		public bool IsRepost => false;

		public IPost? Repost => null;

		public bool IsReply => false;

		public IPost? Reply => null;

		public long RepostCount => 0;

		public Visibility Visiblity => Visibility.Public;

		public string NativeVisiblity => "public";

		public IPoll? Poll => null;

		public CPost(Post p)
		{
			Id = p.Id.ToString();
			User = new CUser(p.User, p.Application.IsAutomated);
			Text = p.Text;
			Via = p.Application.Name;
			Attachments = p.Files.Select(f => new CAttachment(f) as IAttachment).ToList();
		}
	}
}
