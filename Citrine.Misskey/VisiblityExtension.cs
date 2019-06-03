using System;
using Citrine.Core.Api;

namespace Citrine.Misskey
{
	public static class VisiblityExtension
	{
		public static Visiblity ToVisiblity(this string visiblity)
		{
			switch (visiblity)
			{
				case "public":
					return Visiblity.Public;
				case "specified":
					return Visiblity.Direct;
				case "home":
					return Visiblity.Limited;
				case "followers":
					return Visiblity.Private;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public static string ToStr(this Visiblity visiblity)
		{
			switch (visiblity)
			{
				case Visiblity.Default:
					return null;
				case Visiblity.Public:
					return "public";
				case Visiblity.Limited:
					return "home";
				case Visiblity.Private:
					return "followers";
				case Visiblity.Direct:
					return "specified";
				default:
					throw new ArgumentOutOfRangeException(nameof(visiblity));
			}
		}
	}

}
