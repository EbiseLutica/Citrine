using System.Text;
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;

namespace Citrine.Core.Modules
{
	public class StatusModule : ModuleBase
	{
		public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			var storage = core.Storage[n.User];

			int StatOf(string key) => storage.Get(key, 0);

			static string GetRatingString(Rating r) => r switch
			{
				Rating.Partner => "大好き",
				Rating.BestFriend => "親友",
				Rating.Like => "仲良し",
				Rating.Normal => "普通",
				Rating.Hate => "キライ",
				_ => "不明",
			};

			if (n.Text != null && n.Text.IsMatch("ステータスカード"))
			{
				var builder = new StringBuilder();
				builder.AppendLine($"呼び名: {core.GetNicknameOf(n.User)}");
				builder.AppendLine($"なかよし度: {GetRatingString(core.GetRatingOf(n.User))}");
				builder.AppendLine($"所持金: {storage.Get("economy.balance", 0)}クォーツ");
				builder.AppendLine($"占った回数: {StatOf(FortuneModule.StatFortuneCount)}回");
				builder.AppendLine($"会話した回数: {StatOf(GreetingModule.StatTalkedCount)}回");
				builder.AppendLine($"どこ〜って言われた回数: {StatOf(ImHereModule.StatImHereCount)}回");
				builder.AppendLine($"じゃんけんで勝った回数: {StatOf(JankenModule.StatWinCount)}回");
				builder.AppendLine($"じゃんけんで負けた回数: {StatOf(JankenModule.StatLoseCount)}回");
				builder.AppendLine($"セクハラした回数: {StatOf(StorageKey.HarrasmentedCount)}回");
				builder.AppendLine($"悪口を言った回数: {StatOf(ReactModule.StatBadMouthCount)}回");
				builder.AppendLine($"寿司を握った回数: {StatOf(SushiModule.StatSushiCount)}回");
				builder.AppendLine($"計算回数: {StatOf(SearchModule.StatCalculatedCount)}回");
				builder.AppendLine($"言葉を調べた回数: {StatOf(SearchModule.StatSearchedCount)}回");
				builder.AppendLine($"バレンタインチョコを貰った回数: {StatOf(ValentineModule.StatValentineCount)}回");
				await shell.ReplyAsync(n, builder.ToString(), $"{core.GetNicknameOf(n.User)}のステータスカードを作ったよ");
				return true;
			}
			return false;
		}
	}
}
