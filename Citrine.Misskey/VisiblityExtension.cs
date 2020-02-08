using System;
using Citrine.Core.Api;

namespace Citrine.Misskey
{
	public static class VisiblityExtension
	{
		public static Visibility ToVisiblity(this string visiblity)
		{
			switch (visiblity)
			{
				case "public":
					return Visibility.Public;
				case "specified":
					return Visibility.Direct;
				case "home":
					return Visibility.Limited;
				case "followers":
					return Visibility.Private;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public static string? ToStr(this Visibility visiblity)
		{
			switch (visiblity)
			{
				case Visibility.Default:
					return null;
				case Visibility.Public:
					return "public";
				case Visibility.Limited:
					return "home";
				case Visibility.Private:
					return "followers";
				case Visibility.Direct:
					return "specified";
				default:
					throw new ArgumentOutOfRangeException(nameof(visiblity));
			}
		}
	}

}
