using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Citrine.Core;
using static System.Console;

namespace Citrine.Misskey
{
	class Program
	{
		const string ConfigPath = "./config";
		static async Task Main(string[] args)
		{
			WriteLine($"Citrine version{Server.Version}");
			WriteLine($"XelticaBot version{Server.VersionAsXelticaBot}");
			WriteLine($"Citrine.Misskey version{Shell.Version}");
			WriteLine();
			WriteLine("起動中...");
			await Shell.InitializeAsync(new EmojiModule());
			WriteLine("起動しました！");

			while (true)
				await Task.Delay(1000);
		}
	}
}
