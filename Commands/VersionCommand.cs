#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;

namespace Citrine.Core
{
	public class VersionCommand : CommandBase
	{
		public override string Name => "version";

		public override string Usage => "/version or /ver or /v";

		public override string[] Aliases { get; } = { "ver", "v" };

		public override string Description => "バージョン情報を取得します。";

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			return $"BotBone v{Server.Version} Citrine v{Const.Version}";
		}
	}
}
