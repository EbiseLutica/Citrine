#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
#pragma warning disable CS4014 // この呼び出しは待機されなかったため、現在のメソッドの実行は呼び出しの完了を待たずに続行されます

using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Citrine.Core;
using Citrine.Core.Api;
using Citrine.Core.Modules;
using Disboard.Exceptions;
using Disboard.Misskey.Models;

namespace Citrine.Misskey
{
	/// <summary>
	/// Toggling features command
	/// </summary>
	public class ToggleFeatureCommand : CommandBase
	{
		public override string Name => "config";

		public override string Usage => @"使い方:
/config <config-name> <on/off>";

		public override string Description => "コンフィグを行います。";

		public override PermissionFlag Permission => PermissionFlag.AdminOnly;

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			return "Not implemented.";
		}
	}
}
