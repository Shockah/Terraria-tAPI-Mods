using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.ETooltip
{
	public class MInterface : ModInterface
	{
		public override bool PreDrawTooltipLocalHealth(SpriteBatch sb, ref string text)
		{
			MBase.me.FillTooltip(text);
			return false;
		}

		public override bool PreDrawTooltipLocalMana(SpriteBatch sb, ref string text)
		{
			MBase.me.FillTooltip(text);
			return false;
		}

		public override bool PreDrawTooltipText(SpriteBatch sb, string context, ref string text, ref int rare)
		{
			MBase.me.FillTooltip(Drawing.ToColorCode(Item.GetRarityColor(rare)) + text);
			return false;
		}

		public override bool PreDrawTooltipItem(SpriteBatch sb, Item item, ref string text, ref int rare)
		{
			MBase.me.FillTooltip(item);
			return false;
		}

		public override bool PreDrawTooltipPlayer(SpriteBatch sb, Player player, ref string text, ref int difficulty)
		{
			MBase.me.FillTooltip(player);
			return false;
		}

		public override bool PreDrawTooltipNPC(SpriteBatch sb, NPC npc, ref bool isHidingMimic, ref string text)
		{
			MBase.me.FillTooltip(npc);
			return false;
		}

		public override bool PreDrawTooltipItemInWorld(SpriteBatch sb, Item item, ref string text, ref int rare)
		{
			MBase.me.FillTooltipItemInWorld(item);
			return false;
		}

		public override bool PreDrawTooltipBuff(SpriteBatch sb, int buffIndex, int buffType, ref string text, ref bool imbue)
		{
			MBase.me.FillTooltipBuff(buffType);
			return false;
		}
	}
}