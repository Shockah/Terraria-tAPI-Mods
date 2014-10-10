using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using TAPI.UIKit;
using Terraria;
using Terraria.ID;

namespace Shockah.AccSlots
{
	public class ILSlots : InterfaceLayer
	{
		public const int MAX_SLOTS = MPlayer.MAX_EXTRA_SLOTS + 5;
		public const int
			MODE_ITEM = 0,
			MODE_SOCIAL = 1,
			MODE_DYE = 2;
		
		public readonly ModBase modBase;
		public int mode = MODE_ITEM;
		public readonly ItemSlot[]
			slotsItem = new ItemSlot[MAX_SLOTS],
			slotsSocial = new ItemSlot[MAX_SLOTS],
			slotsDye = new ItemSlot[MAX_SLOTS];
		public readonly ItemSlot[][] slots;
		
		public ILSlots(ModBase modBase) : base(modBase.mod.InternalName + ":Slots")
		{
			this.modBase = modBase;
			slots = new ItemSlot[][] { slotsItem, slotsSocial, slotsDye };

			for (int i = 0; i < 5; i++)
			{
				slotsItem[i] = new CustomItemSlotAccessory(modBase, "Item", i, i, i,
					(slot, item) => Main.localPlayer.armor[3 + slot.index] = item,
					(slot) => Main.localPlayer.armor[3 + slot.index],
					(slot, visible) => Main.localPlayer.hideVisual[3 + slot.index] = !visible,
					(slot) => !Main.localPlayer.hideVisual[3 + slot.index]
				);
				slotsSocial[i] = new CustomItemSlotSocial(modBase, "Social", i, i,
					(slot, item) => Main.localPlayer.armor[11 + slot.index] = item,
					(slot) => Main.localPlayer.armor[11 + slot.index]
				);
				slotsDye[i] = new CustomItemSlotDye(modBase, "Dye", i, i,
					(slot, item) => Main.localPlayer.dye[3 + slot.index] = item,
					(slot) => Main.localPlayer.dye[3 + slot.index]
				);
			}
			for (int i = 5; i < MAX_SLOTS; i++)
			{
				int j = i - 5;
				slotsItem[i] = new CustomItemSlotAccessory(modBase, "Item", j, i, i,
					(slot, item) => Main.localPlayer.GetSubClass<MPlayer>().extraItem[slot.index] = item,
					(slot) => Main.localPlayer.GetSubClass<MPlayer>().extraItem[slot.index],
					(slot, visible) => Main.localPlayer.GetSubClass<MPlayer>().visibility[slot.index] = visible,
					(slot) => Main.localPlayer.GetSubClass<MPlayer>().visibility[slot.index]
				);
				slotsSocial[i] = new CustomItemSlotSocial(modBase, "Social", j, i,
					(slot, item) => Main.localPlayer.GetSubClass<MPlayer>().extraSocial[slot.index] = item,
					(slot) => Main.localPlayer.GetSubClass<MPlayer>().extraSocial[slot.index]
				);
				slotsDye[i] = new CustomItemSlotDye(modBase, "Dye", j, i,
					(slot, item) => Main.localPlayer.GetSubClass<MPlayer>().extraDye[slot.index] = item,
					(slot) => Main.localPlayer.GetSubClass<MPlayer>().extraDye[slot.index]
				);
			}
		}

		protected override void OnDraw(SpriteBatch sb)
		{
			if (API.main.showNPCs) return;
			
			Main.inventoryScale = 0.85f;
			int mainX = Main.screenWidth - 64 - 28;
			int mainY = 174 + Main.mH;
			int off = (int)(56 * Main.inventoryScale);
			mainY += 3 * off;

			int mainX2 = mainX - off * 3;
			int mainY2 = mainY + off * 5;

			Action<Vector2, Vector2, int, string, Texture2D> drawButtonMode = (pos, size, myMode, tip, tex) =>
			{
				bool onButton = false;
				if (mode == myMode) onButton = true;
				if (new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y).Contains(Main.mouse))
				{
					Main.localPlayer.mouseInterface = true;
					onButton = true;
					if (Main.mouseLeft && Main.mouseLeftRelease)
					{
						mode = myMode;
						Main.PlaySound(7);
					}
					Main.toolTip = new Item();
					Main.hoverItemName = tip;
				}
				
				Drawing.DrawBox(sb, pos.X, pos.Y, size.X, size.Y, onButton ? 0.785f : 0.5f);
				float scale = 1f * Math.Min(size.X - 8, size.Y - 8) / Math.Max(tex.Width, tex.Height);
				sb.Draw(tex, pos + size / 2, null, Color.White * (onButton ? 1f : .5f), 0f, tex.Size() / 2, scale, SpriteEffects.None, 0f);
			};

			for (int i = 0; i < 3; i++)
			{
				Texture2D tex = ItemDef.byType[i == MODE_ITEM ? ItemID.AnkhShield : (i == MODE_SOCIAL ? ItemID.RedCape : ItemID.FlameDye)].Texture;
				string tip = i == MODE_ITEM ? "Active" : (i == MODE_SOCIAL ? "Social" : "Dye");
				drawButtonMode(new Vector2(mainX2 + 4, mainY + off + 4 + off * i), new Vector2(off - 8, off - 8), i, tip, tex);
			}
			
			for (int i = 0; i < MAX_SLOTS; i++)
			{
				int xx = mainX - off * (i / 5);
				int yy = mainY + off * (i % 5);
				slots[mode][i].UpdateAndDraw(sb, new Vector2(xx, yy), Main.inventoryScale);
			}
		}
	}
}