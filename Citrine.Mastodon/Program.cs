using System.Threading.Tasks;
using Citrine.Core;
using Disboard.Mastodon;
using Disboard.Mastodon.Enums;
using static System.Console;

namespace Citrine.Mastodon
{
	class Program
	{
		static async Task Main(string[] args)
		{
	WriteLine($"Citrine version{Server.Version}");
	WriteLine($"XelticaBot version{Server.VersionAsXelticaBot}");
	WriteLine($"Citrine.Misskey version{Shell.Version}");
	WriteLine();
	WriteLine("起動中...");
	await Shell.InitializeAsync();
	WriteLine("起動しました！");

	while (true)
		await Task.Delay(1000);
		}
	}

}
