using System;
using System.Collections.Generic;

namespace Shockah.FCM
{
	public class Sorter<T> : IComparer<T>
	{
		public readonly string name;
		public Func<T, T, int> compare;
		public Func<T, bool> allow;

		public Sorter(string name, Func<T, T, int> compare, Func<T, bool> allow)
		{
			this.name = name;
			this.compare = compare;
			this.allow = allow;
		}

		public int Compare(T t1, T t2)
		{
			return compare(t1, t2);
		}
	}
}