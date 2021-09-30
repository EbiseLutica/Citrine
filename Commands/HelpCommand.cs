#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;

namespace Citrine.Core
{
	public class HelpCommand : CommandBase
	{
		public override string Name => "help";

		public override string Usage => "/help [name]";

		public override string Description => "Show help.";

		public override string[] Aliases { get; } = { "h" };

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			if (args.Length == 0)
			{
				var descriptions = core.Commands.Select(GetDescription);

				return string.Join("\n", descriptions);
			}
			else
			{
				var name = args[0];
				var cmd = core.TryGetCommand(name);
				if (cmd == null)
					return $"No such command named {name}.";
				var sb = new StringBuilder();
				sb.AppendLine(GetDescription(cmd));
				sb.Append("Usage: ");
				sb.AppendLine(cmd.Usage);
				if (cmd.Aliases != null)
					sb.AppendLine(string.Join(", ", cmd.Aliases));
				sb.AppendLine(DumpPermission(cmd.Permission));

				return sb.ToString();
			}
		}

		private string GetDescription(ICommand cmd) => $"/{cmd.Name} - {cmd.Description ?? "No such descriptions."}";

		private string DumpPermission(PermissionFlag flag)
		{
			var sb = new StringBuilder();
			if (flag.HasFlag(PermissionFlag.AdminOnly))
				sb.Append("(Admin Only)");
			if (flag.HasFlag(PermissionFlag.LocalOnly))
				sb.Append("(Local User Only)");
			return sb.ToString();
		}
	}
}
