#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;

namespace Citrine.Core
{
	public class InspectCommand : CommandBase
	{
		public override string Name => "inspect";

		public override string Usage => "/inspect [commands]";

		public override string Description => "コマンドの引数をそのまま列挙します。";

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			var builder = new StringBuilder();
			builder.AppendFormat("[{0}]: ", args.Length);
			builder.Append(string.Join(", ", args.Select(a => $"\"{a}\"")));
			return builder.ToString();
		}
	}
}
