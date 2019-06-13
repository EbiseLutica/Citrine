#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
using System.Threading.Tasks;
using Citrine.Core.Api;
using Citrine.Core.Modules;

namespace Citrine.Core
{
	using static FortuneModule;
	public class WordCommand : CommandBase
	{
		public override string Name => "word";

		public override string Usage => "/word";

		public override string Description => "ランダムな言葉を生成します。";

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			if (args.Length > 0 && args[0].ToLowerInvariant() == "total")
				return	(ItemPrefixes.Length * ItemSuffixes.Length * Items.Length +
						ItemPrefixes.Length * Items.Length +
						Items.Length).ToString();
			return GenerateWord();
		}
	}
}
