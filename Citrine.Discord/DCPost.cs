using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Citrine.Core.Api;
using C = Citrine.Core.Api;
using Discord;

namespace Citrine.Discord
{
	public class DCPost : IPost
	{
		public string Id { get; }

		public C.IUser User { get; }

		public string Text { get; }

		public bool IsRepost => Repost != null;

		public IPost? Repost => null;

		public bool IsReply => Reply != null;

		public IPost? Reply { get; }

		public long RepostCount => 0;

		public Visibility Visiblity => Visibility.Public;

		public string NativeVisiblity => "";

		public string Via { get; }

		public IPoll? Poll => null;

		public IMessage Native { get; }

		public List<C.IAttachment> Attachments { get; }

		public DCPost(IMessage mes, IPost? reply = null)
		{
			Native = mes;
			Id = mes.GetJumpUrl();
			User = new DCUser(mes.Author);
			Text = TrimMentions(mes.Content);
			Reply = reply;
			Via = mes.Application?.Name ?? "";
			Attachments = mes.Attachments.Select(a => new DCAttachment(a) as C.IAttachment).ToList();
		}

		public static string TrimMentions(string s) => Regex.Replace(s, @"<@[0-9\!]+>", "").Trim();

	}
}
