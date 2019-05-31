using Citrine.Core.Api;
using Disboard.Mastodon.Enums;
using Disboard.Mastodon.Models;

namespace Citrine.Mastodon
{
    using V = Core.Api.Visiblity;

    public class DonPost : IPost
    {
        public string Id { get; private set; }

        public IUser User { get; private set; }

        public string Text { get; private set; }

        public bool IsRepost { get; private set; }

        public IPost Repost { get; private set; }

        public bool IsReply { get; private set; }

        public IPost Reply { get; private set; }

        public long RepostCount { get; private set; }

        public Visiblity Visiblity { get; private set; }

        public string NativeVisiblity { get; private set; }

        public string Via { get; private set; }

        public IPoll Poll { get; private set; }

		public Status Native { get; private set; }

        public DonPost(Status toot, Shell shell)
        {
			Native = toot;
			Id = toot.Id.ToString();
			User = new DonUser(toot.Account);
			Text = toot.Content;
			IsReply = toot.InReplyToId != default;
			Reply = IsReply ? shell.GetPostAsync(toot.InReplyToId.ToString()).Result : default;
			RepostCount = toot.ReblogsCount;
			Visiblity = 
				toot.Visibility == VisibilityType.Public ? V.Public    :
                toot.Visibility == VisibilityType.Unlisted ? V.Limited :
                toot.Visibility == VisibilityType.Private ? V.Private  :
                toot.Visibility == VisibilityType.Direct ? V.Direct    : V.Default;
			NativeVisiblity = toot.Visibility?.ToString().ToLowerInvariant();
			Via = toot.Application?.Name;
			Poll = default;
        }

    }

}
