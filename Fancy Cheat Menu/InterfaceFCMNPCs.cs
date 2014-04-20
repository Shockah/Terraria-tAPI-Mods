using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Shockah.Base;
using System;
using System.Collections.Generic;
using TAPI;
using Terraria;

namespace Shockah.FCM
{
	public class InterfaceFCMNPCs : InterfaceFCM<NPC>
	{
		public const int BASE_OFF_X = 56, BASE_OFF_Y = 56;
		public const int COLS = 6, ROWS = 4, OFF_X = BASE_OFF_X - 6, OFF_Y = BASE_OFF_Y - 6, POS_X = 20, POS_Y = 306;
		public const float FILTER_W = 140, FILTER_H = 25, FILTER_X_OFF = 4;
		public const float SORT_TEXT_SCALE = .75f;

		protected static List<NPC> defs = new List<NPC>();

		public static void Reset()
		{
			defs.Clear();
			foreach (KeyValuePair<string, NPC> kvp in Defs.npcs)
			{
				defs.Add(kvp.Value);
			}
			defs.Sort((i1, i2) =>
				{
					if (i1.type == i2.type) return i2.netID.CompareTo(i1.netID);
					return i1.type.CompareTo(i2.type);
				}
			);
		}

		protected readonly ElSlider slider;
		protected readonly ElChooser<Sorter<NPC>> sortingChooser;
		public List<NPC> filtered = new List<NPC>();
		protected NPCSlot[] slots = new NPCSlot[COLS * ROWS];
		private int _Scroll = 0;
		protected readonly Filter<NPC>
			FTown = new Filter<NPC>("Town", Main.npcHeadTexture[22], (npc) => { return npc.townNPC; }), //Guide
			FOther = new Filter<NPC>("Other", Defs.unloadedItem.GetTexture(), null);
		protected readonly Sorter<NPC>
			SID = new Sorter<NPC>("ID", (i1, i2) => { return i1.type.CompareTo(i2.type); }, (npc) => { return true; }),
			SName = new Sorter<NPC>("Name", (i1, i2) => { return i1.displayName.CompareTo(i2.displayName); }, (npc) => { return true; }),
			SDamage = new Sorter<NPC>("Damage", (i1, i2) => { return i1.damage.CompareTo(i2.damage); }, (npc) => { return npc.damage > 0; }),
			SDefense = new Sorter<NPC>("Defense", (i1, i2) => { return i1.defense.CompareTo(i2.defense); }, (npc) => { return true; });

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

		public InterfaceFCMNPCs()
		{
			FOther.matches = (item) =>
			{
				foreach (Filter<NPC> filter in filters) if (!object.ReferenceEquals(FOther, filter) && filter.matches(item)) return false;
				return true;
			};
			filters.AddRange(new Filter<NPC>[]
				{
					FTown, FOther
				}
			);
			sorters.AddRange(new Sorter<NPC>[] { SID, SName, SDamage, SDefense });

			slider = new ElSlider(
				(scroll) => { if (Scroll != scroll) { Scroll = scroll; Refresh(false); } },
				() => { return Scroll; },
				() => { return ROWS; },
				() => { return (int)Math.Ceiling(1f * filtered.Count / COLS); }
			);

			sorter = sorters[0];
			sortingChooser = new ElChooser<Sorter<NPC>>(
				(item) => { reverseSort = object.ReferenceEquals(sorter, item) ? !reverseSort : false; sorter = item; Refresh(true); },
				() => { return sorter; },
				() => { return MBase.me.textures[reverseSort ? "Images/ArrowDecrease.png" : "Images/ArrowIncrease.png"]; }
			);
			foreach (Sorter<NPC> sorter2 in sorters) sortingChooser.Add(new Tuple<string, Sorter<NPC>>(sorter2.name, sorter2));
		}

		public override void OnOpen()
		{
			base.OnOpen();
			foreach (Filter<NPC> filter in filters) filter.mode = null;
			sorter = sorters[0];
			reverseSort = false;
			Refresh(true);
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

			SDrawing.StringShadowed(sb, Main.fontMouseText, (filtered.Count == defs.Count ? "NPCs" : "Matching NPCs") + ": " + filtered.Count, new Vector2(POS_X, POS_Y - 26));

			Main.inventoryScale = 1f;
			int offX = (int)Math.Ceiling(OFF_X * Main.inventoryScale), offY = (int)Math.Ceiling(OFF_Y * Main.inventoryScale);
			for (int y = 0; y < ROWS; y++) for (int x = 0; x < COLS; x++)
				{
					slots[x + y * COLS].scale = Main.inventoryScale;
					slots[x + y * COLS].UpdatePos(new Vector2(POS_X + x * offX, POS_Y + y * offY));
					slots[x + y * COLS].Draw(sb, true, !blocked);
				}

			Main.inventoryScale = 1f;
			slider.pos = new Vector2(POS_X + 4 + COLS * OFF_X * Main.inventoryScale, POS_Y);
			slider.size = new Vector2(16, ROWS * OFF_Y * Main.inventoryScale);
			blocked = slider.Draw(sb, true, !blocked) || blocked;

			SDrawing.StringShadowed(sb, Main.fontMouseText, "Sort:", new Vector2(POS_X + 16 + COLS * OFF_X * Main.inventoryScale, POS_Y - 26), Color.White, SORT_TEXT_SCALE);
			sortingChooser.pos = new Vector2(POS_X + 48 + COLS * OFF_X * Main.inventoryScale, POS_Y - 30);
			sortingChooser.size = new Vector2(72, 24);
			blocked = sortingChooser.Draw(sb, false, !blocked) || blocked;

			float oldInventoryScale = Main.inventoryScale;
			Main.inventoryScale = .75f;
			float filterW = FILTER_W * Main.inventoryScale;
			float filterH = FILTER_H * Main.inventoryScale;
			for (int i = 0; i < filters.Count; i++)
			{
				Filter<NPC> filter = filters[i];
				Vector2 pos = new Vector2(POS_X + 32 + COLS * OFF_X * oldInventoryScale + (i / 10) * (filterW + FILTER_X_OFF * Main.inventoryScale), POS_Y + (i % 10) * filterH);
				Drawing.DrawBox(sb, pos.X, pos.Y, filterW, filterH * Main.inventoryScale);
				Texture2D tex = filter.mode == null ? filter.tex : (filter.mode.Value ? MBase.me.textures["Images/Tick.png"] : Main.cdTexture);
				if (tex != null)
				{
					float tscale = 1f;
					if (tscale * tex.Width > filterH - 2f) tscale = (filterH - 2f) / tex.Width;
					if (tscale * tex.Height > filterH - 2f) tscale = (filterH - 2f) / tex.Height;
					sb.Draw(tex, pos + new Vector2(filterH / 2f + 2, filterH / 2f), null, Color.White, 0f, tex.Size() / 2, tscale, SpriteEffects.None, 0f);
				}
				Vector2 measure = Main.fontMouseText.MeasureString(filter.name) * Main.inventoryScale;
				SDrawing.StringShadowed(sb, Main.fontMouseText, filter.name, pos + new Vector2(filterH + 4, (filterH - measure.Y) / 2), Color.White, Main.inventoryScale);

				if (new Rectangle((int)pos.X, (int)pos.Y, (int)filterW, (int)filterH).Contains(Main.mouseX, Main.mouseY))
				{
					Main.localPlayer.mouseInterface = true;
					if (Main.mouseLeft && Main.mouseLeftRelease)
					{
						if (!Main.keyState.IsKeyDown(Keys.LeftControl)) foreach (Filter<NPC> filter2 in filters) if (!object.ReferenceEquals(filter, filter2)) filter2.mode = null;
						if (filter.mode == null) filter.mode = true; else filter.mode = null;
						Refresh(true);
					}
					if (Main.mouseRight && Main.mouseRightRelease)
					{
						if (!Main.keyState.IsKeyDown(Keys.LeftControl)) foreach (Filter<NPC> filter2 in filters) if (!object.ReferenceEquals(filter, filter2)) filter2.mode = null;
						if (filter.mode == null) filter.mode = false; else filter.mode = null;
						Refresh(true);
					}
				}
			}

			sortingChooser.Draw(sb, true, false);

			string text = typing == null ? filterText : typing + "|";
			if (!string.IsNullOrEmpty(text))
			{
				Drawing.DrawBox(sb, POS_X, POS_Y + ROWS * OFF_Y * Main.inventoryScale + 4, 20 + COLS * OFF_X * Main.inventoryScale, 32);
				SDrawing.StringShadowed(sb, Main.fontMouseText, text, new Vector2(POS_X + 8, POS_Y + ROWS * OFF_Y * Main.inventoryScale + 8));
			}
		}

		public void Refresh(bool resetScroll)
		{
			Scroll = resetScroll ? 0 : Scroll;
			for (int i = 0; i < slots.Length; i++) slots[i] = new NPCSlot(this, i + Scroll * COLS);
			RunFilters();
		}

		protected void RunFilters()
		{
			filtered.Clear();
			foreach (NPC def in defs)
			{
				if ((typing != null || filterText != null) && def.displayName.ToLower().IndexOf((typing == null ? filterText : typing).ToLower()) == -1) continue;
				if (!sorter.allow(def)) continue;
				foreach (Filter<NPC> filter in filters) if (filter.mode != null) if (filter.mode == !filter.matches(def)) goto L;
				filtered.Add(def);
			L: { }
			}
			filtered.Sort(sorter);
			if (reverseSort) filtered.Reverse();
		}
	}
}