using System;
using Citrine.Core.Api;
using Disboard.Misskey.Models;

namespace Citrine.Misskey
{
	public class MiPost : IPost
	{
		readonly Note note;
		public MiPost(Note n)
		{
			note = n;
			Id = note.Id;
			User = new MiUser(note.User);
			Text = note.Text;
			IsRepost = note.Renote != default;
			Repost = IsRepost ? new MiPost(note.Renote) : default;
			IsReply = note.Reply != default;
			Reply = IsReply ? new MiPost(note.Reply) : default;
			RepostCount = note.RenoteCount;
			Poll = note.Poll != default ? new MiPoll(note.Poll) : default;
			Via = note.App?.Name;
			Visiblity = note.Visibility.ToVisiblity();
			NativeVisiblity = note.Visibility;
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
