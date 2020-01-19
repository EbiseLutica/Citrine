using System;
using System.Threading.Tasks;
using Citrine.Core;
using static System.Console;

namespace Citrine.Misskey
{
	class Program
	{
		static async Task Main(string[] args)
		{
			Console.WriteLine(Server.CitrineAA + " version " + Server.Version);
			var logger = new Logger("Bootstrap");
			logger.Info("Citrine.Misskey " + Shell.Version);
			var sh = await Shell.InitializeAsync();
			logger.Info("シェルを初期化しました！");
			// sh.Core.AddCommand(new EmojiCommand());
			// logger.Info("/emoji コマンドを追加しました。");
			sh.Core.AddCommand(new IsCatCommand());
			logger.Info("/iscat コマンドを追加しました。");
			sh.Core.AddCommand(new ToggleFeatureCommand());
			logger.Info("/config コマンドを追加しました。");
			// sh.Core.AddModule(new InvitationCodeModule());
			// logger.Info($"{nameof(InvitationCodeModule)} モジュールを追加しました。");
			logger.Info("起動しました！");

			await Task.Delay(-1);
		}
	}
}
