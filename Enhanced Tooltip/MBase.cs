using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using Shockah.ETooltip.ModuleItem;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.ETooltip
{
	public class MBase : ModBase
	{
		public static MBase me { get; private set; }
		public static STooltip tip = new STooltip();

		public int maxPowerPick = 0, maxPowerAxe = 0, maxPowerHammer = 0, maxPowerRod = 0, maxPowerBait = 0, maxManaCost = 0;
		public List<Module<Item>> modulesItems = new List<Module<Item>>();
		public bool oneTooltip = false;
		public Player cachePlayer = null;
		public NPC cacheNPC = null;
		
		public override void OnLoad()
		{
			me = this;

			modulesItems.Add(new ModuleItemName());
			modulesItems.Add(new ModuleItemQuest());
			modulesItems.Add(new ModuleItemSocialSlot());
			modulesItems.Add(new ModuleItemDamage());
			modulesItems.Add(new ModuleItemCrit());
			modulesItems.Add(new ModuleItemSpeed());
			modulesItems.Add(new ModuleItemKnockback());
			modulesItems.Add(new ModuleItemHook());
			modulesItems.Add(new ModuleItemFishingRod());
			modulesItems.Add(new ModuleItemBait());
			modulesItems.Add(new ModuleItemEquipable());
			modulesItems.Add(new ModuleItemWand());
			modulesItems.Add(new ModuleItemDefense());
			modulesItems.Add(new ModuleItemPickaxe());
			modulesItems.Add(new ModuleItemAxe());
			modulesItems.Add(new ModuleItemHammer());
			modulesItems.Add(new ModuleItemRange());
			modulesItems.Add(new ModuleItemRestore());
			modulesItems.Add(new ModuleItemManaCost());
			modulesItems.Add(new ModuleItemPlaceable());
			modulesItems.Add(new ModuleItemAmmo());
			modulesItems.Add(new ModuleItemConsumable());
			modulesItems.Add(new ModuleItemMaterial());
			modulesItems.Add(new ModuleItemTipText());
			modulesItems.Add(new ModuleItemBuffDuration());
			modulesItems.Add(new ModuleItemPrefix());
			modulesItems.Add(new ModuleItemSetBonus());
			modulesItems.Add(new ModuleItemPrice());
		}

		public override void OnAllModsLoaded()
		{
			SBase.EventSTooltipDraw += () => { return tip; };

			SBase.EventPreSTooltipDraw += (sb, tip, rect) =>
			{
				tip.scale = (float)options["ttipScale"].Value;
				tip.alpha = null;
				if ((bool)options["ttipBackground"].Value) tip.alpha = .785f;
			};

			SBase.EventPostSTooltipDraw += (sb, tip, rect) =>
			{
				int life = 0, lifeMax = 0;
				if (cachePlayer != null)
				{
					life = cachePlayer.statLife;
					lifeMax = cachePlayer.statLifeMax2;
				}
				if (cacheNPC != null)
				{
					life = cacheNPC.life;
					lifeMax = cacheNPC.lifeMax;
				}

				if (life != 0 && lifeMax != 0)
				{
					float f = 1f * life / lifeMax;
					Color c = Module<Item>.DoubleLerp(Color.Red, Color.Yellow, Color.Lime, f);
					sb.Draw(Main.magicPixel, new Rectangle(rect.X - 2, rect.Y + rect.Height, (int)((rect.Width + 4) * f), 2), null, c, 0f, default(Vector2), SpriteEffects.None, 0f);
				}
				
				cachePlayer = null;
				cacheNPC = null;
			};

			foreach (KeyValuePair<string, Item> kvp in ItemDef.byName)
			{
				if (kvp.Value.type == ItemDef.unloadedItem.type) continue;
				if (kvp.Value.pick > maxPowerPick) maxPowerPick = kvp.Value.pick;
				if (kvp.Value.axe > maxPowerAxe) maxPowerAxe = kvp.Value.axe;
				if (kvp.Value.hammer > maxPowerHammer) maxPowerHammer = kvp.Value.hammer;
				if (kvp.Value.mana > maxManaCost) maxManaCost = kvp.Value.mana;
			}
		}

		public override void PreGameDraw(SpriteBatch sb)
		{
			if (!Main.gameMenu)
			{
				oneTooltip = false;
			}
		}

		private bool FillTooltipBase()
		{
			bool b = oneTooltip;
			oneTooltip = true;

			Main.localPlayer.showItemIcon = false;
			return !b;
		}

		public void FillTooltip(string cursorText)
		{
			if (!FillTooltipBase()) return;

			foreach (string line in cursorText.Split('\n'))
			{
				tip += line;
			}
		}

		public void FillTooltipBuff(int buffType)
		{
			if (!FillTooltipBase()) return;

			tip += new STooltip.Line(Module<int>.CText(Main.meleeBuff[buffType] ? Color.Lime : Color.White, Main.buffName[buffType]), (float)options["buffNameScale"].Value);
			tip += Main.buffTip[buffType];
		}

		public void FillTooltip(Item item, string cursorText = null)
		{
			if (!FillTooltipBase()) return;
			if (cursorText == null) cursorText = item.displayName;

			ETipStyle style = ETipStyle.map[(string)options["itemStyle"].Value];
			foreach (Module<Item> module in modulesItems) module.ModifyTip(style, options, tip, Main.toolTip);
		}

		public void FillTooltipItemInWorld(Item item)
		{
			if (!FillTooltipBase()) return;

			ETipStyle style = ETipStyle.map[(string)options["itemStyle"].Value];
			foreach (Module<Item> module in modulesItems) if (module is ModuleItemName)
			{
				module.ModifyTip(style, options, tip, item);
				break;
			}

			Color color = Color.White;
			switch ((string)options["itemOwnerColor"].Value)
			{
				case "Team": color = Main.teamColor[Main.player[item.owner].team]; break;
				default: break;
			}

			if (item.owner < 255 && Main.showItemOwner)
			{
				switch ((string)options["itemOwnerStyle"].Value)
				{
					case "Merge with name": tip.lines[0].textL += Module<Item>.CText(color, " <", Main.player[item.owner].name, ">"); break;
					case "Separate line": tip += Module<Item>.CText(color, "<", Main.player[item.owner].name, ">"); break;
				}
			}
		}

		public void FillTooltip(NPC npc)
		{
			if (!FillTooltipBase()) return;

			if (npc.active && npc.life > 0 && !npc.dontTakeDamage)
			{
				cacheNPC = npc;
				while (true)
				{
					if (npc.realLife == -1 || npc.realLife == npc.whoAmI) break;
					if (Main.npc[npc.realLife].active && Main.npc[npc.realLife].life > 0 && !Main.npc[npc.realLife].dontTakeDamage) npc = Main.npc[npc.realLife];
					else break;
				}

				Color color = Color.White;
				switch ((string)options["npcLifeColor"].Value)
				{
					case "Life": float f = 1f * npc.life / npc.lifeMax; color = Module<NPC>.DoubleLerp(Color.Red, Color.Yellow, Color.Lime, f); break;
					default: break;
				}

				float scale = (float)options["npcHeaderScale"].Value;
				switch ((string)options["playerStyle"].Value)
				{
					case "1 line": tip += new STooltip.Line(npc.displayName, Module<NPC>.CText(color, npc.life, "/", npc.lifeMax), scale); break;
					case "Multiple lines":
						tip += new STooltip.Line(npc.displayName, scale);
						tip += Module<NPC>.CText(color, npc.life, "/", npc.lifeMax);
						break;
				}
			}
		}

		public void FillTooltip(Player player)
		{
			if (!FillTooltipBase()) return;
			float f;
			cachePlayer = player;

			Color color1 = Color.White;
			switch ((string)options["playerNameColor"].Value)
			{
				case "Life": f = 1f * player.statLife / player.statLifeMax2; color1 = Module<NPC>.DoubleLerp(Color.Red, Color.Yellow, Color.Lime, f); break;
				case "Team": color1 = Main.teamColor[player.team]; break;
				default: break;
			}

			Color color2 = Color.White;
			switch ((string)options["playerLifeColor"].Value)
			{
				case "Life": f = 1f * player.statLife / player.statLifeMax2; color2 = Module<NPC>.DoubleLerp(Color.Red, Color.Yellow, Color.Lime, f); break;
				case "Team": color2 = Main.teamColor[player.team]; break;
				default: break;
			}

			Color color3 = Color.White;
			switch ((string)options["playerPvpColor"].Value)
			{
				case "Red": color3 = Color.Red; break;
				case "Life": f = 1f * player.statLife / player.statLifeMax2; color3 = Module<NPC>.DoubleLerp(Color.Red, Color.Yellow, Color.Lime, f); break;
				case "Team": color3 = Main.teamColor[player.team]; break;
				default: break;
			}

			float scale = (float)options["playerHeaderScale"].Value;
			switch ((string)options["playerStyle"].Value)
			{
				case "1 line": tip += new STooltip.Line(Module<Player>.CText(color1, player.name), Module<Player>.CText(color2, player.statLife, "/", player.statLifeMax2, (player.hostile ? Module<Player>.CText(color3, " PvP") : "")), scale); break;
				case "Multiple lines":
					tip += new STooltip.Line(Module<Player>.CText(color1, player.name), scale);
					tip += Module<Player>.CText(color2, player.statLife, "/", player.statLifeMax2);
					if (player.hostile) tip += Module<Player>.CText(color3, "PvP");
					break;
			}
		}

		public void FillTooltip(STooltip ttip)
		{
			if (!FillTooltipBase()) return;
			for (int i = 0; i < ttip.lines.Count; i++) tip += ttip.lines[i];
			
			ttip.lines.Clear();
		}

		public override object OnModCall(ModBase mod, params object[] args)
		{
			if (args.Length == 0)
			{
				return base.OnModCall(mod, args);
			}

			string call = (string)args[0];
			if (args.Length <= 3 && call == "RequestHooks")
			{
				if (args.Length >= 2)
				{
					bool includeSTooltip = args.Length == 3 && args[2] is bool && (bool)args[2];

					Action<string> ft1 = FillTooltip;
					Action<int> ft2 = FillTooltipBuff;
					Action<Item, string> ft3 = FillTooltip;
					Action<Item> ft4 = FillTooltipItemInWorld;
					Action<NPC> ft5 = FillTooltip;
					Action<Player> ft6 = FillTooltip;
					Action<STooltip> ft7 = FillTooltip;

					if (includeSTooltip)
					{
						var callback = args[1] as Action<
							Action<string>,
							Action<int>,
							Action<Item, string>,
							Action<Item>,
							Action<NPC>,
							Action<Player>,
							Action<STooltip>
						>;
						if (callback == null)
						{
							return new object[] { ft1, ft2, ft3, ft4, ft5, ft6, ft7 };
						}
						else
						{
							callback(ft1, ft2, ft3, ft4, ft5, ft6, ft7);
							return null;
						}
					}
					else
					{
						var callback = args[1] as Action<
							Action<string>,
							Action<int>,
							Action<Item, string>,
							Action<Item>,
							Action<NPC>,
							Action<Player>
						>;
						if (callback == null)
						{
							return new object[] { ft1, ft2, ft3, ft4, ft5, ft6 };
						}
						else
						{
							callback(ft1, ft2, ft3, ft4, ft5, ft6);
							return null;
						}
					}
				}
			}

			return base.OnModCall(mod, args);
		}
	}
}