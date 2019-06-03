using System;
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
			Repost = IsRepost ? new MiPost(Native.Renote) : default;
			IsReply = Native.Reply != default;
			Reply = IsReply ? new MiPost(Native.Reply) : default;
			RepostCount = Native.RenoteCount;
			Poll = Native.Poll != default ? new MiPoll(Native.Poll) : default;
			Via = Native.App?.Name;
			Visiblity = Native.Visibility.ToVisiblity();
			NativeVisiblity = Native.Visibility;
		}

		public string Id  { get; }

		public IUser User  { get; }

		public string Text  { get; }

		public bool IsRepost  { get; }

		public Visiblity Visiblity { get; }

		public string NativeVisiblity { get; }

		public IPost Repost  { get; }

		public bool IsReply  { get; }

		public IPost Reply  { get; }

		public long RepostCount  { get; }

		public IPoll Poll  { get; }

		public string Via { get; }
	}
}
