using Discord;
using ICUser = Citrine.Core.Api.IUser;
using IDCUser = Discord.IUser;

namespace Citrine.Discord
{
	public class DCUser : ICUser
	{
		public string Name { get; }

		public string IconUrl { get; }

		public string ScreenName { get; }

		public string Id { get; }

		public string Description => "";

		public string Host => "";

		public bool IsVerified => false;

		public bool IsBot { get; }

		public long FollowingsCount => 0;

		public long FollowersCount => 0;

		public long PostsCount => 0;

		public DCUser(IDCUser user)
		{
			Name = user.Username;
			IconUrl = user.GetAvatarUrl();
			ScreenName = (user as IGuildUser)?.Nickname ?? Name;
			Id = user.Mention;
			IsBot = user.IsBot || user.IsWebhook;
		}
	}
}
