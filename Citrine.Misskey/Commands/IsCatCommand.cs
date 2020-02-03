#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
#pragma warning disable CS4014 // この呼び出しは待機されなかったため、現在のメソッドの実行は呼び出しの完了を待たずに続行されます

using System.Linq;
using System.Threading.Tasks;
using Citrine.Core;
using Citrine.Core.Api;

namespace Citrine.Misskey
{
	/// <summary>
	/// Misskey-specific cat-check command.
	/// </summary>
	public class IsCatCommand : CommandBase
	{
		public override string Name => "iscat";

		public override string Usage => @"使い方:
/iscat: 猫であるかどうかをチェック
/iscat <userId>";

		public override string Description => "猫であるかどうかのチェックを行います。";

		public override PermissionFlag Permission => PermissionFlag.Any;

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			if (!(sender is PostCommandSender p))
				return IsCat(false);

			var u = (p.User as MiUser)!.Native;
			var mk = (shell as Shell)?.Misskey;

			if (mk == null)
			{
				return "ここは, Misskey ではないようです. このコマンドがここにあるのは有り得ない状況なので, おそらくバグかな.";
			}

			if (args.Length == 0)
				return IsCat(u.IsCat ?? false);
			else
			{
				var id = args[0];
				return IsCat((await mk.Users.ShowAsync(id)).FirstOrDefault()?.IsCat ?? false);
			}
		}

		private string IsCat(bool isCat) => isCat ? "あなたは猫です." : "あなたは猫ではないです.";
	}
}
