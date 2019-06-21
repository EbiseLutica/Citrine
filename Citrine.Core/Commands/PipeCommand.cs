#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
using System;
using System.Linq;
using System.Threading.Tasks;
using Citrine.Core.Api;

namespace Citrine.Core
{
	public class PipeCommand : CommandBase
	{
		public override string Name => "pipe";

		public override string Usage => "/pipe (and write more commands after the new line)";

		public override string Description => "各種コマンドを連結して実行します。前のコマンドの出力は、次のコマンドの末尾に追記されます。";

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			var lines = body.Replace("\r", "\n").Replace("\r\n", "\n").Split('\n').Where(l => !string.IsNullOrWhiteSpace(l));
			var output = "";
			foreach (var line in lines)
			{
				Console.WriteLine(line + " : " + output);
				output = await core.ExecCommand(sender, line.Trim() + " " + output);
				output = output.Trim();
			}
			return output;
		}
	}
}
