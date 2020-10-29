#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;

namespace Citrine.Core
{
	public class WrapWithCommand : CommandBase
	{
		public override string Name => "wrapwith";

		public override string Usage => "/wrapwith <text-to-wrap> <text>";

		public override string Description => "指定した文字列で囲まれた文字列を返します。";

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			if (args.Length < 2)
				throw new CommandException();
			body = body[(args[0].Length + 1)..];
			return args[0] + body + args[0];
		}
	}
}
