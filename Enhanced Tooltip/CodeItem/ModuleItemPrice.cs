using Microsoft.Xna.Framework;
using Shockah.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.ETooltip.ModuleItem
{
	public class ModuleItemPrice : Module<Item>
	{
		public static readonly Color
			COLOR_PLATINUM = new Color(220, 220, 198),
			COLOR_GOLD = new Color(224, 201, 92),
			COLOR_SILVER = new Color(181, 192, 193),
			COLOR_COPPER = new Color(246, 138, 96),
			COLOR_NOCOIN = new Color(120, 120, 120);

		public override void ModifyTip(ETipStyle style, OptionList options, STooltip tip, Item item)
		{
			if (Main.npcShop > 0 || ((bool)options["itemAlwaysDisplayPrice"].Value && item.value > 0))
			{
				StringBuilder sb = new StringBuilder();
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
							Color color = coinP > 0 ? COLOR_PLATINUM : (coinG > 0 ? COLOR_GOLD : (coinS > 0 ? COLOR_SILVER : COLOR_COPPER));
							if (coinP > 0) { if (sb.Length != 0) sb.Append(" "); sb.Append("" + coinP + "p"); }
							if (coinP + coinG > 0) { if (sb.Length != 0) sb.Append(" "); sb.Append("" + coinG + "g"); }
							if (coinP + coinG + coinS > 0) { if (sb.Length != 0) sb.Append(" "); sb.Append("" + coinS + "s"); }
							if (coinP + coinG + coinS + coinC > 0) { if (sb.Length != 0) sb.Append(" "); sb.Append("" + coinC + "c"); }

							if (style == ETipStyle.Vanilla) tip += new STooltip.Line(CText(Main.toolTip.buy ? "Buy" : "Sell", " price: ", color, sb));
							if (style == ETipStyle.TwoCols) tip += new STooltip.Line((Main.toolTip.buy ? "Buy" : "Sell") + " price:", CText(color, sb));
							break;
						case "Multiple colors":
							if (coinP > 0) { if (sb.Length != 0) sb.Append(" "); sb.Append(SDrawing.ToColorCode(COLOR_PLATINUM) + coinP + "p"); }
							if (coinP + coinG > 0) { if (sb.Length != 0) sb.Append(" "); sb.Append(SDrawing.ToColorCode(COLOR_GOLD) + coinG + "g"); }
							if (coinP + coinG + coinS > 0) { if (sb.Length != 0) sb.Append(" "); sb.Append(SDrawing.ToColorCode(COLOR_SILVER) + coinS + "s"); }
							if (coinP + coinG + coinS + coinC > 0) { if (sb.Length != 0) sb.Append(" "); sb.Append(SDrawing.ToColorCode(COLOR_COPPER) + coinC + "c"); }

							if (style == ETipStyle.Vanilla) tip += new STooltip.Line(CText(Main.toolTip.buy ? "Buy" : "Sell", " price: ", sb));
							if (style == ETipStyle.TwoCols) tip += new STooltip.Line((Main.toolTip.buy ? "Buy" : "Sell") + " price:", sb.ToString());
							break;
						default: break;
					}
				}
			}
		}
	}
}