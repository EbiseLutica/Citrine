using System.Threading.Tasks;
using Citrine.Core;
using static System.Console;

namespace Citrine.Mastodon
{
	class Program
	{
		static async Task Main(string[] args)
		{
			WriteLine($"Citrine {Server.Version}");
			WriteLine($"XelticaBot {Server.VersionAsXelticaBot}");
			WriteLine($"Citrine.Mastodon {Shell.Version}");
			WriteLine();
			WriteLine("起動中...");
			await Shell.InitializeAsync();
			WriteLine("起動しました！");

			while (true)
				await Task.Delay(1000);
		}
	}

}
