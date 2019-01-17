using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Citrine
{
	class Program
	{
		const string ConfigPath = "./config";
		static async Task Main(string[] args)
		{
			string domain = null, token = null;
			try
			{
				if (File.Exists(ConfigPath))
				{
					var cp = File.ReadAllLines(ConfigPath);
					if (cp.Length < 2)
						throw new Exception("設定ファイルが不正です。");
					domain = cp[0];
					token = cp[1];
				}
				else
				{
					throw new Exception("");
				}
			}
			catch (Exception ex)
			{
				// ファイルがない場合やエラーのときはここでトークンを取り直す
				if (!string.IsNullOrEmpty(ex.Message))
					Console.WriteLine("エラーが発生しました: " + ex.Message);

			}

			var mods = Assembly.GetExecutingAssembly().GetTypes().Where(a => a.IsSubclassOf(typeof(ModuleBase)));

			Console.WriteLine("読み込まれたモジュール: " + string.Join(", ", mods.Select(m => m.Name)));

			await Citrine.InitializeAsync(
				mods.Select(a => Activator.CreateInstance(a) as ModuleBase)
					.ToArray()
			);

			Console.WriteLine("起動しました！");

			while (true)
				await Task.Delay(1000);
		}
	}
}
