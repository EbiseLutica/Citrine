#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
using System;
using System.Text;
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using Citrine.Core.Modules;

namespace Citrine.Core
{
	using static FortuneModule;
	using static FortuneExtension;
	public class WordCommand : CommandBase
	{
		public override string Name => "word";

		public override string Usage => "/word";

		public override string Description => "ランダムな言葉を生成します。";

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			if (args.Length > 0)
			{
				if (args[0].ToLowerInvariant() == "total")
				{
					return (ItemPrefixes.Length * ItemSuffixes.Length * Items.Length +
							ItemPrefixes.Length * Items.Length +
							Items.Length).ToString();
				}
				else if (int.TryParse(args[0], out var length) && length > 0)
				{
					var sb = new StringBuilder();
					length = Math.Min(length, 100);
					for (var i = 0; i < length; i++)
					{
						sb.AppendLine(GenerateWord());
					}
					return sb.ToString();
				}
				else
				{
					throw new CommandException();
				}
			}
			return GenerateWord();
		}
	}
}
