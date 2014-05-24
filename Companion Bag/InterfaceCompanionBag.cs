using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.CompanionBag
{
	public class InterfaceCompanionBag : Interface
	{
		public const int BASE_OFF_X = 56, BASE_OFF_Y = 56;
		public const int COLS = 8, ROWS = 5, OFF_X = BASE_OFF_X - 6, OFF_Y = BASE_OFF_Y - 6, POS_X = 20, POS_Y = 306;

		public static InterfaceCompanionBag me = null;
		public static bool needUpdate = true;

		private readonly ModBase modBase;
		protected ItemSlotCompanionBag[] slots = new ItemSlotCompanionBag[0];

		public InterfaceCompanionBag(ModBase modBase)
		{
			this.modBase = modBase;
			me = this;
		}

		public void DrawFrame(SpriteBatch sb)
		{
			if (needUpdate && Main.mouseItem.IsBlank())
			{
				needUpdate = false;
				List<Item> companions = Main.localPlayer.GetSubClass<MPlayer>().companions;
				for (int i = 0; i < companions.Count; i++) if (companions[i].IsBlank()) companions.RemoveAt(i);
				companions.Add(new Item());

				slots = new ItemSlotCompanionBag[Main.localPlayer.GetSubClass<MPlayer>().companions.Count];
				for (int i = 0; i < slots.Length; i++) slots[i] = new ItemSlotCompanionBag(modBase, i);
			}
			
			Vector2 pos = FrameCompanionBagInterface.me.FramePos1();
			Main.inventoryScale = .75f;
			for (int i = 0; i < slots.Length; i++)
			{
				int x = i % COLS;
				int y = i / COLS;
				slots[i].scale = Main.inventoryScale;
				slots[i].UpdatePos(new Vector2(pos.X + x * OFF_X * Main.inventoryScale, pos.Y + y * OFF_Y * Main.inventoryScale));
				slots[i].UpdateAndDraw(sb);
			}
		}
	}
}