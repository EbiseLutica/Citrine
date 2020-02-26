using System.Threading.Tasks;
using Citrine.Core.Api;

namespace Citrine.Core.Modules
{
	public class EmptyMentionHandlerModule : ModuleBase
	{
		public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
		{
			// n.Text.TrimMentions() がカラッポであり
			// n.Reply.User.Id == n.User.Id であればハンドルする
			if (string.IsNullOrEmpty(n.Text?.TrimMentions()) && n.Reply?.User.Id == n.User.Id)
			{
				await core.HandleMentionAsync(n.Reply);
				return true;
			}
			return false;
		}
	}
}