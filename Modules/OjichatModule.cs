using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;
using Newtonsoft.Json;

namespace Citrine.Core.Modules
{
	public class OjichatModule : ModuleBase
	{
		public static readonly string StatOjisanedCount = "stat.ojisaned-count";
		public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			if (n.Text is string text && text.IsMatch("おじさんの(真似|まね)"))
			{
				EconomyModule.Pay(n, shell, core);
				await Task.Delay(4000);
				core.LikeWithLimited(n.User);
				core.Storage[n.User].Add(StatOjisanedCount);
				await shell.ReplyAsync(n, await core.ExecCommand("/ojisan " + core.GetNicknameOf(n.User)));
				return true;
			}
			return false;
		}
	}
}
