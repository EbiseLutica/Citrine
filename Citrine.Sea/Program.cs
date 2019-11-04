using System;
using System.Threading.Tasks;
using Citrine.Core;

namespace Citrine.Sea
{
    class Program
    {
        static async Task Main(string[] args)
        {
			Console.WriteLine(Server.CitrineAA + " version " + Server.Version);
			var logger = new Logger("Bootstrap");
			logger.Info("Citrine.Sea " + Shell.Version);
			var sh = await Shell.InitializeAsync();
			logger.Info("シェルを初期化しました！");
			logger.Info("起動しました！");

			await Task.Delay(-1);
        }
    }
}
