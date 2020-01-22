using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Citrine.Core;
using Citrine.Core.Api;
using Citrine.Core.Modules;
using Google.Cloud.Vision.V1;

namespace Citrine.Misskey
{
	public class AttachmentHandlerModule : ModuleBase
	{
		public override int Priority => -1010;

		public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			if (n.Attachments.Count > 0)
			{
				if (Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS") == null)
				{
					logger.Warn("GOOGLE_APPLICATION_CREDENTIALS 環境変数が設定されていないため、このモジュールをご利用いただけません。");
					return false;
				}
				var a = n.Attachments.First();
				var file = (a as MiAttachment).Native;
				var path =
					// 写真の場合普通にURL
					file.Type.StartsWith("image")
						? file.Url :
					// 動画の場合はサムネイルURL
					file.Type.StartsWith("video") && !string.IsNullOrEmpty(file.ThumbnailUrl)
						? file.ThumbnailUrl : null;

				// 変なファイルなら処理しない
				if (path == null)
					return false;

				var img = await Image.FetchFromUriAsync(path, Server.Http);
				var cli = await ImageAnnotatorClient.CreateAsync();
				var labels = await cli.DetectLabelsAsync(img);
				var output = new StringBuilder();
				if (labels.Count == 0)
				{
					output.Append("ごめん, 何の画像かよくわからなかった.");
				}
				else
				{
					var label = labels.First();
					var hasConfidence = label.Score >= .5f;
					output.Append(hasConfidence ? "わかった! これは, " : "うーん, 多分これは");
					output.Append(core.ExecCommand("/translate en ja " + label.Description));
					output.Append(hasConfidence ? "!" : "かなー?");
					if (labels.Count >= 2)
					{
						label = labels.Skip(1).First();
						hasConfidence = label.Score >= .5f;
						output.Append(hasConfidence ? " または," : " それとも, 多分これは");
						output.Append(core.ExecCommand("/translate en ja " + label.Description));
						output.Append(hasConfidence ? "!" : "?");
					}
				}
				await shell.ReplyAsync(n, output.ToString());
				return true;
			}
			return false;
		}
		private Logger logger = new Logger(nameof(AttachmentHandlerModule));
	}
}