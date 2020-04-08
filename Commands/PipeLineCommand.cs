#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;

namespace Citrine.Core
{
	public class PipeLineCommand : CommandBase
	{
		public override string Name => "pipeline";

		public override string Usage => "/pipeline (これ以降に改行で区切ってコマンドを書いて下さい。)";

		public override string Description => "各種コマンドを連結して実行します。前のコマンドの出力は、次のコマンドの末尾に追記されます。";

		public override Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			return PipeCommand.RunPipeAsync(sender, core, body, '|');
		}
	}
}
