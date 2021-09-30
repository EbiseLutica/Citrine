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

		public override string Usage => "/pipeline (new-line separated command line)";

		public override string Description => "Execute commands continuously. The previous command output is added to the end of the argument for the next command.";

		public override Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			return PipeCommand.RunPipeAsync(sender, core, body, '|');
		}
	}
}
