#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます

using System.Linq;
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;

namespace Citrine.Core
{
	public class BalloonGenCommand : CommandBase
	{
		public override string Name => "balloongen";

		public override string Usage => "/balloongen <text>";

		public override string[] Aliases { get; } = { "balloon-gen", "balloon", "genballoon" };

		public override string Description => "Make an ASCII-art balloon.";

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			body = "　" + body + "　";
			var crown = ".\n＿" + new string('人', body.Length) + "＿\n";
			var pate = "＞" + body + "＜\n";
			var heel = "￣" + string.Concat(Enumerable.Repeat("Y^", body.Length - 1)) + "Y￣";
			return crown + pate + heel;
		}
	}
}
