using C3.XNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Shockah.Base;
using System;
using System.Collections.Generic;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.FCM.Standard
{
	public class InterfaceFCMNPCs : InterfaceFCM<NPC>
	{
		public const int BASE_OFF_X = 56, BASE_OFF_Y = 56;
		public const int COLS = 6, ROWS = 4, OFF_X = BASE_OFF_X - 6, OFF_Y = BASE_OFF_Y - 6, POS_X = 20, POS_Y = 306;
		public const float FILTER_W = 140, FILTER_H = 25, FILTER_X_OFF = 4;
		public const float SORT_TEXT_SCALE = .75f;

		public static InterfaceFCMNPCs me = null;
		public static NPC spawning = null;
		public static Vector2? spawnPoint = null;
		public static bool fakeUpdating = false;
		protected static List<NPC> defs = new List<NPC>();

		public static void Reset()
		{
			defs.Clear();
			foreach (KeyValuePair<string, NPC> kvp in NPCDef.byName)
			{
				if (string.IsNullOrEmpty(kvp.Value.displayName) && string.IsNullOrEmpty(kvp.Value.displayName)) continue;
				defs.Add(kvp.Value);
			}
			defs.Sort((i1, i2) =>
				{
					if (i1.type == i2.type) return i2.netID.CompareTo(i1.netID);
					return i1.type.CompareTo(i2.type);
				}
			);
		}

		public static void SpawnNPCs(int netID, int x, int y, float radius, int count, int seed, bool fakeUpdateNPCs = true)
		{
			Random rand = new Random(seed);

			List<NPC> list = new List<NPC>();
			for (int i = 0; i < count; i++)
			{
				Vector2 pos = new Vector2(x, y) + Math2.LdirVector2((float)(rand.NextDouble() * radius), (float)(rand.NextDouble() * 360d));
				int newNPC = NPC.NewNPC((int)pos.X, (int)pos.Y, netID);
				if (newNPC >= 0 && newNPC < Main.npc.Length - 1)
				{
					NPC npcInst = Main.npc[newNPC];
					if (SBase.RequiresAttaching(npcInst))
					{
						npcInst.ai[0] = (int)(pos.X / 16);
						npcInst.ai[1] = (int)(pos.Y / 16);
						npcInst.netUpdate = true;
					}
					if (netID == 70) continue;
					list.Add(npcInst);
				}
			}

			if (list.Count > 1 && fakeUpdateNPCs)
			{
				fakeUpdating = true;
				
				Projectile[] cacheProjectiles = Main.projectile;
				Main.projectile = new Projectile[Main.projectile.Length];
				for (int i = 0; i < Main.projectile.Length; i++) Main.projectile[i] = new Projectile();

				NPC[] cacheNPCs = Main.npc;
				Main.npc = new NPC[Main.npc.Length];
				for (int i = 0; i < Main.npc.Length; i++) Main.npc[i] = new NPC();

				Dust[] cacheDust = Main.dust;
				Main.dust = new Dust[Main.dust.Length];
				for (int i = 0; i < Main.dust.Length; i++) Main.dust[i] = new Dust();

				//apparently i can't block the lighting updates anymore...
				//int tempLightCount = Lighting.tempLightCount;
				//Lighting.tempLightCount = 2000; //Lighting.maxTempLights (which is private)

				foreach (NPC npc in list)
				{
					Vector2 pos = npc.position;
					int times = rand.Next(600);
					for (int i = 0; i < times; i++) npc.UpdateNPC(npc.whoAmI);
					npc.position = pos;
					npc.oldPosition = pos;
				}

				//Lighting.tempLightCount = tempLightCount;
				Main.dust = cacheDust;
				Main.npc = cacheNPCs;
				Main.projectile = cacheProjectiles;

				fakeUpdating = false;
				if (Main.netMode == 2) foreach (NPC npc in list) NetMessage.SendData(23, -1, -1, "", npc.whoAmI);
			}
		}

		protected readonly ElSlider slider;
		protected readonly ElChooser<Sorter<NPC>> sortingChooser;
		protected readonly ElButton bSearch, bSearchBar;
		protected NPCSlot[] slots = new NPCSlot[COLS * ROWS];
		private int _Scroll = 0;
		protected readonly Filter<NPC>
			FFriendly, FTown, FBoss, FOther;
		protected readonly Sorter<NPC>
			SID, SName, SLife, SDamage, SDefense;

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
			me = this;
			if (Main.dedServ) return;

			FFriendly = new Filter<NPC>("Friendly", ItemDef.byName["Vanilla:Carrot"].GetTexture(), (npc) => npc.friendly || npc.damage <= 0);
			FTown = new Filter<NPC>("Town", ItemDef.byName["Vanilla:Guide Voodoo Doll"].GetTexture(), (npc) => npc.townNPC);
			FBoss = new Filter<NPC>("Boss", ItemDef.byName["Vanilla:Suspicious Looking Eye"].GetTexture(), (npc) => SBase.IsBoss(npc));
			FOther = new Filter<NPC>("Other", ItemDef.unloadedItem.GetTexture(), null);

			SID = new Sorter<NPC>("ID", (i1, i2) => { return i1.type.CompareTo(i2.type); }, (npc) => true);
			SName = new Sorter<NPC>("Name", (i1, i2) => {
				string s1 = string.IsNullOrEmpty(i1.displayName) ? i1.name : i1.displayName;
				string s2 = string.IsNullOrEmpty(i2.displayName) ? i2.name : i2.displayName;
				return s1.CompareTo(s2);
			}, (npc) => true);
			SLife = new Sorter<NPC>("LIfe", (i1, i2) => { return i1.lifeMax.CompareTo(i2.lifeMax); }, (npc) => true);
			SDamage = new Sorter<NPC>("Damage", (i1, i2) => { return i1.damage.CompareTo(i2.damage); }, (npc) => npc.damage > 0);
			SDefense = new Sorter<NPC>("Defense", (i1, i2) => { return i1.defense.CompareTo(i2.defense); }, (npc) => true);

			FOther.matches = (item) =>
			{
				foreach (Filter<NPC> filter in filters) if (!object.ReferenceEquals(FOther, filter) && filter.matches(item)) return false;
				return true;
			};
			filters.AddRange(new Filter<NPC>[]
				{
					FFriendly, FTown, FBoss, FOther
				}
			);
			sorters.AddRange(new Sorter<NPC>[] { SID, SName, SLife, SDamage, SDefense });

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
				() => { return Shockah.FCM.MBase.me.textures[reverseSort ? "Images/ArrowDecrease" : "Images/ArrowIncrease"]; }
			);
			foreach (Sorter<NPC> sorter2 in sorters) sortingChooser.Add(new Tuple<string, Sorter<NPC>>(sorter2.name, sorter2));

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

			bSearch.pos = new Vector2(POS_X + COLS * OFF_X - 12, POS_Y + ROWS * OFF_Y + 4);
			bSearch.size = new Vector2(32, 32);
			blocked = bSearch.Draw(sb, false, !blocked) || blocked;

			bSearchBar.pos = new Vector2(POS_X, POS_Y + ROWS * OFF_Y + 4);
			bSearchBar.size = new Vector2(COLS * OFF_X - 16, 32);
			blocked = bSearchBar.Draw(sb, false, !blocked) || blocked;

			if (oldTyping != typing) Refresh(true);

			if (spawning != null)
			{
				Main.localPlayer.mouseInterface = true;

				float radius = Math.Max((spawning.width + spawning.height) * (1f / 3f), 3f);
				if (!spawnPoint.HasValue)
				{
					sb.DrawCircle(Main.mouse, radius, (int)Math.Max(16, radius / 2), Color.White);
					if (Main.mouseLeft && Main.mouseLeftRelease) spawnPoint = Main.mouseWorld;
				}
				else
				{
					float radiusSpawner = Vector2.Distance(spawnPoint.Value, Main.mouseWorld);
					sb.DrawCircle(spawnPoint.Value - Main.screenPosition, radiusSpawner, (int)Math.Max(16, radiusSpawner / 2), Color.White);

					float fieldOne = (float)(Math.PI * Math.Pow(radius, 2));
					float field = (float)(Math.PI * Math.Pow(radiusSpawner, 2));
					float fieldMin = field + fieldOne * 4;
					int randSeed = BitConverter.ToInt32(BitConverter.GetBytes(spawnPoint.Value.X), 0) ^ BitConverter.ToInt32(BitConverter.GetBytes(spawnPoint.Value.Y), 0) ^ Main.mouseX ^ Main.mouseY;
					Random rand = new Random(randSeed);

					int count = (int)(fieldMin / (fieldOne * 4));
					List<NPC> list = new List<NPC>();
					for (int i = 0; i < count; i++)
					{
						Vector2 pos = spawnPoint.Value + Math2.LdirVector2((float)(rand.NextDouble() * radiusSpawner), (float)(rand.NextDouble() * 360d));
						sb.DrawCircle(pos - Main.screenPosition, radius, (int)Math.Max(16, radius / 2), Color.White);
					}

					if (!Main.mouseLeft)
					{
						if (Main.netMode == 1)
						{
							BinBuffer bb = new BinBuffer();
							bb.Write((short)spawning.netID);
							bb.Write((int)spawnPoint.Value.X);
							bb.Write((int)spawnPoint.Value.Y);
							bb.Write(radiusSpawner);
							bb.Write((ushort)count);
							bb.Write(randSeed);
							NetMessage.SendModData(MBase.me, MBase.MSG_SPAWN_NPCS, -1, -1, bb);
						}
						else
						{
							SpawnNPCs(spawning.netID, (int)spawnPoint.Value.X, (int)spawnPoint.Value.Y, radiusSpawner, count, randSeed);
						}
						spawnPoint = null;
						spawning = null;
					}

					if (Main.mouseRight && Main.mouseRightRelease)
					{
						if (spawnPoint.HasValue)
						{
							spawnPoint = null;
						}
						else
						{
							spawning = null;
						}
					}

					Drawing.StringShadowed(sb, Main.fontMouseText, "" + count, Main.mouse - new Vector2(Main.fontMouseText.MeasureString("" + count).X / 2f, 24));
				}

				return;
			}

			int scrollBy = (Main.mouseState.ScrollWheelValue - Main.oldMouseState.ScrollWheelValue) / 120;
			int oldScroll = Scroll;
			Scroll -= scrollBy;
			if (Scroll != oldScroll) Refresh(false);

			Drawing.StringShadowed(sb, Main.fontMouseText, (filtered.Count == defs.Count ? "NPCs" : "Matching NPCs") + ": " + filtered.Count, new Vector2(POS_X, POS_Y - 26));

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

			Drawing.StringShadowed(sb, Main.fontMouseText, "Sort:", new Vector2(POS_X - 8 + COLS * OFF_X * Main.inventoryScale, POS_Y - 22), Color.White, SORT_TEXT_SCALE);
			sortingChooser.pos = new Vector2(POS_X + 24 + COLS * OFF_X * Main.inventoryScale, POS_Y - 26);
			sortingChooser.size = new Vector2(96, 20);
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
						if (!KState.Special.Ctrl.Down()) foreach (Filter<NPC> filter2 in filters) if (!object.ReferenceEquals(filter, filter2)) filter2.mode = null;
						if (filter.mode == null) filter.mode = true; else filter.mode = null;
						Refresh(true);
					}
					if (Main.mouseRight && Main.mouseRightRelease)
					{
						if (!KState.Special.Ctrl.Down()) foreach (Filter<NPC> filter2 in filters) if (!object.ReferenceEquals(filter, filter2)) filter2.mode = null;
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
			for (int i = 0; i < slots.Length; i++) slots[i] = new NPCSlot(this, i + Scroll * COLS);
			RunFilters();
		}

		protected void RunFilters()
		{
			filtered.Clear();
			foreach (NPC def in defs)
			{
				if (typing != null || filterText != null)
				{
					string tocheck = string.IsNullOrEmpty(def.displayName) ? def.name : def.displayName;
					if (tocheck.ToLower().IndexOf((typing == null ? filterText : typing).ToLower()) == -1) continue;
				}
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