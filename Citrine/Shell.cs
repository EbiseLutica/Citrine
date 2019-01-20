using System;
using System.Diagnostics;
using System.IO;
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

namespace Citrine.Misskey
{
	public class Shell
	{
		MisskeyClient misskey;

		Server core;

		public Shell()
		{

		}

		private void InitializeBot()
		{
			var main = misskey.Streaming.MainAsObservable();

			// フォロバ
			main.OfType<FollowedMessage>()
				.Delay(new TimeSpan(0, 0, 5))
				.Subscribe((mes) => misskey.Following.CreateAsync(mes.Id));

			// リプライ
			main.OfType<MentionMessage>()
				.Delay(new TimeSpan(0, 0, 1))
				.Subscribe(HandleMention);

			// Timeline
			misskey.Streaming.HomeTimelineAsObservable().Merge(misskey.Streaming.LocalTimelineAsObservable())
				.OfType<NoteMessage>()
				.DistinctUntilChanged(n => n.Id)
				.Delay(new TimeSpan(0, 0, 1))
				.Subscribe(HandleTimeline);
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
				var cred = System.IO.File.ReadAllText("./token");
				mi = new MisskeyClient(JsonConvert.DeserializeObject<Disboard.Models.Credential>(cred));
			}
			catch (Exception ex)
			{
				Console.WriteLine($"認証中にエラーが発生しました {ex.GetType().Name} {ex.Message}\n{ex.StackTrace}");
				Console.Write("Misskey URL> ");
				var domain = Console.ReadLine();
				mi = new MisskeyClient(domain);
				await AuthorizeAsync(mi);
			}

			return new Shell 
			{
				
			};
		}

		private async void HandleMention(Note mention)
		{
			// React
			// hack 好感度システム実装したらそっちに移動して、好感度に応じて love pudding hmm と切り替えていく
			await misskey.Notes.Reactions.CreateAsync(mention.Id, IsAdmin(mention.User) ? Reaction.Love : Reaction.Pudding);
			Console.WriteLine($"Mentioned: {mention.User.Username}: {mention.Text}");
			foreach (var mod in modules)
			{
				try
				{
					// module が true を返したら終わり
					if (await mod.ActivateAsync(mention, misskey, this))
						break;
				}
				catch (Exception ex)
				{
					WriteException(ex);
				}
			}
		}

		private async void HandleTimeline(Note note)
		{
			Console.WriteLine($"Timeline: {note.User.Username}: {note.Text ?? (note.Renote != null ? "RN: " + note.Renote.Text : null)}");
			foreach (var mod in modules)
			{
				try
				{
					// module が true を返したら終わり
					if (await mod.OnTimelineAsync(note, misskey, this))
						break;
				}
				catch (Exception ex)
				{
					Server.WriteException(ex);
				}
			}
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
	}
}
