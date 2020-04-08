#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using Citrine.Core.Modules.Markov;

namespace Citrine.Core
{
	public class WakachiCommand : CommandBase
	{
		public override string Name => "wakachi";
		public override string Usage => "/wakachi <text>";
		public override string Description => "入力文字列をわかち書きします。";

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			return string.Join(" ", TinySegmenter.Instance.Segment(body));
		}
	}
}
