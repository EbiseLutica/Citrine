#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます

using System.Linq;
using System.Threading.Tasks;
using Citrine.Core.Api;

namespace Citrine.Core
{
	public class HiraganaCommand : CommandBase
	{
		public override string Name => "hiragana";

		public override string Usage => "/hiragana <count=3>";

		public override string[] Aliases { get; } = { "kana" };

		public override string Description => "ひらがなをランダムに出します";

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			var count = 3;
			if (args.Length > 0 && !int.TryParse(args[0], out count))
				throw new CommandException();
			return string.Concat(Enumerable.Repeat(0, count).Select(_ => hiragana.Random()));
		}

		char[] hiragana = "あいうえおかきくけこさしすせそたちつてとなにぬねのはひふへほまみむめもやゆよらりるれろわゐゑをんゔがぎぐげござじずぜぞだぢづでどばびぶべぼぱぽぷぺぽぁぃぅぇぉゃゅょゎっ".ToArray();
	}
}
