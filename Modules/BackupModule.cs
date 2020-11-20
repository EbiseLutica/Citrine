using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;

namespace Citrine.Core.Modules
{
	public class BackupModule : ModuleBase
	{
		public BackupModule()
		{
			timer = new Timer(1000 * 60 * 30)
			{
				AutoReset = true,
				Enabled = true,
			};
			timer.Elapsed += OnTick;
		}

		public override async Task<bool> OnTimelineAsync(IPost n, IShell shell, Server core)
		{
			(this.core, this.shell) = (core, shell);
			await Task.Delay(0);
			return false;
		}

		private async void OnTick(object sender, ElapsedEventArgs e)
		{
			if (core == null || shell == null)
				return;
			logger.Info("Start backup for storage.json");
			if (!Directory.Exists("backup"))
			{
				Directory.CreateDirectory("backup");
			}
			var storage = await File.ReadAllTextAsync("storage.json");
			var n = DateTime.Now;
			await File.WriteAllTextAsync($"backup/storage-{n.Year}-{n.Month}-{n.Day}-{n.Hour}-{n.Minute}-{n.Second}-{n.Millisecond}.json", storage);
			logger.Info("Saved a backup for storage.json");
		}

		private readonly Timer timer;
		private Server? core;
		private IShell? shell;

		private readonly Logger logger = new Logger(nameof(BackupModule));
	}
}
