using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using System;
using System.Collections.Generic;
using TAPI;
using Terraria;

namespace Shockah.FCM
{
	public class ILFCMButtons : InterfaceLayer
	{
		public static SEvent<Action<List<LittleButton>>> EventCreatingButtonList = new SEvent<Action<List<LittleButton>>>();

		protected List<LittleButton> buttons = new List<LittleButton>();
		
		public ILFCMButtons() : base(MBase.me.modName)
		{
			buttons.Add(new LittleButton("Items", MBase.me.textures["Images/ModuleItems.png"], () => { return Interface.current is InterfaceFCMItems; }, () => { new InterfaceFCMItems().Open(); }));
			buttons.Add(new LittleButton("NPCs", MBase.me.textures["Images/ModuleNPCs.png"], () => { return Interface.current is InterfaceFCMNPCs; }, () => { new InterfaceFCMNPCs().Open(); }));
			foreach (Action<List<LittleButton>> h in EventCreatingButtonList) h(buttons);
		}

		protected override void OnDraw(SpriteBatch sb)
		{
			if (Main.netMode != 0) return;
			if (!Main.playerInventory) return;

			Vector2 pos = new Vector2(8, Main.screenHeight - 8);
			foreach (LittleButton lb in buttons)
			{
				lb.Draw(sb, pos - new Vector2(0, lb.Size.Y));
				pos.X += lb.Size.X + 4;
			}
		}
	}
}
