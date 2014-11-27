using System;
using System.Collections.Generic;
using TAPI;

namespace Shockah.Base
{
	public class SActionHandler
	{
		protected Dictionary<string, Delegate> actions = new Dictionary<string, Delegate>();
		protected Dictionary<string, Action> actions0 = new Dictionary<string, Action>();
		protected Dictionary<string, Action<object>> actions1 = new Dictionary<string, Action<object>>();
		protected Dictionary<string, Action<object, object>> actions2 = new Dictionary<string, Action<object, object>>();
		protected Dictionary<string, Action<object, object, object>> actions3 = new Dictionary<string, Action<object, object, object>>();
		protected Dictionary<string, Action<object, object, object, object>> actions4 = new Dictionary<string, Action<object, object, object, object>>();
		protected Dictionary<string, Action<object, object, object, object, object>> actions5 = new Dictionary<string, Action<object, object, object, object, object>>();
		protected Dictionary<string, Action<object, object, object, object, object, object>> actions6 = new Dictionary<string, Action<object, object, object, object, object, object>>();
		public int Count { get { return actions.Count; } }

		public Delegate this[string name]
		{
			get { return actions[name]; }
			set
			{
				actions[name] = value;
				switch (value.Method.GetParameters().Length)
				{
					case 0: actions0[name] = (Action)value; break;
					case 1: actions1[name] = (Action<object>)value; break;
					case 2: actions2[name] = (Action<object, object>)value; break;
					case 3: actions3[name] = (Action<object, object, object>)value; break;
					case 4: actions4[name] = (Action<object, object, object, object>)value; break;
					case 5: actions5[name] = (Action<object, object, object, object, object>)value; break;
					case 6: actions6[name] = (Action<object, object, object, object, object, object>)value; break;
				}
			}
		}

		public void Clear()
		{
			actions.Clear();
			actions0.Clear();
			actions1.Clear();
			actions2.Clear();
			actions3.Clear();
			actions4.Clear();
			actions5.Clear();
			actions6.Clear();
		}

		public object OnModCall(ModBase mod, params object[] args)
		{
			string call = (string)args[0];
			bool returnDelegate = false;
			if (call[0] == '>')
			{
				returnDelegate = true;
				call = call.Substring(1);
			}
			if (call == "Action")
			{
				string actionName = (string)args[1];
				if (actions.ContainsKey(actionName))
				{
					if (returnDelegate)
						return actions[actionName];
					else
					{
						switch (args.Length - 2)
						{
							case 0: actions0[actionName](); return true;
							case 1: actions1[actionName](args[2]); return true;
							case 2: actions2[actionName](args[2], args[3]); return true;
							case 3: actions3[actionName](args[2], args[3], args[4]); return true;
							case 4: actions4[actionName](args[2], args[3], args[4], args[5]); return true;
							case 5: actions5[actionName](args[2], args[3], args[4], args[5], args[6]); return true;
							case 6: actions6[actionName](args[2], args[3], args[4], args[5], args[6], args[7]); return true;
						}
					}
				}
				else
					throw new ArgumentException(string.Format("No externalized action '%s' found.", actionName));
			}

			return null;
		}
	}
}