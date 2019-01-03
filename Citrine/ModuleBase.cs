using System.Threading.Tasks;
using Disboard.Misskey;
using Disboard.Misskey.Models;

namespace Citrine
{
	/// <summary>
	/// Citrine のモジュールベース。
	/// </summary>
	public abstract class ModuleBase
	{
		public virtual int Priority => 0;

		public virtual async Task<bool> ActivateAsync(Note n, MisskeyClient mi, Citrine core) => false;

		public virtual async Task<bool> OnTimelineAsync(Note n, MisskeyClient mi, Citrine core) => false;
	}
}