#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
using System.Linq;
using System.Threading.Tasks;
using Citrine.Core.Api;

namespace Citrine.Core
{
	public class TranslateCommand : CommandBase
	{
		public override string Name => "translate";

		public override string Usage => "/translate <from(autoで自動判定)> <to> <text>";

		public override string Description => "テキストを翻訳します。";

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			if (args.Length < 3)
				throw new CommandException();
			var from = args[0].ToLowerInvariant();
			if (from == "auto") from = "";
			var to = args[1].ToLowerInvariant();
			var text = string.Join(" ", args.Skip(2));

			var url = $"https://script.google.com/macros/s/AKfycby9-QKJAYXlXSH-Evq-f5y2enIb92jIoqtyTdeUR1A/exec?text={text}&source={from}&target={to}";
			return await (await Server.Http.GetAsync(url)).Content.ReadAsStringAsync();
		}
	}
}
