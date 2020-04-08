#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;

namespace Citrine.Core
{
    public class IsAdminCommand : CommandBase
	{
		public override string Name => "isadmin";

		public override string Usage => "/isadmin";

		public override string Description => "管理者であるかどうか取得します";

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			return sender.IsAdmin ? "yes" : "no";
		}
	}
}
