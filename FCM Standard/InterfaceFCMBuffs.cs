using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Shockah.Base;
using System;
using System.Collections.Generic;
using TAPI;
using Terraria;

namespace Shockah.FCM.Standard
{
	public class InterfaceFCMBuffs : InterfaceFCM<BuffDefFCM>
	{
		public const int COLS = 2, ROWS = 5, POS_X = 20, POS_Y = 306;
		public const float SLOT_W = 150, SLOT_H = 40;
		public const float FILTER_W = 140, FILTER_H = 25, FILTER_X_OFF = 4;
		public const float SORT_TEXT_SCALE = .75f;

		public static InterfaceFCMBuffs me = null;
		internal static List<BuffDefFCM> defs = new List<BuffDefFCM>();

		public static void Reset()
		{
			Item fake = new Item();
			fake.displayName = "";
			defs.Clear();
			foreach (KeyValuePair<int, string> kvp in BuffDef.name)
			{
				defs.Add(new BuffDefFCM(kvp.Key, kvp.Value));
				//defsNames.Add(kvp.Value.type == 0 ? "<none>" : kvp.Value.SetItemName(fake).Trim());
			}
		}

		protected readonly ElSlider slider;
		protected readonly ElChooser<Sorter<BuffDefFCM>> sortingChooser;
		protected readonly ElButton bSearch, bSearchBar;
		protected BuffSlot[] slots = new BuffSlot[COLS * ROWS];
		private int _Scroll = 0;
		protected string dragging = null;
		public int buffM = 5, buffS = 0;
		protected readonly Filter<BuffDefFCM>
			FPositive, FNoTimer, FPet;
		protected readonly Sorter<BuffDefFCM>
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

		public InterfaceFCMBuffs()
		{
			me = this;
			if (Main.dedServ) return;

			FPositive = new Filter<BuffDefFCM>("Positive", MBase.me.textures["Images/BuffPositive"], (def) => !Main.debuff[def.type]);
			FNoTimer = new Filter<BuffDefFCM>("No timer", MBase.me.textures["Images/ModuleMisc"], (def) => !SBase.BuffHasTimer(def.type));
			FPet = new Filter<BuffDefFCM>("Pet", Main.buffTexture[27], (def) => Main.vanityPet[def.type] || Main.lightPet[def.type]);

			SID = new Sorter<BuffDefFCM>("ID", (i1, i2) => { return i1.type.CompareTo(i2.type); }, (npc) => true);
			SName = new Sorter<BuffDefFCM>("Name", (i1, i2) => { return i1.noModName.CompareTo(i2.noModName); }, (npc) => true);

			filters.AddRange(new Filter<BuffDefFCM>[]
				{
					FPositive, FNoTimer, FPet
				}
			);
			sorters.AddRange(new Sorter<BuffDefFCM>[] { SID, SName });

			slider = new ElSlider(
				(scroll) => { if (Scroll != scroll) { Scroll = scroll; Refresh(false); } },
				() => { return Scroll; },
				() => { return ROWS; },
				() => { return (int)Math.Ceiling(1f * filtered.Count / COLS); }
			);

			sorter = sorters[0];
			sortingChooser = new ElChooser<Sorter<BuffDefFCM>>(
				(item) => { reverseSort = object.ReferenceEquals(sorter, item) ? !reverseSort : false; sorter = item; Refresh(true); },
				() => { return sorter; },
				() => { return Shockah.FCM.MBase.me.textures[reverseSort ? "Images/ArrowDecrease" : "Images/ArrowIncrease"]; }
			);
			foreach (Sorter<BuffDefFCM> sorter2 in sorters) sortingChooser.Add(new Tuple<string, Sorter<BuffDefFCM>>(sorter2.name, sorter2));

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
					Texture2D tex = typing == null && filterText != null ? Main.cdTexture : Shockah.FCM.MBase.me.textures["Images/Arrow"];
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
					if (typing == null && filterText == null) Drawing.StringShadowed(sb, Main.fontMouseText, "Search...", new Vector2(b.pos.X + 8, b.pos.Y + 4), Color.White * .5f);
					else Drawing.StringShadowed(sb, Main.fontMouseText, typing == null ? filterText : typing + "|", new Vector2(b.pos.X + 8, b.pos.Y + 4));
				}
			);
		}

		public override void OnOpen()
		{
			base.OnOpen();
			if (!resetInterface) { resetInterface = true; return; }
			foreach (Filter<BuffDefFCM> filter in filters) filter.mode = null;
			sorter = sorters[0];
			reverseSort = false;
			buffM = 5; buffS = 0;
			Refresh(true);
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

			Action<string, string, Vector2, Texture2D, int, int, int, Func<int, string>, Action<int>> drawSliderInt = (name, tip, pos, sliderTex, value, vmin, vmax, textBuilder, codeSet) =>
			{
				float ratio = 1f * (value - vmin) / (vmax - vmin);
				sb.Draw(sliderTex, new Vector2(pos.X, pos.Y + 20), Color.White);
				sb.Draw(Main.colorSliderTexture, new Vector2(pos.X + 4 + (sliderTex.Width - 8) * ratio, pos.Y + 20 + sliderTex.Height / 2), null, Color.White, 0f, Main.colorSliderTexture.Size() * .5f, 1f, SpriteEffects.None, 0f);
				Drawing.StringShadowed(sb, Main.fontMouseText, tip, pos, Color.White, .8f);
				string valtext = textBuilder(value);
				float valscale = .8f;
				if (Main.fontMouseText.MeasureString(valtext).X * valscale > sliderTex.Width / 2) valscale = (sliderTex.Width / 2) / Main.fontMouseText.MeasureString(valtext).X;
				Drawing.StringShadowed(sb, Main.fontMouseText, valtext, new Vector2((float)Math.Round(pos.X + sliderTex.Width - Main.fontMouseText.MeasureString(textBuilder(value)).X * valscale), pos.Y), Color.White, valscale);
				if (!blocked && (dragging == name || (dragging == null && Math2.InRegion(Main.mouse, new Vector2(pos.X, pos.Y + 20), sliderTex.Width, sliderTex.Height))))
				{
					Main.localPlayer.mouseInterface = true;
					if (Main.mouseLeft)
					{
						dragging = name;
						int deltax = Main.mouseX - ((int)pos.X + 4);
						ratio = 1f * deltax / (sliderTex.Width - 8);
						ratio = Math.Min(Math.Max(ratio, 0f), 1f);
						codeSet((int)Math.Round(vmin + (vmax - vmin) * ratio));
					}
					else dragging = null;
				}
			};

			int scrollBy = (Main.mouseState.ScrollWheelValue - Main.oldMouseState.ScrollWheelValue) / 120;
			int oldScroll = Scroll;
			Scroll -= scrollBy;
			if (Scroll != oldScroll) Refresh(false);

			Main.inventoryScale = 1f;

			drawSliderInt("BuffTimeM", "", new Vector2(POS_X + COLS * offX + 48, POS_Y + ROWS * offY - Main.colorBarTexture.Height - 60), Main.colorBarTexture, buffM, 0, 60,
			(value) => { return "" + value + "m"; },
			(value) => { buffM = value; });

			drawSliderInt("BuffTimeS", "", new Vector2(POS_X + COLS * offX + 48, POS_Y + ROWS * offY - Main.colorBarTexture.Height - 20), Main.colorBarTexture, buffS, 0, 60,
			(value) => { return "" + value + "s"; },
			(value) => { buffS = value; });

			if (dragging != null) blocked = true;

			Drawing.StringShadowed(sb, Main.fontMouseText, (filtered.Count == defs.Count ? "Buffs" : "Matching buffs") + ": " + filtered.Count, new Vector2(POS_X, POS_Y - 26));

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

			Drawing.StringShadowed(sb, Main.fontMouseText, "Sort:", new Vector2(POS_X - 8 + COLS * offX * Main.inventoryScale, POS_Y - 22), Color.White, SORT_TEXT_SCALE);
			sortingChooser.pos = new Vector2(POS_X + 24 + COLS * offX * Main.inventoryScale, POS_Y - 26);
			sortingChooser.size = new Vector2(96, 20);
			blocked = sortingChooser.Draw(sb, false, !blocked) || blocked;

			float oldInventoryScale = Main.inventoryScale;
			Main.inventoryScale = .75f;
			float filterW = FILTER_W * Main.inventoryScale;
			float filterH = FILTER_H * Main.inventoryScale;
			for (int i = 0; i < filters.Count; i++)
			{
				Filter<BuffDefFCM> filter = filters[i];
				Vector2 pos = new Vector2(POS_X + 32 + COLS * offX + (i / 10) * (filterW + FILTER_X_OFF * Main.inventoryScale), POS_Y + (i % 10) * filterH);
				Drawing.DrawBox(sb, pos.X, pos.Y, filterW, filterH * Main.inventoryScale);
				Texture2D tex = filter.mode == null ? filter.tex : (filter.mode.Value ? Shockah.FCM.MBase.me.textures["Images/Tick"] : Main.cdTexture);
				if (tex != null)
				{
					float tscale = 1f;
					if (tscale * tex.Width > filterH - 2f) tscale = (filterH - 2f) / tex.Width;
					if (tscale * tex.Height > filterH - 2f) tscale = (filterH - 2f) / tex.Height;
					sb.Draw(tex, pos + new Vector2(filterH / 2f + 2, filterH / 2f), null, Color.White, 0f, tex.Size() / 2, tscale, SpriteEffects.None, 0f);
				}
				Vector2 measure = Main.fontMouseText.MeasureString(filter.name) * Main.inventoryScale;
				Drawing.StringShadowed(sb, Main.fontMouseText, filter.name, pos + new Vector2(filterH + 4, (filterH - measure.Y) / 2), Color.White, Main.inventoryScale);

				if (new Rectangle((int)pos.X, (int)pos.Y, (int)filterW, (int)filterH).Contains(Main.mouseX, Main.mouseY))
				{
					Main.localPlayer.mouseInterface = true;
					if (Main.mouseLeft && Main.mouseLeftRelease)
					{
						if (!KState.Special.Ctrl.Down()) foreach (Filter<BuffDefFCM> filter2 in filters) if (!object.ReferenceEquals(filter, filter2)) filter2.mode = null;
						if (filter.mode == null) filter.mode = true; else filter.mode = null;
						Refresh(true);
					}
					if (Main.mouseRight && Main.mouseRightRelease)
					{
						if (!KState.Special.Ctrl.Down()) foreach (Filter<BuffDefFCM> filter2 in filters) if (!object.ReferenceEquals(filter, filter2)) filter2.mode = null;
						if (filter.mode == null) filter.mode = false; else filter.mode = null;
						Refresh(true);
					}
				}
			}

			bSearch.Draw(sb, true, false);
			bSearchBar.Draw(sb, true, false);
			sortingChooser.Draw(sb, true, false);
		}

		public void Refresh(bool resetScroll)
		{
			Scroll = resetScroll ? 0 : Scroll;
			for (int i = 0; i < slots.Length; i++) slots[i] = new BuffSlot(this, i + Scroll * COLS, new Vector2(SLOT_W, SLOT_H));
			RunFilters();
		}

		protected void RunFilters()
		{
			filtered.Clear();
			foreach (BuffDefFCM def in defs)
			{
				if ((typing != null || filterText != null) && def.noModName.ToLower().IndexOf((typing == null ? filterText : typing).ToLower()) == -1) continue;
				if (!sorter.allow(def)) continue;
				foreach (Filter<BuffDefFCM> filter in filters) if (filter.mode != null) if (filter.mode == !filter.matches(def)) goto L;
				filtered.Add(def);
				L: { }
			}
			filtered.Sort(sorter);
			if (reverseSort) filtered.Reverse();
		}
	}
}