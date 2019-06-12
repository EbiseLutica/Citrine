using System;
using System.Threading.Tasks;
using Citrine.Core;

namespace Citrine.Discord
{
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
			sh.Core.AddCommand(new AudioCommand());
			WriteLine("起動しました！");

			await Task.Delay(-1);
        }
    }
}
