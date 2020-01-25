using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Citrine.Core.Api;
using Newtonsoft.Json;

namespace Citrine.Core.Modules
{
	public class OjichatModule : ModuleBase
	{
		public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			if (n.Text is string text && text.IsMatch("おじさんの(真似|まね)"))
			{
				EconomyModule.Pay(n, shell, core);
				core.LikeWithLimited(n.User);
				await shell.ReplyAsync(n, await core.ExecCommand("/ojisan " + core.GetNicknameOf(n.User)));
				return true;
			}
			return false;
		}
	}
}
