﻿using LitJson;
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
		public enum EPage
		{
			Type,
			Mod,
			Tags
		}
		
		public const int BASE_OFF_X = 56, BASE_OFF_Y = 56;
		public const int COLS = 8, ROWS = 5, OFF_X = BASE_OFF_X - 6, OFF_Y = BASE_OFF_Y - 6, POS_X = 20, POS_Y = 306;
		public const float FILTER_W = 140, FILTER_H = 25, FILTER_X_OFF = 4;
		public const float SORT_TEXT_SCALE = .75f;

		public static InterfaceFCMItems me = null;
		protected static List<Item> defs = new List<Item>();
		protected static Dictionary<int, List<string>> tags = new Dictionary<int, List<string>>();
		public static bool displayIds = false;

		public static void Reset()
		{
			defs.Clear();
			foreach (KeyValuePair<string, Item> kvp in ItemDef.byName)
			{
				if (kvp.Value.IsBlank()) continue;
				if (kvp.Value.type == ItemDef.unloadedItem.type) continue;
				if (kvp.Value.tooltip == "You shouldn't have this") continue;
				if (!Main.cEd && kvp.Key == "Vanilla:Carrot") continue;
				if (kvp.Key == "Vanilla:Red Potion") continue;
				if (kvp.Value.type == 58 || kvp.Value.type == 184 || kvp.Value.type == 1734 || kvp.Value.type == 1735 || kvp.Value.type == 1867 || kvp.Value.type == 1868) continue;
				if (kvp.Key.StartsWith("g:")) continue;
				defs.Add(kvp.Value);
				
				ItemDef idef = kvp.Value.def;
				if (idef.json != null)
				{
					JsonData j = idef.json;
					if (j.Has("fcmtags"))
					{
						JsonData jTags = j["fcmtags"];
						if (!jTags.IsArray)
						{
							throw new Exception(string.Format("%s: 'fcmtags' JSON property should be an array", kvp.Value.name));
						}
						else
						{
							if (jTags.Count != 0)
							{
								List<string> tags = new List<string>();
								foreach (JsonData jTag in jTags)
									tags.Add((string)jTag);
								InterfaceFCMItems.tags[kvp.Value.netID] = tags;
							}
						}
					}
				}
			}
			defs.Sort((i1, i2) =>
				{
					if (i1.type == i2.type) return i2.netID.CompareTo(i1.netID);
					return i1.type.CompareTo(i2.type);
				}
			);
		}

		protected readonly ElSlider slider, slider2;
		protected readonly ElChooser<Sorter<Item>> sortingChooser;
		protected readonly ElButton bSearch, bSearchBar;
		protected EPage page = EPage.Type;
		public readonly List<Filter<Item>> tagFilters = new List<Filter<Item>>();
		protected ItemSlotFCM[] slots = new ItemSlotFCM[COLS * ROWS];
		private int _Scroll = 0, _Scroll2 = 0;
		protected readonly Filter<Item>
			FHead, FBody, FLegs, FVanity, FMelee, FRanged, FAmmo, FMagic, FSummon, FAccessory,
			FPickaxe, FAxe, FHammer, FConsumable, FDye, FPaint, FTile, FWall, FPet, FOther;
		protected readonly Sorter<Item>
			SID, SName, SValue, SValueStack, SRarity, SDamage, SMana, SPowerPick, SPowerAxe, SPowerHammer, SDefense, SStack;

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

		protected int Scroll2
		{
			get
			{
				return _Scroll2;
			}
			set
			{
				_Scroll2 = Math.Min(Math.Max(value, 0), Scroll2Max);
			}
		}
		protected int Scroll2Max
		{
			get
			{
				return Math.Max((int)Math.Ceiling(1f * ((page == EPage.Mod ? modFilters.Count : tagFilters.Count) - 10)), 0);
			}
		}

		public InterfaceFCMItems()
		{
			me = this;
			if (Main.dedServ) return;

			FHead = new Filter<Item>("Head", ItemDef.byName["Vanilla:Adamantite Helmet"].GetTexture(), (item) => item.headSlot != -1);
			FBody = new Filter<Item>("Body", ItemDef.byName["Vanilla:Hallowed Plate Mail"].GetTexture(), (item) => item.bodySlot != -1);
			FLegs = new Filter<Item>("Legs", ItemDef.byName["Vanilla:Titanium Leggings"].GetTexture(), (item) => item.legSlot != -1);
			FVanity = new Filter<Item>("Vanity", ItemDef.byName["Vanilla:Clown Shirt"].GetTexture(), (item) => item.vanity || (item.accessory && item.displayName.StartsWith("Music Box (") && item.displayName.EndsWith(")")));
			FMelee = new Filter<Item>("Melee", ItemDef.byName["Vanilla:Terra Blade"].GetTexture(), (item) => item.damage > 0 && item.melee);
			FRanged = new Filter<Item>("Ranged", ItemDef.byName["Vanilla:S.D.M.G."].GetTexture(), (item) => item.damage > 0 && item.ranged && (item.ammo == 0));
			FAmmo = new Filter<Item>("Ammo", ItemDef.byName["Vanilla:Chlorophyte Bullet"].GetTexture(), (item) => item.damage > 0 && item.ranged && item.ammo != 0 && !item.notAmmo);
			FMagic = new Filter<Item>("Magic", ItemDef.byName["Vanilla:Golden Shower"].GetTexture(), (item) => item.damage > 0 && item.magic);
			FSummon = new Filter<Item>("Summon", ItemDef.byName["Vanilla:Pygmy Staff"].GetTexture(), (item) => item.damage > 0 && item.summon);
			FAccessory = new Filter<Item>("Accessory", ItemDef.byName["Vanilla:Ankh Shield"].GetTexture(), (item) => item.accessory);
			FPickaxe = new Filter<Item>("Pickaxe", ItemDef.byName["Vanilla:Picksaw"].GetTexture(), (item) => item.pick > 0);
			FAxe = new Filter<Item>("Axe", ItemDef.byName["Vanilla:Spectre Hamaxe"].GetTexture(), (item) => item.axe > 0);
			FHammer = new Filter<Item>("Hammer", ItemDef.byName["Vanilla:Pwnhammer"].GetTexture(), (item) => item.hammer > 0);
			FConsumable = new Filter<Item>("Consumable", ItemDef.byName["Vanilla:Lesser Healing Potion"].GetTexture(), (item) => item.consumable && item.damage <= 0 && item.createTile == -1 && item.tileWand == -1 && item.createWall == -1 && item.ammo == 0 && item.name != "Xmas decorations");
			FDye = new Filter<Item>("Dye", ItemDef.byName["Vanilla:Flame Dye"].GetTexture(), (item) => item.dye != 0);
			FPaint = new Filter<Item>("Paint", ItemDef.byName["Vanilla:Blue Paint"].GetTexture(), (item) => item.paint != 0);
			FTile = new Filter<Item>("Tile", ItemDef.byName["Vanilla:Stone Block"].GetTexture(), (item) => item.createTile != -1 || item.tileWand != -1 || item.name == "Xmas decorations");
			FWall = new Filter<Item>("Wall", ItemDef.byName["Vanilla:Green Brick Wall"].GetTexture(), (item) => item.createWall != -1);
			FPet = new Filter<Item>("Pet", ItemDef.byName["Vanilla:Wisp in a Bottle"].GetTexture(), (item) => item.damage <= 0 && ((item.shoot > 0 && Main.projPet[item.shoot]) || (item.buffType > 0 && (Main.vanityPet[item.buffType] || Main.lightPet[item.buffType]))));
			FOther = new Filter<Item>("Other", ItemDef.unloadedItem.GetTexture(), null);

			SID = new Sorter<Item>("ID", (i1, i2) => i1.type.CompareTo(i2.type), (item) => true);
			SName = new Sorter<Item>("Name", (i1, i2) => i1.displayName.CompareTo(i2.displayName), (item) => true);
			SValue = new Sorter<Item>("Value", (i1, i2) => i1.value.CompareTo(i2.value), (item) => item.value > 0 && (item.type < 71 || item.type > 74));
			SValueStack = new Sorter<Item>("Stack value", (i1, i2) => (i1.value * i1.maxStack).CompareTo(i2.value * i2.maxStack), (item) => item.value > 0 && (item.type < 71 || item.type > 74));
			SRarity = new Sorter<Item>("Rarity", (i1, i2) => i1.rare.CompareTo(i2.rare), (item) => true);
			SDamage = new Sorter<Item>("Damage", (i1, i2) => i1.damage.CompareTo(i2.damage), (item) => item.damage > 0 && !item.notAmmo);
			SMana = new Sorter<Item>("Mana cost", (i1, i2) => i1.mana.CompareTo(i2.mana), (item) => item.mana > 0);
			SPowerPick = new Sorter<Item>("Pickaxe %", (i1, i2) => i1.pick.CompareTo(i2.pick), (item) => item.pick > 0);
			SPowerAxe = new Sorter<Item>("Axe %", (i1, i2) => i1.axe.CompareTo(i2.axe), (item) => item.axe > 0);
			SPowerHammer = new Sorter<Item>("Hammer %", (i1, i2) => i1.hammer.CompareTo(i2.hammer), (item) => item.hammer > 0);
			SDefense = new Sorter<Item>("Defense", (i1, i2) => i1.defense.CompareTo(i2.defense), (item) => item.headSlot != -1 || item.bodySlot != -1 || item.legSlot != -1);
			SStack = new Sorter<Item>("Stack", (i1, i2) => i1.maxStack.CompareTo(i2.maxStack), (item) => true);
			
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
			sorters.AddRange(new Sorter<Item>[] { SID, SName, SValue, SValueStack, SRarity, SDamage, SMana, SPowerPick, SPowerAxe, SPowerHammer, SDefense, SStack });

			slider = new ElSlider(
				(scroll) => { if (Scroll != scroll) { Scroll = scroll; Refresh(false); } },
				() => { return Scroll; },
				() => { return ROWS; },
				() => { return (int)Math.Ceiling(1f * filtered.Count / COLS); }
			);
			slider2 = new ElSlider(
				(scroll) => { if (Scroll2 != scroll) { Scroll2 = scroll; } },
				() => { return Scroll2; },
				() => { return 10; },
				() => { return (int)Math.Ceiling(1f * (page == EPage.Mod ? modFilters.Count : tagFilters.Count)); }
			);

			sorter = sorters[0];
			sortingChooser = new ElChooser<Sorter<Item>>(
				(item) => { reverseSort = object.ReferenceEquals(sorter, item) ? !reverseSort : false; sorter = item; Refresh(true); },
				() => { return sorter; },
				() => { return Shockah.FCM.MBase.me.textures[reverseSort ? "Images/ArrowDecrease" : "Images/ArrowIncrease"]; }
			);
			foreach (Sorter<Item> sorter2 in sorters) sortingChooser.Add(new Tuple<string, Sorter<Item>>(sorter2.name, sorter2));

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

			List<string> contains = new List<string>();
			foreach (Item def in defs)
			{
				ModBase modBase = def.def.modBase;
				string modName = modBase == null ? "Vanilla" : modBase.mod.InternalName;
				if (!contains.Contains(modName))
				{
					contains.Add(modName);
					modFilters.Add(new Filter<Item>(modBase == null ? "Vanilla" : modBase.mod.DisplayName, modBase == null ? null : modBase.mod.Icon, (item) => item.def.modBase == modBase));
				}
			}

			contains.Clear();
			foreach (KeyValuePair<int, List<string>> kvp in tags)
			{
				foreach (string stag in kvp.Value)
				{
					if (!contains.Contains(stag))
					{
						contains.Add(stag);
						tagFilters.Add(new Filter<Item>(stag, null, (item) => tags.ContainsKey(item.netID) && tags[item.netID].Contains(stag)));
					}
				}
			}

			foreach (Filter<Item> filter in filters) filter.mode = null;
			foreach (Filter<Item> filter in modFilters) filter.mode = null;
			foreach (Filter<Item> filter in tagFilters) filter.mode = null;
			sorter = sorters[0];
			reverseSort = false;
			Refresh(true);
		}

		public override void OnOpen()
		{
			base.OnOpen();
			if (!resetInterface) { resetInterface = true; return; }
			/*foreach (Filter<Item> filter in filters) filter.mode = null;
			sorter = sorters[0];
			reverseSort = false;
			Refresh(true);*/
		}

		public override void Draw(InterfaceLayer layer, SpriteBatch sb)
		{
			bool blocked = false;
			string oldTyping = typing;
			base.Draw(layer, sb);

			Main.inventoryScale = .75f;
			bSearch.pos = new Vector2(POS_X - 12 + COLS * OFF_X * Main.inventoryScale, POS_Y + ROWS * OFF_Y * Main.inventoryScale + 4);
			bSearch.size = new Vector2(32, 32);
			blocked = bSearch.Draw(sb, false, !blocked) || blocked;

			bSearchBar.pos = new Vector2(POS_X, POS_Y + ROWS * OFF_Y * Main.inventoryScale + 4);
			bSearchBar.size = new Vector2(20 + COLS * OFF_X * Main.inventoryScale - 36, 32);
			blocked = bSearchBar.Draw(sb, false, !blocked) || blocked;

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

			Drawing.StringShadowed(sb, Main.fontMouseText, (filtered.Count == defs.Count ? "Items" : "Matching items") + ": " + filtered.Count, new Vector2(POS_X, POS_Y - 26));

			for (int y = 0; y < ROWS; y++) for (int x = 0; x < COLS; x++)
			{
				slots[x + y * COLS].scale = Main.inventoryScale;
				slots[x + y * COLS].UpdatePos(new Vector2(POS_X + x * OFF_X * Main.inventoryScale, POS_Y + y * OFF_Y * Main.inventoryScale));
				slots[x + y * COLS].Draw(sb, true, !blocked);
			}

			slider.pos = new Vector2(POS_X + 4 + COLS * OFF_X * Main.inventoryScale, POS_Y);
			slider.size = new Vector2(16, ROWS * OFF_Y * Main.inventoryScale);
			blocked = slider.Draw(sb, true, !blocked) || blocked;

			Drawing.StringShadowed(sb, Main.fontMouseText, "Sort:", new Vector2(POS_X - 8 + COLS * OFF_X * Main.inventoryScale, POS_Y - 22), Color.White, SORT_TEXT_SCALE);
			sortingChooser.pos = new Vector2(POS_X + 24 + COLS * OFF_X * Main.inventoryScale, POS_Y - 26);
			sortingChooser.size = new Vector2(96, 20);
			blocked = sortingChooser.Draw(sb, false, !blocked) || blocked;

			bool hasTags = tags.Count != 0;
			switch (page)
			{
				case EPage.Type:
					{
						float filterW = FILTER_W * Main.inventoryScale;
						float filterH = FILTER_H * Main.inventoryScale;
						for (int i = 0; i < filters.Count; i++)
						{
							Filter<Item> filter = filters[i];
							Vector2 pos = new Vector2(POS_X + 32 + COLS * OFF_X * Main.inventoryScale + (i / 10) * (filterW + FILTER_X_OFF * Main.inventoryScale), POS_Y + (i % 10) * filterH);
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
									if (!KState.Special.Ctrl.Down()) foreach (Filter<Item> filter2 in filters) if (!object.ReferenceEquals(filter, filter2)) filter2.mode = null;
									if (filter.mode == null) filter.mode = true; else filter.mode = null;
									Refresh(true);
								}
								if (Main.mouseRight && Main.mouseRightRelease)
								{
									if (!KState.Special.Ctrl.Down()) foreach (Filter<Item> filter2 in filters) if (!object.ReferenceEquals(filter, filter2)) filter2.mode = null;
									if (filter.mode == null) filter.mode = false; else filter.mode = null;
									Refresh(true);
								}
							}
						}
						break;
					}
				case EPage.Mod:
					{
						float sliderW = 16;
						float filterW = (FILTER_W * Main.inventoryScale) * 2 + FILTER_X_OFF * Main.inventoryScale - sliderW - 4;
						float filterH = FILTER_H * Main.inventoryScale;

						slider2.pos = new Vector2(POS_X + 32 + COLS * OFF_X * Main.inventoryScale + filterW + 4, POS_Y);
						slider2.size = new Vector2(sliderW, filterH * 10);
						blocked = slider2.Draw(sb, true, !blocked) || blocked;
						
						for (int i = 0; i < modFilters.Count; i++)
						{
							Filter<Item> filter = modFilters[i];
							Vector2 pos = new Vector2(POS_X + 32 + COLS * OFF_X * Main.inventoryScale, POS_Y + i * filterH);
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
							float txtscale = Main.inventoryScale;
							if (measure.X * txtscale > filterW - filterH - 8)
								txtscale = (filterW - filterH - 8) / measure.X;
							Drawing.StringShadowed(sb, Main.fontMouseText, filter.name, pos + new Vector2(filterH + 4, (filterH - measure.Y) / 2), Color.White, txtscale);

							if (new Rectangle((int)pos.X, (int)pos.Y, (int)filterW, (int)filterH).Contains(Main.mouseX, Main.mouseY))
							{
								Main.localPlayer.mouseInterface = true;
								if (Main.mouseLeft && Main.mouseLeftRelease)
								{
									if (!KState.Special.Ctrl.Down()) foreach (Filter<Item> filter2 in modFilters) if (!object.ReferenceEquals(filter, filter2)) filter2.mode = null;
									if (filter.mode == null) filter.mode = true; else filter.mode = null;
									Refresh(true);
								}
								if (Main.mouseRight && Main.mouseRightRelease)
								{
									if (!KState.Special.Ctrl.Down()) foreach (Filter<Item> filter2 in modFilters) if (!object.ReferenceEquals(filter, filter2)) filter2.mode = null;
									if (filter.mode == null) filter.mode = false; else filter.mode = null;
									Refresh(true);
								}
							}
						}
						break;
					}
				case EPage.Tags:
					{
						float sliderW = 16;
						float filterW = (FILTER_W * Main.inventoryScale) * 2 + FILTER_X_OFF * Main.inventoryScale - sliderW - 4;
						float filterH = FILTER_H * Main.inventoryScale;

						slider2.pos = new Vector2(POS_X + 32 + COLS * OFF_X * Main.inventoryScale + filterW + 4, POS_Y);
						slider2.size = new Vector2(sliderW, filterH * 10);
						blocked = slider2.Draw(sb, true, !blocked) || blocked;

						for (int i = 0; i < modFilters.Count; i++)
						{
							Filter<Item> filter = tagFilters[i];
							Vector2 pos = new Vector2(POS_X + 32 + COLS * OFF_X * Main.inventoryScale, POS_Y + i * filterH);
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
							float txtscale = Main.inventoryScale;
							if (measure.X * txtscale > filterW - filterH - 8)
								txtscale = (filterW - filterH - 8) / measure.X;
							Drawing.StringShadowed(sb, Main.fontMouseText, filter.name, pos + new Vector2(filterH + 4, (filterH - measure.Y) / 2), Color.White, txtscale);

							if (new Rectangle((int)pos.X, (int)pos.Y, (int)filterW, (int)filterH).Contains(Main.mouseX, Main.mouseY))
							{
								Main.localPlayer.mouseInterface = true;
								if (Main.mouseLeft && Main.mouseLeftRelease)
								{
									if (!KState.Special.Ctrl.Down()) foreach (Filter<Item> filter2 in tagFilters) if (!object.ReferenceEquals(filter, filter2)) filter2.mode = null;
									if (filter.mode == null) filter.mode = true; else filter.mode = null;
									Refresh(true);
								}
								if (Main.mouseRight && Main.mouseRightRelease)
								{
									if (!KState.Special.Ctrl.Down()) foreach (Filter<Item> filter2 in tagFilters) if (!object.ReferenceEquals(filter, filter2)) filter2.mode = null;
									if (filter.mode == null) filter.mode = false; else filter.mode = null;
									Refresh(true);
								}
							}
						}
						break;
					}
			}

			float pageW = (((FILTER_W * Main.inventoryScale) * 2 + FILTER_X_OFF * Main.inventoryScale) - (hasTags ? 2 : 1) * FILTER_X_OFF * Main.inventoryScale) / (hasTags ? 3 : 2);
			float pageH = FILTER_H * Main.inventoryScale;
			for (int i = 0; i < (hasTags ? 3 : 2); i++)
			{
				bool onButton = (int)page == i;
				Vector2 pos = new Vector2(POS_X + 32 + COLS * OFF_X * Main.inventoryScale + i * (pageW + FILTER_X_OFF * Main.inventoryScale), POS_Y + ROWS * OFF_Y * Main.inventoryScale + 34 - pageH);
				if (new Rectangle((int)pos.X, (int)pos.Y, (int)pageW, (int)pageH).Contains(Main.mouse))
				{
					Main.localPlayer.mouseInterface = true;
					if (Main.mouseLeft && Main.mouseLeftRelease)
					{
						page = (EPage)i;
						Main.PlaySound(7);
					}
					onButton = true;
				}
				
				string pageName = i == 0 ? "Type" : (i == 1 ? "Mod" : "Tag");
				
				Drawing.DrawBox(sb, pos.X, pos.Y, pageW, pageH, onButton ? 0.785f : 0.5f);
				Vector2 measure = Main.fontMouseText.MeasureString(pageName) * Main.inventoryScale;
				Drawing.StringShadowed(sb, Main.fontMouseText, pageName, pos + new Vector2((pageW - measure.X) / 2, (pageH - measure.Y) / 2), Color.White, Main.inventoryScale);
			}

			bSearch.Draw(sb, true, false);
			bSearchBar.Draw(sb, true, false);
			sortingChooser.Draw(sb, true, false);
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
			string typing = this.typing == null ? filterText : this.typing;

			NPC searchNPC = null;
			List<Item> searchNPCItems = null;
			if (typing != null)
			{
				if (typing.StartsWith("npc:"))
				{
					typing = typing.Substring(4).Trim();
					if (NPCDef.byName.ContainsKey(typing))
					{
						searchNPC = NPCDef.byName[typing];
						searchNPCItems = new List<Item>();
						List<LootRule> rules = new List<LootRule>();
						if (LootRule.rulesName.ContainsKey(searchNPC.name))
							rules.AddRange(LootRule.rulesName[searchNPC.name]);
						if (LootRule.rulesType.ContainsKey(searchNPC.type))
							rules.AddRange(LootRule.rulesType[searchNPC.type]);
						
						while (rules.Count != 0)
						{
							LootRule rule = rules[0];
							rules.RemoveAt(0);
							if (rule.item.func == null && rule.item.value != null && !rule.item.value.IsBlank())
							{
								searchNPCItems.Add(rule.item.value);
							}
							rules.AddRange(rule.subrules);
						}

						rules.AddRange(LootRule.globalRules);

						while (rules.Count != 0)
						{
							LootRule rule = rules[0];
							rules.RemoveAt(0);
							
							bool fail = false;
							foreach (Func<NPC, bool> coderule in rule.rules)
							{
								if (!coderule(searchNPC))
								{
									fail = true;
									break;
								}
							}

							if (!fail)
							{
								if (rule.item.func == null && rule.item.value != null && !rule.item.value.IsBlank())
								{
									searchNPCItems.Add(rule.item.value);
								}
								rules.AddRange(rule.subrules);
							}
						}
					}
				}
				else
				{
					typing = typing.ToLower();
				}
			}

			foreach (Item def in defs)
			{
				if (!sorter.allow(def)) continue;
				if (searchNPC != null)
				{
					bool fail = true;
					foreach (Item lootitem in searchNPCItems)
					{
						if (lootitem.IsTheSameAs(def))
						{
							fail = false;
							break;
						}
					}
					if (fail) continue;
				}
				else
				{
					if (typing != null && def.displayName.ToLower().IndexOf(typing) == -1) continue;
				}
				foreach (Filter<Item> filter in filters) if (filter.mode != null) if (filter.mode == !filter.matches(def)) goto L;
				foreach (Filter<Item> filter in modFilters) if (filter.mode != null) if (filter.mode == !filter.matches(def)) goto L;
				foreach (Filter<Item> filter in tagFilters) if (filter.mode != null) if (filter.mode == !filter.matches(def)) goto L;
				filtered.Add(def);
				L: { }
			}
			filtered.Sort(sorter);
			if (reverseSort) filtered.Reverse();
		}
	}
}