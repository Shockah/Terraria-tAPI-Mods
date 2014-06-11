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
	public class InterfaceFCMItems : InterfaceFCM<Item>
	{
		public const int BASE_OFF_X = 56, BASE_OFF_Y = 56;
		public const int COLS = 8, ROWS = 5, OFF_X = BASE_OFF_X - 6, OFF_Y = BASE_OFF_Y - 6, POS_X = 20, POS_Y = 306;
		public const float FILTER_W = 140, FILTER_H = 25, FILTER_X_OFF = 4;
		public const float SORT_TEXT_SCALE = .75f;

		public static InterfaceFCMItems me = null;
		protected static List<Item> defs = new List<Item>();

		public static void Reset()
		{
			defs.Clear();
			foreach (KeyValuePair<string, Item> kvp in Defs.items)
			{
				if (kvp.Value.type == Defs.unloadedItem.type) continue;
				if (kvp.Value.toolTip == "You shouldn't have this") continue;
				if (!Main.cEd && kvp.Key == "Vanilla:Carrot") continue;
				if (kvp.Key == "Vanilla:Red Potion") continue;
				if (kvp.Value.type == 58 || kvp.Value.type == 184 || kvp.Value.type == 1734 || kvp.Value.type == 1735 || kvp.Value.type == 1867 || kvp.Value.type == 1868) continue;
				if (kvp.Key.StartsWith("g:")) continue;
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
		protected readonly ElChooser<Sorter<Item>> sortingChooser;
		protected ItemSlotFCM[] slots = new ItemSlotFCM[COLS * ROWS];
		private int _Scroll = 0;
		protected readonly Filter<Item>
			FHead = new Filter<Item>("Head", Defs.items["Vanilla:Adamantite Helmet"].GetTexture(), (item) => item.headSlot != -1),
			FBody = new Filter<Item>("Body", Defs.items["Vanilla:Hallowed Plate Mail"].GetTexture(), (item) => item.bodySlot != -1),
			FLegs = new Filter<Item>("Legs", Defs.items["Vanilla:Titanium Leggings"].GetTexture(), (item) => item.legSlot != -1),
			FVanity = new Filter<Item>("Vanity", Defs.items["Vanilla:Clown Shirt"].GetTexture(), (item) => item.vanity),
			FMelee = new Filter<Item>("Melee", Defs.items["Vanilla:Terra Blade"].GetTexture(), (item) => item.damage > 0 && item.melee),
			FRanged = new Filter<Item>("Ranged", Defs.items["Vanilla:S.D.M.G."].GetTexture(), (item) => item.damage > 0 && item.ranged && (item.ammo == 0)),
			FAmmo = new Filter<Item>("Ammo", Defs.items["Vanilla:Chlorophyte Bullet"].GetTexture(), (item) => item.damage > 0 && item.ranged && item.ammo != 0 && !item.notAmmo),
			FMagic = new Filter<Item>("Magic", Defs.items["Vanilla:Golden Shower"].GetTexture(), (item) => item.damage > 0 && item.magic),
			FSummon = new Filter<Item>("Summon", Defs.items["Vanilla:Pygmy Staff"].GetTexture(), (item) => item.damage > 0 && item.summon),
			FAccessory = new Filter<Item>("Accessory", Defs.items["Vanilla:Ankh Shield"].GetTexture(), (item) => item.accessory),
			FPickaxe = new Filter<Item>("Pickaxe", Defs.items["Vanilla:Picksaw"].GetTexture(), (item) => item.pick > 0),
			FAxe = new Filter<Item>("Axe", Defs.items["Vanilla:Spirit Hamaxe"].GetTexture(), (item) => item.axe > 0),
			FHammer = new Filter<Item>("Hammer", Defs.items["Vanilla:Pwnhammer"].GetTexture(), (item) => item.hammer > 0),
			FConsumable = new Filter<Item>("Consumable", Defs.items["Vanilla:Lesser Healing Potion"].GetTexture(), (item) => item.consumable && item.damage <= 0 && item.createTile == -1 && item.tileWand == -1 && item.createWall == -1 && item.ammo == 0 && item.name != "Xmas decorations"),
			FDye = new Filter<Item>("Dye", Defs.items["Vanilla:Flame Dye"].GetTexture(), (item) => item.dye != 0),
			FPaint = new Filter<Item>("Paint", Defs.items["Vanilla:Blue Paint"].GetTexture(), (item) => item.paint != 0),
			FTile = new Filter<Item>("Tile", Defs.items["Vanilla:Stone Block"].GetTexture(), (item) => item.createTile != -1 || item.tileWand != -1 || item.name == "Xmas decorations"),
			FWall = new Filter<Item>("Wall", Defs.items["Vanilla:Green Brick Wall"].GetTexture(), (item) => item.createWall != -1),
			FPet = new Filter<Item>("Pet", Defs.items["Vanilla:Wisp in a Bottle"].GetTexture(), (item) => item.damage <= 0 && ((item.shoot > 0 && Main.projPet[item.shoot]) || (item.buffType > 0 && (Main.vanityPet[item.buffType] || Main.lightPet[item.buffType])))),
			FOther = new Filter<Item>("Other", Defs.unloadedItem.GetTexture(), null);
		protected readonly Sorter<Item>
			SID = new Sorter<Item>("ID", (i1, i2) => { return i1.type.CompareTo(i2.type); }, (item) => true),
			SName = new Sorter<Item>("Name", (i1, i2) => { return i1.displayName.CompareTo(i2.displayName); }, (item) => true),
			SValue = new Sorter<Item>("Value", (i1, i2) => { return i1.value.CompareTo(i2.value); }, (item) => item.value > 0),
			SRarity = new Sorter<Item>("Rarity", (i1, i2) => { return i1.rare.CompareTo(i2.rare); }, (item) => true),
			SDamage = new Sorter<Item>("Damage", (i1, i2) => { return i1.damage.CompareTo(i2.damage); }, (item) => item.damage > 0 && !item.notAmmo),
			SMana = new Sorter<Item>("Mana", (i1, i2) => { return i1.mana.CompareTo(i2.mana); }, (item) => item.mana > 0),
			SStack = new Sorter<Item>("Stack", (i1, i2) => { return i1.maxStack.CompareTo(i2.maxStack); }, (item) => true);

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

		public InterfaceFCMItems()
		{
			me = this;
			
			FOther.matches = (item) =>
			{
				foreach (Filter<Item> filter in filters) if (!object.ReferenceEquals(FOther, filter) && filter.matches(item)) return false;
				return true;
			};
			filters.AddRange(new Filter<Item>[]
				{
					FHead, FBody, FLegs, FVanity, FMelee, FRanged, FAmmo, FMagic, FSummon, FAccessory,
					FPickaxe, FAxe, FHammer, FConsumable, FDye, FPaint, FTile, FWall, FPet, FOther
				}
			);
			sorters.AddRange(new Sorter<Item>[] { SID, SName, SValue, SRarity, SDamage, SMana, SStack });

			slider = new ElSlider(
				(scroll) => { if (Scroll != scroll) { Scroll = scroll; Refresh(false); } },
				() => { return Scroll; },
				() => { return ROWS; },
				() => { return (int)Math.Ceiling(1f * filtered.Count / COLS); }
			);

			sorter = sorters[0];
			sortingChooser = new ElChooser<Sorter<Item>>(
				(item) => { reverseSort = object.ReferenceEquals(sorter, item) ? !reverseSort : false; sorter = item; Refresh(true); },
				() => { return sorter; },
				() => { return Shockah.FCM.MBase.me.textures[reverseSort ? "Images/ArrowDecrease.png" : "Images/ArrowIncrease.png"]; }
			);
			foreach (Sorter<Item> sorter2 in sorters) sortingChooser.Add(new Tuple<string, Sorter<Item>>(sorter2.name, sorter2));
		}

		public override void OnOpen()
		{
			base.OnOpen();
			if (!resetInterface) { resetInterface = true; return; }
			foreach (Filter<Item> filter in filters) filter.mode = null;
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
			
			if (Main.mouseItem.IsBlank())
			{
				int scrollBy = (Main.mouseState.ScrollWheelValue - Main.oldMouseState.ScrollWheelValue) / 120;
				int oldScroll = Scroll;
				Scroll -= scrollBy;
				if (Scroll != oldScroll) Refresh(false);
				
				for (int i = 0; i < slots.Length; i++)
				{
					if (slots[i].item.IsBlank())
					{
						Item hold = slots[i].ShouldHold();
						if (hold != null) slots[i].item.netDefaults(hold.netID);
					}
					slots[i].item.stack = slots[i].item.maxStack;
				}
			}

			SDrawing.StringShadowed(sb, Main.fontMouseText, (filtered.Count == defs.Count ? "Items" : "Matching items") + ": " + filtered.Count, new Vector2(POS_X, POS_Y - 26));

			Main.inventoryScale = .75f;
			for (int y = 0; y < ROWS; y++) for (int x = 0; x < COLS; x++)
			{
				slots[x + y * COLS].scale = Main.inventoryScale;
				slots[x + y * COLS].UpdatePos(new Vector2(POS_X + x * OFF_X * Main.inventoryScale, POS_Y + y * OFF_Y * Main.inventoryScale));
				slots[x + y * COLS].Draw(sb, true, !blocked);
			}

			slider.pos = new Vector2(POS_X + 4 + COLS * OFF_X * Main.inventoryScale, POS_Y);
			slider.size = new Vector2(16, ROWS * OFF_Y * Main.inventoryScale);
			blocked = slider.Draw(sb, true, !blocked) || blocked;

			SDrawing.StringShadowed(sb, Main.fontMouseText, "Sort:", new Vector2(POS_X + 16 + COLS * OFF_X * Main.inventoryScale, POS_Y - 26), Color.White, SORT_TEXT_SCALE);
			sortingChooser.pos = new Vector2(POS_X + 48 + COLS * OFF_X * Main.inventoryScale, POS_Y - 30);
			sortingChooser.size = new Vector2(72, 24);
			blocked = sortingChooser.Draw(sb, false, !blocked) || blocked;

			float filterW = FILTER_W * Main.inventoryScale;
			float filterH = FILTER_H * Main.inventoryScale;
			for (int i = 0; i < filters.Count; i++)
			{
				Filter<Item> filter = filters[i];
				Vector2 pos = new Vector2(POS_X + 32 + COLS * OFF_X * Main.inventoryScale + (i / 10) * (filterW + FILTER_X_OFF * Main.inventoryScale), POS_Y + (i % 10) * filterH);
				Drawing.DrawBox(sb, pos.X, pos.Y, filterW, filterH * Main.inventoryScale);
				Texture2D tex = filter.mode == null ? filter.tex : (filter.mode.Value ? Shockah.FCM.MBase.me.textures["Images/Tick.png"] : Main.cdTexture);
				float tscale = 1f;
				if (tscale * tex.Width > filterH - 2f) tscale = (filterH - 2f) / tex.Width;
				if (tscale * tex.Height > filterH - 2f) tscale = (filterH - 2f) / tex.Height;
				sb.Draw(tex, pos + new Vector2(filterH / 2f + 2, filterH / 2f), null, Color.White, 0f, tex.Size() / 2, tscale, SpriteEffects.None, 0f);
				Vector2 measure = Main.fontMouseText.MeasureString(filter.name) * Main.inventoryScale;
				SDrawing.StringShadowed(sb, Main.fontMouseText, filter.name, pos + new Vector2(filterH + 4, (filterH - measure.Y) / 2), Color.White, Main.inventoryScale);

				if (new Rectangle((int)pos.X, (int)pos.Y, (int)filterW, (int)filterH).Contains(Main.mouseX, Main.mouseY))
				{
					Main.localPlayer.mouseInterface = true;
					if (Main.mouseLeft && Main.mouseLeftRelease)
					{
						if (!Main.keyState.IsKeyDown(Keys.LeftControl)) foreach (Filter<Item> filter2 in filters) if (!object.ReferenceEquals(filter, filter2)) filter2.mode = null;
						if (filter.mode == null) filter.mode = true; else filter.mode = null;
						Refresh(true);
					}
					if (Main.mouseRight && Main.mouseRightRelease)
					{
						if (!Main.keyState.IsKeyDown(Keys.LeftControl)) foreach (Filter<Item> filter2 in filters) if (!object.ReferenceEquals(filter, filter2)) filter2.mode = null;
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
			for (int i = 0; i < slots.Length; i++) slots[i] = new ItemSlotFCM(this, i + Scroll * COLS);
			RunFilters();
		}

		protected void RunFilters()
		{
			filtered.Clear();
			foreach (Item def in defs)
			{
				if ((typing != null || filterText != null) && def.displayName.ToLower().IndexOf((typing == null ? filterText : typing).ToLower()) == -1) continue;
				if (!sorter.allow(def)) continue;
				foreach (Filter<Item> filter in filters) if (filter.mode != null) if (filter.mode == !filter.matches(def)) goto L;
				filtered.Add(def);
				L: { }
			}
			filtered.Sort(sorter);
			if (reverseSort) filtered.Reverse();
		}
	}
}