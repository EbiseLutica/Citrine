#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
#pragma warning disable CS4014 // この呼び出しは待機されなかったため、現在のメソッドの実行は呼び出しの完了を待たずに続行されます

using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Citrine.Core;
using File = System.IO.File;
using Disboard.Misskey;
using Disboard.Misskey.Enums;
using Disboard.Misskey.Models;
using Disboard.Misskey.Models.Streaming;
using Newtonsoft.Json;
using Citrine.Core.Api;
using System.Linq;
using Disboard.Misskey.Extensions;

namespace Citrine.Misskey
{
	public class Shell : IShell
	{
		public static string Version => "1.1.2";

		MisskeyClient misskey;

		Server core;

		public bool CanFollow => true;

		public IUser Myself { get; private set; }

		private IDisposable followed, reply, tl, dm;


		private void InitializeBot()
		{
			var main = misskey.Streaming.MainAsObservable();

			// 再接続時にいらないストリームを切断
			followed?.Dispose();
			reply?.Dispose();
			tl?.Dispose();
			dm?.Dispose();

			// フォロバ
			followed = main.OfType<FollowedMessage>()
				.Delay(new TimeSpan(0, 0, 5))
				.Subscribe((mes) => misskey.Following.CreateAsync(mes.Id), (e) => InitializeBot());
			Console.WriteLine("フォロー監視開始");

			// リプライ
			reply = main.OfType<MentionMessage>()
				.Delay(new TimeSpan(0, 0, 1))
				.Subscribe((mes) => core.HandleMentionAsync(new MiPost(mes), this));
			Console.WriteLine("リプライ監視開始");
		
			// Timeline
			tl = misskey.Streaming.HomeTimelineAsObservable().Merge(misskey.Streaming.LocalTimelineAsObservable())
				.OfType<NoteMessage>()
				.DistinctUntilChanged(n => n.Id)
				.Delay(new TimeSpan(0, 0, 1))
				.Subscribe((mes) => core.HandleTimelineAsync(new MiPost(mes), this));
			Console.WriteLine("タイムライン監視開始");

			// Direct Message
			dm = main.OfType<MessagingMessage>()
				.Delay(new TimeSpan(0, 0, 1))
				.Subscribe(async (mes) => 
				{
					if (mes.UserId == Myself.Id)
						return;
					await misskey.Messaging.Messages.ReadAsync(mes.Id);
					await core.HandleDmAsync(new MiDmPost(mes), this);
				});
			Console.WriteLine("トーク監視開始");
		}

		/// <summary>
		/// bot を初期化します。
		/// </summary>
		/// <returns>初期化された <see cref="Shell"/> のインスタンス。</returns>
		public static async Task<Shell> InitializeAsync()
		{
			MisskeyClient mi;
			try
			{
				var cred = File.ReadAllText("./token");
				mi = new MisskeyClient(JsonConvert.DeserializeObject<Disboard.Models.Credential>(cred));
				Console.WriteLine("Misskey に接続しました。");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"認証中にエラーが発生しました {ex.GetType().Name} {ex.Message}\n{ex.StackTrace}");
				Console.Write("Misskey URL> ");
				var domain = Console.ReadLine();
				mi = new MisskeyClient(domain);
				await AuthorizeAsync(mi);
			}


			var myself = await mi.IAsync();
			await mi.Streaming.ConnectAsync();

			Console.WriteLine($"bot ユーザーを取得しました");

			var sh = new Shell
			{
				core = new Server(),
				misskey = mi,
				Myself = new MiUser(myself),
			};

			sh.InitializeBot();
			return sh;
		}

		private static async Task AuthorizeAsync(MisskeyClient mi)
		{
			var app = await mi.App.CreateAsync("Citrine", "Citrine", ((Permission[])Enum.GetValues(typeof(Permission))).Select(p => p.ToStr()).ToArray(), "http://xeltica.work");

			var session = await mi.Auth.Session.GenerateAsync();


			string url = session.Url;

			// from https://brockallen.com/2016/09/24/process-start-for-urls-on-net-core/
			// hack because of this: https://github.com/dotnet/corefx/issues/10361
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				url = url.Replace("&", "^&");
				Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				Process.Start("xdg-open", url);
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				Process.Start("open", url);
			}
			else
			{
				throw new NotSupportedException("このプラットフォームはサポートされていません。");
			}

			Console.WriteLine("ユーザー認証を行います。ウェブブラウザ上で認証が終わったら、コンソールで何かキーを押してください。");

			Console.ReadLine();

			await mi.Auth.Session.UserKeyAsync(session.Token);
			var credential = JsonConvert.SerializeObject(mi.Credential);

			File.WriteAllText("./token", credential);
		}

		public async Task<IPost> ReplyAsync(IPost post, string text, string cw = null, Visiblity visiblity = Visiblity.Default)
		{
			if (post is MiDmPost dm)
			{
				return new MiDmPost(await misskey.Messaging.Messages.CreateAsync(post.User.Id, $"{(cw != default ? "**" + cw + "**\n\n" : "")}{text}"));
			}
			else
			{
				return new MiPost(await misskey.Notes.CreateAsync(text, MapVisiblity(post, visiblity), cw: cw, replyId: post.Id));
			}
		}

		public async Task<IPost> PostAsync(string text, string cw = null, Visiblity visiblity = Visiblity.Default)
		{
			return new MiPost(await misskey.Notes.CreateAsync(text, visiblity.ToStr(), cw: cw));
		}

		public async Task ReactAsync(IPost post, string reactionChar)
		{
			if (post is MiDmPost) throw new NotSupportedException("You cannot react DM message.");
			await misskey.Notes.Reactions.CreateAsync(post.Id, ConvertReaction(reactionChar));
		}

		public async Task<IPost> RepostAsync(IPost post, string text = null, string cw = null, Visiblity visiblity = Visiblity.Default)
		{
			if (post is MiDmPost) throw new NotSupportedException("You cannot react DM message.");
			return new MiPost(await misskey.Notes.CreateAsync(text, MapVisiblity(post, visiblity), cw: cw, renoteId: post.Id));
		}

		public async Task<IPost> SendDirectMessageAsync(IUser user, string text)
		{
			return new MiDmPost(await misskey.Messaging.Messages.CreateAsync(user.Id, text));
		}

		public async Task VoteAsync(IPost post, int choice)
		{
			await misskey.Notes.Polls.VoteAsync(post.Id, choice);
		}

		public string MapVisiblity(IPost post, Visiblity v)
		{
			return (v == Visiblity.Default ? post.Visiblity : v).ToStr();
		}

		public static Reaction ConvertReaction(string reactionChar)
		{
			switch (reactionChar)
			{
				case "👍":
					return Reaction.Like;
				case "❤️":
					return Reaction.Love;
				case "😆":
					return Reaction.Laugh;
				case "🤔":
					return Reaction.Hmm;
				case "😮":
					return Reaction.Surprise;
				case "🎉":
					return Reaction.Congrats;
				case "💢":
					return Reaction.Angry;
				case "😥":
					return Reaction.Confused;
				case "😇":
					return Reaction.Rip;
				case "🍮":
				// プリンより寿司が好き
				case "🍣":
					return Reaction.Pudding;

				default:
					throw new ArgumentOutOfRangeException(nameof(reactionChar), reactionChar, null);
			}
		}
	}
}
