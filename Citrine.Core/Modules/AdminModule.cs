using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Citrine.Core.Api;

namespace Citrine.Core.Modules
{
	public class AdminModule : ModuleBase
    {
        public override async Task<bool> ActivateAsync(IPost n, IShell shell, Server core)
        {
            if (n.Text == null)
                return false;

            var text = n.Text.TrimMentions();

            var cmd = text.Split(' ');

            if (text.StartsWith("/stop", StringComparison.Ordinal))
            {
                if (core.IsAdmin(n.User))
                {
                    await shell.ReplyAsync(n, "またねー。");
                    // good bye
                    Environment.Exit(0);
                }
                else
                {
                    var mes = core.GetRatingOf(n.User) == Rating.Partner ? "いくらあなたでも, その頼みだけは聞けない. ごめんね..." : "申し訳ないけど, 他の人に言われてもするなって言われてるから...";
                    await shell.ReplyAsync(n, mes);
                }
                return true;
            }

            if (text.StartsWith("/modules", StringComparison.Ordinal) || text.StartsWith("/mods", StringComparison.Ordinal))
            {
                var mods = core.Modules.Select(mod => mod.GetType().Name);
                await shell.ReplyAsync(n, string.Join(",", mods), $"モジュール数: {mods.Count()}");
                return true;
            }

            if (text.StartsWith("/version", StringComparison.Ordinal) || text.StartsWith("/v", StringComparison.Ordinal))
            {
                await shell.ReplyAsync(n, $"Citrine v{Server.Version} / XelticaBot v{Server.VersionAsXelticaBot}");
                return true;
            }

            const string usage = "/love <inc|dec|set|query> <id> <amount>";
            if (cmd[0] == "/love")
            {
                var output = usage;
                if (core.IsAdmin(n.User))
                {
                    if (cmd.Length > 2)
                    {
                        if (cmd[2] == "me")
                            cmd[2] = n.User.Id;
                        switch (cmd[1])
                        {
                            case "inc":
                                if (cmd.Length > 3)
                                {
                                    core.Like(cmd[2], int.Parse(cmd[3]));
                                    output = "OK";
                                }
                                break;
                            case "dec":
                                if (cmd.Length > 3)
                                {
                                    core.Dislike(cmd[2], int.Parse(cmd[3]));
                                    output = "OK";
                                }
                                break;
                            case "set":
                                if (cmd.Length > 3)
                                {
                                    core.Like(cmd[2], int.Parse(cmd[3]) - core.GetRatingNumber(cmd[2]));
                                    output = "OK";
                                }
                                break;
                            case "query":
                                output = core.GetRatingNumber(cmd[2]).ToString();
                                break;
                        }
                    }
                }
                else
                {
                    output = "permission denied";
                }
                await shell.ReplyAsync(n, output);
                return true;
            }
            return false;
        }

        public override Task<bool> OnDmReceivedAsync(IPost n, IShell shell, Server core) => ActivateAsync(n, shell, core);
	}
}