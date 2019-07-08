using System;
using System.Threading.Tasks;
using Citrine.Core;

namespace Citrine.Discord
{
	class Program
    {
        static async Task Main(string[] args)
        {
			Console.WriteLine(Server.CitrineAA + " version " + Server.Version);
			var logger = new Logger("Bootstrap");
			logger.Info("Citrine.Discord " + Shell.Version);
			var sh = await Shell.InitializeAsync();
			logger.Info("Initialized Shell!");
			logger.Info("Launched!");

			await Task.Delay(-1);
        }
    }
}
