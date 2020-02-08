using System.Threading.Tasks;
using Citrine.Core.Api;

namespace Citrine.Core
{
	/// <summary>
	/// コマンドの基底クラスです。
	/// </summary>
	public abstract class CommandBase : ICommand
	{
		/// <summary>
		/// コマンド名を取得します。
		/// </summary>
		public abstract string Name { get; }

		/// <summary>
		/// コマンドのエイリアス一覧を取得します。
		/// </summary>
		public virtual string[] Aliases { get; } = new string[0];

		/// <summary>
		/// コマンド名の大文字小文字を区別しないかどうかを示す値を取得します。
		/// </summary>
		public virtual bool IgnoreCase => true;

		/// <summary>
		/// このコマンドの権限フラグを取得します
		/// </summary>
		public virtual PermissionFlag Permission => PermissionFlag.Any;

		/// <summary>
		/// コマンドの実行に失敗したときに表示される文字列を取得します。
		/// </summary>
		public abstract string Usage { get; }

		public virtual string Description { get; } = "";

		/// <summary>
		/// コマンドを実行します。
		/// </summary>
		/// <param name="source">コマンドのアクティベート元となる投稿。</param>
		/// <param name="args">引数。</param>
		/// <param name="body">コマンドに渡された、名前を除く全データ。</param>
		/// <returns>リプライに含まれる文字列。<c>null</c> を返した場合リプライが行われません。</returns>
		public abstract Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body);
	}

	/// <summary>
	/// コマンドの権限フラグです。
	/// </summary>
	[System.Flags]
	public enum PermissionFlag
	{
		Any = 0,
		AdminOnly = 1,
		LocalOnly = 2,
		RemoteOnly = 4,
	}
}
