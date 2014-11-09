using System.Collections.Generic;

namespace Shockah.FCM
{
	public class InterfaceFCM<T> : InterfaceFCMBase
	{
		public readonly List<Filter<T>> filters = new List<Filter<T>>();
		public readonly List<Filter<T>> modFilters = new List<Filter<T>>();
		public readonly List<Sorter<T>> sorters = new List<Sorter<T>>();
		public Sorter<T> sorter = null;
		public bool reverseSort = false;
		public List<T> filtered = new List<T>();
	}
}
