using System;
using System.Threading.Tasks;
using Citrine.Core;

namespace Citrine.Discord
{
	using System.Linq;
	using Citrine.Core.Api;
	using static Console;
	class Program
    {
        static async Task Main(string[] args)
        {
			WriteLine($"Citrine {Server.Version}");
			WriteLine($"XelticaBot {Server.VersionAsXelticaBot}");
			WriteLine($"Citrine.Discord {Shell.Version}");
			WriteLine();
			WriteLine("起動中...");
			var sh = await Shell.InitializeAsync();
			sh.Core.AddCommand(new YuusakuCommand());
			WriteLine("起動しました！");

			await Task.Delay(-1);
        }
    }

	public class YuusakuCommand : CommandBase
	{
		public override string Name => "yuusaku";

		public override string Usage => "/yuusaku";

		public override string Description => "コマンドには気をつけよう！";

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			var t = animations.First();
			var s = shell as Shell;
			var c = s.CurrentChannel;

			var p = await c.SendMessageAsync(t);

			await animations.Skip(1)
							.ForEach(async a =>
							{
								await p.ModifyAsync(msg => msg.Content = a);
								await Task.Delay(250);
							});

			return "";
		}

		private string[] animations = {
			".　🙄　　　　　　🐝",
			".　🙄　　　　　🐝",
			".　🙄　　　　🐝",
			".　🙄　　　🐝",
			".　🙄　　🐝",
			".　🙄　🐝",
			".　😒🐝",
			".　🐝",
			".🐝😱",
			".　😨",
			".　😱",
			".　😨",
			".　💀",
			".　💀",
			".　💀",
			"スズメバチには気をつけよう！\n🙄 🙄 🙄",
		 };
	}
}
