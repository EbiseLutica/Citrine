using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Citrine.Core.Api;

namespace Citrine.Core.Modules
{
	public class EraitModule : ModuleBase
	{
		public static readonly string StatEraitedCount = "stat.eraited-count";

		public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			if (n.Text == null)
				return false;
			if (core.GetRatingOf(n.User) == Rating.Hate)
				return false;
			await Task.Delay(4000);
			var reg = Regex.Match(n.Text.TrimMentions(), @"(.+)(から褒めて|の(えら|偉)い)");
			var reg2 = Regex.Match(n.Text.TrimMentions(), @"褒めて|(えら|偉)い\?？");
			if (reg.Success)
			{
				await shell.ReplyAsync(n, $"{reg.Groups[1].Value}のえらい!");
				core.Storage[n.User].Add(StatEraitedCount);
				core.LikeWithLimited(n.User);
				EconomyModule.Pay(n, shell, core);
				return true;
			}
			else if (reg2.Success)
			{
				await shell.ReplyAsync(n, "えらい!");
				core.Storage[n.User].Add(StatEraitedCount);
				EconomyModule.Pay(n, shell, core);
				core.LikeWithLimited(n.User);
				return true;
			}
			return false;
		}
	}
}
