#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
using System.Collections.Generic;
using System.Threading.Tasks;
using Citrine.Core.Api;

namespace Citrine.Core.Modules
{
	/// <summary>
	/// これを実装することで、モジュールとして動作します。
	/// </summary>
	public interface IModule
	{
		int Priority { get; }

		Task<bool> ActivateAsync(IPost n, IShell shell, Server core);
		Task<bool> OnDmReceivedAsync(IPost n, IShell shell, Server core);
		Task<bool> OnFollowedAsync(IUser user, IShell shell, Server core);
		Task<bool> OnRepliedContextually(IPost n, IPost? context, Dictionary<string, object> store, IShell shell, Server core);
		Task<bool> OnTimelineAsync(IPost n, IShell shell, Server core);
	}
}
