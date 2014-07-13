using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using System;
using System.Collections.Generic;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.ETooltip
{
	public class MBase : ModBase
	{
		public static readonly Color
			COLOR_PLATINUM = new Color(220, 220, 198),
			COLOR_GOLD = new Color(224, 201, 92),
			COLOR_SILVER = new Color(181, 192, 193),
			COLOR_COPPER = new Color(246, 138, 96),
			COLOR_NOCOIN = new Color(120, 120, 120);
		
		public static MBase me { get; private set; }
		public static STooltip tip = new STooltip();

		private static Color DoubleLerp(Color c1, Color c2, Color c3, float f)
		{
			return f < .5f ? Color.Lerp(c1, c2, f * 2f) : Color.Lerp(c2, c3, (f - .5f) * 2f);
		}

		public int maxPowerPick = 0, maxPowerAxe = 0, maxPowerHammer = 0, maxManaCost = 0;
		
		public override void OnLoad()
		{
			me = this;
		}

		public override void OnAllModsLoaded()
		{
			SBase.EventSTooltipDraw += () => { return tip; };

			foreach (KeyValuePair<string, Item> kvp in Defs.items)
			{
				if (kvp.Value.type == Defs.unloadedItem.type) continue;
				if (kvp.Value.pick > maxPowerPick) maxPowerPick = kvp.Value.pick;
				if (kvp.Value.axe > maxPowerAxe) maxPowerAxe = kvp.Value.axe;
				if (kvp.Value.hammer > maxPowerHammer) maxPowerHammer = kvp.Value.hammer;
				if (kvp.Value.mana > maxManaCost) maxManaCost = kvp.Value.mana;
			}
		}

		public void FillTooltip(string cursorText)
		{
			if (API.main.mouseNPC > -1) return;
			if (cursorText == null) return;

			Player player = Main.localPlayer;
			Item item = Main.toolTip.item;
			if (!item.IsBlank())
			{
				StringBuilder sb = null;
				
				string stackText = null;
				switch ((string)options["itemNameStackFormat"].Value)
				{
					case "(100)": if (item.stack > 1) stackText = "(" + item.stack + ")"; break;
					case "(100/999)": stackText = "(" + item.stack + "/" + item.maxStack + ")"; break;
					case "x100": if (item.stack > 1) stackText = "x" + item.stack; break;
					case "x100/999": stackText = "x" + item.stack + "/" + item.maxStack; break;
					default: break;
				}
				Color stackColor = Color.White;
				switch ((string)options["itemNameStackColor"].Value)
				{
					case "Rarity": stackColor = item.GetRarityColor(); break;
					case "Stack": float f = 1f * item.stack / item.maxStack; stackColor = DoubleLerp(Color.Red, Color.Yellow, Color.Lime, f); break;
					default: break;
				}
				tip += new STooltip.Line(SDrawing.ToColorCode(item.GetRarityColor()) + item.AffixName() + (stackText == null ? "" : " " + SDrawing.ToColorCode(stackColor) + stackText));

				if (item.damage > 0 && !item.notAmmo)
				{
					float itemDamage = item.damage;

					sb = new StringBuilder();
					if (item.melee) {
						if (sb.Length != 0) sb.Append(", ");
						sb.Append("melee");
						itemDamage *= player.meleeDamage;
					}
					if (item.ranged) {
						if (sb.Length != 0) sb.Append(", ");
						sb.Append("ranged");
						itemDamage *= player.rangedDamage;
						if (item.useAmmo == 1 || item.useAmmo == 323 || item.ammo == 1 || item.ammo == 323) {
							itemDamage *= player.arrowDamage;
							sb.Append(" (arrow)");
						}
						else if (item.useAmmo == 14 || item.useAmmo == 311 || item.ammo == 14 || item.ammo == 311) {
							itemDamage *= player.bulletDamage;
							sb.Append(" (bullet)");
						}
						else if (item.useAmmo == 771 || item.useAmmo == 246 || item.useAmmo == 312 || item.ammo == 771 || item.ammo == 246 || item.ammo == 312) {
							itemDamage *= player.rocketDamage;
							sb.Append(" (rocket)");
						}
					}
					if (item.magic) {
						if (sb.Length != 0) sb.Append(", ");
						sb.Append("magic");
						itemDamage *= player.magicDamage;
					}
					if (item.summon) {
						if (sb.Length != 0) sb.Append(", ");
						sb.Append("summon");
						itemDamage *= player.minionDamage;
					}
					if (sb.Length != 0) sb.Append(" ");

					Color damageColor = Color.White;
					switch ((string)options["itemDamageColor"].Value)
					{
						case "Viability":
							float min = Math2.Min(player.meleeDamage, player.rangedDamage, player.rangedDamage * player.arrowDamage, player.rangedDamage * player.bulletDamage, player.rangedDamage * player.rocketDamage, player.magicDamage, player.minionDamage);
							float max = Math2.Max(player.meleeDamage, player.rangedDamage, player.rangedDamage * player.arrowDamage, player.rangedDamage * player.bulletDamage, player.rangedDamage * player.rocketDamage, player.magicDamage, player.minionDamage);
							float cur = -1f;
							if (item.melee && cur < player.meleeDamage) cur = player.meleeDamage;
							if (item.ranged && cur < player.rangedDamage)
							{
								cur = player.rangedDamage;
								if (item.useAmmo == 1 || item.useAmmo == 323) cur *= player.arrowDamage;
								if (item.useAmmo == 14 || item.useAmmo == 311) cur *= player.bulletDamage;
								if (item.useAmmo == 771 || item.useAmmo == 246 || item.useAmmo == 312) cur *= player.rocketDamage;
							}
							if (item.magic && cur < player.magicDamage) cur = player.magicDamage;
							if (item.summon && cur < player.minionDamage) cur = player.minionDamage;
							float f = (cur - min) / (max - min);
							damageColor = cur == -1f ? Color.White : DoubleLerp(Color.Red, Color.Yellow, Color.Lime, f);
							break;
						default: break;
					}

					tip += new STooltip.Line(SDrawing.ToColorCode(damageColor) + (int)itemDamage + "#; " + sb + "damage");

					if (!item.summon)
					{
						int itemCrit = item.crit;
						if (item.melee) itemCrit = player.meleeCrit - player.heldItem.crit + itemCrit;
						else if (item.ranged) itemCrit = player.rangedCrit - player.heldItem.crit + itemCrit;
						else if (item.magic) itemCrit = player.magicCrit - player.heldItem.crit + itemCrit;

						if (itemCrit > 0)
						{
							Color critColor = Color.White;
							switch ((string)options["itemCritColor"].Value)
							{
								case "Chance": float f = itemCrit / 100f; critColor = DoubleLerp(Color.Red, Color.Yellow, Color.Lime, f); break;
								default: break;
							}
							tip += new STooltip.Line(SDrawing.ToColorCode(critColor) + (int)itemCrit + "%#; critical strike chance");
						}
					}

					if (item.useStyle > 0 && item.useAnimation > 0)
					{
						string speedText = null;
						if (item.useAnimation <= 8) speedText = "Insanely fast";
						else if (item.useAnimation <= 20) speedText = "Very fast";
						else if (item.useAnimation <= 25) speedText = "Fast";
						else if (item.useAnimation <= 30) speedText = "Average";
						else if (item.useAnimation <= 35) speedText = "Slow";
						else if (item.useAnimation <= 45) speedText = "Very slow";
						else if (item.useAnimation <= 55) speedText = "Extremely slow";
						else speedText = "Snail";
						Color speedColor = Color.White;
						switch ((string)options["itemSpeedColor"].Value)
						{
							case "Speed": float f = 1f * Math.Min(item.useAnimation, 55) / 55; speedColor = DoubleLerp(Color.Lime, Color.Yellow, Color.Red, f); break;
							default: break;
						}
						tip += new STooltip.Line(SDrawing.ToColorCode(speedColor) + speedText + ((bool)options["itemSpeedDetails"].Value ? " (" + item.useAnimation + "/60s)" : "") + "#; speed");
					}

					float knockback = item.knockBack;
					if (player.kbGlove) knockback *= 1.7f;
					if (item.ranged && player.armorSteath) knockback *= 1f + (1f - player.stealth) * 0.5f;
					if (item.summon) knockback += player.minionKB;

					string knockbackText = null;
					if (knockback == 0f) knockbackText = "No";
					else if (knockback <= 1.5f) knockbackText = "Extremely weak";
					else if (knockback <= 3f) knockbackText = "Very weak";
					else if (knockback <= 4f) knockbackText = "Weak";
					else if (knockback <= 6f) knockbackText = "Average";
					else if (knockback <= 7f) knockbackText = "Strong";
					else if (knockback <= 9f) knockbackText = "Very strong";
					else if (knockback <= 11f) knockbackText = "Extremely strong";
					else knockbackText = "Insane";
					Color knockbackColor = Color.White;
					switch ((string)options["itemKnockbackColor"].Value)
					{
						case "Knockback": float f = Math.Min(knockback, 11f) / 11f; knockbackColor = DoubleLerp(Color.Red, Color.Yellow, Color.Lime, f); break;
						default: break;
					}
					tip += new STooltip.Line(SDrawing.ToColorCode(knockbackColor) + knockbackText + "#; knockback");
				}

				if (item.headSlot > 0 || item.bodySlot > 0 || item.legSlot > 0 || item.accessory)
				{
					sb = new StringBuilder();
					if (item.headSlot > 0) { if (sb.Length != 0) sb.Append(", "); sb.Append("head"); }
					if (item.bodySlot > 0) { if (sb.Length != 0) sb.Append(", "); sb.Append("body"); }
					if (item.legSlot > 0) { if (sb.Length != 0) sb.Append(", "); sb.Append("legs"); }
					if (item.accessory) { if (sb.Length != 0) sb.Append(", "); sb.Append("accessory"); }
					tip += new STooltip.Line("Equipable" + (item.vanity ? " vanity" : "") + ": " + sb);
				}

				if (item.tileWand > 0)
				{
					Item itemDef = Defs.items[Defs.itemNames[item.tileWand]];
					tip += new STooltip.Line("Consumes " + SDrawing.ToColorCode((bool)options["itemWandItemColor"].Value ? itemDef.GetRarityColor() : Color.White) + itemDef.displayName);
				}

				if (item.defense != 0)
				{
					Color defenseColor = Color.White;
					switch ((string)options["itemDefenseColor"].Value)
					{
						case "Green/Red": defenseColor = item.defense > 0 ? Color.Lime : Color.Red; break;
						default: break;
					}
					tip += new STooltip.Line(SDrawing.ToColorCode(defenseColor) + item.defense + "#; defense");
				}

				if (item.pick > 0)
				{
					Color toolPowerColor = Color.White;
					switch ((string)options["itemToolPowerColor"].Value)
					{
						case "Green": toolPowerColor = Color.Lime; break;
						case "Power": float f = 1f * item.pick / maxPowerPick; toolPowerColor = DoubleLerp(Color.Red, Color.Yellow, Color.Lime, f); break;
						default: break;
					}
					tip += new STooltip.Line(SDrawing.ToColorCode(toolPowerColor) + item.pick + "%#; pickaxe power");
				}
				if (item.axe > 0)
				{
					Color toolPowerColor = Color.White;
					switch ((string)options["itemToolPowerColor"].Value)
					{
						case "Green": toolPowerColor = Color.Lime; break;
						case "Power": float f = 1f * item.axe / maxPowerAxe; toolPowerColor = DoubleLerp(Color.Red, Color.Yellow, Color.Lime, f); break;
						default: break;
					}
					tip += new STooltip.Line(SDrawing.ToColorCode(toolPowerColor) + (item.axe * 5) + "%#; axe power");
				}
				if (item.hammer > 0)
				{
					Color toolPowerColor = Color.White;
					switch ((string)options["itemToolPowerColor"].Value)
					{
						case "Green": toolPowerColor = Color.Lime; break;
						case "Power": float f = 1f * item.hammer / maxPowerHammer; toolPowerColor = DoubleLerp(Color.Red, Color.Yellow, Color.Lime, f); break;
						default: break;
					}
					tip += new STooltip.Line(SDrawing.ToColorCode(toolPowerColor) + item.hammer + "%#; hammer power");
				}

				if (item.tileBoost != 0)
				{
					Color toolRangeColor = Color.White;
					switch ((string)options["itemToolRangeColor"].Value)
					{
						case "Green/Red": toolRangeColor = item.tileBoost > 0 ? Color.Lime : Color.Red; break;
						default: break;
					}
					tip += new STooltip.Line(SDrawing.ToColorCode(toolRangeColor) + (item.tileBoost > 0 ? "+" : "") + item.tileBoost + "#; range");
				}

				bool optToolRangeColorEffect = (bool)options["itemRestoreColor"].Value;
				bool optToolRangeColorType = (bool)options["itemRestoreTypeColor"].Value;
				if (item.healLife > 0)
				{
					tip += new STooltip.Line("Restores " + SDrawing.ToColorCode(optToolRangeColorEffect ? Color.Lime : Color.White) + item.healLife + SDrawing.ToColorCode(optToolRangeColorType ? Color.Red : Color.White) + " life");
				}
				if (item.healMana > 0)
				{
					tip += new STooltip.Line("Restores " + SDrawing.ToColorCode(optToolRangeColorEffect ? Color.Lime : Color.White) + item.healMana + SDrawing.ToColorCode(optToolRangeColorType ? Color.DeepSkyBlue : Color.White) + " mana");
				}

				if (item.mana > 0)
				{
					if (item.type != 127 || !player.spaceGun)
					{
						float itemMana = item.mana;
						itemMana *= player.manaCost;
						if ((int)itemMana > 0)
						{
							Color manaCostColor = Color.White;
							float f;
							switch ((string)options["itemManaCostColor"].Value)
							{
								case "Blue": manaCostColor = Color.DeepSkyBlue; break;
								case "Mana cost": f = 1f * itemMana / maxManaCost; manaCostColor = DoubleLerp(Color.Lime, Color.Yellow, Color.Red, f); break;
								case "Max mana": f = 1f * itemMana / (player.statManaMax2 / 5); manaCostColor = DoubleLerp(Color.Lime, Color.Yellow, Color.Red, Math.Min(f, 1f)); break;
								default: break;
							}
							tip += new STooltip.Line("Uses " + SDrawing.ToColorCode(manaCostColor) + itemMana + SDrawing.ToColorCode((bool)options["itemManaCostTypeColor"].Value ? Color.DeepSkyBlue : Color.White) + " mana");
						}
					}
				}

				if (item.createWall > 0 || item.createTile > -1 || item.name == "Xmas decorations")
				{
					if (item.type != 213 && item.type != 832)
					{
						tip += new STooltip.Line("Can be placed");
					}
				}
				else if (item.ammo > 0 && !item.notAmmo)
				{
					if (item.ammo == 1 || item.ammo == 323) tip += new STooltip.Line("Ammo: arrow");
					else if (item.ammo == 14 || item.ammo == 311) tip += new STooltip.Line("Ammo: bullet");
					else if (item.ammo == 771 || item.ammo == 246 || item.useAmmo == 312) tip += new STooltip.Line("Ammo: rocket");
					else tip += new STooltip.Line("Ammo");
				}
				else if (item.consumable)
				{
					tip += new STooltip.Line("Consumable");
				}

				if (item.material && !item.notMaterial)
				{
					tip += new STooltip.Line("Material");
				}

				if (item.toolTips != null)
				{
					List<string> effective = new List<string>(item.toolTips);
					if (item.allSubClasses.Length > 0) item.ModifyToolTip(player, effective);
					foreach (string tt in effective) tip += new STooltip.Line(tt);
				}

				if (item.buffTime > 0)
				{
					Color buffDurationColor = Color.White;
					switch ((string)options["itemBuffDurationColor"].Value)
					{
						case "Green": buffDurationColor = Color.Lime; break;
						default: break;
					}
					tip += new STooltip.Line(SDrawing.ToColorCode(buffDurationColor) + " " + (item.buffTime > 60 * 60 ? "minute" : "second") + "#; duration");
				}

				if (!item.prefix.Equals(Prefix.None))
				{
					foreach (Tuple<string, bool> t in item.prefix.TooltipText(item))
					{
						tip += new STooltip.Line(SDrawing.ToColorCode(t.Item2 ? new Color(120, 190, 120) : new Color(190, 120, 120)) + t.Item1);
					}
				}

				if (Main.toolTip.wornArmor && player.setBonus != "")
				{
					tip += new STooltip.Line("Set bonus: " + player.setBonus);
				}

				if (Main.npcShop > 0 || ((bool)options["itemAlwaysDisplayPrice"].Value && item.value > 0))
				{
					int itemValue = item.value;
					if (!Main.toolTip.buy) itemValue /= 5;
					itemValue *= item.stack;

					int coinP = 0, coinG = 0, coinS = 0, coinC = 0;
					coinC = itemValue;
					if (coinC >= 100)
					{
						coinS = coinC / 100;
						coinC %= 100;
						if (coinS >= 100)
						{
							coinG = coinS / 100;
							coinS %= 100;
							if (coinG >= 100)
							{
								coinP = coinG / 100;
								coinG %= 100;
							}
						}
					}

					if (itemValue == 0)
					{
						tip += new STooltip.Line(SDrawing.ToColorCode(COLOR_NOCOIN) + "No Value");
					}
					else
					{
						switch ((string)options["itemPriceStyle"].Value)
						{
							case "One color":
								Color valueColor = coinP > 0 ? COLOR_PLATINUM : (coinG > 0 ? COLOR_GOLD : (coinS > 0 ? COLOR_SILVER : COLOR_COPPER));
								sb = new StringBuilder();
								if (coinP > 0) { if (sb.Length != 0) sb.Append(" "); sb.Append("" + coinP + "p"); }
								if (coinP + coinG > 0) { if (sb.Length != 0) sb.Append(" "); sb.Append("" + coinG + "g"); }
								if (coinP + coinG + coinS > 0) { if (sb.Length != 0) sb.Append(" "); sb.Append("" + coinS + "s"); }
								if (coinP + coinG + coinS + coinC > 0) { if (sb.Length != 0) sb.Append(" "); sb.Append("" + coinC + "c"); }
								tip += new STooltip.Line((Main.toolTip.buy ? "Buy" : "Sell") + " price: " + SDrawing.ToColorCode(valueColor) + sb);
								break;
							case "Multiple colors":
								sb = new StringBuilder();
								if (coinP > 0) { if (sb.Length != 0) sb.Append(" "); sb.Append(SDrawing.ToColorCode(COLOR_PLATINUM) + coinP + "p"); }
								if (coinP + coinG > 0) { if (sb.Length != 0) sb.Append(" "); sb.Append(SDrawing.ToColorCode(COLOR_GOLD) + coinG + "g"); }
								if (coinP + coinG + coinS > 0) { if (sb.Length != 0) sb.Append(" "); sb.Append(SDrawing.ToColorCode(COLOR_SILVER) + coinS + "s"); }
								if (coinP + coinG + coinS + coinC > 0) { if (sb.Length != 0) sb.Append(" "); sb.Append(SDrawing.ToColorCode(COLOR_COPPER) + coinC + "c"); }
								tip += new STooltip.Line((Main.toolTip.buy ? "Buy" : "Sell") + " price: " + sb);
								break;
							default: break;
						}
					}
				}
			}
		}
	}
}