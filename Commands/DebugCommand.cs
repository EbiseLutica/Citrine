#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます

using System.Linq;
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;

namespace Citrine.Core
{
	public class DebugCommand : CommandBase
	{
		public override string Name => "debug";

		public override string Usage => "/debug";

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			if (sender is not PostCommandSender p) return "call from post";

			switch (args[0])
			{
				case "set":
					core.Storage[p.User].Set(args[1], args[2]);
					return "success";
				case "get":
					return (core.Storage[p.User].Get<object>(args[1], (object)"null")).ToString();
				case "has":
					return core.Storage[p.User].Has(args[1]).ToString();
				default:
					return "set / get / has";
			}
		}
	}
}
