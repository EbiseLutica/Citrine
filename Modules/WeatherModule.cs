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
		const string endPoint = "http://weather.livedoor.com/forecast/webservice/json/v1?city=";
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
				EconomyModule.Pay(n, shell, core);
				core.LikeWithLimited(n.User);
				var place = m.Groups[1].Value;
				var pair = areaDefinitions.FirstOrDefault(k => place.Contains(k.name));
				if (pair.id == default)
				{
					await shell.ReplyAsync(n, $"ごめん, {place}という場所は聞いたことがないのでわからない.");
					return true;
				}

				var res = await cli.GetAsync(endPoint + pair.id);
				var tenki = JsonConvert.DeserializeObject<TenkiModel>(await res.Content.ReadAsStringAsync());
				var result = $@"{RenderForecasts(tenki.Forecasts)}
> {tenki.Description.Text}

{tenki.Link}";
				await shell.ReplyAsync(n, result, $"[ {tenki.Title ?? "NULL"} ]");
				core.Storage[n.User].Add(StatForecastCount);
				return true;
			}
			return false;
		}

		public override Task<bool> OnDmReceivedAsync(IPost n, IShell shell, Server core) => ActivateAsync(n, shell, core);

		private string RenderForecasts(IEnumerable<TenkiModel.Forecast> forecasts)
		{
			var builder = new StringBuilder();
			foreach (var forecast in forecasts)
			{
				builder
					.Append(forecast.DateLabel)
					.Append(": ")
					.Append(RenderTelop(forecast.Telop))
					.Append("　")
					.Append(forecast.Temperature?.Max?.Celsius ?? "--")
					.Append(" / ")
					.Append(forecast.Temperature?.Min?.Celsius ?? "--")
					.AppendLine();
			}
			return builder.ToString();
		}

		private string RenderTelop(string telop)
		{
			return telop.Replace("時々", "／")
						.Replace("のち", "→")
						.Replace("晴れ", "☀")
						.Replace("晴", "☀")
						.Replace("曇り", "☁")
						.Replace("曇", "☁")
						.Replace("雪", "⛄")
						.Replace("雨", "☔");
		}

		private readonly HttpClient cli = new HttpClient() { };
		private readonly (string name, string id)[] areaDefinitions = new[]
		{
			( "北海道", "016010" ),
			( "稚内", "011000" ),
			( "旭川", "012010" ),
			( "留萌", "012020" ),
			( "網走", "013010" ),
			( "北見", "013020" ),
			( "紋別", "013030" ),
			( "根室", "014010" ),
			( "釧路", "014020" ),
			( "帯広", "014030" ),
			( "室蘭", "015010" ),
			( "浦河", "015020" ),
			( "札幌", "016010" ),
			( "岩見沢", "016020" ),
			( "倶知安", "016030" ),
			( "函館", "017010" ),
			( "江差", "017020" ),
			( "青森", "020010" ),
			( "むつ", "020020" ),
			( "八戸", "020030" ),
			( "岩手", "030010" ),
			( "盛岡", "030010" ),
			( "宮古", "030020" ),
			( "大船渡", "030030" ),
			( "宮城", "040010" ),
			( "仙台", "040010" ),
			( "白石", "040020" ),
			( "秋田", "050010" ),
			( "秋田", "050010" ),
			( "横手", "050020" ),
			( "山形", "060010" ),
			( "米沢", "060020" ),
			( "酒田", "060030" ),
			( "新庄", "060040" ),
			( "福島", "070010" ),
			( "小名浜", "070020" ),
			( "若松", "070030" ),
			( "茨城", "080010" ),
			( "水戸", "080010" ),
			( "土浦", "080020" ),
			( "栃木", "090010" ),
			( "宇都宮", "090010" ),
			( "大田原", "090020" ),
			( "群馬", "100010" ),
			( "前橋", "100010" ),
			( "みなかみ", "100020" ),
			( "埼玉", "110010" ),
			( "さいたま", "110010" ),
			( "熊谷", "110020" ),
			( "秩父", "110030" ),
			( "千葉", "120010" ),
			( "銚子", "120020" ),
			( "館山", "120030" ),
			( "東京", "130010" ),
			( "大島", "130020" ),
			( "八丈島", "130030" ),
			( "父島", "130040" ),
			( "神奈川", "140010" ),
			( "横浜", "140010" ),
			( "小田原", "140020" ),
			( "新潟", "150010" ),
			( "長岡", "150020" ),
			( "高田", "150030" ),
			( "相川", "150040" ),
			( "富山", "160010" ),
			( "伏木", "160020" ),
			( "石川", "170010" ),
			( "金沢", "170010" ),
			( "輪島", "170020" ),
			( "福井", "180010" ),
			( "敦賀", "180020" ),
			( "山梨", "190010" ),
			( "甲", "190010" ),
			( "河口湖", "190020" ),
			( "長野", "200010" ),
			( "松本", "200020" ),
			( "飯田", "200030" ),
			( "岐阜", "210010" ),
			( "高山", "210020" ),
			( "静岡", "220010" ),
			( "網代", "220020" ),
			( "三島", "220030" ),
			( "浜松", "220040" ),
			( "愛知", "230010" ),
			( "名古屋", "230010" ),
			( "豊橋", "230020" ),
			( "三重", "240010" ),
			( "津", "240010" ),
			( "尾鷲", "240020" ),
			( "滋賀", "250010" ),
			( "大津", "250010" ),
			( "彦根", "250020" ),
			( "京都", "260010" ),
			( "舞鶴", "260020" ),
			( "大阪", "270000" ),
			( "兵庫", "280010" ),
			( "神戸", "280010" ),
			( "豊岡", "280020" ),
			( "奈良", "290010" ),
			( "風屋", "290020" ),
			( "和歌山", "300010" ),
			( "潮岬", "300020" ),
			( "鳥取", "310010" ),
			( "米子", "310020" ),
			( "島根", "320010" ),
			( "松江", "320010" ),
			( "浜田", "320020" ),
			( "西郷", "320030" ),
			( "岡山", "330010" ),
			( "津山", "330020" ),
			( "広島", "340010" ),
			( "庄原", "340020" ),
			( "下関", "350010" ),
			( "山口", "350020" ),
			( "柳井", "350030" ),
			( "萩", "350040" ),
			( "徳島", "360010" ),
			( "日和佐", "360020" ),
			( "香川", "370000" ),
			( "愛媛", "380010" ),
			( "新居浜", "380020" ),
			( "宇和島", "380030" ),
			( "高知", "390010" ),
			( "室戸岬", "390020" ),
			( "清水", "390030" ),
			( "福岡", "400010" ),
			( "八幡", "400020" ),
			( "飯塚", "400030" ),
			( "久留米", "400040" ),
			( "佐賀", "410010" ),
			( "伊万里", "410020" ),
			( "長崎", "420010" ),
			( "佐世保", "420020" ),
			( "厳原", "420030" ),
			( "福江", "420040" ),
			( "熊本", "430010" ),
			( "阿蘇乙姫", "430020" ),
			( "牛深", "430030" ),
			( "人吉", "430040" ),
			( "大分", "440010" ),
			( "中津", "440020" ),
			( "日田", "440030" ),
			( "佐伯", "440040" ),
			( "宮崎", "450010" ),
			( "延岡", "450020" ),
			( "都城", "450030" ),
			( "高千穂", "450040" ),
			( "鹿児島", "460010" ),
			( "鹿屋", "460020" ),
			( "種子島", "460030" ),
			( "名瀬", "460040" ),
			( "沖縄", "471010" ),
			( "那覇", "471010" ),
			( "名護", "471020" ),
			( "久米島", "471030" ),
			( "南大東", "472000" ),
			( "宮古島", "473000" ),
			( "石垣島", "474010" ),
			( "与那国島", "474020" )
		}.OrderByDescending(rec => rec.Item1.Length).ToArray();

		class TenkiModel
		{
			[JsonProperty("pinpointLocations")]
			public List<PinpointLocation> PinpointLocations { get; set; } = new List<PinpointLocation>();

			[JsonProperty("link")]
			public string Link { get; set; } = "";

			[JsonProperty("forecasts")]
			public List<Forecast> Forecasts { get; set; } = new List<Forecast>();

			[JsonProperty("title")]
			public string Title { get; set; } = "";

			[JsonProperty("description")]
			public Desc Description { get; set; } = new Desc();

			public class PinpointLocation
			{
				[JsonProperty("link")]
				public string Link { get; set; } = "";

				[JsonProperty("name")]
				public string Name { get; set; } = "";
			}

			public class Forecast
			{
				[JsonProperty("dateLabel")]
				public string DateLabel { get; set; } = "";

				[JsonProperty("telop")]
				public string Telop { get; set; } = "";

				[JsonProperty("date")]
				public string Date { get; set; } = "";

				[JsonProperty("temperature")]
				public TemperatureModel Temperature { get; set; } = new TemperatureModel();

				public class TemperatureModel
				{
					[JsonProperty("min")]
					public TemperatureChild Min { get; set; } = new TemperatureChild();
					[JsonProperty("max")]
					public TemperatureChild Max { get; set; } = new TemperatureChild();

					public class TemperatureChild
					{
						[JsonProperty("celsius")]
						public string Celsius { get; set; } = "";
						[JsonProperty("fahrenheit")]
						public string Fahrenheit { get; set; } = "";
					}
				}
			}

			public class Desc
			{
				[JsonProperty("text")]
				public string Text { get; set; } = "";
			}
		}
	}
}
