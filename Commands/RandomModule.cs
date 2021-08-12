#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BotBone.Core;
using BotBone.Core.Api;
using BotBone.Core.Modules;
using Newtonsoft.Json;

namespace Citrine.Core
{
	public class RandomModule : CommandBase
	{
		public override string Name => "random";

		public override string Usage => "/random <items...>";

		public override string Description => "指定した引数の中からどれか1つを返します。";

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			if (args.Length == 0) throw new CommandException();

			return args.Random();
		}
	}
}
