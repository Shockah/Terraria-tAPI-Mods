using System;
using System.Collections.Generic;
using TAPI;

namespace Shockah.Base
{
	public class SFuncHandler
	{
		protected Dictionary<string, Delegate> funcs = new Dictionary<string, Delegate>();
		protected Dictionary<string, Func<object>> funcs0 = new Dictionary<string, Func<object>>();
		protected Dictionary<string, Func<object, object>> funcs1 = new Dictionary<string, Func<object, object>>();
		protected Dictionary<string, Func<object, object, object>> funcs2 = new Dictionary<string, Func<object, object, object>>();
		protected Dictionary<string, Func<object, object, object, object>> funcs3 = new Dictionary<string, Func<object, object, object, object>>();
		protected Dictionary<string, Func<object, object, object, object, object>> funcs4 = new Dictionary<string, Func<object, object, object, object, object>>();
		protected Dictionary<string, Func<object, object, object, object, object, object>> funcs5 = new Dictionary<string, Func<object, object, object, object, object, object>>();
		protected Dictionary<string, Func<object, object, object, object, object, object, object>> funcs6 = new Dictionary<string, Func<object, object, object, object, object, object, object>>();
		public int Count { get { return funcs.Count; } }

		public Delegate this[string name]
		{
			get { return funcs[name]; }
			set
			{
				funcs[name] = value;
				switch (value.Method.GetParameters().Length)
				{
					case 0: funcs0[name] = (Func<object>)value; break;
					case 1: funcs1[name] = (Func<object, object>)value; break;
					case 2: funcs2[name] = (Func<object, object, object>)value; break;
					case 3: funcs3[name] = (Func<object, object, object, object>)value; break;
					case 4: funcs4[name] = (Func<object, object, object, object, object>)value; break;
					case 5: funcs5[name] = (Func<object, object, object, object, object, object>)value; break;
					case 6: funcs6[name] = (Func<object, object, object, object, object, object, object>)value; break;
				}
			}
		}

		public void Clear()
		{
			funcs.Clear();
			funcs0.Clear();
			funcs1.Clear();
			funcs2.Clear();
			funcs3.Clear();
			funcs4.Clear();
			funcs5.Clear();
			funcs6.Clear();
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
			if (call == "Func")
			{
				string funcName = (string)args[1];
				if (funcs.ContainsKey(funcName))
				{
					if (returnDelegate)
						return funcs[funcName];
					else
					{
						switch (args.Length - 2)
						{
							case 0: return funcs0[funcName]();
							case 1: return funcs1[funcName](args[2]);
							case 2: return funcs2[funcName](args[2], args[3]);
							case 3: return funcs3[funcName](args[2], args[3], args[4]);
							case 4: return funcs4[funcName](args[2], args[3], args[4], args[5]);
							case 5: return funcs5[funcName](args[2], args[3], args[4], args[5], args[6]);
							case 6: return funcs6[funcName](args[2], args[3], args[4], args[5], args[6], args[7]);
						}
					}
				}
				else
					throw new ArgumentException(string.Format("No externalized func '%s' found.", funcName));
			}

			return null;
		}
	}
}