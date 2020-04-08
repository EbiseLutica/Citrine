using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;

namespace Citrine.Core.Modules
{
	public class CommandModule : ModuleBase
	{
		public override int Priority => -10000;
		public static readonly string StatCommandUsedCount = "stat.command.used-count";
		public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			var t = n.Text?.TrimMentions();
			if (string.IsNullOrEmpty(t))
				return false;
			if (t.StartsWith("/"))
			{
				string response;
				try
				{
					response = await core.ExecCommand(new PostCommandSender(n, core.IsSuperUser(n.User)), t);
					core.Storage[n.User].Add(StatCommandUsedCount);
				}
				catch (AdminOnlyException)
				{
					response = "ごめん, そのコマンドは管理者以外に言われても実行するなと言われてるの";
				}
				catch (LocalOnlyException)
				{
					response = "このコマンドは同じインスタンスのユーザーしか使えないよ. ごめんね.";
				}
				catch (RemoteOnlyException)
				{
					response = "このコマンドは違うインスタンスのユーザーしか使えないよ. ごめんね.";
				}
				catch (NoSuchCommandException)
				{
					response = $"No such command.";
				}

				if (response != default)
				{
					await shell.ReplyAsync(n, response);
				}
				EconomyModule.Pay(n, shell, core);
				return true;
			}
			return false;
		}
	}
}
