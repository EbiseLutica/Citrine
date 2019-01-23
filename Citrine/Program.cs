using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Citrine.Core;

namespace Citrine.Misskey
{
	class Program
	{
		const string ConfigPath = "./config";
		static async Task Main(string[] args)
		{
			await Shell.InitializeAsync();

			Console.WriteLine("起動しました！");

			while (true)
				await Task.Delay(1000);
		}
	}
}
