#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
#pragma warning disable CS4014 // この呼び出しは待機されなかったため、現在のメソッドの実行は呼び出しの完了を待たずに続行されます

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Citrine.Core;
using Citrine.Core.Api;
using Citrine.Core.Modules;
using Disboard.Mastodon;
using Disboard.Mastodon.Enums;
using Disboard.Mastodon.Models;
using Disboard.Mastodon.Models.Streaming;
using Newtonsoft.Json;

namespace Citrine.Mastodon
{
	using static Console;
	public class Shell : IShell
	{
		public static string Version => "2.0.0";

		public IUser Myself { get; private set; }

		public bool CanCreatePoll => false;

		public MastodonClient Mastodon { get; private set; }

		public Shell(ModuleBase[] additionalModule, MastodonClient don, Account myself)
		{
			core = new Server(this, additionalModule);
            Mastodon = don;
            Myself = new DonUser(myself);
            SubscribeStreams();
        }

		/// <summary>
		/// bot を初期化します。
		/// </summary>
		/// <returns>初期化された <see cref="Shell"/> のインスタンス。</returns>
		public static async Task<Shell> InitializeAsync(params ModuleBase[] additionalModule)
		{
			MastodonClient don;
			try
			{
				var cred = File.ReadAllText("./token");
				don = new MastodonClient(JsonConvert.DeserializeObject<Disboard.Models.Credential>(cred));
				WriteLine("Mastodon に接続しました。");
			}
			catch (Exception ex)
			{
				WriteLine($"認証中にエラーが発生しました {ex.GetType().Name} {ex.Message}\n{ex.StackTrace}");
				Write("Mastodon URL> ");
				var domain = ReadLine();
				don = new MastodonClient(domain);
				await AuthorizeAsync(don);
			}

			var myself = await don.Account.VerifyCredentialsAsync();

			WriteLine($"bot ユーザーを取得しました");

            var sh = new Shell(additionalModule, don, myself);
			return sh;
		}

		public void AddModule(ModuleBase mod)
		{
			core.AddModule(mod);
		}

		public async Task<IPost> GetPostAsync(string id)
		{
			return new DonPost(await Mastodon.Statuses.ShowAsync(long.Parse(id)), this);
		}

		public async Task<IUser> GetUserAsync(string id)
		{
			return new DonUser(await Mastodon.Account.ShowAsync(long.Parse(id)));
		}

		public async Task<IUser> GetUserByNameAsync(string name)
		{
			var list = await Mastodon.Account.SearchAsync(name, 1);
			return list.FirstOrDefault() is Account acct ? new DonUser(acct) : default;
		}

		public async Task LikeAsync(IPost post)
		{
			Mastodon.Statuses.FavouriteAsync(long.Parse(post.Id));
		}

		public async Task<IPost> PostAsync(string text, string cw = null, Visiblity visiblity = Visiblity.Default)
		{
			return new DonPost(await Mastodon.Statuses.UpdateAsync(text, null, null, cw != null, cw, MapVisibility(visiblity, Visiblity.Public)), this);
		}

		public Task ReactAsync(IPost post, string reactionChar)
		{
			return LikeAsync(post);
		}

		public async Task<IPost> ReplyAsync(IPost post, string text, string cw = null, Visiblity visiblity = Visiblity.Default)
		{
			return new DonPost(await Mastodon.Statuses.UpdateAsync($"@{post.User.Name} {text}", long.Parse(post.Id), null, cw != null, cw, MapVisibility(visiblity, post.Visiblity)), this);
		}

		public async Task<IPost> RepostAsync(IPost post, string text = null, string cw = null, Visiblity visiblity = Visiblity.Default)
		{
			var reposted = new DonPost(await Mastodon.Statuses.ReblogAsync(long.Parse(post.Id)), this);
			await Task.Delay(750);
			if (text != null)
			{
				PostAsync($"RT> {text}", cw, visiblity);
			}
			return reposted;
		}

		public async Task<IPost> SendDirectMessageAsync(IUser user, string text)
		{
			return await PostAsync($"@{user.Name} {text}", null, Visiblity.Direct);
		}

		public async Task UnlikeAsync(IPost post)
		{
			await Mastodon.Statuses.UnfavouriteAsync(long.Parse(post.Id));
		}

		public Task VoteAsync(IPost post, int choice)
		{
			// Unsupported
			return Task.Delay(1);
		}

		private VisibilityType MapVisibility(Visiblity vis, Visiblity vis2)
		{
			switch (vis)
			{
				case Visiblity.Public:
					return VisibilityType.Public;
				case Visiblity.Private:
					return VisibilityType.Private;
				case Visiblity.Limited:
					return VisibilityType.Unlisted;
				case Visiblity.Direct:
					return VisibilityType.Direct;
				case Visiblity.Default:
				default:
					return MapVisibility(vis2, Visiblity.Public);
			}
		}

		private void SubscribeStreams()
		{
			var main = Mastodon.Streaming.UserAsObservable();

			// 再接続時にいらないストリームを切断
			followed?.Dispose();
			reply?.Dispose();
			tl?.Dispose();

			// フォロバ
			followed = main.OfType<NotificationMessage>()
				.Where(notif => notif.Type == NotificationType.Follow)
				.Delay(new TimeSpan(0, 0, 5))
				.Subscribe((n) => Mastodon.Account.FollowAsync(n.Account.Id));
			WriteLine("フォロー監視開始");

			// リプライ
			reply = main.OfType<NotificationMessage>()
				.Where(notif => notif.Type == NotificationType.Mention)
				.Delay(new TimeSpan(0, 0, 1))
				.Subscribe((mes) => core.HandleMentionAsync(new DonPost(mes.Status, this)));
			WriteLine("リプライ監視開始");

			// Timeline
			tl = Mastodon.Streaming.LocalPublicAsObservable(false).Merge(Mastodon.Streaming.UserAsObservable())
				.OfType<StatusMessage>()
				.DistinctUntilChanged(n => n.Id)
				.Delay(new TimeSpan(0, 0, 1))
				.Subscribe((mes) => core.HandleTimelineAsync(new DonPost(mes, this)));
			WriteLine("タイムライン監視開始");
		}

		private static async Task AuthorizeAsync(MastodonClient don)
		{
			var redirect = "urn:ietf:wg:oauth:2.0:oob";
			var scope = AccessScope.Read | AccessScope.Write | AccessScope.Follow;
			var app = await don.Apps.RegisterAsync("Citrine for Mastodon", redirect, scope);

			var url = don.Auth.AuthorizeUrl(redirect, scope);
			try
			{
				Server.OpenUrl(url);
			}
			catch (Exception)
			{
				WriteLine("ユーザー認証のためのURLを開くことができませんでした。以下のURLにアクセスして認証を進めてください。");
				WriteLine("> " + url);
			}

			WriteLine("ユーザー認証を行います。ウェブブラウザ上で認証が終わったら、コンソールにコードを入力してください。");

			var code = ReadLine();

			await don.Auth.AccessTokenAsync(redirect, code);
			var credential = JsonConvert.SerializeObject(don.Credential);

			File.WriteAllText("./token", credential);
		}

		private IDisposable followed, reply, tl;
		private Server core;
	}
}
