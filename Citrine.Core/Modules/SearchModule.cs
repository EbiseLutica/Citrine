using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Citrine.Core.Api;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Citrine.Core.Modules
{
	public class SearchModule : ModuleBase
	{
		public override int Priority => -100;
		private static readonly HttpClient TheClient = new HttpClient();
		private static readonly string CalcApiUrl = "http://www.rurihabachi.com/web/webapi/calculator/json?exp={0}";
		private static readonly string WikipediaApiUrl = "https://ja.wikipedia.org/w/api.php?action=query&format=json&prop=extracts&redirects=1&exchars=300&explaintext=1&titles={0}";
		private static readonly Regex regexMath = new Regex(@"([1234567890\+\-\*/\(\)\^piesqrtlogabnc]+?)(っ?て|と?は|[=＝])");
		private static readonly Regex regexPedia = new Regex(@"(.+?)(っ?て|と?は)(何|なに|なん|誰|だれ|どなた|何方)");

		public SearchModule()
		{
			// UA を指定する
			TheClient.DefaultRequestHeaders.Add("User-Agent", $"Mozilla/5.0 Citrine/{Server.Version} XelticaBot/{Server.VersionAsXelticaBot} (https://github.com/xeltica/citrine) .NET/{Environment.Version}");
		}

		public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			if (n.User.Id == shell.Myself.Id)
				return false;
			if (n.Text == default)
				return false;

			var m = regexMath.Match(n.Text);
			var mpedia = regexPedia.Match(n.Text);
			if (m.Success || mpedia.Success)
			{
				string response = default;
				string query = default;
				if (m.Success)
				{
					query = m.Groups[1].Value.TrimMentions();
					response = await FromCalcAsync(query);
				}
				else if (mpedia.Success)
				{
					query = mpedia.Groups[1].Value.TrimMentions();
					response = await FromWikipediaAsync(query);
				}

				response = response ?? $"{query} について調べてみたけどわからなかった. ごめん...";
				await shell.ReplyAsync(n, response);
				return true;
			}
			return false;
		}

		private async Task<string> FromCalcAsync(string query)
		{
			var res = await (await TheClient.GetAsync(CreateUrl(CalcApiUrl, query))).Content.ReadAsStringAsync();
			Console.WriteLine(CreateUrl(CalcApiUrl, query));
			var json = JsonConvert.DeserializeObject<CalcModel>(res);
			if (json.Status > 0)
				return json.Status == 60 ? $"{query} は計算できないよ..." : default;
			return $"{json.Expression} = {json.Value[0].CalculatedValue}";
		}

		public async Task<string> FromWikipediaAsync(string query)
		{
			var res = JObject.Parse(await (await TheClient.GetAsync(CreateUrl(WikipediaApiUrl, query))).Content.ReadAsStringAsync());
			if (!res.ContainsKey("query"))
			{
				Console.WriteLine("res has no query");
				return default;
			}
			var q = res["query"] as JObject;
			if (!q?.ContainsKey("pages") ?? false)
			{
				Console.WriteLine("query has no pages");
				return default;
			}
			var pages = q["pages"].First.First;
			var text = pages["extract"].ToObject<string>();
			var title = pages["title"].ToObject<string>();

			// 整形
			// headingを表す == が出てきた当たりで切ればいい感じになる気がする。ダメでも240文字までにトリミングする。
			if (text.IndexOf("==") > 0)
				text = text.Substring(0, text.IndexOf("=="));
			if (text.Length > 240)
				text = text.Substring(0, 240);
			text = text.Replace("\n", "").Replace("\r", "");

			return $@"「{title}」について調べてきたよ〜.
> {text}

出典: https://ja.wikipedia.org/wiki/{HttpUtility.UrlEncode(title)}";
		}

		public static string CreateUrl(string path, params string[] pars) => string.Format(path, pars.Select(HttpUtility.UrlEncode).ToArray());

		public class CalcModel
		{
			[JsonProperty("expression")]
			public string Expression { get; set; }
			[JsonProperty("status")]
			public int Status { get; set; }
			[JsonProperty("message")]
			public string Message { get; set; }
			[JsonProperty("count")]
			public int Count { get; set; }
			[JsonProperty("value")]
			public CalcModelValue[] Value { get; set; }

			public class CalcModelValue
			{
				[JsonProperty("calculatedvalue")]
				public string CalculatedValue { get; set; }
			}
		}


	}
}
