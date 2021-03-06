﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TAPI;
using Terraria;

namespace Shockah.FCM
{
	public class InterfaceFCMBase : Interface
	{
		public static bool resetInterface = true;
		public static bool lockSlotInteraction = false;

		public string typing = null, filterText = null;

		public override void OnOpen()
		{
			typing = null;
		}

		public override void Draw(InterfaceLayer layer, SpriteBatch sb)
		{
			if (!Main.mouseLeft && !Main.mouseRight)
				lockSlotInteraction = false;
			if (Keys.Enter.Pressed())
			{
				Main.GetInputText("");
				if (typing == null) typing = "";
				else
				{
					filterText = typing;
					if (filterText == "") filterText = null;
					typing = null;
				}
			}
			else if (typing != null) typing = Main.GetInputText(typing);
		}
	}
}