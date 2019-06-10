using Citrine.Core.Api;
using Discord;
using ICUser = Citrine.Core.Api.IUser;

namespace Citrine.Discord
{
	public class DCPost : IPost
	{
		public string Id { get; }

		public ICUser User { get; }

		public string Text { get; }

		public bool IsRepost => Repost != null;

		public IPost Repost => null;

		public bool IsReply => Reply != null;

		public IPost Reply { get; }

		public long RepostCount => 0;

		public Visiblity Visiblity => Visiblity.Public;

		public string NativeVisiblity => "";

		public string Via { get; }

		public IPoll Poll => null;

		public DCPost(IMessage mes, IPost reply = null)
		{
			Id = mes.GetJumpUrl();
			User = new DCUser(mes.Author);
			Text = mes.Content;
			Reply = reply;
			Via = mes.Application?.Name ?? "";
		}
	}
}
