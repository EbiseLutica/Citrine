#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
using System.Threading.Tasks;
using Citrine.Core.Api;

namespace Citrine.Core
{
    public class RatingModule : CommandBase
	{
		public override string Name => "rating";

		public override string Usage => "/rating <set/add/get>";

		public override PermissionFlag Permission => PermissionFlag.AdminOnly;

		public override async Task<string> OnActivatedAsync(ICommandSender sender, Server core, IShell shell, string[] args, string body)
		{
			if(!(sender is PostCommandSender p))
				return "use from post";
			if (args.Length < 1)
				throw new CommandException();
			
			switch (args[0].ToLowerInvariant().Trim())
			{
				case "set": 
					core.SetRatingValueOf(p.User.Id, int.Parse(args[1]));
					break;
				case "add": 
					core.Like(p.User.Id, int.Parse(args[1]));
					break;
				case "get":
					return core.GetRatingValueOf(p.User.Id).ToString();
				case "status":
					return core.GetRatingOf(p.User.Id).ToString();
			}
			return "ok";
		}
	}
}
