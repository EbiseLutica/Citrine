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
			logger.Info("Initialized Shell!");
			sh.Core.AddCommand(new EmojiCommand());
			logger.Info("Added command /emoji");
			logger.Info("Launched!");

			await Task.Delay(-1);
		}
	}
}
