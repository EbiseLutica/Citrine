#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
using System.Collections.Generic;
using System.Threading.Tasks;
using Citrine.Core.Api;

namespace Citrine.Core.Modules
{
	/// <summary>
	/// Citrine のモジュールベース。
	/// </summary>
	public abstract class ModuleBase
	{
		public virtual int Priority => 0;

		public virtual async Task<bool> ActivateAsync(IPost n, IShell shell, Server core) => false;

		public virtual async Task<bool> OnTimelineAsync(IPost n, IShell shell, Server core) => false;

		public virtual async Task<bool> OnDmReceivedAsync(IPost n, IShell shell, Server core) => await ActivateAsync(n, shell, core);

		public virtual async Task<bool> OnFollowedAsync(IUser user, IShell shell, Server core) => false;

		public virtual async Task<bool> OnRepliedContextually(IPost n, IPost context, Dictionary<string, object> store, IShell shell, Server core) => false;
	}
}
