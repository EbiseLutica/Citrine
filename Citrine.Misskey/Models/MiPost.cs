using System;
using System.Collections.Generic;
using System.Linq;
using Citrine.Core.Api;
using Disboard.Misskey.Models;

namespace Citrine.Misskey
{
	public class MiPost : IPost
	{
		public Note Native { get; }
		public MiPost(Note n)
		{
			Native = n;
			Id = Native.Id;
			User = new MiUser(Native.User);
			Text = Native.Text;
			IsRepost = Native.Renote != default;
			Repost = Native.Renote != null ? new MiPost(Native.Renote) : default;
			IsReply = Native.Reply != default;
			Reply = Native.Reply != null ? new MiPost(Native.Reply) : default;
			RepostCount = Native.RenoteCount;
			Poll = Native.Poll != default ? new MiPoll(Native.Poll) : default;
			Via = Native.App?.Name;
			Visiblity = Native.Visibility.ToVisiblity();
			NativeVisiblity = Native.Visibility;
			Attachments = new List<IAttachment>();
			if (n.Files != null)
				Attachments = n.Files.Select(file => new MiAttachment(file) as IAttachment).ToList();
		}

		public string Id { get; }

		public IUser User { get; }

		public string Text { get; }

		public bool IsRepost { get; }

		public Visibility Visiblity { get; }

		public string? NativeVisiblity { get; }

		public IPost? Repost { get; }

		public bool IsReply { get; }

		public IPost? Reply { get; }

		public long RepostCount { get; }

		public IPoll? Poll { get; }

		public string? Via { get; }

		public List<IAttachment> Attachments { get; }
	}
}
