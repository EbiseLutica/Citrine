using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Citrine.Core.Modules
{
	public class SearchModule : ModuleBase
	{
		public override int Priority => -100;

		public static readonly string StatSearchedCount = "stat.searched-count";
		public static readonly string StatCalculatedCount = "stat.calculated-count";

		private static readonly string CalcApiUrl = "http://www.rurihabachi.com/web/webapi/calculator/json?exp={0}";
		private static readonly string WikipediaApiUrl = "https://ja.wikipedia.org/w/api.php?action=query&format=json&prop=extracts&redirects=1&exchars=300&explaintext=1&titles={0}";
		private static readonly string EnWikipediaApiUrl = "https://en.wikipedia.org/w/api.php?action=query&format=json&prop=extracts&redirects=1&exchars=300&explaintext=1&titles={0}";
		private static readonly string NicopediaApiUrl = "http://api.nicodic.jp/page.summary/n/a/{0}";
		private static readonly Regex regexMath = new Regex(@"^([1234567890\+\-\*/\(\)\^piesqrtlogabnc]+?)(っ?て|と?は|[=＝])");
		private static readonly Regex regexPedia = new Regex(@"(.+?)((っ?て|と?は)(何|なに|なん|誰|だれ|どなた|何方)|について(教|おし)えて)");

		private static readonly (string regex, string value)[] myDictionary = {
			("シトリン|citrine|しとりん", "私の名前"),
			("ゼルチカ|ぜるちか|xeltica|ぜるち", "私の生みの親"),
			("生命、?宇宙、?そして万物についての究極の疑問の(答|こた)え|answer to the ultimate question of life,? the universe,? and everything|人類、?宇宙、?(全|すべ)ての(答|こた)え", "42"),
		};

		public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			if (n.User.Id == shell.Myself?.Id)
				return false;
			if (n.Text == default)
				return false;

			var m = regexMath.Match(n.Text.TrimMentions());
			var mpedia = regexPedia.Match(n.Text);
			if (m.Success || mpedia.Success)
			{
				string? response = default;
				string? query = default;
				if (m.Success)
				{
					query = m.Groups[1].Value.TrimMentions();
					response = await FromCalcAsync(query);
					core.Storage[n.User].Add(StatCalculatedCount);
				}
				else if (mpedia.Success)
				{
					query = mpedia.Groups[1].Value.TrimMentions();
					response = FromMyKnowledge(query);
					response ??= await FromWikipediaAsync(query, WikipediaApiUrl, "ja");
					response ??= await FromWikipediaAsync(query, EnWikipediaApiUrl, "en");
					response ??= await FromNicopediaAsync(query);
					core.Storage[n.User].Add(StatSearchedCount);
				}

				response ??= $"{query} について調べてみたけどわからなかった. ごめん...";
				await shell.ReplyAsync(n, response);

				EconomyModule.Pay(n, shell, core);
				core.LikeWithLimited(n.User);
				return true;
			}
			return false;
		}

		private string FromMyKnowledge(string query)
			=> myDictionary.FirstOrDefault(dic => Regex.IsMatch(query, dic.regex)).value;

		private async Task<string?> FromNicopediaAsync(string query)
		{
			var json = await (await Server.Http.GetAsync(CreateUrl(NicopediaApiUrl, query))).Content.ReadAsStringAsync();
			// JSONP なので対策
			json = json.Remove(json.Length - 2, 2).Remove(0, 2);
			if (json.Trim() == "null")
				return null;
			var res = JObject.Parse(json);
			var summary = res["summary"].ToObject<string>();
			var title = res["title"].ToObject<string>().Replace(" ", "_");
			return $@"「{title}」について調べてきたよ〜.
> {summary}...

出典: https://dic.nicovideo.jp/a/{HttpUtility.UrlEncode(title)}
";

		}

		private async Task<string?> FromCalcAsync(string query)
		{
			var res = await (await Server.Http.GetAsync(CreateUrl(CalcApiUrl, query))).Content.ReadAsStringAsync();
			var json = JsonConvert.DeserializeObject<CalcModel>(res);
			if (json.Status > 0)
				return json.Status == 60 ? $"{query} は計算できないよ..." : default;
			return $"{json.Expression} = {json.Value[0].CalculatedValue}";
		}

		public async Task<string?> FromWikipediaAsync(string query, string url, string langCode)
		{
			var res = JObject.Parse(await (await Server.Http.GetAsync(CreateUrl(url, query))).Content.ReadAsStringAsync());
			if (!res.ContainsKey("query"))
			{
				return default;
			}
			var q = res["query"] as JObject;
			if (!q?.ContainsKey("pages") ?? false)
			{
				return default;
			}
			if (q == null) return default;
			var pages = q["pages"].First.First;
			if (pages["extract"] == null)
				return default;
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

出典: https://{langCode}.wikipedia.org/wiki/{HttpUtility.UrlEncode(title.Replace(" ", "_"))}";
		}

		public static string CreateUrl(string path, params string[] pars) => string.Format(path, pars.Select(HttpUtility.UrlEncode).ToArray());

		public class CalcModel
		{
			[JsonProperty("expression")]
			public string Expression { get; set; } = "";
			[JsonProperty("status")]
			public int Status { get; set; }
			[JsonProperty("message")]
			public string Message { get; set; } = "";
			[JsonProperty("count")]
			public int Count { get; set; }
			[JsonProperty("value")]
			public CalcModelValue[] Value { get; set; } = new CalcModelValue[0];

			public class CalcModelValue
			{
				[JsonProperty("calculatedvalue")]
				public string CalculatedValue { get; set; } = "";
			}
		}

	}
}
