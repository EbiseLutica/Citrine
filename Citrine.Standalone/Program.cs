using System;

namespace Citrine.Standalone
{
	using System.Threading.Tasks;
	using Core;
	using Core.Api;
	using static Console;
	class Program
	{
		const string Version = "1.0.0";
		static async Task Main(string[] args)
		{
			WriteLine($"Citrine.Standalone version {Version}");
			var shell = new Shell();
			var server = new Server();
			WriteLine($"Citrine version {Core.Server.Version}");
			WriteLine($"XelticaBot version {Core.Server.VersionAsXelticaBot}");
			WriteLine("(C)2019 Xeltica");
			WriteLine();
			WriteLine("終了時は exit と入力してください");
			while (true)
			{
				Write("> ");
				var text = ReadLine();
				if (string.IsNullOrWhiteSpace(text))
					continue;
				var post = new Post
				{
					User = UserStore.You,
					Text = text,
				};
				WriteLine($"{post.User.ScreenName}: {post.Text}");
				await Task.WhenAll
				(
					server.HandleMentionAsync(post, shell),
					server.HandleTimelineAsync(post, shell)
				);
			}
		}
	}

	public class User : IUser
	{
		public string Name { get; set; }

		public string IconUrl { get; set; }

		public string ScreenName { get; set; }

		public string Id { get; set; }

		public string Description { get; set; }

		public string Host => default;

		public bool IsVerified { get; set; }

		public bool IsBot { get; set; }

		public long FollowingsCount => 0;

		public long FollowersCount => 0;

		public long PostsCount => 0;
	}

	public class Post : IPost
	{
		public string Id { get; set; }

		public IUser User { get; set; }

		public string Text { get; set; }

		public bool IsRepost { get; set; }

		public IPost Repost { get; set; }

		public bool IsReply { get; set; }

		public IPost Reply { get; set; }

		public long RepostCount { get; set; }

		public Visiblity Visiblity { get; set; }

		public string NativeVisiblity { get; set; }

		public string Via { get; set; }

		public IPoll Poll => null;
	}

	public class Shell : IShell
	{
		public IUser Myself => UserStore.Citrine;

		public bool CanCreatePoll => false;

		public Task<IPost> GetPostAsync(string id)
		{
			return null;
		}

		public async Task<IUser> GetUserAsync(string id)
		{
			return id == "0" ? UserStore.Citrine : id == "1" ? UserStore.You : null;
		}

		public async Task<IUser> GetUserByNameAsync(string name)
		{
			name = name.ToLowerInvariant();
			return name == "citrine" ? UserStore.Citrine : name == "you" ? UserStore.You : null;
		}

		public async Task LikeAsync(IPost post)
		{
			WriteLine($"{Myself.ScreenName} がいいねしました");
		}

		public async Task<IPost> PostAsync(string text, string cw = null, Visiblity visiblity = Visiblity.Default)
		{
			WriteLine($"{Myself.ScreenName}: {text}");
			return null;
		}

		public async Task ReactAsync(IPost post, string reactionChar)
		{
			WriteLine($"{Myself.ScreenName} がリアクション: {reactionChar}");
		}

		public async Task<IPost> ReplyAsync(IPost post, string text, string cw = null, Visiblity visiblity = Visiblity.Default)
		{
			WriteLine($"{Myself.ScreenName} » {post.User.ScreenName}: {text}");
			return new Post();
		}

		public async Task<IPost> RepostAsync(IPost post, string text = null, string cw = null, Visiblity visiblity = Visiblity.Default)
		{
			WriteLine($"{Myself.ScreenName} RP: {post.User.ScreenName}: {post.Text}");
			return new Post();
		}

		public async Task<IPost> SendDirectMessageAsync(IUser user, string text)
		{
			WriteLine($"Message @{Myself.Name}: {text}");
			return null;
		}

		public async Task UnlikeAsync(IPost post)
		{
			// do nothing
		}

		public async Task VoteAsync(IPost post, int choice)
		{
			// do nothing
		}
	}

	public static class UserStore
	{
		public static IUser Citrine = new User
		{
			Id = "0",
			Name = "Citrine",
			ScreenName = "シトリン",
			IsBot = true,
			IsVerified = true
		};

		public static IUser You = new User
		{
			Id = "1",
			Name = "You",
			ScreenName = "あなた",
		};
	}
}
