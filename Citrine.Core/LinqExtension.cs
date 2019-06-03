using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Citrine.Core
{
	public static class LinqExtension
	{
		static readonly Random r = new Random();

		public static T Random<T>(this IList<T> list, Random r = null) => list.Count == 0 ? default : list[(r ?? LinqExtension.r).Next(list.Count)];

		public static T Random<T>(this IEnumerable<T> list, Random r = null) => !list.Any() ? default : list.Skip((r ?? LinqExtension.r).Next(list.Count() - 1)).First();

		public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
		{
			foreach (var value in enumerable)
			{
				action(value);
			}
		}

		public static async Task ForEach<T>(this IEnumerable<T> enumerable, Func<T, Task> action)
		{
			foreach (var value in enumerable)
			{
				await action(value);
			}
		}

		public static IEnumerable<char> Wa()
		{
			yield return '#';
			yield return 'わ';

			while (true) yield return 'ー';
		}

		public static IEnumerable<char> Wa(int length)
		{
			yield return '#';
			yield return 'わ';

			for (int i = 0; i < length; i++) yield return 'ー';
		}
	}
}
