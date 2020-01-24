using System.Threading.Tasks;
using Citrine.Core.Api;

namespace Citrine.Core.Modules
{
	public class CommandModule : ModuleBase
	{
		public override int Priority => -10000;
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
					return false;
				}

				if (response != default)
				{
					await shell.ReplyAsync(n, response);
				}
				return true;
			}
			return false;
		}
	}
}
