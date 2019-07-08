using System;
using System.Threading.Tasks;
using Citrine.Core;

namespace Citrine.Mastodon
{
	class Program
	{
		static async Task Main(string[] args)
		{
			Console.WriteLine(Server.CitrineAA + " version " + Server.Version);
			var logger = new Logger("Bootstrap");
			logger.Info("Citrine.Mastodon " + Shell.Version);
			var sh = await Shell.InitializeAsync();
			logger.Info("シェルを初期化しました！");
			logger.Info("起動しました！");

			await Task.Delay(-1);
		}
	}

}
