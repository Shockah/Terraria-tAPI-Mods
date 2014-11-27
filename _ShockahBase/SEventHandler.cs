using System;
using System.Collections.Generic;
using TAPI;

namespace Shockah.Base
{
	public class SEventHandler
	{
		protected Dictionary<string, SEventBase<Delegate>> events = new Dictionary<string, SEventBase<Delegate>>();
		public int Count { get { return events.Count; } }

		public SEventBase<Delegate> this[string name]
		{
			get { return events[name]; }
			set { events[name] = value; }
		}

		public void Clear()
		{
			events.Clear();
		}

		public object OnModCall(ModBase mod, params object[] args)
		{
			string call = (string)args[0];
			if (call == "RegisterEvent")
			{
				string eventName = (string)args[1];
				if (events.ContainsKey(eventName))
				{
					Delegate handler = (Delegate)args[2];
					double priority = args.Length > 3 ? (double)args[3] : 0d;
					events[eventName] += Tuple.Create(handler, priority);
					return true;
				}
				else
					throw new ArgumentException(string.Format("No event '%s' to register for.", eventName));
			}

			return null;
		}
	}
}