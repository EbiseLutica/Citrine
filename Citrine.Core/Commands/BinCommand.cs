#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
using System.Text;
using System.Threading.Tasks;
using Citrine.Core.Api;

namespace Citrine.Core
{
    public class BinCommand : CommandBase
	{
		public override string Name => "tobyte";

		public override string Usage => "/tobyte <data>";

		public override string Description => "テキストをバイナリダンプします。";

		public override string[] Aliases => new[] { "bin" };

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			var sb = new StringBuilder();
			var cur = 0;
			foreach (var ch in Encoding.UTF8.GetBytes(body))
			{
				sb.Append($"{ch:x2} ");
				cur++;
				if (cur == 16)
				{
					cur = 0;
					sb.AppendLine();
				}
			}
			return sb.ToString();
		}
	}
}
