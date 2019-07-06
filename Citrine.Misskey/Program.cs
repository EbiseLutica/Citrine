using System.Threading.Tasks;
using Citrine.Core;
using static System.Console;

namespace Citrine.Misskey
{
	class Program
	{
		static async Task Main(string[] args)
		{
			WriteLine($"Citrine {Server.Version}");
			WriteLine($"Citrine.Misskey {Shell.Version}");
			WriteLine();
			WriteLine("起動中...");
			var sh = await Shell.InitializeAsync();
			sh.Core.AddCommand(new EmojiCommand());

			WriteLine("起動しました！");

			while (true)
				await Task.Delay(1000);
		}
	}
}
