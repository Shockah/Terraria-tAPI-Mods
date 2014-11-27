using System;
using System.Collections.Generic;
using System.Collections;
using TAPI;

namespace Shockah.Base
{
	public class SEventBase<T> : IEnumerable<T> where T : Delegate
	{
		protected List<Tuple<T, double>> handlers = new List<Tuple<T, double>>();
		public int Count { get { return handlers.Count; } }
		protected bool dirty = true;

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
			dirty = true;
		}
		public void Remove(T a)
		{
			handlers.Remove(a);
			dirty = true;
		}
		public void Clear()
		{
			handlers.Clear();
			dirty = true;
		}

		protected void Prepare()
		{
			if (!dirty) return;
			handlers.Sort((t1, t2) => -t1.Item2.CompareTo(t2.Item2));
			dirty = false;
		}

		public static SEventBase<T> operator +(SEventBase<T> ev, T a)
		{
			ev.handlers.Add(new Tuple<T, double>(a, 0d));
			dirty = true;
			return ev;
		}
		public static SEventBase<T> operator +(SEventBase<T> ev, Tuple<T, double> tuple)
		{
			ev.handlers.Add(tuple);
			dirty = true;
			return ev;
		}
		public static SEventBase<T> operator -(SEventBase<T> ev, T a)
		{
			ev.handlers.Remove(a);
			dirty = true;
			return ev;
		}
		public static SEventBase<T> operator -(SEventBase<T> ev, Tuple<T, double> tuple)
		{
			ev.handlers.Remove(tuple.Item1);
			dirty = true;
			return ev;
		}
	}

	public class SEvent : SEventBase<Action>
	{
		public void Call()
		{
			Prepare();
			foreach (Tuple<Action, double> handler in handlers)
				handler.Item1();
		}
	}
	public class SEvent<A> : SEventBase<Action<A>>
	{
		public void Call(A a)
		{
			Prepare();
			foreach (Tuple<Action<A>, double> handler in handlers)
				handler.Item1(a);
		}
	}
	public class SEvent<A, B> : SEventBase<Action<A, B>>
	{
		public void Call(A a, B b)
		{
			Prepare();
			foreach (Tuple<Action<A, B>, double> handler in handlers)
				handler.Item1(a, b);
		}
	}
	public class SEvent<A, B, C> : SEventBase<Action<A, B, C>>
	{
		public void Call(A a, B b, C c)
		{
			Prepare();
			foreach (Tuple<Action<A, B, C>, double> handler in handlers)
				handler.Item1(a, b, c);
		}
	}
	public class SEvent<A, B, C, D> : SEventBase<Action<A, B, C, D>>
	{
		public void Call(A a, B b, C c, D d)
		{
			Prepare();
			foreach (Tuple<Action<A, B, C, D>, double> handler in handlers)
				handler.Item1(a, b, c, d);
		}
	}
	public class SEvent<A, B, C, D, E> : SEventBase<Action<A, B, C, D, E>>
	{
		public void Call(A a, B b, C c, D d, E e)
		{
			Prepare();
			foreach (Tuple<Action<A, B, C, D, E>, double> handler in handlers)
				handler.Item1(a, b, c, d, e);
		}
	}
	public class SEvent<A, B, C, D, E, F> : SEventBase<Action<A, B, C, D, E, F>>
	{
		public void Call(A a, B b, C c, D d, E e, F f)
		{
			Prepare();
			foreach (Tuple<Action<A, B, C, D, E, F>, double> handler in handlers)
				handler.Item1(a, b, c, d, e, f);
		}
	}

	public class SEventFTrue : SEventBase<Func<bool>>
	{
		public bool Call()
		{
			Prepare();
			foreach (Tuple<Func<bool>, double> handler in handlers)
				if (handler.Item1())
					return true;
			return false;
		}
	}
	public class SEventFTrue<A> : SEventBase<Func<A, bool>>
	{
		public bool Call(A a)
		{
			Prepare();
			foreach (Tuple<Func<A, bool>, double> handler in handlers)
				if (handler.Item1(a))
					return true;
			return false;
		}
	}
	public class SEventFTrue<A, B> : SEventBase<Func<A, B, bool>>
	{
		public bool Call(A a, B b)
		{
			Prepare();
			foreach (Tuple<Func<A, B, bool>, double> handler in handlers)
				if (handler.Item1(a, b))
					return true;
			return false;
		}
	}
	public class SEventFTrue<A, B, C> : SEventBase<Func<A, B, C, bool>>
	{
		public bool Call(A a, B b, C c)
		{
			Prepare();
			foreach (Tuple<Func<A, B, C, bool>, double> handler in handlers)
				if (handler.Item1(a, b, c))
					return true;
			return false;
		}
	}
	public class SEventFTrue<A, B, C, D> : SEventBase<Func<A, B, C, D, bool>>
	{
		public bool Call(A a, B b, C c, D d)
		{
			Prepare();
			foreach (Tuple<Func<A, B, C, D, bool>, double> handler in handlers)
				if (handler.Item1(a, b, c, d))
					return true;
			return false;
		}
	}
	public class SEventFTrue<A, B, C, D, E> : SEventBase<Func<A, B, C, D, E, bool>>
	{
		public bool Call(A a, B b, C c, D d, E e)
		{
			Prepare();
			foreach (Tuple<Func<A, B, C, D, E, bool>, double> handler in handlers)
				if (handler.Item1(a, b, c, d, e))
					return true;
			return false;
		}
	}
	public class SEventFTrue<A, B, C, D, E, F> : SEventBase<Func<A, B, C, D, E, F, bool>>
	{
		public bool Call(A a, B b, C c, D d, E e, F f)
		{
			Prepare();
			foreach (Tuple<Func<A, B, C, D, E, F, bool>, double> handler in handlers)
				if (handler.Item1(a, b, c, d, e, f))
					return true;
			return false;
		}
	}

	public class SEventFBool : SEventBase<Func<bool?>>
	{
		public bool? Call()
		{
			Prepare();
			foreach (Tuple<Func<bool?>, double> handler in handlers)
			{
				bool? r = handler.Item1();
				if (r.HasValue)
					return r;
			}
			return null;
		}
	}
	public class SEventFBool<A> : SEventBase<Func<A, bool?>>
	{
		public bool? Call(A a)
		{
			Prepare();
			foreach (Tuple<Func<A, bool?>, double> handler in handlers)
			{
				bool? r = handler.Item1(a);
				if (r.HasValue)
					return r;
			}
			return null;
		}
	}
	public class SEventFBool<A, B> : SEventBase<Func<A, B, bool?>>
	{
		public bool? Call(A a, B b)
		{
			Prepare();
			foreach (Tuple<Func<A, B, bool?>, double> handler in handlers)
			{
				bool? r = handler.Item1(a, b);
				if (r.HasValue)
					return r;
			}
			return null;
		}
	}
	public class SEventFBool<A, B, C> : SEventBase<Func<A, B, C, bool?>>
	{
		public bool? Call(A a, B b, C c)
		{
			Prepare();
			foreach (Tuple<Func<A, B, C, bool?>, double> handler in handlers)
			{
				bool? r = handler.Item1(a, b, c);
				if (r.HasValue)
					return r;
			}
			return null;
		}
	}
	public class SEventFBool<A, B, C, D> : SEventBase<Func<A, B, C, D, bool?>>
	{
		public bool? Call(A a, B b, C c, D d)
		{
			Prepare();
			foreach (Tuple<Func<A, B, C, D, bool?>, double> handler in handlers)
			{
				bool? r = handler.Item1(a, b, c, d);
				if (r.HasValue)
					return r;
			}
			return null;
		}
	}
	public class SEventFBool<A, B, C, D, E> : SEventBase<Func<A, B, C, D, E, bool?>>
	{
		public bool? Call(A a, B b, C c, D d, E e)
		{
			Prepare();
			foreach (Tuple<Func<A, B, C, D, E, bool?>, double> handler in handlers)
			{
				bool? r = handler.Item1(a, b, c, d, e);
				if (r.HasValue)
					return r;
			}
			return null;
		}
	}
	public class SEventFBool<A, B, C, D, E, F> : SEventBase<Func<A, B, C, D, E, F, bool?>>
	{
		public bool? Call(A a, B b, C c, D d, E e, F f)
		{
			Prepare();
			foreach (Tuple<Func<A, B, C, D, E, F, bool?>, double> handler in handlers)
			{
				bool? r = handler.Item1(a, b, c, d, e, f);
				if (r.HasValue)
					return r;
			}
			return null;
		}
	}
}