using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Citrine.Core.Api;

namespace Citrine.Core.Modules
{
	public class ValentineModule : ModuleBase
	{
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

			if (t.Month == 2 && t.Day == 14)
			{
				var fans = core.Storage.Records.Keys.Where(id => core.GetRatingOf(id) >= Rating.BestFriend);
				foreach (var fan in fans)
				{
					var user = await shell.GetUserAsync(fan);
					var storage = core.Storage[user];
					if (storage.Get("lastValentineYear", 0) == t.Year)
						continue;
					var msg = $"{core.GetNicknameOf(user)}, ハッピーバレンタイン! 💝受け取ってほしいな.";
					await shell.SendDirectMessageAsync(user, msg);
					storage.Set("lastValentineYear", t.Year);
				}
			}
		}

		public async override Task<bool> OnTimelineAsync(IPost n, IShell shell, Server core)
		{
			(this.core, this.shell) = (core, shell);
			return false;
		}

		private readonly Timer timer;
		private DateTime prevDate;
		private Server core;
		private IShell shell;
	}
}
