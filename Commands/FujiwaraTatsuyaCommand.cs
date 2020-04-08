#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
using System.Linq;
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;

namespace Citrine.Core
{
	public class FujiwaraTatsuyaCommand : CommandBase
	{
		public override string Name => "fujiwaratatsuya";

		public override string Usage => "/fujiwaratatsuya";

		public override string[] Aliases => new []{ "fujiwara", "fjwr", "fujitatsu" };

		public override string Description => "テ゛キ゛ス゛ト゛を゛返゛す゛だ゛け゛の゛コ゛マ゛ン゛ド゛だ゛ぞ゛。";

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			return string.Concat(body.Select(c => c + "゛"));
		}
	}
}
