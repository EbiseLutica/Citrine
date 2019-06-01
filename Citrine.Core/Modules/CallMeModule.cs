using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Citrine.Core.Api;

namespace Citrine.Core.Modules
{
    public class CallMeModule : ModuleBase
    {
        public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
        {
            if (n.Text == null)
                return false;
            var m = Regex.Match(n.Text.TrimMentions(), @"(.+)(って|と)呼[べびん]");
            if (m.Success)
            {
                var nick = m.Groups[1].Value;
                core.SetNicknameOf(n.User, nick);
                await shell.ReplyAsync(n, $"わかった. これからは君のことを{core.GetNicknameOf(n.User)}と呼ぶね.");
                return true;
            }
            return false;
        }
    }
}
