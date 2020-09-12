#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます

using System.Linq;
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;

namespace Citrine.Core
{
	public class KatakanaCommand : CommandBase
	{
		public override string Name => "katakana";

		public override string Usage => "/katakana <count=3>";

        public override string[] Aliases { get; } = { "katakanya" };

        public override string Description => "カタカナをランダムに出します";

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			var count = 3;
			if (args.Length > 0 && !int.TryParse(args[0], out count))
				throw new CommandException();
			return string.Concat(Enumerable.Repeat(0, count).Select(_ => katakana.Random()));
		}

		readonly char[] katakana = "アイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワヰヱヲンヴガギグゲゴザジズゼゾダヂヅデドバビブベボパポプペポァィゥェォャュョヮッヵヶ".ToArray();
	}
}
