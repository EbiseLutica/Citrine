#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;

namespace Citrine.Core
{
	public class UnyaizeCommand : CommandBase
	{
		public override string Name => "unnyaize";

		public override string Usage => "/unnyaize";

		public override string[] Aliases => new []{ "unnya" };

		public override string Description => "発言のねこを除去します。";

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			return body.Replace("にゃ", "な").Replace("ニャ", "ナ").Replace("ﾆｬ", "ﾅ");
		}
	}
}
