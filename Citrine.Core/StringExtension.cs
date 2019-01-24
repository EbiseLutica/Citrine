using System.Text.RegularExpressions;

namespace Citrine.Core
{
	public static class StringExtension
	{
		public static bool IsMatch(this string input, string pattern) => Regex.IsMatch(input, pattern);
	}
}