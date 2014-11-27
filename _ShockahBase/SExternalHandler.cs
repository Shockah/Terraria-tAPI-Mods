using System;
using TAPI;

namespace Shockah.Base
{
	public class SExternalHandler
	{
		public readonly SEventHandler events = new SEventHandler();
		public readonly SFuncHandler funcs = new SFuncHandler();
		public readonly SActionHandler actions = new SActionHandler();

		public object OnModCall(ModBase mod, params object[] args)
		{
			return events.OnModCall(mod, args) ?? funcs.OnModCall(mod, args) ?? actions.OnModCall(mod, args);
		}
	}
}