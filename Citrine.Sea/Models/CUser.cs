#pragma warning disable CS8618 // API POCO クラスは除外
using System.Linq;
using Citrine.Core.Api;

namespace Citrine.Sea
{
    public class CUser : IUser
    {
        public string Name { get; }

        public string? IconUrl { get; }

        public string ScreenName { get; }

        public string Id { get; }

		// 投稿を見て判断する必要がある
        public bool IsBot { get; }

        public long PostsCount { get; }

        public string Description => "";

        public string Host => "";

        public bool IsVerified => false;

        public long FollowingsCount => 0;

        public long FollowersCount => 0;

		public CUser(User u, bool isBot)
		{
			Name = u.ScreenName;
			ScreenName = u.Name;
			IconUrl = u.AvatarFile?.Variants.FirstOrDefault()?.Url;
			Id = u.Id.ToString();
			IsBot = isBot;
		}
    }
}
