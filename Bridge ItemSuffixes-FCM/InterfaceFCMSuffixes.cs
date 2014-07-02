using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Shockah.Base;
using Shockah.FCM;
using System;
using System.Collections.Generic;
using TAPI;
using Terraria;

namespace Shockah.ItemSuffixes
{
	public class InterfaceFCMSuffixes : InterfaceFCM<ItemSuffix>
	{
		public const int COLS = 2, ROWS = 7, POS_X = 20, POS_Y = 306;
		public const float SLOT_W = 150, SLOT_H = 28;
		public const float SORT_TEXT_SCALE = .75f;

		public static InterfaceFCMSuffixes me = null;
		internal static List<ItemSuffix> defs = new List<ItemSuffix>();
		internal static List<string> defsNames = new List<string>();

		public static void Reset()
		{
			defs.Clear();
			foreach (ItemSuffix suffix in ItemSuffix.list)
			{
				defs.Add(suffix);
				defsNames.Add(suffix.displayName == null ? "<none>" : suffix.displayName);
			}
		}

		protected readonly ElSlider slider;
		protected SuffixSlot[] slots = new SuffixSlot[COLS * ROWS];
		internal ItemSlotSuffixFCM slotItem = null;
		private int _Scroll = 0;

		protected int Scroll
		{
			get
			{
				return _Scroll;
			}
			set
			{
				_Scroll = Math.Min(Math.Max(value, 0), ScrollMax);
			}
		}
		protected int ScrollMax
		{
			get
			{
				return Math.Max((int)Math.Ceiling(1f * (filtered.Count - ROWS * COLS) / COLS), 0);
			}
		}

		public InterfaceFCMSuffixes()
		{
			me = this;

			slider = new ElSlider(
				(scroll) => { if (Scroll != scroll) { Scroll = scroll; Refresh(false); } },
				() => { return Scroll; },
				() => { return ROWS; },
				() => { return (int)Math.Ceiling(1f * filtered.Count / COLS); }
			);

			slotItem = new ItemSlotSuffixFCM(this);
		}

		public override void OnOpen()
		{
			base.OnOpen();
			if (!resetInterface) { resetInterface = true; return; }
			Refresh(true);
		}

		public override void OnClose()
		{
			base.OnClose();
			if (!slotItem.MyItem.IsBlank())
			{
				Main.localPlayer.GetItem(Main.myPlayer, slotItem.MyItem.DeepClone());
				slotItem.MyItem.SetDefaults(0);
			}
		}

		public override void Draw(InterfaceLayer layer, SpriteBatch sb)
		{
			bool blocked = false;
			string oldTyping = typing;
			base.Draw(layer, sb);
			if (oldTyping != typing) Refresh(true);

			int scrollBy = (Main.mouseState.ScrollWheelValue - Main.oldMouseState.ScrollWheelValue) / 120;
			int oldScroll = Scroll;
			Scroll -= scrollBy;
			if (Scroll != oldScroll) Refresh(false);

			SDrawing.StringShadowed(sb, Main.fontMouseText, (filtered.Count == defs.Count ? "Suffixes" : "Matching suffixes") + ": " + filtered.Count, new Vector2(POS_X, POS_Y - 26));

			Main.inventoryScale = 1f;
			int offX = (int)Math.Ceiling(SLOT_W * Main.inventoryScale), offY = (int)Math.Ceiling(SLOT_H * Main.inventoryScale);
			for (int y = 0; y < ROWS; y++) for (int x = 0; x < COLS; x++)
				{
					slots[x + y * COLS].scale = Main.inventoryScale;
					slots[x + y * COLS].UpdatePos(new Vector2(POS_X + x * offX, POS_Y + y * offY));
					slots[x + y * COLS].Draw(sb, true, !blocked);
				}

			slider.pos = new Vector2(POS_X + 4 + COLS * offX * Main.inventoryScale, POS_Y);
			slider.size = new Vector2(16, ROWS * offY * Main.inventoryScale);
			blocked = slider.Draw(sb, true, !blocked) || blocked;

			slotItem.scale = Main.inventoryScale;
			slotItem.UpdatePos(new Vector2(POS_X + COLS * offX * Main.inventoryScale + 32, POS_Y + ROWS * offY * Main.inventoryScale - 56));
			slotItem.Draw(sb, true, !blocked);

			float oldInventoryScale = Main.inventoryScale;
			Main.inventoryScale = .75f;

			string text = typing == null ? filterText : typing + "|";
			if (!string.IsNullOrEmpty(text))
			{
				Drawing.DrawBox(sb, POS_X, POS_Y + ROWS * offY * oldInventoryScale + 4, 20 + COLS * offX * oldInventoryScale, 32);
				SDrawing.StringShadowed(sb, Main.fontMouseText, text, new Vector2(POS_X + 8, POS_Y + ROWS * offY * oldInventoryScale + 8));
			}
		}

		public void Refresh(bool resetScroll)
		{
			Scroll = resetScroll ? 0 : Scroll;
			for (int i = 0; i < slots.Length; i++) slots[i] = new SuffixSlot(this, i + Scroll * COLS, new Vector2(SLOT_W, SLOT_H));
			RunFilters();
		}

		protected void RunFilters()
		{
			filtered.Clear();
			foreach (ItemSuffix suffix in defs)
			{
				if ((typing != null || filterText != null) && (defsNames[defs.IndexOf(suffix)].ToLower().IndexOf((typing == null ? filterText : typing).ToLower()) == -1)) continue;
				if (!slotItem.MyItem.IsBlank() && !suffix.IsAllowed(slotItem.MyItem) && suffix.displayName != null) continue;
				filtered.Add(suffix);
			}
		}
	}
}