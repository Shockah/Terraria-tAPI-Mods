using System.Collections;
using System.Collections.Generic;

namespace Shockah.Base
{
	public class SEvent<T> : IEnumerable<T> where T : class
	{
		internal List<T> handlers = new List<T>();
		public int Count { get { return handlers.Count; } }

		public IEnumerator<T> GetEnumerator()
		{
			return handlers.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public void Add(T a)
		{
			handlers.Add(a);
		}
		public void Remove(T a)
		{
			handlers.Remove(a);
		}
		public void Clear()
		{
			handlers.Clear();
		}

		public static SEvent<T> operator +(SEvent<T> ev, T a)
		{
			ev.handlers.Add(a);
			return ev;
		}
		public static SEvent<T> operator -(SEvent<T> ev, T a)
		{
			ev.handlers.Remove(a);
			return ev;
		}
	}
}