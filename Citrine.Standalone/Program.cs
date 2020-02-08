#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
using System;

namespace Citrine.Standalone
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Core;
	using Core.Api;
	using static Console;
	class Program
	{
		const string Version = "1.0.0";
		static async Task Main(string[] args)
		{
			Console.WriteLine(Server.CitrineAA + " version " + Server.Version);
			WriteLine($"Citrine.Standalone version {Version}");
			WriteLine("(C)2019-2020 Xeltica");
			WriteLine();

			var logger = new Logger("Client");
			var shell = new Shell();
			var server = new Server(shell);
			logger.Info("終了時は .exit と入力してください");
			var mode = 0;
			var modeText = new[]
			{
				"Reply",
				"DM",
				"Timeline",
			};
			while (true)
			{
				Write("> ");
				var text = ReadLine();
				if (string.IsNullOrWhiteSpace(text))
					continue;
				switch (text.ToLowerInvariant().Trim())
				{
					case ".exit":
						logger.Info("bye");
						return;
					case ".modetoggle":
						mode = (mode + 1) % 3;
						logger.Info("Changed the mode to " + modeText[mode]);
						continue;
				}
				var post = new Post(UserStore.You, UserStore.Citrine, text);
				WriteLine($"{post.User.ScreenName}: {post.Text}");
#pragma warning disable CS4014
				switch (mode)
				{
					case 0:
						server.HandleMentionAsync(post);
						break;
					case 1:
						server.HandleDmAsync(post);
						break;
				}
				server.HandleTimelineAsync(post);
#pragma warning restore CS4014
			}
		}
	}

	public class User : IUser
	{
		public string Name { get; set; } = "";

		public string IconUrl { get; set; } = "";

		public string ScreenName { get; set; } = "";

		public string Id { get; set; } = "";

		public string Description { get; set; } = "";

		public string Host => "";

		public bool IsVerified { get; set; }

		public bool IsBot { get; set; }

		public long FollowingsCount => 0;

		public long FollowersCount => 0;

		public long PostsCount => 0;
	}

	public class Post : IDirectMessage
	{
		public string Id { get; set; } = new Guid().ToString();

		public IUser User { get; set; }

		public string Text { get; set; }

		public bool IsRepost => Repost != null;

		public IPost? Repost { get; set; }

		public bool IsReply => Reply != null;

		public IPost? Reply { get; set; }

		public long RepostCount => 0;

		public Visibility Visiblity => Visibility.Public;

		public string? NativeVisiblity { get; set; }

		public string? Via { get; set; }

		public IPoll? Poll => null;

		public List<IAttachment> Attachments => new List<IAttachment>();

		public IUser Recipient { get; set; }

		public bool IsRead => false;

		public Post(IUser user, IUser recipent, string text)
		{
			User = user;
			Recipient = recipent;
			Text = text;
		}
	}

	public class Shell : IShell
	{
		public IUser Myself => UserStore.Citrine;

		public bool CanCreatePoll => false;

		public bool CanBlock => false;

		public bool CanMute => false;

		public bool CanFollow => false;

		public AttachmentType AttachmentType => AttachmentType.Unsupported;

		public int AttachmentMaxCount => 0;

		public Task BlockAsync(IUser user)
		{
			throw new NotSupportedException();
		}

		public Task DeleteFileAsync(IAttachment attachment)
		{
			throw new NotSupportedException();
		}

		public async Task DeletePostAsync(IPost post)
		{
			await Task.Yield();
		}

		public Task FollowAsync(IUser user)
		{
			throw new NotSupportedException();
		}

		public Task<IAttachment?> GetAttachmentAsync(string fileId)
		{
			throw new NotSupportedException();
		}

		public async Task<IPost?> GetPostAsync(string id)
		{
			return null;
		}

		public async Task<IUser?> GetUserAsync(string id)
		{
			return id == UserStore.Citrine.Id ? UserStore.Citrine : id == UserStore.You.Id ? UserStore.You : null;
		}

		public async Task<IUser?> GetUserByNameAsync(string name)
		{
			name = name.ToLowerInvariant();
			return name == UserStore.Citrine.Name ? UserStore.Citrine : name == UserStore.You.Name ? UserStore.You : null;
		}

		public async Task LikeAsync(IPost post)
		{
			WriteLine($"{Myself.ScreenName} がいいねしました");
		}

		public Task MuteAsync(IUser user)
		{
			throw new NotSupportedException();
		}

		public async Task<IPost?> PostAsync(string? text, string? cw = null, Visibility visiblity = Visibility.Default, List<string>? choices = null, List<IAttachment>? attachments = null)
		{
			WriteLine($"{Myself.ScreenName}: {text}");
			return new Post(Myself, UserStore.You, text ?? "");
		}

		public Task<IPost?> PostWithFilesAsync(string? text, string? cw = null, Visibility visiblity = Visibility.Default, List<string>? choices = null, params string[] filePaths)
		{
			throw new NotSupportedException();
		}

		public async Task<IPost?> ReplyAsync(IPost post, string? text, string? cw = null, Visibility visiblity = Visibility.Default, List<string>? choices = null, List<IAttachment>? attachments = null)
		{
			WriteLine($"{Myself.ScreenName} » {post.User.ScreenName}: {text}");
			return new Post(Myself, UserStore.You, text ?? "")
			{
				Reply = post
			};
		}

		public Task<IPost?> ReplyWithFilesAsync(IPost post, string? text, string? cw = null, Visibility visiblity = Visibility.Default, List<string>? choices = null, List<string>? filePaths = null)
		{
			throw new NotSupportedException();
		}

		public async Task ReactAsync(IPost post, string reactionChar)
		{
			WriteLine($"{Myself.ScreenName} がリアクション: {reactionChar}");
		}

		public async Task<IPost?> RepostAsync(IPost post, string? text = null, string? cw = null, Visibility visiblity = Visibility.Default)
		{
			WriteLine($"{Myself.ScreenName} RP: {post.User.ScreenName}: {post.Text}");

			return new Post(Myself, UserStore.You, "")
			{
				Repost = post,
			};
		}

		public async Task<IPost?> SendDirectMessageAsync(IUser user, string text)
		{
			WriteLine($"Message @{Myself.Name}: {text}");
			return new Post(Myself, UserStore.You, "")
			{
				Text = text,
			};
		}

		public Task UnblockAsync(IUser user)
		{
			throw new NotSupportedException();
		}

		public Task UnfollowAsync(IUser user)
		{
			throw new NotSupportedException();
		}

		public async Task UnlikeAsync(IPost post)
		{
			// do nothing
		}

		public Task UnmuteAsync(IUser user)
		{
			throw new NotSupportedException();
		}

		public Task<IAttachment?> UploadAsync(string path, string? name)
		{
			throw new NotSupportedException();
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
