using System;
using System.Linq;
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

			if (n.Text.Contains("/restart"))
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

			if (n.Text.Contains("/modules") || n.Text.Contains("/mods"))
			{
				var mods = core.Modules.Select(mod => mod.GetType().Name);
				await shell.ReplyAsync(n, string.Join(",", mods), $"モジュール数: {mods.Count()}");
				return true;
			}

			if (n.Text.Contains("/server") || n.Text.Contains("/srv"))
			{
				await shell.ReplyAsync(n, $"");
				return true;
			}

			return false;
		}
	}

}