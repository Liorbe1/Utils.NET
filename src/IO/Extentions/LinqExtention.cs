using System.Collections.Generic;

namespace CS.Utils.IO.Extentions
{
	public static class LinqExtention
	{
		public static IEnumerable<T> Repeat<T>(this T item, int times)
		{
			for (int i = 0; i < times; i++)
			{
				yield return item;
			}
		}
	}
}
