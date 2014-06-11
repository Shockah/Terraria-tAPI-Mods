using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using System.Collections.Generic;
using TAPI;
using Terraria;

namespace Shockah.FCM
{
	public class MInterface : ModInterface
	{
		public MInterface(ModBase modBase) : base(modBase) { }

		public override bool OverrideChat()
		{
			return Interface.current is InterfaceFCMBase && ((InterfaceFCMBase)Interface.current).typing == null;
		}
		public override bool KeyboardInputFocused()
		{
			return Interface.current is InterfaceFCMBase && ((InterfaceFCMBase)Interface.current).typing != null;
		}

		public override bool PreDrawCrafting(SpriteBatch sb)
		{
			if (Interface.current is InterfaceFCMBase) return false;
			return true;
		}
	}
}