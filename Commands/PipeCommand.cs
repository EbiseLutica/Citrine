#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
using System;
using System.Linq;
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;

namespace Citrine.Core
{
	public class PipeCommand : CommandBase
	{
		public override string Name => "pipe";

		public override string Usage => "/pipe (これ以降にコマンドを | で区切って並べます。)";

		public override string Description => "各種コマンドを連結して実行します。前のコマンドの出力は、次のコマンドの末尾に追記されます。";

		public override Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			return RunPipeAsync(sender, core, body, '|');
		}

		public static async Task<string> RunPipeAsync(ICommandSender sender, Server core, string body, params char[] split)
		{
			var lines = body.Replace("\r", "\n").Replace("\r\n", "\n").Split(split).Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => l.Trim());
			var output = "";
			foreach (var line in lines)
			{
				output = await core.ExecCommand(sender, line + " " + output);
				output = output.Trim();
			}
			return output;
		}
	}
}
