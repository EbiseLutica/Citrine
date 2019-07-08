#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
#pragma warning disable CS4014 // この呼び出しは待機されなかったため、現在のメソッドの実行は呼び出しの完了を待たずに続行されます

using System;
using System.Threading.Tasks;
using Citrine.Core.Api;
using Discord.WebSocket;
using Discord;
using ICUser = Citrine.Core.Api.IUser;
using System.Linq;
using System.Text.RegularExpressions;
using Citrine.Core;
using System.IO;
using System.Collections.Generic;

namespace Citrine.Discord
{
	public class Shell : IShell
	{
		public static string Version => "1.0.0";

		public ICUser Myself { get; private set; }

		public Server Core { get; }

		public DiscordSocketClient Client { get; private set; }

		public IMessageChannel CurrentChannel { get; private set; }

		public bool CanCreatePoll => false;

		public bool CanBlock => false;

		public bool CanMute => false;

		public bool CanFollow => false;

		public AttachmentType AttachmentType => Citrine.Core.Api.AttachmentType.BindToThePost;

		public int AttachmentMaxCount => 1;

		private Shell()
		{
			Client = new DiscordSocketClient();
			Core = new Server(this);
		}

		~Shell()
		{
			Client?.StopAsync();
		}

		public static async Task<Shell> InitializeAsync()
		{
			var sh = new Shell();

			string token;
			if (File.Exists("./token"))
			{
				token = await File.ReadAllTextAsync("./token");
			}
			else
			{
				Console.Write("Bot Token> ");
				token = Console.ReadLine();
				await File.WriteAllTextAsync("./token", token);
			}

			await sh.Client.LoginAsync(TokenType.Bot, token);
			await sh.Client.StartAsync();


			sh.Client.MessageReceived += sh.HandleMessageAsync;
			sh.Client.CurrentUserUpdated += async (_, __) => sh.Myself = new DCUser(sh.Client.CurrentUser);
			return sh;
		}

		public Task<IPost> GetPostAsync(string id)
		{
			return null;
		}

		public async Task<ICUser> GetUserAsync(string id)
		{
			var splitted = id.Split('#');
			var (name, discriminator) = (splitted[0], splitted[1]);
			return new DCUser(Client.GetUser(name, discriminator));
		}

		public Task<ICUser> GetUserByNameAsync(string name)
		{
			return null;
		}

		public Task LikeAsync(IPost post)
		{
			return ReactAsync(post, "⭐️");
		}

		public async Task<IPost> PostAsync(string text, string cw = null, Visiblity visiblity = Visiblity.Default, List<string> choices = null, List<Core.Api.IAttachment> attachments = null)
		{
			return new DCPost(await PostAsync(text, cw, attachments));
		}

		public async Task<IPost> ReplyAsync(IPost post, string text, string cw = null, Visiblity visiblity = Visiblity.Default, List<string> choices = null, List<Core.Api.IAttachment> attachments = null)
		{
			if (string.IsNullOrEmpty(text))
				return null;
			var mention = (post.User as DCUser).Native.Mention;
			return new DCPost(await PostAsync($"{text}", cw, attachments), post);
		}

		public async Task ReactAsync(IPost post, string reactionChar)
		{
			if ((post as DCPost).Native is IUserMessage um)
			{
				Match m = Regex.Match(reactionChar, "^:(.+):$");
				var guild = (um.Channel as IGuildChannel)?.Guild;

				// 鯖でない場所に、カスタム絵文字は存在しない
				if (m.Success && guild == null)
					return;
				// カスタム絵文字とそうでない場合で場合分けが必要らしい
				var emote = m.Success ? (IEmote)guild.Emotes.FirstOrDefault(e => e.Name == m.Groups[1].Value) : new Emoji(reactionChar);
				await um.AddReactionAsync(new Emoji(reactionChar));
			}
		}

		public async Task<IPost> RepostAsync(IPost post, string text = null, string cw = null, Visiblity visiblity = Visiblity.Default)
		{
			var t = $"{text ?? ""} RP: {(post.User as DCUser).Native.Mention}\n```{post.Text ?? ""}```\n{(post as DCPost).Native.GetJumpUrl()}";
			return new DCPost(await PostAsync(t, cw, null));
		}

		public async Task<IPost> SendDirectMessageAsync(ICUser user, string text)
		{
			var ch = await (user as DCUser).Native.GetOrCreateDMChannelAsync();
			return new DCPost(await ch.SendMessageAsync(text));
		}

		public async Task UnlikeAsync(IPost post)
		{
			await (post as IUserMessage)?.RemoveAllReactionsAsync();
		}

		public async Task VoteAsync(IPost post, int choice)
		{
			// Discord has no vote feature :3
			throw new NotSupportedException();
		}

		public async Task<IPost> ReplyWithFilesAsync(IPost post, string text, string cw = null, Visiblity visiblity = Visiblity.Default, List<string> choices = null, List<string> filePaths = null)
		{
			foreach (var f in filePaths)
			{
				await UploadAsync(f);
			}
			return await ReplyAsync(post, text, cw, visiblity, choices);
		}

		public async Task<IPost> PostWithFilesAsync(string text, string cw = null, Visiblity visiblity = Visiblity.Default, List<string> choices = null, params string[] filePaths)
		{
			foreach (var f in filePaths)
			{
				await UploadAsync(f);
			}
			return await PostAsync(text, cw, visiblity, choices);
		}

		public async Task<Core.Api.IAttachment> UploadAsync(string path, string name = null)
		{
			var file = (await CurrentChannel?.SendFileAsync(path, name)).Attachments?.FirstOrDefault();
			return file == null ? null : new DCAttachment(file);
		}

		public async Task DeleteFileAsync(Core.Api.IAttachment attachment)
		{
			// Discord has no api to delete files
			throw new NotSupportedException();
		}

		public async Task FollowAsync(ICUser user)
		{
			throw new NotSupportedException();
		}

		public async Task UnfollowAsync(ICUser user)
		{
			throw new NotSupportedException();
		}

		public async Task BlockAsync(ICUser user)
		{
			throw new NotSupportedException();
		}

		public async Task UnblockAsync(ICUser user)
		{
			throw new NotSupportedException();
		}

		public async Task MuteAsync(ICUser user)
		{
			throw new NotSupportedException();
		}

		public async Task UnmuteAsync(ICUser user)
		{
			throw new NotSupportedException();
		}

		public async Task<Core.Api.IAttachment> GetAttachmentAsync(string fileId)
		{
			throw new NotSupportedException();
		}

		public async Task DeletePostAsync(IPost post)
		{
			await (post as DCPost).Native.DeleteAsync();
		}

		// || で挟むと隠し文字列として機能する
		public string Cw(string cw, string text)
		{
			return cw == null ? text : $"{cw}\n||{text}||";
		}

		private async Task HandleMessageAsync(SocketMessage arg)
		{
			CurrentChannel = arg.Channel;

			if ((Myself as DCUser)?.Native != Client.CurrentUser && Client.CurrentUser != null)
			{
				Myself = new DCUser(Client.CurrentUser);
			}
			else if (Client.CurrentUser == null)
			{
				logger.Error("CurrentUser is null");
			}

			if (arg is IDMChannel dm)
			{
				await Core.HandleDmAsync(new DCPost(arg));
			}
			else if (arg.MentionedUsers?.Any(m => m.Id == (Myself as DCUser).Native.Id) ?? false)
			{
				await Core.HandleMentionAsync(new DCPost(arg));
			}
			else
			{
				await Core.HandleTimelineAsync(new DCPost(arg));
			}
		}

		private async Task<IMessage> PostAsync(string text, string cw, List<Core.Api.IAttachment> attachments)
		{
			// Discord has no api to send file from ID
			return await CurrentChannel?.SendMessageAsync(Cw(cw, text));
		}

		private Logger logger = new Logger(nameof(Shell));
	}
}
