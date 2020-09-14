#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
using System.Collections.Generic;
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;

namespace Citrine.Core
{
	public class WaCommand : CommandBase
	{
		public override string Name => "wa";

		public override string Usage => "/wa [amount=15]";

		public override string Description => "#わーーーーーーーーーーーーーーー";

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			var amount = args.Length < 1 ? 15 : int.Parse(args[0]);

			return string.Concat(Wa(amount));
		}

		private IEnumerable<char> Wa(int amount)
		{
			yield return '#';
			yield return 'わ';
			for (var i = 0; i < amount; i++)
				yield return 'ー';
		}
	}
}
