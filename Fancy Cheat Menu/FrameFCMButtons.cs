using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using System;
using System.Collections.Generic;
using TAPI;
using Terraria;

namespace Shockah.FCM
{
	public class FrameFCMButtons : SFrame
	{
		public static SEvent<Action<List<LittleButton>>> EventCreatingButtonList;

		static FrameFCMButtons()
		{
			Clear();
		}

		internal static void Clear()
		{
			EventCreatingButtonList = new SEvent<Action<List<LittleButton>>>();
		}

		protected List<LittleButton> buttons = new List<LittleButton>();

		public FrameFCMButtons() : base(MBase.me, "Buttons", Anchor.BottomLeft, new Vector2(8, Main.screenHeight - 8)) { }

		protected override void OnCreate()
		{
			buttons.Add(new LittleButton("Items", MBase.me.textures["Images/ModuleItems.png"], () => Interface.current is InterfaceFCMItems, () => InterfaceFCMItems.me.Open()));
			buttons.Add(new LittleButton("NPCs", MBase.me.textures["Images/ModuleNPCs.png"], () => Interface.current is InterfaceFCMNPCs, () => InterfaceFCMNPCs.me.Open()));
			buttons.Add(new LittleButton("Prefixes", MBase.me.textures["Images/ModulePrefixes.png"], () => Interface.current is InterfaceFCMPrefixes, () => InterfaceFCMPrefixes.me.Open()));
			foreach (Action<List<LittleButton>> h in EventCreatingButtonList) h(buttons);

			size.X = -4;
			size.Y = 0;
			foreach (LittleButton lb in buttons)
			{
				size.X += lb.Size.X + 4;
				size.Y = Math.Max(size.Y, lb.Size.Y);
			}
			if (size.X < 0) size.X = 0;
		}

		protected override void OnRender(SpriteBatch sb)
		{
			if (Main.netMode != 0) return;
			if (!Main.playerInventory) return;

			if (InterfaceFCMItems.me == null)
			{
				new InterfaceFCMItems();
				new InterfaceFCMNPCs();
				new InterfaceFCMPrefixes();
			}

			Vector2 pos = FramePos1();
			foreach (LittleButton lb in buttons)
			{
				lb.Draw(this, sb, pos);
				pos.X += lb.Size.X + 4;
			}
		}
	}
}
