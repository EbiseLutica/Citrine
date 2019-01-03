using System;
using System.IO;
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
			await Citrine.InitializeAsync(
				new GreetingModule(),
				new VoteModule(),
				new FortuneModule(),
				new AdminModule()
			);

			while (true)
				await Task.Delay(1000);
		}
	}
}
