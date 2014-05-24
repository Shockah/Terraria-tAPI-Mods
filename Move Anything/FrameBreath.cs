using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.MoveAnything
{
	public class FrameBreath : FrameHiding
	{
		public bool hidden = false;

		public FrameBreath() : base("Breath", Anchor.TopLeft, new Vector2(500f + (Main.screenWidth - 800f), 32f + (76f + Main.mH)), new Vector2(260, 26)) { }

		protected override void OnRender2(SpriteBatch sb)
		{
			Vector2 pos = FramePos1();
			bool flag3 = false;
			if (Main.localPlayer.lavaTime < Main.localPlayer.lavaMax && Main.localPlayer.lavaWet)
			{
				flag3 = true;
			}
			else
			{
				if (Main.localPlayer.lavaTime < Main.localPlayer.lavaMax && Main.localPlayer.breath == Main.localPlayer.breathMax)
				{
					flag3 = true;
				}
			}
			if (Main.localPlayer.breath < Main.localPlayer.breathMax && !Main.localPlayer.ghost && !flag3)
			{
				int num14 = 76 + Main.mH;
				int num15 = 20;
				for (int j = 1; j < Main.localPlayer.breathMax / num15 + 1; j++)
				{
					float num16 = 1f;
					int num17;
					if (Main.localPlayer.breath >= j * num15)
					{
						num17 = 255;
					}
					else
					{
						float num18 = (float)(Main.localPlayer.breath - (j - 1) * num15) / (float)num15;
						num17 = (int)(30f + 225f * num18);
						if (num17 < 30)
						{
							num17 = 30;
						}
						num16 = num18 / 4f + 0.75f;
						if ((double)num16 < 0.75)
						{
							num16 = 0.75f;
						}
					}
					int num19 = 0;
					int num20 = 0;
					if (j > 10)
					{
						num19 -= 260;
						num20 += 26;
					}
					sb.Draw(Main.bubbleTexture, new Vector2((float)(pos.X + 26 * (j - 1) + num19), pos.Y + ((float)Main.bubbleTexture.Height - (float)Main.bubbleTexture.Height * num16) / 2f + (float)num20), null, new Color(num17, num17, num17, num17), 0f, default(Vector2), num16, SpriteEffects.None, 0f);
				}
			}
			if (Main.localPlayer.lavaTime < Main.localPlayer.lavaMax && !Main.localPlayer.ghost && flag3)
			{
				int num21 = 76 + Main.mH;
				int num22 = Main.localPlayer.lavaMax / 10;
				for (int k = 1; k < Main.localPlayer.lavaMax / num22 + 1; k++)
				{
					float num23 = 1f;
					int num24;
					if (Main.localPlayer.lavaTime >= k * num22)
					{
						num24 = 255;
					}
					else
					{
						float num25 = (float)(Main.localPlayer.lavaTime - (k - 1) * num22) / (float)num22;
						num24 = (int)(30f + 225f * num25);
						if (num24 < 30)
						{
							num24 = 30;
						}
						num23 = num25 / 4f + 0.75f;
						if ((double)num23 < 0.75)
						{
							num23 = 0.75f;
						}
					}
					int num26 = 0;
					int num27 = 0;
					if (k > 10)
					{
						num26 -= 260;
						num27 += 26;
					}
					sb.Draw(Main.flameTexture, new Vector2((float)(pos.X + 26 * (k - 1) + num26), pos.Y + ((float)Main.flameTexture.Height - (float)Main.flameTexture.Height * num23) / 2f + (float)num27), null, new Color(num24, num24, num24, num24), 0f, default(Vector2), num23, SpriteEffects.None, 0f);
				}
			}
		}
	}
}
