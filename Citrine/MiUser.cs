using Citrine.Core.Api;
using Disboard.Misskey.Models;

namespace Citrine.Misskey
{
	public class MiUser : IUser
	{
		public User Native { get; }
		public MiUser(User u)
		{
			Native = u;
			Name = u.Username;
			IconUrl = u.AvatarUrl;
			ScreenName = u.Name;
			Id = u.Id;
			Description = u.Description;
			Host = u.Host;
			IsVerified = u.IsVerified ?? false;
			IsBot = u.IsBot ?? false;
			FollowingsCount = u.FollowingCount;
			FollowersCount = u.FollowersCount;
			PostsCount = u.NotesCount;
		}
		public string Name { get; }

		public string IconUrl { get; }

		public string ScreenName { get; }

		public string Id { get; }

		public string Description { get; }

		public string Host { get; }

		public bool IsVerified { get; }

		public bool IsBot { get; }

		public long FollowingsCount { get; }

		public long FollowersCount { get; }

		public long PostsCount { get; }
	}
}
