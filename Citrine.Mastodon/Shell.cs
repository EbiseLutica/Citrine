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
	using System.Collections.Generic;
	using static Console;
	public class Shell : IShell
	{
		public static string Version => "2.1.0";

		public IUser Myself { get; private set; }

		public bool CanCreatePoll => false;

		public MastodonClient Mastodon { get; private set; }

		public Server Core { get; private set; }

		public bool CanBlock => true;

		public bool CanMute => true;

		public bool CanFollow => true;

		public Core.Api.AttachmentType AttachmentType => Citrine.Core.Api.AttachmentType.BindToThePost;

		public int AttachmentMaxCount => 4;

		public Shell(MastodonClient don, Account myself, Logger logger)
		{
			Core = new Server(this);
			Mastodon = don;
			Myself = new DonUser(myself);
			this.logger = logger;
			SubscribeStreams();
		}

		/// <summary>
		/// bot を初期化します。
		/// </summary>
		/// <returns>初期化された <see cref="Shell"/> のインスタンス。</returns>
		public static async Task<Shell> InitializeAsync()
		{
			MastodonClient don;
			var logger = new Logger();
			try
			{
				var cred = File.ReadAllText("./token");
				don = new MastodonClient(JsonConvert.DeserializeObject<Disboard.Models.Credential>(cred));
				logger.Info("Mastodon に接続しました。");
			}
			catch (Exception ex)
			{
				logger.Error($"認証中にエラーが発生しました {ex.GetType().Name} {ex.Message}\n{ex.StackTrace}");
				Write("Mastodon URL> ");
				var domain = ReadLine();
				don = new MastodonClient(domain);
				await AuthorizeAsync(don, logger);
			}

			var myself = await don.Account.VerifyCredentialsAsync();

			logger.Info($"bot ユーザーを取得しました (@{myself.Username}");

			var sh = new Shell(don, myself, logger);
			return sh;
		}

		public async Task<IPost?> GetPostAsync(string id)
		{
			return new DonPost(await Mastodon.Statuses.ShowAsync(long.Parse(id)), this);
		}

		public async Task<IUser?> GetUserAsync(string id)
		{
			return new DonUser(await Mastodon.Account.ShowAsync(long.Parse(id)));
		}

		public async Task<IUser?> GetUserByNameAsync(string name)
		{
			var list = await Mastodon.Account.SearchAsync(name, 1);
			return list.FirstOrDefault() is Account acct ? new DonUser(acct) : default;
		}

		public async Task LikeAsync(IPost post)
		{
			Mastodon.Statuses.FavouriteAsync(long.Parse(post.Id));
		}

		public async Task<IPost?> PostAsync(string? text, string? cw = null, Visibility visiblity = Visibility.Default, List<string>? choices = null, List<IAttachment>? attachments = null)
		{
			if (text?.Length > 500)
				text = text.Substring(500);
			return new DonPost(await Mastodon.Statuses.UpdateAsync(text, null, attachments?.Select(a => a.Id.ToLong()).ToList(), cw != null, cw, MapVisibility(visiblity, Visibility.Public)), this);
		}

		public async Task<IPost?> PostWithFilesAsync(string? text, string? cw = null, Visibility visiblity = Visibility.Default, List<string>? choices = null, params string[] filePaths)
		{
			if (filePaths == null)
				return null;
			var attachments = (await Task.WhenAll(filePaths.Select(path => UploadAsync(path)))).ToList();
			return await PostAsync(text, cw, visiblity, choices, attachments);
		}

		public async Task<IPost?> ReplyAsync(IPost post, string? text, string? cw = null, Visibility visiblity = Visibility.Default, List<string>? choices = null, List<IAttachment>? attachments = null)
		{
			if (text?.Length > 500)
				text = text.Substring(500);
			return new DonPost(await Mastodon.Statuses.UpdateAsync($"@{post.User.Name}{(!string.IsNullOrEmpty(post.User.Host) ? "@" + post.User.Host : "")} {text}", long.Parse(post.Id), attachments?.Select(a => long.Parse(a.Id)).ToList(), cw != null, cw, MapVisibility(visiblity, post.Visiblity)), this);
		}

		public async Task<IPost?> ReplyWithFilesAsync(IPost post, string? text, string? cw = null, Visibility visiblity = Visibility.Default, List<string>? choices = null, List<string>? filePaths = null)
		{
			if (filePaths == null)
				return null;
			var attachments = (await Task.WhenAll(filePaths.Select(path => UploadAsync(path)))).ToList();
			return await ReplyAsync(post, text, cw, visiblity, choices, attachments);
		}

		public Task ReactAsync(IPost post, string reactionChar)
		{
			return LikeAsync(post);
		}

		public async Task<IPost?> RepostAsync(IPost post, string? text = null, string? cw = null, Visibility visiblity = Visibility.Default)
		{
			var reposted = new DonPost(await Mastodon.Statuses.ReblogAsync(long.Parse(post.Id)), this);
			await Task.Delay(750);
			if (text != null)
			{
				PostAsync($"RT> {text}", cw, visiblity);
			}
			return reposted;
		}

		public async Task<IPost?> SendDirectMessageAsync(IUser user, string text)
		{
			return await PostAsync($"@{user.Name} {text}", null, Visibility.Direct);
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

		public async Task<IAttachment?> UploadAsync(string path, string? name = null)
		{
			return new DonAttachment(await Mastodon.Media.CreateAsync(path));
		}

		public Task DeleteFileAsync(IAttachment attachment)
		{
			throw new NotSupportedException();
		}

		public async Task FollowAsync(IUser user)
		{
			await Mastodon.Account.FollowAsync(user.Id.ToLong());
		}

		public async Task UnfollowAsync(IUser user)
		{
			await Mastodon.Account.UnfollowAsync(user.Id.ToLong());
		}

		public async Task BlockAsync(IUser user)
		{
			await Mastodon.Account.BlockAsync(user.Id.ToLong());
		}

		public async Task UnblockAsync(IUser user)
		{
			await Mastodon.Account.UnblockAsync(user.Id.ToLong());
		}

		public async Task MuteAsync(IUser user)
		{
			await Mastodon.Account.MuteAsync(user.Id.ToLong());
		}

		public async Task UnmuteAsync(IUser user)
		{
			await Mastodon.Account.UnmuteAsync(user.Id.ToLong());
		}

		public async Task DeletePostAsync(IPost post)
		{
			await Mastodon.Statuses.DestroyAsync(post.Id.ToLong());
		}

		public async Task<IAttachment?> GetAttachmentAsync(string fileId)
		{
			// Mastodon has no Media Showing API
			throw new NotSupportedException();
		}

		private VisibilityType MapVisibility(Visibility vis, Visibility vis2)
		{
			switch (vis)
			{
				case Visibility.Public:
					return VisibilityType.Public;
				case Visibility.Private:
					return VisibilityType.Private;
				case Visibility.Limited:
					return VisibilityType.Unlisted;
				case Visibility.Direct:
					return VisibilityType.Direct;
				case Visibility.Default:
				default:
					return MapVisibility(vis2, Visibility.Public);
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
				.Delay(new TimeSpan(0, 0, 1))
				.Subscribe((n) => Core.HandleFollowedAsync(new DonUser(n.Account)));
			logger.Info("フォロー監視開始");

			// リプライ
			reply = main.OfType<NotificationMessage>()
				.Where(notif => notif.Type == NotificationType.Mention)
				.Delay(new TimeSpan(0, 0, 1))
				.Subscribe((mes) => Core.HandleMentionAsync(new DonPost(mes.Status, this)));
			logger.Info("リプライ監視開始");

			// Timeline
			tl = Mastodon.Streaming.LocalPublicAsObservable(false).Merge(Mastodon.Streaming.UserAsObservable())
				.OfType<StatusMessage>()
				.DistinctUntilChanged(n => n.Id)
				.Delay(new TimeSpan(0, 0, 1))
				.Subscribe((mes) => Core.HandleTimelineAsync(new DonPost(mes, this)));
			logger.Info("タイムライン監視開始");
		}

		private static async Task AuthorizeAsync(MastodonClient don, Logger logger)
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
				logger.Error("ユーザー認証のためのURLを開くことができませんでした。以下のURLにアクセスして認証を進めてください。");
				logger.Error("> " + url);
			}

			logger.Info("ユーザー認証を行います。ウェブブラウザ上で認証が終わったら、コンソールにコードを入力してください。");
			Console.Write("> ");
			var code = ReadLine();

			await don.Auth.AccessTokenAsync(redirect, code);
			var credential = JsonConvert.SerializeObject(don.Credential);

			File.WriteAllText("./token", credential);
		}

		private IDisposable? followed, reply, tl;
		private Logger logger;
	}

	public static class ConvertHelper
	{
		public static long ToLong(this string s) => long.Parse(s);
	}
}
