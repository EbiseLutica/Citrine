using Citrine.Core.Api;
using Disboard.Mastodon.Models;

namespace Citrine.Mastodon
{
    internal class DonUser : IUser
    {
        public Account Native { get; private set; }

        public string Name { get; private set; }

        public string IconUrl { get; private set; }

        public string ScreenName { get; private set; }

        public string Id { get; private set; }

        public string Description { get; private set; }

        public string Host { get; private set; }

        public bool IsVerified { get; private set; }

        public bool IsBot { get; private set; }

        public long FollowingsCount { get; private set; }

        public long FollowersCount { get; private set; }

        public long PostsCount { get; private set; }

        public DonUser(Account user)
        {
            Native = user;
            Name = user.Username;
            ScreenName = user.DisplayName;
            IconUrl = user.Avatar;
            Id = user.Id.ToString();
            Description = user.Note;
            // 取れない？
            Host = default;
            IsVerified = false;
            IsBot = user.IsBot ?? false;
            FollowingsCount = user.FollowingCount;
            FollowersCount = user.FollowersCount;
            PostsCount = user.StatusesCount;
        }

        // <("[^"]*"|'[^']*'|[^'">])*>
    }
}