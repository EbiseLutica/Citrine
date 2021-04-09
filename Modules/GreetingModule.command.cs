using System;
using System.Linq;
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;

namespace Citrine.Core.Modules
{
	public partial class GreetingModule
	{
		public string Name => "greetings";

		public string[] Aliases => Array.Empty<string>();

		public bool IgnoreCase => false;

		public PermissionFlag Permission => PermissionFlag.AdminOnly;

		public string Usage => "";

		public string Description => "";

		public async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			await Task.Delay(0);
			return string.Join(", ", patterns.Select(p => p.Regex));
		}
	}
}
