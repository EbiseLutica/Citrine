using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Citrine.Core.Api;

namespace Citrine.Core.Modules
{
	public class AdminModule : ModuleBase
	{
		public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			if (n.Text == null)
				return false;

			var text = n.Text.TrimMentions();

			var cmd = text.Split(' ');

			if (text.StartsWith("/stop", StringComparison.Ordinal))
			{
				if (core.IsAdmin(n.User))
				{
					await shell.ReplyAsync(n, "またねー。");
					// good bye
					Environment.Exit(0);
				}
				else
				{
					var mes = core.GetRatingOf(n.User) == Rating.Partner ? "いくらあなたでも, その頼みだけは聞けない. ごめんね..." : "申し訳ないけど, 他の人に言われてもするなって言われてるから...";
					await shell.ReplyAsync(n, mes);
				}
				return true;
			}

			if (text.StartsWith("/modules", StringComparison.Ordinal) || text.StartsWith("/mods", StringComparison.Ordinal))
			{
				var mods = core.Modules.Select(mod => mod.GetType().Name);
				await shell.ReplyAsync(n, string.Join(",", mods), $"モジュール数: {mods.Count()}");
				return true;
			}

			if (text.StartsWith("/version", StringComparison.Ordinal) || text.StartsWith("/v", StringComparison.Ordinal))
			{
				await shell.ReplyAsync(n, $"Citrine v{Server.Version} / XelticaBot v{Server.VersionAsXelticaBot}");
				return true;
			}

			if (text.StartsWith("/useragents", StringComparison.Ordinal) || text.StartsWith("/ua", StringComparison.Ordinal))
			{
				await shell.ReplyAsync(n, Server.Http.DefaultRequestHeaders.UserAgent.ToString());
				return true;
			}

			return false;
		}

		public override Task<bool> OnDmReceivedAsync(IPost n, IShell shell, Server core) => ActivateAsync(n, shell, core);
	}
}
