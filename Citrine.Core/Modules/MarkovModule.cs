using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Citrine.Core.Api;
using Citrine.Core.Modules.Markov;
using Newtonsoft.Json;

namespace Citrine.Core.Modules
{
	public class MarkovModule : ModuleBase
	{
		public MarkovNode Root { get; private set; }
		public List<MarkovNode> Nodes { get; private set; }

		public override async Task<bool> OnTimelineAsync(IPost n, IShell shell, Server core)
		{
			InitializeIfNeeded(core);

			if (n.User == shell.Myself) return false;

			Input(n);
			Save(core);

			return false;
		}

		public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			InitializeIfNeeded(core);
			if (n.Text.IsMatch("(何|な[にん])か[喋話]([しっ]て|せ|れ)"))
			{
				await shell.ReplyAsync(n, Say());
				return true;
			}
			return false;
		}

		public override async Task<bool> OnRepliedContextually(IPost n, IPost context, Dictionary<string, object> store, IShell shell, Server core)
		{
			await shell.ReplyAsync(n, Say());
			return true;
		}

		private void InitializeIfNeeded(Server core)
		{
			if (Root != null)
				return;
			// Nodes を生成する
			Nodes = new List<MarkovNode>();

			if (!File.Exists("markov.root.json"))
			{
				Root = MarkovNode.Start;
				return;
			}

			var serialized = File.ReadAllText("markov.root.json");
			Root = JsonConvert.DeserializeObject<MarkovNode>(serialized, new JsonSerializerSettings
			{
				PreserveReferencesHandling = PreserveReferencesHandling.All,
			});
			Pick(Root.Children);
		}

		private void Pick(List<MarkovNode> nodes)
		{
			nodes.ForEach(node =>
			{
				if (node == MarkovNode.End)
					EON = node;

				if (Nodes.Contains(node)) return;
				Nodes.Add(node);
				Pick(node.Children);
			});
		}

		private void Save(Server core)
		{
			var serialized = JsonConvert.SerializeObject(Root, new JsonSerializerSettings
			{
				PreserveReferencesHandling = PreserveReferencesHandling.All,
			});
			File.WriteAllText("markov.root.json", serialized);
		}

		private string Say()
		{
			var node = Root.Children.Random();
			if (node == null) return null;
			var builder = new StringBuilder();
			while (node != EON)
			{
				builder.Append(node.Value);
				node = node.Children.Random() ?? EON;
			}
			return Filter(builder.ToString());
		}

		private string Filter(string text)
		{
			// 一度形態素解析する
			var tokenized = TinySegmenter.Instance.Segment(text);

			var patternYou = new Regex("^(おまえ|お前|[てお]め[ー〜え]|[テオ]メ[エー〜]|貴様|おぬし|お主|君|きみ)$");
			var patternMe = new Regex("^(俺|おれ|オレ|おら|私|わたく?し|ワ[オイシ]|ぼく|ボク|僕|ワイ|わい|ウチ|うち)$");


			for (var i = 0; i < tokenized.Count; i++)
			{

			}

			return text;
		}

		private void Input(IPost n)
		{
			if (string.IsNullOrEmpty(n.Text) && n.IsRepost)
			{
				n = n.Repost;
			}
			if (!string.IsNullOrEmpty(n.Text) && n.Visiblity == Visiblity.Public)
			{
				// 句点や感嘆符、疑問符などで区切る
				var texts = Regex.Split(n.Text.TrimMentions(), @"([\.。．…‥？！\?!・･]+)");
				for (var i = 0; i < texts.Length; i += 2)
				{
					var text = texts[i];
					if (i + 1 < texts.Length)
						text += texts[i + 1];
					Learn(text);
					logger.Info($"Learned {text}");
				}
			}
			if (n.Reply is IPost)
				Input(n.Reply);
			if (n.Repost is IPost)
				Input(n.Repost);
		}

		private void Learn(string text)
		{
			if (string.IsNullOrEmpty(text)) return;

			var tokenized = TinySegmenter.Instance.Segment(text);

			var prevNode = Root;
			foreach (var token in tokenized)
			{
				var node = GetOrCreateNode(token);
				prevNode.Children.Add(node);
				prevNode = node;
			}
			prevNode.Children.Add(EON);
		}

		private MarkovNode GetOrCreateNode(string token)
		{
			if (Nodes.FirstOrDefault(n => n.Value == token) is MarkovNode n)
				return n;

			var newNode = new MarkovNode(token);
			Nodes.Add(newNode);
			return newNode;
		}
		private Logger logger = new Logger(nameof(MarkovModule));

		private MarkovNode EON = MarkovNode.End;

		private Server core;
	}
}