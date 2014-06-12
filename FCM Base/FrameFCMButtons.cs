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

		public FrameFCMButtons() : base(MBase.me, "Buttons", Anchor.BottomLeft, Anchor.BottomLeft, new Vector2(8, -8)) { }

		protected override void OnCreate()
		{
			foreach (Action<List<LittleButton>> h in EventCreatingButtonList) h(buttons);
			buttons.Sort((lb1, lb2) => lb2.priority.CompareTo(lb1.priority));

			size.X = -4;
			size.Y = 0;
			foreach (LittleButton lb in buttons)
			{
				size.X += lb.Size.X + 4;
				size.Y = Math.Max(size.Y, lb.Size.Y);
			}
			if (size.X < 0) size.X = 0;

			if (buttons.Count == 0) Destroy();
		}

		protected override void OnRender(SpriteBatch sb)
		{
			if (!Main.playerInventory) return;

			Vector2 pos = FramePos1();
			foreach (LittleButton lb in buttons)
			{
				lb.Draw(this, sb, pos);
				pos.X += lb.Size.X + 4;
			}
		}
	}
}
