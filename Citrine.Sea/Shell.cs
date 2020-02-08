using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IOFile = System.IO.File;
using Citrine.Core;
using Citrine.Core.Api;
using static System.Console;
using Newtonsoft.Json;
using System.Net.Http;
using System.Linq;
using System.Web;
using System.Runtime.CompilerServices;
using WebSocket4Net;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Citrine.Sea
{
	public class Shell : IShell
	{
		public static string Version => "1.0.0";

		public IUser? Myself { get; private set; }

		public Server? Core { get; private set; }

		public Logger Logger { get; private set; }

		public HttpClient Http { get; }
		public bool CanCreatePoll => false;

		public bool CanBlock => false;

		public bool CanMute => false;

		public bool CanFollow => false;

		public AttachmentType AttachmentType => AttachmentType.UploadAndAttach;

		public int AttachmentMaxCount => int.MaxValue;

		public Credential Credential { get; private set; }

		public WebSocket? WSClient { get; private set; }

		public Shell(Credential credential, Logger logger)
		{
			Logger = logger;
			Core = new Server(this);
			Credential = credential;
			Http = new HttpClient();
			Http.DefaultRequestHeaders.Add("User-Agent", Server.Http.DefaultRequestHeaders.GetValues("User-Agent").FirstOrDefault());
			Http.DefaultRequestHeaders.Add("Authorization", $"Bearer {Credential.AccessToken}");
		}

		public static async Task<Shell> InitializeAsync()
		{
			var logger = new Logger(nameof(Shell));
			Credential credential;
			try
			{
				credential = JsonConvert.DeserializeObject<Credential>(IOFile.ReadAllText("./token"));
				logger.Info("Sea に接続しました。");
			}
			catch (Exception)
			{
				logger.Error("認証に失敗しました。セットアップを開始します。");
				Write("Sea URL を入力してください。>"); var url = ReadLine();
				if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
				{
					logger.Error("URL が正しくありません。http または https から始めてください。");
					Environment.Exit(-1);
					return null;
				}
				if (url.EndsWith('/')) url = url.Remove(url.Length - 1);

				Write("アクセストークンを入力してください。アクセストークンはウェブサイトから発行できます。>"); var accessToken = ReadLine();
				credential = new Credential
				{
					AccessToken = accessToken,
					Uri = url,
				};
				IOFile.WriteAllText("./token", JsonConvert.SerializeObject(credential));
			}

			var s = new Shell(credential, logger);
			var user = await s.GetAccountAsync();

			s.Myself = new CUser(user, true);

			s.Core = new Server(s);

			await s.SubscribeStreamsAsync();
			return s;
		}

		public async Task SubscribeStreamsAsync()
		{
			var uri = Credential.Uri;
			// スキームの置き換え
			if (uri.StartsWith("http")) uri = "ws" + uri.Substring(4);
			// 末尾に /api を付加
			uri = (uri.EndsWith("/") ? uri : uri + "/") + "api";
			Logger.Info($"'{uri}' に接続します。");

			WSClient = new WebSocket(uri);

			WSClient.Opened += (s, e) => Logger.Info("Streaming API に接続しました。");
			WSClient.Error += async (s, e) =>
			{
				Logger.Error($"ストリーミング エラー {e.Exception.GetType().Name}: {e.Exception.Message}");
				Logger.Error("再接続します。");
				await WSClient.CloseAsync();
				await WSClient.OpenAsync();
			};
			WSClient.MessageReceived += HandleMessage;

			WSClient.EnableAutoSendPing = true;
			await WSClient.OpenAsync();

			WSClient.Send(JsonConvert.SerializeObject(new
			{
				stream = "v1/timelines/public",
				token = Credential.AccessToken,
				type = "connect",
			}));
		}

		public async void HandleMessage(object? sender, MessageReceivedEventArgs e)
		{
			var obj = JObject.Parse(e.Message);
			var type = obj.Value<string>("type");
			switch (type)
			{
				case "message":
					// 遊び時間
					await Task.Delay(1000);

					var post = new CPost(obj["content"].ToObject<Post>());
					await Core!.HandleTimelineAsync(post);

					if (post.Text != null && post.Text.ToLowerInvariant().Contains($"@{Myself!.Name.ToLowerInvariant()}"))
					{
						await Core!.HandleMentionAsync(post);
					}
					break;
				case "success":
					Logger.Info("リクエストは成功しました。");
					break;
				default:
					Logger.Info($"unknown message type {type}");
					break;
			}
		}

		#region Sea API

		public async Task<User> GetAccountAsync()
		{
			return await GetAsync<User>("account");
		}

		public async Task<T> GetAsync<T>(string endPoint, string? query = null)
		{
			if (endPoint.StartsWith('/')) endPoint = endPoint.Substring(1);
			if (endPoint.EndsWith('/')) endPoint = endPoint.Remove(endPoint.Length - 1);
			var uri = new Uri(new Uri(Credential.Uri), $"/api/v1/{endPoint}/{HttpUtility.UrlEncode(query)}");
			var json = await Http.GetStringAsync(uri);
			return JsonConvert.DeserializeObject<T>(json);
		}

		public async Task<T> PostAsync<T>(string endPoint, HttpContent? content = null)
		{
			if (endPoint.StartsWith('/')) endPoint = endPoint.Substring(1);
			if (endPoint.EndsWith('/')) endPoint = endPoint.Remove(endPoint.Length - 1);

			var res = await Http.PostAsync($"{Credential.Uri}/api/v1/{endPoint}", content).Stay();
			var json = await res.Content.ReadAsStringAsync().Stay();
			return JsonConvert.DeserializeObject<T>(json);
		}

		public async Task<T> PatchAsync<T>(string endPoint, HttpContent content)
		{
			if (endPoint.StartsWith('/')) endPoint = endPoint.Substring(1);
			if (endPoint.EndsWith('/')) endPoint = endPoint.Remove(endPoint.Length - 1);

			var res = await Http.PostAsync($"{Credential.Uri}/api/v1/{endPoint}", content).Stay();
			var json = await res.Content.ReadAsStringAsync().Stay();
			return JsonConvert.DeserializeObject<T>(json);
		}

		#endregion

		#region Citrine API
		public Task<IAttachment?> GetAttachmentAsync(string fileId)
		{
			throw new NotImplementedException();
		}

		public Task<IPost?> GetPostAsync(string id)
		{
			throw new NotImplementedException();
		}

		public Task LikeAsync(IPost post)
		{
			return ReactAsync(post, "❤️");
		}

		public async Task<IPost?> PostAsync(string? text, string? cw = null, Visibility visiblity = Visibility.Default, List<string>? choices = null, List<IAttachment>? attachments = null)
		{
			var payload = JsonConvert.SerializeObject(new
			{
				fileIds = new int[0],
				text = cw == null ? text : $"{cw}\n\n{text}",
			});
			return new CPost(await PostAsync<Post>("posts", new StringContent(payload, Encoding.UTF8, "application/json")));
		}

		public Task<IPost?> PostWithFilesAsync(string? text, string? cw = null, Visibility visiblity = Visibility.Default, List<string>? choices = null, params string[] filePaths)
		{
			throw new NotImplementedException();
		}

		public Task ReactAsync(IPost post, string reactionChar)
		{
			return ReplyAsync(post, reactionChar);
		}

		public Task<IPost?> ReplyAsync(IPost post, string? text, string? cw = null, Visibility visiblity = Visibility.Default, List<string>? choices = null, List<IAttachment>? attachments = null)
		{
			return PostAsync($"@{post.User.Name} {text}", cw, visiblity, choices, attachments);
		}

		public Task<IPost?> ReplyWithFilesAsync(IPost post, string? text, string? cw = null, Visibility visiblity = Visibility.Default, List<string>? choices = null, List<string>? filePaths = null)
		{
			throw new NotImplementedException();
		}

		public Task<IPost?> RepostAsync(IPost post, string? text = null, string? cw = null, Visibility visiblity = Visibility.Default)
		{
			return PostAsync($"RP @{post.User.Name}: {post.Text}", cw, visiblity);
		}

		public Task<IAttachment?> UploadAsync(string path, string? name)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region Not supported APIs
		public Task MuteAsync(IUser user) => throw new NotSupportedException();
		public Task UnmuteAsync(IUser user) => throw new NotSupportedException();
		public Task BlockAsync(IUser user) => throw new NotSupportedException();
		public Task UnblockAsync(IUser user) => throw new NotSupportedException();
		public Task FollowAsync(IUser user) => throw new NotSupportedException();
		public Task UnfollowAsync(IUser user) => throw new NotSupportedException();
		public Task DeletePostAsync(IPost post) => throw new NotSupportedException();
		public Task VoteAsync(IPost post, int choice) => throw new NotSupportedException();
		public Task DeleteFileAsync(IAttachment attachment) => throw new NotSupportedException();
		public Task<IPost?> SendDirectMessageAsync(IUser user, string text) => throw new NotSupportedException();
		public Task<IUser?> GetUserAsync(string id) => throw new NotSupportedException();
		public Task<IUser?> GetUserByNameAsync(string name) => throw new NotSupportedException();
		public async Task UnlikeAsync(IPost post) => await Task.Yield();
		#endregion
	}

	public class Credential
	{
		public string AccessToken { get; set; } = "";
		public string Uri { get; set; } = "";
	}

	public static class AsyncExtension
	{
		public static ConfiguredTaskAwaitable Stay(this Task t) => t.ConfigureAwait(false);
		public static ConfiguredTaskAwaitable<T> Stay<T>(this Task<T> t) => t.ConfigureAwait(false);
	}
}
