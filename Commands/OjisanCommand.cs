#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;
using Newtonsoft.Json;

namespace Citrine.Core
{
	public class OjisanCommand : CommandBase
	{
		public override string Name => "ojisan";

		public override string Usage => "/ojisan [name]";

		public override string Description => "おじさん構文を返します。";

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			var req = !string.IsNullOrEmpty(body) ? new FormUrlEncodedContent(new[]{
				new KeyValuePair<string, string>("name", body)
			}) : new StringContent("") as HttpContent;
			var res = await (await Server.Http.PostAsync("https://ojichat.appspot.com/post", req)).Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<OjichatResponse>(res).Message ?? "";
		}

		class OjichatResponse
		{
			[JsonProperty("message")]
			public string? Message { get; set; }
		}
	}
}
