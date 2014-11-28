using System;
using System.Collections.Generic;
using System.Collections;
using TAPI;

namespace Shockah.Base
{
	public class SEventBase
	{
		public readonly List<Tuple<object, double>> handlers = new List<Tuple<object, double>>();
		public int Count { get { return handlers.Count; } }
		protected bool dirty = true;

		public void Add(object a)
		{
			Add(a, 0d);
		}
		public void Add(object a, double priority)
		{
			Add(new Tuple<object, double>(a, priority));
		}
		public void Add(Tuple<object, double> tuple)
		{
			handlers.Add(tuple);
			dirty = true;
		}
		public void Remove(object a)
		{
			for (int i = 0; i < handlers.Count; i++)
				if (handlers[i].Item1 == a)
				{
					handlers.RemoveAt(i);
					break;
				}
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

		public static SEventBase operator +(SEventBase ev, object a)
		{
			ev.handlers.Add(new Tuple<object, double>(a, 0d));
			ev.dirty = true;
			return ev;
		}
		public static SEventBase operator +(SEventBase ev, Tuple<object, double> tuple)
		{
			ev.handlers.Add(tuple);
			ev.dirty = true;
			return ev;
		}
		public static SEventBase operator -(SEventBase ev, object a)
		{
			ev.Remove(a);
			return ev;
		}
		public static SEventBase operator -(SEventBase ev, Tuple<object, double> tuple)
		{
			ev.Remove(tuple.Item1);
			return ev;
		}
	}

	public class SEvent : SEventBase
	{
		public void Call()
		{
			Prepare();
			foreach (Tuple<object, double> handler in handlers)
				((Action)handler.Item1)();
		}
	}
	public class SEvent<A> : SEventBase
	{
		public void Call(A a)
		{
			Prepare();
			foreach (Tuple<object, double> handler in handlers)
				((Action<A>)handler.Item1)(a);
		}
	}
	public class SEvent<A, B> : SEventBase
	{
		public void Call(A a, B b)
		{
			Prepare();
			foreach (Tuple<object, double> handler in handlers)
				((Action<A, B>)handler.Item1)(a, b);
		}
	}
	public class SEvent<A, B, C> : SEventBase
	{
		public void Call(A a, B b, C c)
		{
			Prepare();
			foreach (Tuple<object, double> handler in handlers)
				((Action<A, B, C>)handler.Item1)(a, b, c);
		}
	}
	public class SEvent<A, B, C, D> : SEventBase
	{
		public void Call(A a, B b, C c, D d)
		{
			Prepare();
			foreach (Tuple<object, double> handler in handlers)
				((Action<A, B, C, D>)handler.Item1)(a, b, c, d);
		}
	}
	public class SEvent<A, B, C, D, E> : SEventBase
	{
		public void Call(A a, B b, C c, D d, E e)
		{
			Prepare();
			foreach (Tuple<object, double> handler in handlers)
				((Action<A, B, C, D, E>)handler.Item1)(a, b, c, d, e);
		}
	}
	public class SEvent<A, B, C, D, E, F> : SEventBase
	{
		public void Call(A a, B b, C c, D d, E e, F f)
		{
			Prepare();
			foreach (Tuple<object, double> handler in handlers)
				((Action<A, B, C, D, E, F>)handler.Item1)(a, b, c, d, e, f);
		}
	}

	public class SEventFTrue : SEventBase
	{
		public bool Call()
		{
			Prepare();
			foreach (Tuple<object, double> handler in handlers)
				if (((Func<bool>)handler.Item1)())
					return true;
			return false;
		}
	}
	public class SEventFTrue<A> : SEventBase
	{
		public bool Call(A a)
		{
			Prepare();
			foreach (Tuple<object, double> handler in handlers)
				if (((Func<A, bool>)handler.Item1)(a))
					return true;
			return false;
		}
	}
	public class SEventFTrue<A, B> : SEventBase
	{
		public bool Call(A a, B b)
		{
			Prepare();
			foreach (Tuple<object, double> handler in handlers)
				if (((Func<A, B, bool>)handler.Item1)(a, b))
					return true;
			return false;
		}
	}
	public class SEventFTrue<A, B, C> : SEventBase
	{
		public bool Call(A a, B b, C c)
		{
			Prepare();
			foreach (Tuple<object, double> handler in handlers)
				if (((Func<A, B, C, bool>)handler.Item1)(a, b, c))
					return true;
			return false;
		}
	}
	public class SEventFTrue<A, B, C, D> : SEventBase
	{
		public bool Call(A a, B b, C c, D d)
		{
			Prepare();
			foreach (Tuple<object, double> handler in handlers)
				if (((Func<A, B, C, D, bool>)handler.Item1)(a, b, c, d))
					return true;
			return false;
		}
	}
	public class SEventFTrue<A, B, C, D, E> : SEventBase
	{
		public bool Call(A a, B b, C c, D d, E e)
		{
			Prepare();
			foreach (Tuple<object, double> handler in handlers)
				if (((Func<A, B, C, D, E, bool>)handler.Item1)(a, b, c, d, e))
					return true;
			return false;
		}
	}
	public class SEventFTrue<A, B, C, D, E, F> : SEventBase
	{
		public bool Call(A a, B b, C c, D d, E e, F f)
		{
			Prepare();
			foreach (Tuple<object, double> handler in handlers)
				if (((Func<A, B, C, D, E, F, bool>)handler.Item1)(a, b, c, d, e, f))
					return true;
			return false;
		}
	}

	public class SEventFBool : SEventBase
	{
		public bool? Call()
		{
			Prepare();
			foreach (Tuple<object, double> handler in handlers)
			{
				bool? r = ((Func<bool?>)handler.Item1)();
				if (r.HasValue)
					return r;
			}
			return null;
		}
	}
	public class SEventFBool<A> : SEventBase
	{
		public bool? Call(A a)
		{
			Prepare();
			foreach (Tuple<object, double> handler in handlers)
			{
				bool? r = ((Func<A, bool?>)handler.Item1)(a);
				if (r.HasValue)
					return r;
			}
			return null;
		}
	}
	public class SEventFBool<A, B> : SEventBase
	{
		public bool? Call(A a, B b)
		{
			Prepare();
			foreach (Tuple<object, double> handler in handlers)
			{
				bool? r = ((Func<A, B, bool?>)handler.Item1)(a, b);
				if (r.HasValue)
					return r;
			}
			return null;
		}
	}
	public class SEventFBool<A, B, C> : SEventBase
	{
		public bool? Call(A a, B b, C c)
		{
			Prepare();
			foreach (Tuple<object, double> handler in handlers)
			{
				bool? r = ((Func<A, B, C, bool?>)handler.Item1)(a, b, c);
				if (r.HasValue)
					return r;
			}
			return null;
		}
	}
	public class SEventFBool<A, B, C, D> : SEventBase
	{
		public bool? Call(A a, B b, C c, D d)
		{
			Prepare();
			foreach (Tuple<object, double> handler in handlers)
			{
				bool? r = ((Func<A, B, C, D, bool?>)handler.Item1)(a, b, c, d);
				if (r.HasValue)
					return r;
			}
			return null;
		}
	}
	public class SEventFBool<A, B, C, D, E> : SEventBase
	{
		public bool? Call(A a, B b, C c, D d, E e)
		{
			Prepare();
			foreach (Tuple<object, double> handler in handlers)
			{
				bool? r = ((Func<A, B, C, D, E, bool?>)handler.Item1)(a, b, c, d, e);
				if (r.HasValue)
					return r;
			}
			return null;
		}
	}
	public class SEventFBool<A, B, C, D, E, F> : SEventBase
	{
		public bool? Call(A a, B b, C c, D d, E e, F f)
		{
			Prepare();
			foreach (Tuple<object, double> handler in handlers)
			{
				bool? r = ((Func<A, B, C, D, E, F, bool?>)handler.Item1)(a, b, c, d, e, f);
				if (r.HasValue)
					return r;
			}
			return null;
		}
	}
}