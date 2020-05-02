#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます

using System.Linq;
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;

namespace Citrine.Core
{
	public class ModuleCommand : CommandBase
	{
		public override string Name => "modules";

		public override string Usage => "/modules or /mods";

		public override string[] Aliases { get; } = { "mods" };

		public override string Description => "全モジュールを列挙します。";

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			var mods = core.Modules.Select(mod => mod.GetType().Name);
			return $"モジュール数: {mods.Count()}\n{string.Join(",", mods)}";
		}
	}
}
