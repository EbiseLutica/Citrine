using System.Text.RegularExpressions;

namespace Citrine.Core
{
	public static class StringExtension
	{
		public static bool IsMatch(this string input, string pattern) => Regex.IsMatch(input, pattern);

		public static string TrimMentions(this string str) => Regex.Replace(str, @"@[a-zA-Z0-9_]+(@[a-zA-Z0-9\-\.]+)?", "").Trim();
	}
}
