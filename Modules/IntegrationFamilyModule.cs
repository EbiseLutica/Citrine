using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;

namespace Citrine.Core
{
	public class IntegrationFamilyModule : ModuleBase
	{
		public static readonly string CitrineIntegrationInteractedNoteIds = "citrine.integration.interactedNoteIds";
		// 反応する単語。完全一致
		public static readonly string[] Keywords = {
			"アカウント作りました！よろしくね",
		};

		public override async Task<bool> OnTimelineAsync(IPost n, IShell shell, Server core)
		{
			var s = core.GetMyStorage();
			var list = s.Get(CitrineIntegrationInteractedNoteIds, new List<string>());
			var post = n.Repost is not null ? n.Repost : n;

			// 既に反応していれば関わらない
			if (list.Contains(post.Id)) return false;
			// テキスト一致判定
			var isMatch = Keywords.Contains(post.Text?.Trim());
			// うちの子ファミリーのbotであるかどうかの雑判定
			// 名前一致と同一ホストかどうかの判定
			var isFamily = post.User.Name.ToLowerInvariant() == "kaho" && string.IsNullOrEmpty(post.User.Host);
			if (isMatch && isFamily)
			{
				// 既に反応したノートとしてリストアップする
				s.Set(CitrineIntegrationInteractedNoteIds, list);
				list.Add(post.Id);
				await Task.Delay(1200);

				await shell.RepostAsync(post);
				await Task.Delay(800);
				await shell.FollowAsync(post.User);
			}
			return false;
		}
	}
}
