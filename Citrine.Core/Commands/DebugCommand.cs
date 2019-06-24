#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます

using System.Linq;
using System.Threading.Tasks;
using Citrine.Core.Api;
using Citrine.Core.Modules;

namespace Citrine.Core
{
	public class DebugCommand : CommandBase
	{
		public override string Name => "debug";

		public override string Usage => "/debug";

		public override PermissionFlag Permission => PermissionFlag.AdminOnly;

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			// 1: block & unblock
			// 2: mute & unmute
			// 3: post & delete
			// 4: create vote
			// 5: unfollow
			// 6: create file

			if (!(sender is PostCommandSender p)) return "call from post";

			switch (args[0])
			{
				case "1":
					await shell.BlockAsync(p.User);
					await Task.Delay(5000);
					await shell.UnblockAsync(p.User);
					return "success";
				case "2":
					await shell.MuteAsync(p.User);
					await Task.Delay(5000);
					await shell.UnmuteAsync(p.User);
					return "success";
				case "3":
					var post = await shell.PostAsync("This post is for debugging.");
					await Task.Delay(5000);
					await shell.DeletePostAsync(post);
					return "success";
				case "4":
					if (!shell.CanCreatePoll)
						break;
					await shell.PostAsync("好きなものは?", null, choices: Enumerable.Repeat(0, 4).Select(_ => FortuneExtension.GenerateWord()).ToList());
					break;
				case "5":
					await shell.UnfollowAsync(p.User);
					return "success";
				case "6":
					await shell.ReplyWithFilesAsync(p.Post, ".", filePaths: args.Skip(1).ToList());
					break;
			}

			return "";
		}
	}
}
