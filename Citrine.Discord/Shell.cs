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

		public async Task<IPost> PostAsync(string text, string cw = null, Visiblity visiblity = Visiblity.Default)
		{
			return new DCPost(await PostAsync(text, cw));
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

		public async Task<IPost> ReplyAsync(IPost post, string text, string cw = null, Visiblity visiblity = Visiblity.Default)
		{
			if (string.IsNullOrEmpty(text))
				return null;
			var mention = (post.User as DCUser).Native.Mention;
			return new DCPost(await PostAsync($"{text}", cw), post);
		}

		public async Task<IPost> RepostAsync(IPost post, string text = null, string cw = null, Visiblity visiblity = Visiblity.Default)
		{
			var t = $"{text ?? ""} RP: {(post.User as DCUser).Native.Mention}\n```{post.Text ?? ""}```\n{(post as DCPost).Native.GetJumpUrl()}";
			return new DCPost(await PostAsync(t, cw));
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

		public Task VoteAsync(IPost post, int choice)
		{
			// Discord has no vote feature :3
			throw new NotSupportedException();
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
				Console.WriteLine("CurrentUser is null");
			}

			if (arg is IDMChannel dm)
			{
				Console.Write("DM");
				await Core.HandleDmAsync(new DCPost(arg));
			}
			else if (arg.MentionedUsers?.Any(m => m.Id == (Myself as DCUser).Native.Id) ?? false)
			{
				Console.Write("MENTION");
				await Core.HandleMentionAsync(new DCPost(arg));
			}
			else
			{
				Console.Write("GROUP");
				await Core.HandleTimelineAsync(new DCPost(arg));
			}
			Console.WriteLine($": {arg.Content}");
		}

		private async Task<IMessage> PostAsync(string text, string cw)
		{
			return await CurrentChannel?.SendMessageAsync(Cw(cw, text));
		}
	}
}
