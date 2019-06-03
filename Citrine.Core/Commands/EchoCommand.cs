#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
using System.Threading.Tasks;
using Citrine.Core.Api;

namespace Citrine.Core
{
	public class EchoCommand : CommandBase
	{
		public override string Name => "echo";

		public override string Usage => "/echo <text>";

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			return body;
		}
	}
}
