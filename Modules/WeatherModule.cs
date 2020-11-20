using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;
using Newtonsoft.Json;

namespace Citrine.Core.Modules
{
	public class WeatherModule : ModuleBase
	{
		public override int Priority => -9000;

		public static readonly string StatForecastCount = "stat.forecast-count";

		public async override Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			var req = n.Text?.TrimMentions();
			if (string.IsNullOrEmpty(req))
				return false;

			var m = Regex.Match(req, "(.+)の天気");
			if (m.Success)
			{
                await shell.ReplyAsync(n, sorryMessage.Random());
                return true;
			}
			return false;
		}

        private readonly string[] sorryMessage =
        {
            "ごめん，天気予報マシンが壊れちゃって. 修理中なので少し待っててほしい",
            "ごめんだけど，天気予報マシンが動かなくなっちゃったので，今は答えられないの",
            "残念だけど，天気予報マシンが故障して使えなくなっちゃったの. 修理後にまた聞いてくれる?",
		};
    }
}
