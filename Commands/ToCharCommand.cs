#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;

namespace Citrine.Core
{
    public class ToCharCommand : CommandBase
	{
		public override string Name => "tochar";

		public override string Usage => "/tochar <data>";

		public override string Description => "16進文字列をテキストに変換します。";

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			body = Regex.Replace(body, @"\s", "");
			if (body.Length % 2 != 0)
				return "正しい16進文字列ではありません.";

			var data = new byte[body.Length / 2];
			for (var i = 0; i < body.Length / 2; i++)
			{
				data[i] = Convert.ToByte(body.Substring(i * 2, 2), 16);
			}

			return Encoding.UTF8.GetString(data);
		}
	}
}
