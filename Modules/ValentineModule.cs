using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;

namespace Citrine.Core.Modules
{
	public class ValentineModule : ModuleBase
	{
		public static readonly string StatValentineCount = "stat.valentine-count";

		public ValentineModule()
		{
			timer = new Timer(1000)
			{
				AutoReset = true,
				Enabled = true,
			};
			timer.Elapsed += OnTick;
		}

		private async void OnTick(object sender, ElapsedEventArgs e)
		{
			if (core == null || shell == null)
				return;
			var t = DateTime.Today;

			if (prevDate.Day == t.Day)
				return;

			prevDate = t;

			if (IsValentineDay(t))
			{
				var fans = core.Storage.Records.Keys.Where(id => core.GetRatingOf(id) >= Rating.BestFriend);
				foreach (var fan in fans)
				{
					var user = await shell.GetUserAsync(fan);
					if (user == null)
						continue;

					var storage = core.Storage[user];
					if (storage.Get("lastValentineYear", 0) == t.Year)
						continue;

					var msg = $"{core.GetNicknameOf(user)}, ハッピーバレンタイン! 💝受け取ってほしいな.";
					await shell.SendDirectMessageAsync(user, msg);
					storage.Add(StatValentineCount);
					storage.Set("lastValentineYear", t.Year);
				}
			}
		}

		public async override Task<bool> OnTimelineAsync(IPost n, IShell shell, Server core)
		{
			(this.core, this.shell) = (core, shell);
			await Task.Delay(0);
			return false;
		}

		public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			if (IsValentineDay(DateTime.Today) && n.Text is string text)
			{
				if (text.IsMatch("(チョコ|ちょこ|🍫).*(あげる|どうぞ)") && core.GetRatingOf(n.User) >= Rating.Like)
				{
					await shell.ReplyAsync(n, thanksMessage.Random());
					return true;
				}
			}
			return false;
		}

		public bool IsValentineDay(DateTime date) => date.Month == 2 && date.Day == 14;

		private readonly Timer timer;
		private DateTime prevDate;
		private Server? core;
		private IShell? shell;

		private readonly string[] thanksMessage =
		{
			"ありがと〜!",
			"ほんと!? 嬉しい, ありがとう",
			"わぁ, ありがとう!",
			"私に!? ありがとう!",
		};
	}
}
