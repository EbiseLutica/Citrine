using System.Linq;
using System.Threading.Tasks;
using Citrine.Core.Api;

namespace Citrine.Core
{
    public class ModuleCommand : CommandBase
	{
		public override string Name => "modules";

		public override string Usage => "/modules or /mods";

		public override string[] Aliases { get; } = { "mods" };

		public override async Task<string> OnActivatedAsync(IPost source, Server core, IShell shell, string[] args, string body)
		{
			var mods = core.Modules.Select(mod => mod.GetType().Name);
			await shell.ReplyAsync(source, string.Join(",", mods), $"モジュール数: {mods.Count()}");
			return null;
		}
	}
}
