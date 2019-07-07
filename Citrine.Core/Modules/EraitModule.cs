using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Citrine.Core.Api;

namespace Citrine.Core.Modules
{
    public class EraitModule : ModuleBase
	{
		public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			if (n.Text == null)
				return false;
			var reg = Regex.Match(n.Text.TrimMentions(), @"(.+)(から褒めて|の(えら|偉)い)");
			var reg2 = Regex.Match(n.Text.TrimMentions(), @"褒めて|(えら|偉)い\?？");
			if (reg.Success)
			{
                await shell.ReplyAsync(n, $"{reg.Groups[1].Value}のえらい!");
				return true;
			}
			else if (reg2.Success)
			{
				await shell.ReplyAsync(n, "えらい!");
				return true;
			}
			return false;
		}
	}
}
