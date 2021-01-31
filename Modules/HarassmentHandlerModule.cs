using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;
using Newtonsoft.Json;

namespace Citrine.Core.Modules
{
    /* === リプライ文字列の仕様 ===
	 * $user$ は相手のユーザー名, もしくはニックネームに置き換わる
	 * $prefix$ はラッキーアイテムの修飾子辞書からランダムに取る
	 * $item$ はラッキーアイテム辞書からランダムに取る
	 * $rndA,B$はAからBまでの乱数
	 */
    public class HarassmentHandlerModule : ModuleBase
    {
        // コマンドよりも優先的
        public override int Priority => -10005;

        public List<string> NgWords { get; } = new List<string>();

        public List<string> ExcludedWords { get; } = new List<string>();

        public HarassmentHandlerModule()
        {
            using var reader = new StreamReader(GetType().Assembly.GetManifestResourceStream("Citrine.Resources.ngwords.txt"));
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine().Trim().ToLowerInvariant().ToHiragana();
                if (line.StartsWith("#"))
                    continue;
                if (line.StartsWith("-"))
                {
                    ExcludedWords.Add(line[1..]);
                }
                else
                {
                    NgWords.Add(line);
                }
            }
        }

        public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
        {
            if (n.Text == null)
                return false;

            if (IsHarassmented(n.Text))
            {
                // セクハラ
                // switch (core.GetRatingOf(n.User))
                // {
                //     case Rating.Hate:
                //         await shell.ReplyAsync(n, replyHate.Random());
                //         break;
                //     case Rating.Normal:
                //     case Rating.Like:
                //         await shell.ReplyAsync(n, reply.Random());
                //         break;
                //     case Rating.BestFriend:
                //     case Rating.Partner:
                //         await shell.ReplyAsync(n, replyLove.Random());
                //         return true;
                // }
				await shell.ReactAsync(n, reactions.Random());
                // core.OnHarassment(n.User, 1);
                return true;
            }
            return false;
        }

        public override async Task<bool> OnTimelineAsync(IPost n, IShell shell, Server core)
        {
            await Task.Delay(0);
            // セクハラ投稿であれば見なかったことにする
            return n.Text is string t && IsHarassmented(t);
        }


        public bool IsHarassmented(string text)
        {
            text = Regex.Replace(text.TrimMentions(), @"[\s\.,/／○●◯]", "").ToLowerInvariant().ToHiragana();
            foreach (var w in ExcludedWords)
                text = text.Replace(w.ToHiragana(), "");
            var m = "";
            var res = NgWords.Any(w =>
            {
                m = w;
                return text.Contains(w.ToHiragana());
            });
            if (res)
            {
                logger.Info($"Detected NG word '{m}' at the text '{text}'");
            }
            return res;
        }

        private readonly string[] replyHate =
        {
            "はぁ...なんでそんなことしか言えないの？",
            "ほんとキモい",
            "最低",
			"いい加減にして，そういう言葉嫌いだって言ってますよね",
            "...",
			"もうしりません",
        };

        private readonly string[] reply =
        {
            "ねぇ，そういう言葉嫌ですよ",
            "そういうのいやですー",
			"うーん，セクハラですか?",
        };

        private readonly string[] replyLove =
        {
			"すぐ，そういうこと言うんだから",
            "下ネタ好きですか",
            "笑",
			"そうだねー"
        };

		private readonly string[] reactions = {
			"🥴", "🤔", "😇", "🤯"
		};

        private readonly Logger logger = new Logger(nameof(HarassmentHandlerModule));
    }

}
