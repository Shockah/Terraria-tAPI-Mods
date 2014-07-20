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
		protected readonly ElChooser<Sorter<ItemSuffix>> sortingChooser;
		protected readonly ElButton bSearch, bSearchBar;
		protected SuffixSlot[] slots = new SuffixSlot[COLS * ROWS];
		internal ItemSlotSuffixFCM slotItem = null;
		private int _Scroll = 0;
		protected readonly Sorter<ItemSuffix>
			SID, SName;

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
			if (Main.dedServ) return;

			SID = new Sorter<ItemSuffix>("ID", (i1, i2) => { if (i1 == null || i1.displayName == null) return reverseSort ? 1 : -1; if (i2 == null || i2.displayName == null) return reverseSort ? -1 : 1; return i1.id.CompareTo(i2.id); }, (s) => true);
			SName = new Sorter<ItemSuffix>("Name", (i1, i2) => { if (i1 == null || i1.displayName == null) return reverseSort ? 1 : -1; if (i2 == null || i2.displayName == null) return reverseSort ? -1 : 1; return i1.displayName.CompareTo(i2.displayName); }, (s) => true);

			sorters.AddRange(new Sorter<ItemSuffix>[] { SID, SName });

			slider = new ElSlider(
				(scroll) => { if (Scroll != scroll) { Scroll = scroll; Refresh(false); } },
				() => { return Scroll; },
				() => { return ROWS; },
				() => { return (int)Math.Ceiling(1f * filtered.Count / COLS); }
			);

			sorter = sorters[0];
			sortingChooser = new ElChooser<Sorter<ItemSuffix>>(
				(item) => { reverseSort = object.ReferenceEquals(sorter, item) ? !reverseSort : false; sorter = item; Refresh(true); },
				() => { return sorter; },
				() => { return Shockah.FCM.MBase.me.textures[reverseSort ? "Images/ArrowDecrease.png" : "Images/ArrowIncrease.png"]; }
			);
			foreach (Sorter<ItemSuffix> sorter2 in sorters) sortingChooser.Add(new Tuple<string, Sorter<ItemSuffix>>(sorter2.name, sorter2));

			bSearch = new ElButton(
				(b, mb) =>
				{
					if (typing == null && filterText != null)
					{
						filterText = null;
						Refresh(true);
					}
					else
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
				},
				(b, sb, mb) =>
				{
					Texture2D tex = typing == null && filterText != null ? Main.cdTexture : Shockah.FCM.MBase.me.textures["Images/Arrow.png"];
					float tscale = 1f;
					if (tex.Width * tscale > b.size.X - 4) tscale = (b.size.X - 4) / (tex.Width * tscale);
					if (tex.Height * tscale > b.size.Y - 4) tscale = (b.size.Y - 4) / (tex.Height * tscale);
					sb.Draw(tex, b.pos + b.size / 2, null, Color.White, 0f, tex.Size() / 2, tscale, SpriteEffects.None, 0f);
				}
			);

			bSearchBar = new ElButton(
				(b, mb) =>
				{
					Main.GetInputText("");
					if (typing == null) typing = "";
					else
					{
						filterText = typing;
						if (filterText == "") filterText = null;
						typing = null;
					}
				},
				(b, sb, mb) =>
				{
					if (typing == null && filterText == null) SDrawing.StringShadowed(sb, Main.fontMouseText, "Search...", new Vector2(b.pos.X + 8, b.pos.Y + 4), Color.White * .5f);
					else SDrawing.StringShadowed(sb, Main.fontMouseText, typing == null ? filterText : typing + "|", new Vector2(b.pos.X + 8, b.pos.Y + 4));
				}
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
			Main.inventoryScale = 1f;
			int offX = (int)Math.Ceiling(SLOT_W * Main.inventoryScale), offY = (int)Math.Ceiling(SLOT_H * Main.inventoryScale);
			
			bool blocked = false;
			string oldTyping = typing;
			base.Draw(layer, sb);

			bSearch.pos = new Vector2(POS_X + COLS * offX - 12, POS_Y + ROWS * offY + 4);
			bSearch.size = new Vector2(32, 32);
			blocked = bSearch.Draw(sb, false, !blocked) || blocked;

			bSearchBar.pos = new Vector2(POS_X, POS_Y + ROWS * offY + 4);
			bSearchBar.size = new Vector2(COLS * offX - 16, 32);
			blocked = bSearchBar.Draw(sb, false, !blocked) || blocked;

			if (oldTyping != typing) Refresh(true);

			int scrollBy = (Main.mouseState.ScrollWheelValue - Main.oldMouseState.ScrollWheelValue) / 120;
			int oldScroll = Scroll;
			Scroll -= scrollBy;
			if (Scroll != oldScroll) Refresh(false);

			SDrawing.StringShadowed(sb, Main.fontMouseText, (filtered.Count == defs.Count ? "Suffixes" : "Matching suffixes") + ": " + filtered.Count, new Vector2(POS_X, POS_Y - 26));

			Main.inventoryScale = 1f;
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

			SDrawing.StringShadowed(sb, Main.fontMouseText, "Sort:", new Vector2(POS_X - 8 + COLS * offX * Main.inventoryScale, POS_Y - 22), Color.White, SORT_TEXT_SCALE);
			sortingChooser.pos = new Vector2(POS_X + 24 + COLS * offX * Main.inventoryScale, POS_Y - 26);
			sortingChooser.size = new Vector2(96, 20);
			blocked = sortingChooser.Draw(sb, false, !blocked) || blocked;

			float oldInventoryScale = Main.inventoryScale;
			Main.inventoryScale = .75f;

			bSearch.Draw(sb, true, false);
			bSearchBar.Draw(sb, true, false);
			sortingChooser.Draw(sb, true, false);
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
				if (!sorter.allow(suffix)) continue;
				if (!slotItem.MyItem.IsBlank() && !suffix.IsAllowed(slotItem.MyItem) && suffix.displayName != null) continue;
				filtered.Add(suffix);
			}
			filtered.Sort(sorter);
			if (reverseSort) filtered.Reverse();
		}
	}
}