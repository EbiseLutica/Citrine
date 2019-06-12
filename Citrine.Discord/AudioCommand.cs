using System.Threading.Tasks;
using Citrine.Core;

namespace Citrine.Discord
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using Citrine.Core.Api;
	using global::Discord;

	public class AudioCommand : CommandBase
	{
		public override string Name => "audio";

		public override string Description => "音楽を再生します。";

		public override string Usage => @"/audio play <url> - 再生
/audio skip - スキップ
/audio queue [page] - キューを見る
/audio clear - キューを全消し
/audio np - 再生中の曲表示
/audio summon [channel name] - MusicBot 参上";

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			if (!(sender is PostCommandSender p))
			{
				return "call from post";
			}

			// サブコマンドは必須
			Assert(args.Length > 0);

			var s = shell as Shell;

			var subCommand = args.First();

			// サブコマンドを除いたものを引数と再定義
			args = args.Skip(1).ToArray();

			var post = (p.Post as DCPost).Native;

			if (post.Channel is IGuildChannel ch)
			{
				var id = ch.GuildId;
				servers.TryGetValue(id, out var srv);
				if (srv == null)
				{
					srv = servers[id] = new AudioServer(s);
				}

				srv.LogChannel = post.Channel;
				switch (subCommand.ToLowerInvariant())
				{
					case "summon":
						var vcs = await ch.Guild.GetVoiceChannelsAsync();
						var vc = args.Length >= 1
							? vcs.FirstOrDefault(c => c.Name == args[0])
							: vcs.FirstOrDefault();
						srv.SummonAsync(vc);
						break;
					case "play":
						{
							Assert(args.Length >= 1);
							try
							{
								var info = await srv.PlayAsync(string.Concat(args).Trim(), p.User);

								var embed = new EmbedBuilder()
									.WithAuthor("キューに追加しました👍")
									.WithTitle(info.Title)
									.WithThumbnailUrl(info.ThumbnailUrl)
									.WithUrl(info.WebpageUrl)
									.WithFields(
										Field("作者", info.Uploader),
										Field("時間", ToTimeString(info.Duration)),
										Field("再生まで残り", ToTimeString(srv.AudioQueue.Sum(i => i.Duration) - info.Duration)),
										Field("リクエスト", core.GetNicknameOf(p.User))
									).Build();
								await post.Channel.SendMessageAsync(embed: embed);
							}
							catch (System.Exception ex)
							{
								return $"エラーが起きちゃった. {ex.GetType().Name}: {ex.Message}";
							}
							break;
						}
					case "skip":
						srv.Skip();
						break;
					case "queue":
						{
							const int itemsPerPage = 10;
							var embed = new EmbedBuilder()
								.WithTitle("キュー")
								.WithFooter("DJ Citrine");
							var sb = new StringBuilder();
							if (srv.CurrentAudio is MusicInfo mi)
							{
								sb.AppendLine($"\n__**再生中:**__ [{mi.Title}]({mi.WebpageUrl}) ({ToTimeString(mi.Duration)}, 追加者: {core.GetNicknameOf(mi.AddedBy)})\n\n-----\n\n**__再生予定:__**\n");
							}
							else
							{
								sb.AppendLine("\n__**なにも再生していません**__\n\n-----\n\n**__再生予定:__**\n");
							}
							var totalPage = srv.AudioQueue.Count / itemsPerPage + 1;
							var page = 1;
							if (args.Length > 0)
								int.TryParse(args[0], out page);
							if (page < 1)
								page = 1;
							if (page > totalPage)
								page = totalPage;
							var items = srv.AudioQueue.Skip((page - 1) * itemsPerPage).Take(itemsPerPage);
							if (items.Any())
							{
								var i = (page - 1) * itemsPerPage + 1;
								foreach (var item in items)
								{
									sb.AppendLine($"{i++}. [{item.Title}]({item.WebpageUrl}) ({ToTimeString(item.Duration)}, 追加者: {core.GetNicknameOf(item.AddedBy)})");
								}
								sb.AppendLine($"\nページ {page} / {totalPage}");
							}
							embed.WithDescription(sb.ToString());
							await post.Channel.SendMessageAsync(embed: embed.Build());
							break;
						}
					case "clear":
						srv.Clear();
						return "キューをカラッポにしました.";
					case "np":
						return "工事中";
				}
			}
			else
			{
				return "ここはサーバーではないから, それはできないよ.";
			}
			throw new CommandException();
		}

		private string ToTimeString(int duration)
		{
			var ts = TimeSpan.FromSeconds(duration);
			var sb = new StringBuilder();
			if (ts.Hours > 0)
				sb.Append(ts.Hours).Append(':');
			sb.Append($"{ts.Minutes:00}:{ts.Seconds:00}");
			return sb.ToString();
		}

		private void Assert(bool cond)
		{
			if (!cond) throw new CommandException();
		}

		private EmbedFieldBuilder Field(string name, string value) => new EmbedFieldBuilder
		{
			Name = name,
			Value = value
		};

		private readonly Dictionary<ulong, AudioServer> servers = new Dictionary<ulong, AudioServer>();
	}
}
