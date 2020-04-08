#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
using System.Text;
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;

namespace Citrine.Core
{
	public class UserAgentCommand : CommandBase
	{
		public override string Name => "useragent";

		public override string Usage => "/useragent or /ua";

		public override string[] Aliases { get; } = { "ua" };

		public override string Description => "シトリンが使用する HTTP Client のユーザーエージェントを取得します。";

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			return Server.Http.DefaultRequestHeaders.UserAgent.ToString();
		}
	}
}
