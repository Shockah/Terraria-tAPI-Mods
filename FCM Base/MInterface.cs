using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using System.Collections.Generic;
using TAPI;
using Terraria;

namespace Shockah.FCM
{
	public class MInterface : ModInterface
	{
		public override bool OverrideChat()
		{
			return UICore.currentInterface is InterfaceFCMBase && ((InterfaceFCMBase)UICore.currentInterface).typing == null;
		}
		public override bool KeyboardInputFocused()
		{
			return UICore.currentInterface is InterfaceFCMBase && ((InterfaceFCMBase)UICore.currentInterface).typing != null;
		}

		public override bool PreDrawCrafting(SpriteBatch sb)
		{
			if (UICore.currentInterface is InterfaceFCMBase) return false;
			return true;
		}
	}
}