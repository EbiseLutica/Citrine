using System;
using System.Collections.Generic;
using System.Linq;

namespace Citrine
{
	public static class Ext
	{
		static readonly Random r = new Random();

		public static T Random<T>(this IList<T> list, Random r = null) => list.Count == 0 ? default : list[(r ?? Ext.r).Next(list.Count)];

		public static T Random<T>(this IEnumerable<T> list, Random r = null) => !list.Any() ? default : list.Skip((r ?? Ext.r).Next(list.Count() - 1)).First();
	}
}