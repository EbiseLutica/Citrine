#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
#pragma warning disable CS4014 // この呼び出しは待機されなかったため、現在のメソッドの実行は呼び出しの完了を待たずに続行されます

using System;
using System.Reactive.Linq;
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
using Citrine.Core.Modules;

namespace Citrine.Misskey
{
	using System.Collections.Generic;
	using static Console;

	public class Shell : IShell
	{
		public static string Version => "2.0.0";

		public MisskeyClient Misskey { get; private set; }

		public bool CanCreatePoll => true;

		public IUser Myself { get; private set; }

		public Server Core { get; private set; }

		public bool CanBlock => throw new NotImplementedException();

		public bool CanMute => throw new NotImplementedException();

		public bool CanFollow => throw new NotImplementedException();

		public AttachmentType AttachmentType => throw new NotImplementedException();

		public int AttachmentMaxCount => throw new NotImplementedException();

		public Shell(MisskeyClient mi, User myself)
		{
			Core = new Server(this);
			Misskey = mi;
			Myself = new MiUser(myself);
			SubscribeStreams();
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
				var credential = File.ReadAllText("./token");
				mi = new MisskeyClient(JsonConvert.DeserializeObject<Disboard.Models.Credential>(credential));
				WriteLine("Misskey に接続しました。");
			}
			catch (Exception)
			{
				WriteLine($"認証に失敗しました。セットアップを開始します。");
				Write("Misskey URLを入力してください。> ");
				var host = ReadLine();
				mi = new MisskeyClient(host);
				await AuthorizeAsync(mi);
			}

			var myself = await mi.IAsync();
			WriteLine($"bot ユーザーを取得しました (@{myself.Username})");

			// 呼ばないとストリームの初期化ができないらしい
			await mi.Streaming.ConnectAsync();

			var sh = new Shell(mi, myself);
			return sh;
		}

		public async Task<IPost> ReplyAsync(IPost post, string text, string cw = null, Visiblity visiblity = Visiblity.Default, List<string> choices = null, List<IAttachment> attachments = null)
		{
			if (post is MiDmPost dm)
			{
				return new MiDmPost(await Misskey.Messaging.Messages.CreateAsync(post.User.Id, $"{(cw != default ? "**" + cw + "**\n\n" : "")}{text}"));
			}
			else
			{
				return new MiPost(await CreateNoteAsync(text, visiblity, cw, reply: post));
			}
		}

		public async Task<IPost> PostAsync(string text, string cw = null, Visiblity visiblity = Visiblity.Default, List<string> choices = null, List<IAttachment> attachments = null)
		{
			return new MiPost(await CreateNoteAsync(text, visiblity, cw, choices: choices, attachments: attachments));
		}

		public async Task ReactAsync(IPost post, string reactionChar)
		{
			if (post is MiDmPost) throw new NotSupportedException("You cannot react DM message.");
			await Misskey.Notes.Reactions.CreateAsync(post.Id, reactionChar);
		}

		public async Task<IPost> RepostAsync(IPost post, string text = null, string cw = null, Visiblity visiblity = Visiblity.Default)
		{
			if (post is MiDmPost) throw new NotSupportedException("You cannot react DM message.");
			return new MiPost(await CreateNoteAsync(text, visiblity, cw, repost: post));
		}

		public Task<Note> CreateNoteAsync(string text, Visiblity vis, string cw = null, IPost repost = null, IPost reply = null, List<string> choices = null, List<IAttachment> attachments = null)
		{
			if (cw == null && (text.Length > 140 || text.Split("\n").Length > 5))
				cw = "ながい";
			Poll poll = null;
			List<string> files = attachments?.Select(a => a.Id).ToList();
			if (choices != null)
			{
				poll = new Poll
				{
					Choices = choices.Select(c => new Choice
					{
						Text = c
					}).ToList()
				};
			}
			return Misskey.Notes.CreateAsync(text, (reply ?? repost) != null ? MapVisiblity(reply ?? repost, vis) : vis.ToStr(), null, cw, false, null, files, reply?.Id, repost?.Id, poll);
		}

		public async Task<IPost> SendDirectMessageAsync(IUser user, string text)
		{
			return new MiDmPost(await Misskey.Messaging.Messages.CreateAsync(user.Id, text));
		}

		public async Task VoteAsync(IPost post, int choice)
		{
			await Misskey.Notes.Polls.VoteAsync(post.Id, choice);
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

		public async Task<IPost> GetPostAsync(string id) => new MiPost(await Misskey.Notes.ShowAsync(id));

		public async Task<IUser> GetUserAsync(string id) => new MiUser((await Misskey.Users.ShowAsync(userId: id)).First());

		public async Task<IUser> GetUserByNameAsync(string name) => new MiUser((await Misskey.Users.ShowAsync(username: name)).First());

		public async Task LikeAsync(IPost post)
		{
			await Misskey.Notes.Reactions.CreateAsync(post.Id, Reaction.Like);
		}

		public async Task UnlikeAsync(IPost post)
		{
			await Misskey.Notes.Reactions.DeleteAsync(post.Id);
		}

		private static async Task AuthorizeAsync(MisskeyClient mi)
		{
			var app = await mi.App.CreateAsync("Citrine for Misskey", "バーチャル嫁bot", ((Permission[])Enum.GetValues(typeof(Permission))).Select(p => p.ToStr()).ToArray(), "http://xeltica.work");

			var session = await mi.Auth.Session.GenerateAsync();

			try
			{
				Server.OpenUrl(session.Url);
			}
			catch (Exception)
			{
				WriteLine("ユーザー認証のためのURLを開くことができませんでした。以下のURLにアクセスして認証を進めてください。");
				WriteLine("> " + session.Url);
			}

			WriteLine("ユーザー認証を行います。ウェブブラウザ上で認証が終わったら、コンソールで何かキーを押してください。");

			ReadLine();

			await mi.Auth.Session.UserKeyAsync(session.Token);
			var credential = JsonConvert.SerializeObject(mi.Credential);

			File.WriteAllText("./token", credential);
		}


		private void SubscribeStreams()
		{
			var main = Misskey.Streaming.MainAsObservable();

			// 再接続時にいらないストリームを切断
			followed?.Dispose();
			reply?.Dispose();
			tl?.Dispose();
			dm?.Dispose();

			// フォロバ
			followed = main.OfType<FollowedMessage>()
				.Delay(new TimeSpan(0, 0, 5))
				.Subscribe((mes) => Misskey.Following.CreateAsync(mes.Id), (e) => SubscribeStreams());
			WriteLine("フォロー監視開始");

			// リプライ
			reply = main.OfType<MentionMessage>()
				.Delay(new TimeSpan(0, 0, 1))
				.Subscribe((mes) => Core.HandleMentionAsync(new MiPost(mes)));
			WriteLine("リプライ監視開始");

			// Timeline
			tl = Misskey.Streaming.HomeTimelineAsObservable().Merge(Misskey.Streaming.LocalTimelineAsObservable())
				.OfType<NoteMessage>()
				.DistinctUntilChanged(n => n.Id)
				.Delay(new TimeSpan(0, 0, 1))
				.Subscribe((mes) => Core.HandleTimelineAsync(new MiPost(mes)));
			WriteLine("タイムライン監視開始");

			// Direct Message
			dm = main.OfType<MessagingMessage>()
				.Delay(new TimeSpan(0, 0, 1))
				.Subscribe(async (mes) =>
				{
					if (mes.UserId == Myself.Id)
						return;
					await Misskey.Messaging.Messages.ReadAsync(mes.Id);
					await Core.HandleDmAsync(new MiDmPost(mes));
				});
			WriteLine("トーク監視開始");
		}

		public Task<IPost> ReplyWithFilesAsync(IPost post, string text, string cw = null, Visiblity visiblity = Visiblity.Default, List<string> choices = null, List<string> filePaths = null)
		{
			throw new NotImplementedException();
		}

		public Task<IPost> PostWithFilesAsync(string text, string cw = null, Visiblity visiblity = Visiblity.Default, List<string> choices = null, params string[] filePaths)
		{
			throw new NotImplementedException();
		}

		public Task<IAttachment> UploadAsync(string path, string name)
		{
			throw new NotImplementedException();
		}

		public Task DeleteFileAsync(IAttachment attachment)
		{
			throw new NotImplementedException();
		}

		public Task FollowAsync(IUser user)
		{
			throw new NotImplementedException();
		}

		public Task UnfollowAsync(IUser user)
		{
			throw new NotImplementedException();
		}

		public Task BlockAsync(IUser user)
		{
			throw new NotImplementedException();
		}

		public Task UnblockAsync(IUser user)
		{
			throw new NotImplementedException();
		}

		public Task MuteAsync(IUser user)
		{
			throw new NotImplementedException();
		}

		public Task UnmuteAsync(IUser user)
		{
			throw new NotImplementedException();
		}

		public Task DeletePostAsync(IPost post)
		{
			throw new NotImplementedException();
		}

		public Task<IAttachment> GetAttachmentAsync(string fileId)
		{
			throw new NotImplementedException();
		}

		private IDisposable followed, reply, tl, dm;
	}
}
