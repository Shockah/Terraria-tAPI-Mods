﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.ChestContents
{
	public abstract class DisplayStyle
	{
		public static readonly DisplayStyle
			OneCircle = new DisplayStyleAImpl((sb, ilcc, items) =>
				{
					const int MAXLIFE = 12; ilcc.life = Math.Min(ilcc.life, MAXLIFE);
					const float OFFSETS_MIN = 40, OFFSETS_MAX = 70;
					Vector2 cpos = Main.mouse;
					
					float fLen = 1f * ilcc.life / MAXLIFE;
					float offset = OFFSETS_MIN + (OFFSETS_MAX - OFFSETS_MIN) * (1 - (1f * items.Count / Main.chest[ilcc.cId].item.Length));
					float r = (float)(items.Count * offset / 2 / Math.PI);

					float minR = offset / 2;
					if (r < minR) r = minR;

					float curR = r * fLen;
					float alpha = 1f * ilcc.life / MAXLIFE;

					for (int i = 0; i < items.Count; i++) ILChestContents.DrawItem(sb, cpos + Math2.LdirVector2(curR, 90 - 360d / items.Count * i + (1 - fLen) * 90), items[i], alpha);
				}
			),
			TwoCircles = new DisplayStyleAImpl((sb, ilcc, items) =>
				{
					const int MAXLIFE = 12; ilcc.life = Math.Min(ilcc.life, MAXLIFE);
					const float OFFSETS_MIN = 40, OFFSETS_MAX = 70, OFFSET_CIRCLES = 50;
					Vector2 cpos = Main.mouse;

					int firstc = items.Count >= 8 ? items.Count / 2 : items.Count;
					float fLen = 1f * ilcc.life / MAXLIFE;
					float offset = OFFSETS_MIN + (OFFSETS_MAX - OFFSETS_MIN) * (1 - (1f * firstc / Main.chest[ilcc.cId].item.Length));
					float r = (float)(firstc * offset / 2 / Math.PI);

					float minR = offset / 2;
					if (r < minR) r = minR;

					float curR = r * fLen;
					float alpha = 1f * ilcc.life / MAXLIFE;

					for (int i = 0; i < firstc; i++) ILChestContents.DrawItem(sb, cpos + Math2.LdirVector2(curR, 90 - 360d / firstc * i + (1 - fLen) * 90), items[i], alpha);
					for (int i = firstc; i < items.Count; i++) ILChestContents.DrawItem(sb, cpos + Math2.LdirVector2(curR + OFFSET_CIRCLES, 90 - 360d / (items.Count - firstc) * i + (1 - fLen) * 90), items[i], alpha);
				}
			),
			Rect = new DisplayStyleAImpl((sb, ilcc, items) =>
				{
					const int MAXLIFE = 12; ilcc.life = Math.Min(ilcc.life, MAXLIFE);
					const float OFFSETS = 50;
					Vector2 cpos = Main.mouse;

					float alpha = 1f * ilcc.life / MAXLIFE;

					int rw = 0, rh = 0;
					for (int i = 3; true; i++)
					{
						rw = 2 + i / 2;
						rh = (i - 1) / 2;
						if (rw * rh >= items.Count) break;
					}
					int totalw = (int)((rw - 1) * OFFSETS + 40);
					int totalh = (int)((rh - 1) * OFFSETS + 40);

					for (int y = 0; y < rh; y++)
					{
						if (items.Count < rh * y) break;
						int diff = items.Count - rw * y;
						int inrow = diff >= rw ? rw : diff;
						int roww = (int)((inrow - 1) * OFFSETS + 40);
						for (int xx = 0; xx < inrow; xx++)
						{
							Vector2 ipos = cpos - new Vector2(totalw / 2, totalh + OFFSETS / 2) + new Vector2(xx * OFFSETS + (totalw - roww) / 2, y * OFFSETS + (1f - alpha) * OFFSETS) + new Vector2(20, 20);
							ILChestContents.DrawItem(sb, ipos, items[xx + y * rw], alpha);
						}
					}
				}
			);
		
		public abstract void Draw(SpriteBatch sb, ILChestContents ilcc, List<Item> items);
	}

	public class DisplayStyleAImpl : DisplayStyle
	{
		protected readonly Action<SpriteBatch, ILChestContents, List<Item>> action;
		
		public DisplayStyleAImpl(Action<SpriteBatch, ILChestContents, List<Item>> a)
		{
			action = a;
		}

		public override void Draw(SpriteBatch sb, ILChestContents ilcc, List<Item> items)
		{
 			action(sb, ilcc, items);
		}
	}
}