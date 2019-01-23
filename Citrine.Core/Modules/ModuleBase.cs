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
	}
}