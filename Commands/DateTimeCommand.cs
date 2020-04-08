#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
using System;
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;

namespace Citrine.Core
{
	public class DateTimeCommand : CommandBase
	{
		public override string Name => "datetime";

		public override string Usage => "/datetime";

		public override string Description => "現在時刻を返します。";

		public override string[] Aliases => new[] { "date", "time", "dt" };

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			return DateTime.Now.ToString();
		}
	}
}
