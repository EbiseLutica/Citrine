#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
using System.Linq;
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;

namespace Citrine.Core
{
	public class ReverseCommand : CommandBase
	{
		public override string Name => "reverse";

		public override string Usage => "/reverse <text>";

		public override string Description => "テキストを逆さに変換します。";

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			return string.Concat(body.Reverse());
		}
	}
}
