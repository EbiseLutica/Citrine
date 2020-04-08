#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
using System;
using System.Threading.Tasks;
using System.Web;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;
using Newtonsoft.Json.Linq;

namespace Citrine.Core
{
	public class NyaizeCommand : CommandBase
	{
		public override string Name => "nyaize";

		public override string Usage => "/nyaize";

		public override string[] Aliases => new []{ "nya" };

		public override string Description => "発言をねこにします。";

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			return body.Replace("な", "にゃ").Replace("ナ", "ニャ").Replace("ﾅ", "ﾆｬ");
		}
	}
}
